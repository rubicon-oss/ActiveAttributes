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
using System.Reflection;
using ActiveAttributes.Core.Configuration;

namespace ActiveAttributes.Core.Assembly.CompileTimeAspects
{
  public class TypeArgsCompileTimeAspect : CompileTimeAspectBase
  {
    private readonly Type _aspectType;
    private readonly object[] _arguments;

    public TypeArgsCompileTimeAspect (Type aspectType, object[] arguments)
    {
      _aspectType = aspectType;
      _arguments = arguments;
    }

    public override CompileTimeAspectType CompileTimeType
    {
      get { return CompileTimeAspectType.TypeArgsCompileTimeAspect; }
    }

    public override int Priority
    {
      get { throw new NotSupportedException(); }
    }

    public override AspectScope Scope
    {
      get { throw new NotImplementedException(); }
    }

    public override Type AspectType
    {
      get { return _aspectType; }
    }

    public override ConstructorInfo ConstructorInfo
    {
      get { throw new NotSupportedException (); }
    }

    public override IList<CustomAttributeTypedArgument> ConstructorArguments
    {
      get { throw new NotSupportedException (); }
    }

    public override IList<CustomAttributeNamedArgument> NamedArguments
    {
      get { throw new NotSupportedException (); }
    }

    public override object[] Arguments
    {
      get { return _arguments; }
    }

    public override object IfType
    {
      get { throw new NotImplementedException(); }
    }

    public override object IfSignature
    {
      get { throw new NotImplementedException(); }
    }

  }
}