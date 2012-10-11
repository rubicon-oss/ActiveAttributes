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
using System.Reflection;
using ActiveAttributes.Core.Contexts;
using XValidation;
using System.Linq;

namespace ActiveAttributes.Common
{
  /// <summary>
  /// Wraps an <see cref="IInvocationContext"/> into an <see cref="IValidationContext"/>.
  /// </summary>
  public class ValidationContext : IValidationContext
  {
    private readonly IInvocationContext _invocationContext;

    public ValidationContext (IInvocationContext invocationContext)
    {
      _invocationContext = invocationContext;
    }

    public MethodBase Method
    {
      get { return _invocationContext.MethodInfo; }
    }

    public object Instance
    {
      get { return _invocationContext.Instance; }
    }

    public object[] Arguments
    {
      get { return _invocationContext.Arguments.Select(x => x).ToArray(); }
    }

    public object ReturnValue
    {
      get { return _invocationContext.ReturnValue; }
    }
  }
}