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
using System.Collections.ObjectModel;
using System.Reflection;
using ActiveAttributes.Core.Contexts.ArgumentCollection;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Contexts
{
  public class ActionInvocationContext<TInstance> : IInvocationContext, IReadOnlyInvocationContext
  {
    private EmptyArgumentCollection _argumentCollection;

    public ActionInvocationContext (MethodInfo methodInfo, TInstance instance)
    {
      MethodInfo = methodInfo;
      Instance = instance;
    }

    public MethodInfo MethodInfo { get; private set; }

    public TInstance Instance { get; private set; }

    object IReadOnlyInvocationContext.Instance
    {
      get { return Instance; }
    }

    object IInvocationContext.Instance
    {
      get { return Instance; }
    }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new EmptyArgumentCollection()); }
    }

    IArgumentCollection IInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new EmptyArgumentCollection()); }
    }

    public object ReturnValue { get; set; }
  }

  public class ActionInvocationContext<TInstance, TA0> : IInvocationContext, IReadOnlyInvocationContext
  {
    private ActionArgumentCollection<TInstance, TA0> _argumentCollection;

    public ActionInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0)
    {
      MethodInfo = methodInfo;
      Instance = instance;
      Arg0 = arg0;
    }

    public MethodInfo MethodInfo { get; private set; }

    public TInstance Instance { get; private set; }

    object IReadOnlyInvocationContext.Instance
    {
      get { return Instance; }
    }

    object IInvocationContext.Instance
    {
      get { return Instance; }
    }

    public TA0 Arg0 { get; set; }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new ActionArgumentCollection<TInstance, TA0> (this)); }
    }

    IArgumentCollection IInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new ActionArgumentCollection<TInstance, TA0> (this)); }
    }

    public object ReturnValue { get; set; }
  }

  public class ActionInvocationContext<TInstance, TA0, TA1> : IInvocationContext, IReadOnlyInvocationContext
  {
    private ActionArgumentCollection<TInstance, TA0, TA1> _argumentCollection;

    public ActionInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1)
    {
      MethodInfo = methodInfo;
      Instance = instance;
      Arg0 = arg0;
      Arg1 = arg1;
    }

    public MethodInfo MethodInfo { get; private set; }

    public TInstance Instance { get; private set; }

    object IReadOnlyInvocationContext.Instance
    {
      get { return Instance; }
    }

    object IInvocationContext.Instance
    {
      get { return Instance; }
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new ActionArgumentCollection<TInstance, TA0, TA1> (this)); }
    }

    IArgumentCollection IInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new ActionArgumentCollection<TInstance, TA0, TA1> (this)); }
    }

    public object ReturnValue { get; set; }
  }
}