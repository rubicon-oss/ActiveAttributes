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
// ReSharper disable RedundantUsingDirective
using Remotion;
// ReSharper restore RedundantUsingDirective

namespace ActiveAttributes.Weaving.Context
{
  // @begin-template first=0 template=0 generate=0..8 suppressTemplate=true
  // @replace ", TA<n> arg<n>"
  // @replace ", TA<n>"
  // @replace "TA<n>" ", " "<" ">"
  // @replace "Arg<n>" ", "
  public class ActionContext<TInstance, TA0> : ActionContextBase<TInstance>
  {
    private readonly Action<TA0> _action;

    // @begin-repeat
    // @replace-one "<n>"
    public TA0 Arg0;
    // @end-repeat

    public ActionContext (MemberInfo memberInfo, TInstance instance, TA0 arg0, Action<TA0> action)
        : base (memberInfo, instance)
    {
      //ArgumentUtility.CheckNotNull ("action", action);

      _action = action;
      // @begin-repeat
      // @replace-one "<n>"
      Arg0 = arg0;
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
        switch (idx)
        {
          // @begin-repeat
          // @replace-one "<n>"
          case 0: return Arg0;
          // @end-repeat
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          // @begin-repeat
          // @replace-one "<n>"
          case 0: Arg0 = (TA0) value; break;
          // @end-repeat
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }

    public override void Proceed ()
    {
      _action (Arg0);
    }
  }
  // @end-template
}