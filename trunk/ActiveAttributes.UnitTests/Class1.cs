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
using System.Linq;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Configuration2.AspectDescriptorProviders;
using ActiveAttributes.Core.Invocations;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.TypePipe.UnitTests.MutableReflection;

namespace ActiveAttributes.UnitTests
{

  [TestFixture]
  public class Class1
  {

    //private string Method (string arg)
    //{
    //  arg = 
    //  return arg;
    //}

    // [After(
    private string Aspect (string arg)
    {
      arg += "aspect" + arg;
      return arg;
    }

    [Test]
    public void name123 ()
    {

    }

    [Test]
    [Ignore]
    public void name2 ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType();
      var method2 = mutableType.GetMethod ("ToString");
      var method3 = typeof (object).GetMethod ("ToString");

      //Assert.That (method2, Is.EqualTo (method3));
      //var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((object obj) => obj.ToString());
      var inst = new MethodBasedAspectDescriptorProvider();

      inst.GetDescriptors (method2).ToArray();
    }

    public event EventHandler Event;

    [Test]
    [Ignore]
    public void name ()
    {
      var handler = new EventHandler (Method);
      AddHandler2 (handler);
    }

    private void Method (object sender, EventArgs eventArgs) {}

    private void AddHandler2 (EventHandler handler)
    {
      var typeArgs = handler.Method.GetParameters().Select (x => x.ParameterType)
          .Concat (new[] { typeof (object[]) })
          .ToArray();
      var delegateType = Expression.GetDelegateType (typeArgs);
      var parameters = handler.Method.GetParameters().Select (x => Expression.Parameter (x.ParameterType, x.Name)).ToArray();
      var lambda = Expression.Lambda (
          delegateType,
          Expression.NewArrayInit (typeof (object), parameters.Cast<Expression>()),
          false,
          parameters);
      var del = lambda.Compile();

      var action = new Action<object[]> (
          (args) => handler.DynamicInvoke (args));
    }

    private object Trigger (object[] args)
    {
      return _event.DynamicInvoke (args);
    }

    private void AddHandler (EventHandler handler)
    {
      _event = (EventHandler) Delegate.Combine (_event, handler);
    }

    private EventHandler _event;
  }
}