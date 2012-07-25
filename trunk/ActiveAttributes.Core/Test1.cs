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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.IntegrationTests
{

  public class DomainAspectAttribute : PropertyInterceptionAspectAttribute
  {
    public override void OnInterceptGet (IInvocation invocation)
    {
    }

    public override void OnInterceptSet (IInvocation invocation)
    {
    }
  }

  public class DomainClass : IDisposable
  {
    private const int A = 1;

    private static int IntegerStatic;

    #region Nested type: NestedClass

    private class NestedClass
    {
    }

    #endregion

    #region Delegates

    public delegate string StringEventHandler ();

    #endregion

    #region DomainEnum enum

    public enum DomainEnum
    {


    }

    #endregion


    public static string GetString ()
    {
      return string.Empty;
    }

    public event EventHandler DomainEvent;

    private readonly int Integer;

    public DomainClass (string arg)
    {
    }

    protected DomainClass ()
    {
      Integer = 1;
    }

    public static int IntegerPropStatic { get; set; }

    ~DomainClass ()
    {
    }

    public void Dispose ()
    {
      throw new NotImplementedException();
    }

    public event StringEventHandler MyEvent;

    public int MethodB ()
    {
      return 1;
    }

    public int ZZMethod ()
    {
      return 1;
    }

    private void Method ()
    {
    }
  }
}