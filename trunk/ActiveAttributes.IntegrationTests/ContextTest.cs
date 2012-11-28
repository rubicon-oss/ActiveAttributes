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
using ActiveAttributes.Aspects;
using ActiveAttributes.Assembly;
using ActiveAttributes.Extensions;
using ActiveAttributes.Interception.Invocations;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ContextTest
  {
    private DomainType _instance;

    [SetUp]
    public void SetUp ()
    {
      _instance = ObjectFactory.Create<DomainType> ();
    }

    [Test]
    public void InstanceAndMember ()
    {
      _instance.ContextMethod();

      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.ContextMethod());
      Assert.That (ContextAspectAttribute.Instance, Is.SameAs (_instance));
      Assert.That (ContextAspectAttribute.Member.Name, Is.EqualTo (method.Name));
    }

    [Test]
    public void Argument ()
    {
      var result = _instance.ArgumentMethod (10);

      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void ReturnValue ()
    {
      var result = _instance.ReturnValueMethod();

      Assert.That (result, Is.EqualTo ("advice"));
    }

    public class DomainType
    {
      [ContextAspect]
      public virtual void ContextMethod () {}

      [ArgumentAspect]
      public virtual int ArgumentMethod (int arg) { return arg; }

      [ReturnValueAspect]
      public virtual string ReturnValueMethod () { return "test"; }
    }

    public class ContextAspectAttribute : MethodInterceptionAspectAttributeBase
    {
      public static object Instance { get; set; }
      public static MemberInfo Member { get; set; }

      public override void OnIntercept (IInvocation invocation)
      {
        Instance = invocation.Instance;
        Member = invocation.MemberInfo;
      }
    }

    public class ArgumentAspectAttribute : MethodInterceptionAspectAttributeBase
    {
      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Arguments[0] = 1;
        invocation.Proceed();
      }
    }

    public class ReturnValueAspectAttribute : MethodInterceptionAspectAttributeBase
    {
      public override void OnIntercept (IInvocation invocation)
      {
        invocation.ReturnValue = "advice";
      }
    }
  }
}