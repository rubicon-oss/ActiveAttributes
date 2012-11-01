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

using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Aspects
{
  /// <summary>
  ///   Provides base functionality (i.e., Scope, Priority, Filter, Matching, etc.) for derived aspects.
  /// </summary>
  [AttributeUsage (AttributeTargets.All, AllowMultiple = true)]
  public abstract class AspectAttribute : Attribute
  {
    /// <summary>
    ///   Scope of an aspect. It can be either <see cref = "Infrastructure.AdviceInfo.Scope.Static" /> or <see cref = "Infrastructure.AdviceInfo.Scope.Instance" />.
    /// </summary>
    public Scope Scope { get; set; }

    /// <summary>
    ///   Gets or sets the priority of an aspect. Priorities can define the order of aspects applied to a target element. 
    ///   Priorities are superior to <see cref = "IAspectOrderingRule" />.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    ///   Gets or sets the member name as filter for selected targets. Wildcards are supported.
    /// </summary>
    public string MemberNameFilter { get; set; }

    /// <summary>
    ///   Gets or sets the member visibility as filter for selected targets.
    /// </summary>
    public Visibility MemberVisibilityFilter { get; set; }

    /// <summary>
    ///   Gets or sets the member custom attributes as filter for selected targets. Base types do match.
    /// </summary>
    public Type[] MemberCustomAttributeFilter { get; set; }

    /// <summary>
    ///   Gets or sets the member flags as filter for selected targets.
    /// </summary>
    public MemberFlags MemberFlagsFilter { get; set; }

    /// <summary>
    ///   Gets or sets the member return type as filter for selected targets. Base types do match.
    /// </summary>
    public Type MemberReturnTypeFilter { get; set; }

    /// <summary>
    ///   Gets or sets the member arguments as filter for selected targets. Base types do match.
    /// </summary>
    public Type[] MemberArgumentsFilter { get; set; }

    // TODO ApplyToType
    /// <summary>
    ///   Use only for assembly-level attributes.
    /// </summary>
    /// <remarks>
    ///   Type = Interface: target type must implement interface
    ///   Type = Class: (a) target type must be equal to the class, or (b) target type inherits from class and aspect inheritance is set to true
    /// </remarks>
    //public Type ApplyToType { get; set; }
    //public Type[] ApplyToTypes { get; set; }

    // TODO ApplyToTypeNamePattern
    //public string ApplyToTypeNamePattern { get; set; }
    //public string[] ApplyToTypeNamePatterns { get; set; }

    /// <summary>
    ///   Returns true if the <see cref="MethodInfo"/> is applicable for the aspect, otherwise false.
    /// </summary>
    /// <param name="methodInfo">The method on which a aspect should be applied.</param>
    public virtual bool Matches (MethodInfo methodInfo)
    {
      //var declaringType = methodInfo.DeclaringType;
      //var applyToTypes = GetApplyToTypes();
      //var applyToTypeNamePatterns = GetApplyToTypeNamePatterns();

      return
          //applyToTypes.All (x => MatchesType (x, declaringType)) &&
          //applyToTypeNamePatterns.All (x => MatchesNamespace (x, declaringType.Namespace)) &&
          MatchesMemberName (methodInfo.Name) &&
          MatchesMemberVisibility (methodInfo) &&
          MatchesMemberFlags (methodInfo) &&
          MatchesCustomAttributes (methodInfo) &&
          MatchesType (MemberReturnTypeFilter, methodInfo.ReturnType) &&
          MatchesArguments (methodInfo);
    }

    #region Match methods

    //private IEnumerable<Type> GetApplyToTypes ()
    //{
    //  yield return ApplyToType;

    //  if (ApplyToTypes != null)
    //  {
    //    foreach (var type in ApplyToTypes)
    //      yield return type;
    //  }
    //}

    //private IEnumerable<string> GetApplyToTypeNamePatterns ()
    //{
    //  yield return ApplyToTypeNamePattern;

    //  if (ApplyToTypeNamePatterns != null)
    //  {
    //    foreach (var pattern in ApplyToTypeNamePatterns)
    //      yield return pattern;
    //  }
    //}

    private bool MatchesType (Type expected, Type actual)
    {
      return expected == null || expected.IsAssignableFrom (actual);
    }

    //private bool MatchesNamespace (string expected, string actual)
    //{
    //  return expected == null || Regex.IsMatch (actual, GetRegexPattern (expected));
    //}

    private bool MatchesMemberName (string methodName)
    {
      return MemberNameFilter == null || Regex.IsMatch (methodName, GetRegexPattern (MemberNameFilter));
    }

    private bool MatchesMemberVisibility (MethodBase methodInfo)
    {
      return !(
                  (MemberVisibilityFilter.HasFlags (Visibility.Assembly) && !methodInfo.IsAssembly) ||
                  (MemberVisibilityFilter.HasFlags (Visibility.Public) && !methodInfo.IsPublic) ||
                  (MemberVisibilityFilter.HasFlags (Visibility.Family) && !methodInfo.IsFamily) ||
                  (MemberVisibilityFilter.HasFlags (Visibility.Private) && !methodInfo.IsPrivate) ||
                  (MemberVisibilityFilter.HasFlags (Visibility.FamilyAndAssembly) && !methodInfo.IsFamilyAndAssembly) ||
                  (MemberVisibilityFilter.HasFlags (Visibility.FamilyOrAssembly) && !methodInfo.IsFamilyOrAssembly));
    }

    private bool MatchesMemberFlags (MethodBase methodInfo)
    {
      return !(
                  (MemberFlagsFilter.HasFlags (MemberFlags.Final) && !methodInfo.IsFinal) ||
                  (MemberFlagsFilter.HasFlags (MemberFlags.NonFinal) && methodInfo.IsFinal) ||
                  (MemberFlagsFilter.HasFlags (MemberFlags.Static) && !methodInfo.IsStatic) ||
                  (MemberFlagsFilter.HasFlags (MemberFlags.NonStatic) && methodInfo.IsStatic) ||
                  (MemberFlagsFilter.HasFlags (MemberFlags.Virtual) && !methodInfo.IsVirtual) ||
                  (MemberFlagsFilter.HasFlags (MemberFlags.NonVirtual) && methodInfo.IsVirtual) ||
                  (MemberFlagsFilter.HasFlags (MemberFlags.NewSlot) && !methodInfo.Attributes.HasFlags (MethodAttributes.NewSlot)) ||
                  (MemberFlagsFilter.HasFlags (MemberFlags.NonNewSlot) && methodInfo.Attributes.HasFlags (MethodAttributes.NewSlot)));
    }

    private bool MatchesCustomAttributes (MethodInfo methodInfo)
    {
      if (MemberCustomAttributeFilter == null)
        return true;

      var customAttributes = methodInfo.GetCustomAttributes (true);
      return MemberCustomAttributeFilter.All (marker => customAttributes.Any (marker.IsInstanceOfType));
    }

    private bool MatchesArguments (MethodInfo methodInfo)
    {
      if (MemberArgumentsFilter == null)
        return true;

      var arguments = methodInfo.GetParameters();
      var zipped = MemberArgumentsFilter.Zip (arguments, (expected, actual) => new { Expected = expected, Actual = actual.ParameterType });
      return zipped.All (zip => zip.Expected.IsAssignableFrom (zip.Actual));
    }

    private string GetRegexPattern (string wildcardPattern)
    {
      return "^" + wildcardPattern.Replace ("*", ".*") + "$";
    }

    #endregion
  }
}