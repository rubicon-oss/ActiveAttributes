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
// ReSharper disable RedundantUsingDirective
using Remotion;
// ReSharper restore RedundantUsingDirective

namespace ActiveAttributes.Core.Interception.Invocations
{
  // @begin-template first=1 template=1 generate=0..8 suppressTemplate=true
  // @replace ", TA<n> arg<n>"
  // @replace "TA<n>" ", " "<" ">"
  // @replace "TA<n>, "
  // @replace "Arg<n>" ", "
  public class ActionInvocation<TA1> : ActionInvocationBase
  {
    private readonly Action<TA1> _action;

    // @begin-repeat
    // @replace-one "<n>"
    public TA1 Arg1;
    // @end-repeat

    public ActionInvocation (MemberInfo memberInfo, object instance, TA1 arg1, Action<TA1> action)
        : base (memberInfo, instance)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      _action = action;
      // @begin-repeat
      // @replace-one "<n>"
      Arg1 = arg1;
      // @end-repeat
    }

    public override int Count
    {
      // @replace-one "<n>"
      get { return 1; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx + 1)
        {
          // @begin-repeat
          // @replace-one "<n>"
          case 1: return Arg1;
          // @end-repeat
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx + 1)
        {
          // @begin-repeat
          // @replace-one "<n>"
          case 1: Arg1 = (TA1) value; break;
          // @end-repeat
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }

    public override void Proceed ()
    {
      _action (Arg1);
    }
  }
  // @end-template
}