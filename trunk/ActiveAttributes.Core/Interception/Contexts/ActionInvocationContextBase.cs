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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Logging;

namespace ActiveAttributes.Core.Interception.Contexts
{
  public abstract class InvocationContextBase
  {
    protected static readonly ILog Log = LogManager.GetLogger (typeof (IInvocationContext));
  }

  public abstract class ActionInvocationContextBase<TInstance> : InvocationContextBase, IInvocationContext, IReadOnlyInvocationContext, IArgumentCollection, IReadOnlyArgumentCollection
  {
    protected ActionInvocationContextBase (MethodInfo methodInfo, TInstance instance)
    {
      MethodInfo = methodInfo;
      Instance = instance;
    }

    public virtual MethodInfo MethodInfo { get; private set; }
    public TInstance Instance { get; private set; }

    public object ReturnValue { get; set; }

    public abstract int Count { get; }
    public abstract object this [int idx] { get; set; }

    object IInvocationContext.Instance
    {
      get { return Instance; }
    }

    object IReadOnlyInvocationContext.Instance
    {
      get { return Instance; }
    }

    IArgumentCollection IInvocationContext.Arguments
    {
      get { return this; }
    }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return this; }
    }

    public IEnumerator<object> GetEnumerator ()
    {
      for (var i = 0; i < Count; i++)
        yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void CopyTo (Array array, int index)
    {
      for (var i = 0; i < Count; i++)
        array.SetValue (this[i], i);
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }

    object IReadOnlyArgumentCollection.this [int idx]
    {
      get { return this[idx]; }
    }
  }
}