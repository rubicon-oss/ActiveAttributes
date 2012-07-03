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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class MethodPatcherTest : TestBase
  {
    private MethodPatcher _patcher;

    private BindingFlags _instanceBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic;

    [SetUp]
    public void SetUp ()
    {
      _patcher = new MethodPatcher();
    }

    [Test]
    public void CopyMethod ()
    {
      var fieldInfo = typeof (DomainType1).GetField ("_m_aspects_for_Method");
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType1 obj) => obj.Method ());
      var aspectAttributes = new AspectAttribute[] { new DomainAspectAttribute () };
      var type = PatchMethodWithAspects<DomainType1> (methodInfo, fieldInfo, aspectAttributes);
      var instance = (DomainType1) Activator.CreateInstance (type);

      var copiedMethodInfo = type.GetMethod ("_m_Method", _instanceBindingFlags);

      Assert.That (copiedMethodInfo, Is.Not.Null);
      copiedMethodInfo.Invoke (instance, new object[0]);

      Assert.That (instance.MethodCalled, Is.True);
    }

    [Test]
    public void InvokesAspects_SimpleMethod ()
    {
      var fieldInfo = typeof (DomainType1).GetField ("_m_aspects_for_Method");
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType1 obj) => obj.Method ());
      var aspectAttributes = new AspectAttribute[] { new DomainAspectAttribute () };
      var type = PatchMethodWithAspects<DomainType1> (methodInfo, fieldInfo, aspectAttributes);
      var instance = (DomainType1) Activator.CreateInstance (type);
      var aspectsArray = (AspectAttribute[]) fieldInfo.GetValue (instance);

      instance.Method ();

      var aspectAttribute = (DomainAspectAttribute) aspectsArray[0];
      Assert.That (aspectAttribute.OnInterceptCalled, Is.True);
      Assert.That (aspectAttribute.Invocation, Is.Not.Null);
      Assert.That (aspectAttribute.Invocation.Context.Arguments.Count, Is.EqualTo (0));
    }

    [Test]
    public void InvokesAspects_MethodWithArg ()
    {
      var fieldInfo = typeof (DomainType2).GetField ("_m_aspects_for_Method");
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.Method (1));
      var aspectAttributes = new AspectAttribute[] { new DomainAspectAttribute() };
      var type = PatchMethodWithAspects<DomainType2> (methodInfo, fieldInfo, aspectAttributes);
      var instance = (DomainType2) Activator.CreateInstance (type);
      var aspectsArray = (AspectAttribute[]) fieldInfo.GetValue (instance);

      instance.Method (2);

      var aspectAttribute = (DomainAspectAttribute) aspectsArray[0];
      Assert.That (aspectAttribute.Invocation.Context, Is.TypeOf<ActionInvocationContext<DomainType2, int>> ());
      Assert.That (aspectAttribute.Invocation.Context.Arguments[0], Is.EqualTo (2));
    }

    [Test]
    public void InvokesAspects_ReturningMethod ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType3 obj) => obj.Method (1));
      var fieldInfo = typeof (DomainType3).GetField ("_m_aspects_for_Method");
      var aspectAttributes = new AspectAttribute[] { new DomainAspectAttribute() };
      var type = PatchMethodWithAspects<DomainType3> (methodInfo, fieldInfo, aspectAttributes);
      var instance = (DomainType3) Activator.CreateInstance (type);
      var aspectsArray = (AspectAttribute[]) fieldInfo.GetValue (instance);

      var result = instance.Method (2);

      var aspectAttribute = (DomainAspectAttribute) aspectsArray[0];
      Assert.That (aspectAttribute.Invocation.Context, Is.TypeOf<FuncInvocationContext<DomainType3, int, int>>());
      Assert.That (aspectAttribute.Invocation.Context.ReturnValue, Is.EqualTo (result));
    }

    [Test]
    public void InvokeAspects_MultipleAspects ()
    {
      SkipDeletion();
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType4 obj) => obj.Method (1));
      var fieldInfo = typeof (DomainType4).GetField ("_m_aspects_for_Method");
      var aspectAttributes = new AspectAttribute[] { new DomainAspect2Attribute (), new DomainAspect2Attribute() };
      var type = PatchMethodWithAspects<DomainType4> (methodInfo, fieldInfo, aspectAttributes);
      var instance = (DomainType4) Activator.CreateInstance (type);

      var result = instance.Method (0);

      Assert.That (result, Is.EqualTo (2));
    }

    private Type PatchMethodWithAspects<T> (MethodInfo methodInfo, FieldInfo fieldInfo, AspectAttribute[] aspectAttributes)
    {
      return AssembleType<T> (
          mutableType =>
          {
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);
            _patcher.PatchMethod (mutableType, mutableMethod, fieldInfo, aspectAttributes);
          });
    }
  }

 

  public class DomainAspectAttribute : MethodInterceptionAspectAttribute
  {
    public bool OnInterceptCalled { get; private set; }
    public IInvocation Invocation { get; private set; }

    public override void OnIntercept (IInvocation invocation)
    {
      OnInterceptCalled = true;
      Invocation = invocation;
      invocation.Proceed();
    }
  }

  public class DomainType1
  {
    public AspectAttribute[] _m_aspects_for_Method = new[] { new DomainAspectAttribute () };

    public bool MethodCalled { get; private set; }

    public virtual void Method ()
    {
      MethodCalled = true;
    }
  }

  public class DomainType2
  {
    public AspectAttribute[] _m_aspects_for_Method = new[] { new DomainAspectAttribute () };

    public virtual void Method (int i)
    {
    }
  }

  public class DomainType3
  {
    public AspectAttribute[] _m_aspects_for_Method = new[] { new DomainAspectAttribute () };

    public virtual int Method (int i)
    {
      return i + 1;
    }
  }

  public class DomainAspect2Attribute : MethodInterceptionAspectAttribute
  {
    public override void OnIntercept (IInvocation invocation)
    {
      var context = (FuncInvocationContext<DomainType4, int, int>) invocation.Context;
      context.Arg0++;
      invocation.Proceed();
    }
  }

  public class DomainType4
  {
    public AspectAttribute[] _m_aspects_for_Method = new[] { new DomainAspect2Attribute (), new DomainAspect2Attribute () };

    public virtual int Method (int i)
    {
      return i;
    }
  }
}