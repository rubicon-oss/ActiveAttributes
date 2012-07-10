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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Extensions;

namespace ActiveAttributes.Core.Assembly.CompileTimeAspects
{
  public interface ICompileTimeAspect
  {
    int Priority { get; }
    AspectScope Scope { get; }
    Type AspectType { get; }
    ConstructorInfo ConstructorInfo { get; }
    IList<CustomAttributeTypedArgument> ConstructorArguments { get; }
    IList<CustomAttributeNamedArgument> NamedArguments { get; }
    object[] Arguments { get; }
    object IfType { get; }
    object IfSignature { get; }
  }

  public class CompileTimeAspect : ICompileTimeAspect
  {
    private readonly AspectAttribute _attribute;
    private readonly CustomAttributeData _customData;

    public CompileTimeAspect (CustomAttributeData customData)
    {
      if (!typeof (AspectAttribute).IsAssignableFrom (customData.Constructor.DeclaringType))
        throw new ArgumentException ("CustomAttributeData must be from an AspectAttribute");

      _customData = customData;
      _attribute = (AspectAttribute) customData.CreateAttribute();
    }

    public int Priority
    {
      get { return _attribute.Priority; }
    }

    public AspectScope Scope
    {
      get { return _attribute.Scope; }
    }

    public Type AspectType
    {
      get { return _attribute.GetType(); }
    }

    public ConstructorInfo ConstructorInfo
    {
      get { return _customData.Constructor; }
    }
    public IList<CustomAttributeTypedArgument> ConstructorArguments
    {
      get { return _customData.ConstructorArguments; }
    }
    public IList<CustomAttributeNamedArgument> NamedArguments
    {
      get { return _customData.NamedArguments; }
    }

    public object[] Arguments
    {
      get { throw new NotSupportedException(); }
    }

    public object IfType
    {
      get { return _attribute.IfType; }
    }

    public object IfSignature
    {
      get { return _attribute.IfSignature; }
    }
  }
}