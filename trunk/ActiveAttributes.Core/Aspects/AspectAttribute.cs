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
    public Type RequiresTargetType { get; set; }      // AffectedType = typeof(T1), AffectedTypes = new [] {typeof(T1),typeof(T2)}
    // condition for interface types: target type implements interface-type
    //           for class types: target type is equal to class-type
    //              if aspect has [AspectUsage (Inherits=true)] also target type inherits from class-type
    /// <summary>Use only for assembly-level attributes.</summary>
    public string RequiresNamespace { get; set; }     // AffectedTypeNamePattern/Patterns
    /// <summary>Use only for assembly-level attributes.</summary>
    public bool ApplyToDerivedTypes { get; set; }


    //public Type ApplyTo { get; set; }
    //[assembly: LogAspect ("LoggingNamespace....", ApplyTo = typeof (Muh))]
    //[assembly: LogAspect ("LoggingNamespace....", ApplyTo = typeof (Woof))]

    //void Setup ()
    //{
    //   ApplyAspect (() => new LogAspectAttribute("LoggingNamespace..."), typeof (Meow)); 
    //}

    // MemberNameFilter, MemberVisibilityFilter, MemberCustomAttributeFilter, MemberFlagsFilter
    public string RequiresMemberName { get; set; }    // 
    public Visibility RequiresMemberVisibility { get; set; }
    public Type[] RequiresMarkers { get; set; }
    public MemberFlags RequiresMemberAttributes { get; set; }
    public Type RequiresReturnType { get; set; }
    public Type[] RequiresArguments { get; set; }

    public virtual bool Matches (MethodInfo methodInfo)
    {
      var declaringType = methodInfo.DeclaringType;

      return MatchesType (RequiresTargetType, declaringType) &&
             MatchesMemberName (methodInfo.Name) &&
             MatchesNamespace (declaringType.Namespace) &&
             MatchesMemberVisibility (methodInfo.Attributes) &&
             MatchesMarkers (methodInfo) &&
             MatchesType (RequiresReturnType, methodInfo.ReturnType) &&
             MatchesArguments (methodInfo);
    }

    #region Match methods

    private bool MatchesType (Type expected, Type actual)
    {
      return expected == null || expected.IsAssignableFrom (actual);
    }

    private bool MatchesMemberName (string methodName)
    {
      return RequiresMemberName == null || Regex.IsMatch (methodName, GetRegexPattern(RequiresMemberName));
    }

    private bool MatchesNamespace (string ns)
    {
      return RequiresNamespace == null || Regex.IsMatch (ns, GetRegexPattern (RequiresNamespace));
    }

    private bool MatchesMemberVisibility (MethodAttributes attributes)
    {
      if (RequiresMemberVisibility == Visibility.None)
        return true;

      var flags = Visibility.None;

      if (attributes.HasFlags (MethodAttributes.Assembly)) flags |= Visibility.Assembly;
      if (attributes.HasFlags (MethodAttributes.Public)) flags |= Visibility.Public;
      if (attributes.HasFlags (MethodAttributes.Family)) flags |= Visibility.Family;
      if (attributes.HasFlags (MethodAttributes.Private)) flags |= Visibility.Private;
      if (attributes.HasFlags (MethodAttributes.FamANDAssem)) flags |= Visibility.FamilyAndAssembly;
      if (attributes.HasFlags (MethodAttributes.FamORAssem)) flags |= Visibility.FamilyOrAssembly;
      // TODO: ?
      
      return (RequiresMemberVisibility & flags) == RequiresMemberVisibility;
    }

    private bool MatchesMarkers (MethodInfo methodInfo)
    {
      if (RequiresMarkers == null)
        return true;

      var customAttributes = methodInfo.GetCustomAttributes (true);
      return RequiresMarkers.All (marker => customAttributes.Any (x => marker.IsAssignableFrom (x.GetType())));
    }

    private bool MatchesArguments (MethodInfo methodInfo)
    {
      if (RequiresArguments == null)
        return true;

      var arguments = methodInfo.GetParameters();
      var zipped = RequiresArguments.Zip (arguments, (expected, actual) => new { Expected = expected, Actual = actual.ParameterType });
      return zipped.All (zip => zip.Actual == zip.Expected);
    }

    private string GetRegexPattern (string wildcardPattern)
    {
      return "^" + wildcardPattern.Replace ("*", ".*") + "$";
    }

    #endregion
  }
}