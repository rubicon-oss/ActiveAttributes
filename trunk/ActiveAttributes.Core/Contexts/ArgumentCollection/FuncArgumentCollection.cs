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
using System.Collections;
using System.Collections.Generic;

namespace ActiveAttributes.Core.Contexts.ArgumentCollection
{
  public class FuncArgumentCollection<TInstance, TA0, TR> : IArgumentCollection, IReadOnlyArgumentCollection
  {
    private readonly FuncInvocationContext<TInstance, TA0, TR> _invocationContext;

    public FuncArgumentCollection (FuncInvocationContext<TInstance, TA0, TR> invocationContext)
    {
      _invocationContext = invocationContext;
    }

    public object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return _invocationContext.Arg0;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: _invocationContext.Arg0 = (TA0) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }

    public IEnumerator<object> GetEnumerator ()
    {
      yield return _invocationContext.Arg0;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void CopyTo (Array array, int index)
    {
      throw new NotImplementedException();
    }

    public int Count
    {
      get { return 1; }
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }
  }
}