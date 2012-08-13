// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Extensions;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Aspects
{
  /// <summary>
  ///   Provides base functionality of an aspect.
  /// </summary>
  [AttributeUsage (AttributeTargets.All, AllowMultiple = true)]
  public abstract class AspectAttribute : Attribute, ISerializable
  {
    protected AspectAttribute ()
    {
    }

    #region Serialization

    protected AspectAttribute (SerializationInfo info, StreamingContext context)
    {
      Scope = (AspectScope) info.GetInt32 ("scope");
      Priority = info.GetInt32 ("priority");
    }

    [SecurityPermission (SecurityAction.Demand, SerializationFormatter = true)]
    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("scope", (int) Scope);
      info.AddValue ("priority", Priority);
    }

    #endregion

    public AspectScope Scope { get; set; }

    public int Priority { get; set; }


    public virtual bool Validate (MethodInfo method)
    {
      return true;
    }

    public object Clone ()
    {
      return MemberwiseClone();
    }

    /// <summary>Use only for assembly-level attributes.</summary>
    /// <remarks>
    /// Type = Interface: target type must implement interface
    /// Type = Class: (a) target type must be equal to the class, or (b) target type inherits from class and aspect inheritance is set to true
    /// </remarks>
    public Type ApplyToType { get; set; }
    public Type[] ApplyToTypes { get; set; }

    public string ApplyToTypeNamePattern { get; set; }
    public string[] ApplyToTypeNamePatterns { get; set; }

    public string MemberNameFilter { get; set; }
    public Visibility MemberVisibilityFilter { get; set; }
    public Type[] MemberCustomAttributeFilter { get; set; }
    public MemberFlags MemberFlagsFilter { get; set; }
    public Type MemberReturnTypeFilter { get; set; }
    public Type[] MemberArgumentsFilter { get; set; }

    private IEnumerable<Type> GetApplyToTypes ()
    {
      yield return ApplyToType;

      if (ApplyToTypes != null)
      {
        foreach (var type in ApplyToTypes)
          yield return type;
      }
    }

    private IEnumerable<string> GetApplyToTypeNamePatterns ()
    {
      yield return ApplyToTypeNamePattern;

      if (ApplyToTypeNamePatterns != null)
      {
        foreach (var pattern in ApplyToTypeNamePatterns)
          yield return pattern;
      }
    }

    public virtual bool Matches (MethodInfo methodInfo)
    {
      var declaringType = methodInfo.DeclaringType;
      var applyToTypes = GetApplyToTypes();
      var applyToTypeNamePatterns = GetApplyToTypeNamePatterns();

      return applyToTypes.All (x => MatchesType (x, declaringType)) &&
             applyToTypeNamePatterns.All(x => MatchesNamespace(x, declaringType.Namespace)) &&
             MatchesMemberName (methodInfo.Name) &&
             MatchesMemberVisibility (methodInfo.Attributes) &&
             MatchesMarkers (methodInfo) &&
             MatchesType (MemberReturnTypeFilter, methodInfo.ReturnType) &&
             MatchesArguments (methodInfo);
    }

    #region Match methods

    private bool MatchesType (Type expected, Type actual)
    {
      return expected == null || expected.IsAssignableFrom (actual);
    }

    private bool MatchesNamespace (string expected, string actual)
    {
      return expected == null || Regex.IsMatch (actual, GetRegexPattern (expected));
    }

    private bool MatchesMemberName (string methodName)
    {
      return MemberNameFilter == null || Regex.IsMatch (methodName, GetRegexPattern(MemberNameFilter));
    }

    private bool MatchesMemberVisibility (MethodAttributes attributes)
    {
      if (MemberVisibilityFilter == Visibility.None)
        return true;

      var flags = Visibility.None;

      if (attributes.HasFlags (MethodAttributes.Assembly)) flags |= Visibility.Assembly;
      if (attributes.HasFlags (MethodAttributes.Public)) flags |= Visibility.Public;
      if (attributes.HasFlags (MethodAttributes.Family)) flags |= Visibility.Family;
      if (attributes.HasFlags (MethodAttributes.Private)) flags |= Visibility.Private;
      if (attributes.HasFlags (MethodAttributes.FamANDAssem)) flags |= Visibility.FamilyAndAssembly;
      if (attributes.HasFlags (MethodAttributes.FamORAssem)) flags |= Visibility.FamilyOrAssembly;
      // TODO: ?
      
      return (MemberVisibilityFilter & flags) == MemberVisibilityFilter;
    }

    private bool MatchesMarkers (MethodInfo methodInfo)
    {
      if (MemberCustomAttributeFilter == null)
        return true;

      var customAttributes = methodInfo.GetCustomAttributes (true);
      return MemberCustomAttributeFilter.All (marker => customAttributes.Any (x => marker.IsAssignableFrom (x.GetType())));
    }

    private bool MatchesArguments (MethodInfo methodInfo)
    {
      if (MemberArgumentsFilter == null)
        return true;

      var arguments = methodInfo.GetParameters();
      var zipped = MemberArgumentsFilter.Zip (arguments, (expected, actual) => new { Expected = expected, Actual = actual.ParameterType });
      return zipped.All (zip => zip.Actual == zip.Expected);
    }

    private string GetRegexPattern (string wildcardPattern)
    {
      return "^" + wildcardPattern.Replace ("*", ".*") + "$";
    }

    #endregion
  }
}