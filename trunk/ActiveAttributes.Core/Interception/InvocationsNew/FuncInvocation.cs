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
using Remotion.Utilities;

namespace ActiveAttributes.Core.Interception.InvocationsNew
{
  public class FuncInvocation<TA1, TReturn> : FuncInvocationBase<TReturn>
  {
    private readonly Func<TA1, TReturn> _func;

    private TA1 Arg1;

    public FuncInvocation (MemberInfo memberInfo, object instance, TA1 arg1, Func<TA1, TReturn> func)
        : base (memberInfo, instance)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      _func = func;
      Arg1 = arg1;
    }

    public override int Count
    {
      get { return 1; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx + 1)
        {
          case 1:
            return Arg1;
          default:
            throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx + 1)
        {
          case 1:
            Arg1 = (TA1) value;
            break;
          default:
            throw new IndexOutOfRangeException ("idx");
        }
      }
    }

    public override void Proceed ()
    {
      ReturnValue = _func (Arg1);
    }
  }
}