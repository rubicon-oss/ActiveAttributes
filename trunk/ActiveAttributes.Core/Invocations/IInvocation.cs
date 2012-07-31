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
using System.Reflection;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  /// <summary>
  ///   Provides facilities to change the behavior of an intercepted action.
  /// </summary>
  public interface IInvocation
  {
    /// <summary>
    ///   The context in which the particular action was intercepted.
    /// </summary>
    IInvocationContext Context { get; }

    /// <summary>
    ///   Proceed with the intercepted action.
    /// </summary>
    void Proceed ();
  }

  //public interface IEventInvocation : IInvocation
  //{
  //  new IEventInvocationContext Context { get; }
  //}

  //public interface IEventInvocationContext : IInvocationContext
  //{
  //  Delegate Event { get; }
  //}

  //public interface IPropertyInvocation : IInvocation
  //{
  //  new IPropertyInvocationContext Context { get; }
  //}

  //public interface IPropertyInvocationContext : IInvocationContext
  //{
  //  object Value { get; }
  //  object Index { get; }
  //}

  //public class PropertyInvocation : IPropertyInvocation
  //{
  //  private readonly IInvocation _invocation;

  //  public PropertyInvocation (IInvocation invocation)
  //  {
  //    _invocation = invocation;
  //  }

  //  IInvocationContext IInvocation.Context
  //  {
  //    get { return Context; }
  //  }

  //  public IPropertyInvocationContext Context
  //  {
  //    get { }
  //  }

  //  public void Proceed ()
  //  {
  //    _invocation.Proceed();
  //  }
  //}

  //public class PropertyInvocationContext : IPropertyInvocationContext
  //{
  //  private readonly IInvocationContext _invocationContext;

  //  public PropertyInvocationContext (IInvocationContext invocationContext)
  //  {
  //    _invocationContext = invocationContext;
  //  }

  //  public object Value
  //  {
  //    // TODO: this will fail because getter of index has one argument.
  //    get { return Arguments[Arguments.Count == 1 ? 0 : 1]; }
  //    set { Arguments[Arguments.Count == 1 ? 0 : 1] = value; }
  //  }

  //  public object Index
  //  {
  //    get { throw new NotImplementedException(); }
  //  }

  //  public MethodInfo MethodInfo
  //  {
  //    get { return _invocationContext.MethodInfo; }
  //  }

  //  public object Instance
  //  {
  //    get { return _invocationContext.Instance; }
  //  }

  //  public IArgumentCollection Arguments
  //  {
  //    get { return _invocationContext.Arguments; }
  //  }

  //  public object ReturnValue
  //  {
  //    get { return _invocationContext.ReturnValue; }
  //    set { _invocationContext.ReturnValue = value; }
  //  }
  //}


}