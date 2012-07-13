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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Invocations;
using JetBrains.Annotations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.Utilities;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class MethodPatcherTest : TestBase
  {
    private MethodPatcher _patcher;

    private IAspectGenerator _generator1;
    private IAspectGenerator _generator2;

    private IAspectDescriptor _descriptor1;
    private IAspectDescriptor _descriptor2;

    private IEnumerable<IAspectGenerator> _oneGenerator;
    private IEnumerable<IAspectGenerator> _twoGenerators;

    private AspectAttribute _dummyField1;
    private AspectAttribute _dummyField2;

    public MethodPatcherTest ()
    {
    }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      
      _patcher = new MethodPatcher ();

      _descriptor1 = MockRepository.GenerateMock<IAspectDescriptor> ();
      _descriptor2 = MockRepository.GenerateMock<IAspectDescriptor> ();

      _generator1 = MockRepository.GenerateMock<IAspectGenerator> ();
      _generator2 = MockRepository.GenerateMock<IAspectGenerator>();

      _generator1.Stub (x => x.Descriptor).Return (_descriptor1);
      _generator2.Stub (x => x.Descriptor).Return (_descriptor2);

      var fieldExpression1 = Expression.Field(Expressions.Constant  ())

      _generator1.Stub (x => x.GetStorageExpression (null)).IgnoreArguments().Return (Expression.Variable (typeof (AspectAttribute), "aspect1"));
      _generator2.Stub (x => x.GetStorageExpression (null)).IgnoreArguments().Return (Expression.Variable (typeof (AspectAttribute), "aspect2"));
      

      _oneGenerator = new[] { _generator1 };
      _twoGenerators = new[] { _generator1, _generator2 };
    }


    [Test]
    public void ContainsInvocationContext ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // InvocationContext<TInstance, ...> ctx;
            var invocationContext = InvocationContext<DomainType> (mutableMethod);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, invocationContext);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _oneGenerator, test);
    }

    [Test]
    public void ContainsInvocationContextCreate ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // new InvocationContext<TInstance> (this, methodInfo);
            var invocationContextCreate = InvocationContextCreate<DomainType> (mutableMethod, methodInfoField);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, invocationContextCreate);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _oneGenerator, test);
    }

    [Test]
    public void ContainsInvocationContextCreateWithArgs ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // new InvocationContext<TInstance, TA1, TA2> (this, methodInfo, arg1, arg2);
            var invocationContextCreate = InvocationContextCreate<DomainType2> (mutableMethod, methodInfoField);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, invocationContextCreate);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.MethodWithArgs ("a", 1)));
      PatchAndTest<DomainType2> (methodInfo, _oneGenerator, test);
    }

    [Test]
    public void ContainsInvocationContextAssign ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // var ctx = GetInvocationContext();
            var invocationContext = InvocationContext<DomainType> (mutableMethod);
            var invocationContextCreate = InvocationContextCreate<DomainType> (mutableMethod, methodInfoField);
            var expression = Expression.Assign (invocationContext, invocationContextCreate);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, expression);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _oneGenerator, test);
    }

    [Test]
    public void ContainsFirstInvocation ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // Invocation invocation1;
            var invocation = Invocation<DomainType2> (mutableMethod, 1);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, invocation);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.MethodWithArgs ("a", 1)));
      PatchAndTest<DomainType2> (methodInfo, _oneGenerator, test);
    }

    [Test]
    public void ContainsInnermostInvocationCreate ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // new Invocation(ctx, originalMethodDelegate)
            var invocationContext = InvocationContext<DomainType2> (mutableMethod);
            var invocationCreate = InvocationCreate<DomainType2> (mutableMethod, invocationContext, delegateField);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, invocationCreate);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.MethodWithArgs ("a", 1)));
      PatchAndTest<DomainType2> (methodInfo, _oneGenerator, test);
    }

    [Test]
    public void ContainsInnermostInvocationAssign ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // var invocation1 = GetInvocation();
            var invocationContext = InvocationContext<DomainType2> (mutableMethod);
            var invocation = Invocation<DomainType2> (mutableMethod, 1);
            var invocationCreate = InvocationCreate<DomainType2> (mutableMethod, invocationContext, delegateField);
            var expression = Expression.Assign (invocation, invocationCreate);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, expression);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.MethodWithArgs ("a", 1)));
      PatchAndTest<DomainType2> (methodInfo, _oneGenerator, test);
    }

    [Test]
    public void ContainsOuterInvocation ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // OuterInvocation invocation2;
            var invocation = Invocation<DomainType2> (mutableMethod, 2);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, invocation);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.MethodWithArgs ("a", 1)));
      PatchAndTest<DomainType2> (methodInfo, _twoGenerators, test);
    }



    public class DomainType
    {
      public MethodInfo MethodInfo;
      public Action Delegate;
      public virtual void Method () { }
    }

    public class DomainType2
    {
      public MethodInfo MethodInfo;
      public Action<string, int> Delegate;
      public virtual void MethodWithArgs (string a, int b) { }
    }






    private Expression InvocationCreate<T> (MutableMethodInfo mutableMethod, Expression invocationContext, FieldInfo delegateField)
    {
      var invocationType = new TypeProvider (mutableMethod).GetInvocationType ();
      var invocationCreate = Expression.New (
          invocationType.GetConstructors().Single(),
          invocationContext,
          Expression.Field (new ThisExpression (typeof (T)), delegateField));
      return invocationCreate;
    }

    private Expression InvocationContext<T> (MutableMethodInfo mutableMethod)
    {
      var invocationContextType = new TypeProvider (mutableMethod).GetInvocationContextType();
      return Expression.Variable (invocationContextType, "ctx");
    }

    private NewExpression InvocationContextCreate<T> (MutableMethodInfo mutableMethod, FieldInfo methodInfoFieldInfo)
    {
      var invocationContextType = new TypeProvider(mutableMethod).GetInvocationContextType();
      var methodInfoField = GetField<T> (methodInfoFieldInfo);
      var thisExpression = new ThisExpression (typeof (T));
      var parameters = mutableMethod.GetParameters().Select (x => Expression.Parameter (x.ParameterType, x.Name)).Cast<Expression>();
      var invocationContextCreate = Expression.New (
          invocationContextType.GetConstructors().Single(),
          new[] { methodInfoField, thisExpression }.Concat (parameters));
      return invocationContextCreate;
    }

    private Expression Invocation<T> (MutableMethodInfo mutableMethod, int index)
    {
      if (index == 1)
      {
        var invocationType = new TypeProvider (mutableMethod).GetInvocationType ();
        return Expression.Variable (invocationType, "invocation" + index);
      }
      else
      {
        return Expression.Variable (typeof (OuterInvocation), "invocation" + index);
      }
    }

    private Expression GetField<T> (FieldInfo fieldInfo)
    {
      return Expression.Field (new ThisExpression (typeof (T)), fieldInfo);
    }


    private void PatchAndTest<T> (MethodInfo methodInfo, IEnumerable<IAspectGenerator> aspects, Action<MutableMethodInfo, FieldInfo, FieldInfo> test)
    {
      AssembleType<T> (
          mutableType =>
          {
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

            var methodInfoField = typeof (T).GetFields().Where (x => x.Name == "MethodInfo").Single();
            var delegateField = typeof (T).GetFields().Where (x => x.Name == "Delegate").Single();

            _patcher.AddMethodInterception (mutableMethod, methodInfoField, delegateField, aspects);

            test (mutableMethod, methodInfoField, delegateField);
          });
    }







    //private MethodPatcher _patcher;
    //private BindingFlags _bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance;
    //private FieldIntroducer.Data _fieldData;



    //[Test]
    //public void CallAspect_MethodInterception ()
    //{
    //  var aspects = new[] { new DomainAspectAttribute () };
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.Method ());
    //  var instance = CreateInstance<DomainType2> (aspects, methodInfo);

    //  instance.Method ();

    //  Assert.That (aspects[0].OnInterceptCalled, Is.True);
    //}

    //[Test]
    //public void CallAspect_PropertyInterception_Set ()
    //{
    //  var aspects = new[] { new DomainPropertyAspectAttribute () };
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.set_Property ());
    //  var instance = CreateInstance<DomainType2> (aspects, methodInfo);

    //  instance.set_Property ();

    //  Assert.That (aspects[0].OnSetInterceptCalled, Is.True);
    //  Assert.That (aspects[0].OnGetInterceptCalled, Is.False);
    //}

    //[Test]
    //public void CallAspect_PropertyInterception_Get ()
    //{
    //  var aspects = new[] { new DomainPropertyAspectAttribute () };
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.get_Property ());
    //  var instance = CreateInstance<DomainType2> (aspects, methodInfo);

    //  instance.get_Property ();

    //  Assert.That (aspects[0].OnGetInterceptCalled, Is.True);
    //  Assert.That (aspects[0].OnSetInterceptCalled, Is.False);
    //}

    //[Test]
    //public void CallAspect_WithInvocation ()
    //{
    //  var aspects = new[] { new DomainAspectAttribute () };
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.Method ());
    //  var instance = CreateInstance<DomainType2> (aspects, methodInfo);

    //  instance.Method ();

    //  Assert.That (aspects[0].Invocation, Is.Not.Null);
    //  Assert.That (aspects[0].Invocation, Is.TypeOf<ActionInvocation<DomainType2>> ());
    //}

    //[Test]
    //public void CallAspect_WithInvocation_WithContext ()
    //{
    //  var aspects = new[] { new DomainAspectAttribute () };
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.Method ());
    //  var instance = CreateInstance<DomainType2> (aspects, methodInfo);

    //  instance.Method ();

    //  var ctx = aspects[0].Invocation.Context;
    //  Assert.That (ctx, Is.Not.Null);
    //  Assert.That (ctx, Is.TypeOf<ActionInvocationContext<DomainType2>>());
    //  Assert.That (ctx.MethodInfo, Is.EqualTo (methodInfo));
    //  Assert.That (ctx.Instance, Is.EqualTo (instance));
    //}

    //[Test]
    //public void CallAspect_Proceeding ()
    //{
    //  var aspects = new[] { new ProceedingDomainAspectAttribute () };
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType3 obj) => obj.Method ());
    //  var called = false;
    //  var instance = CreateInstance<DomainType3> (aspects, methodInfo, new Action (() => { called = true; }));

    //  instance.Method ();

    //  Assert.That (called, Is.True);
    //}

    //[Test]
    //public void CallAspect_MethodWithArgs ()
    //{
    //  var aspects = new[] { new DomainAspectAttribute () };
    //  _fieldData.DelegateField = MemberInfoFromExpressionUtility.GetField (((DomainType4 obj) => obj.Delegate));
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType4 obj) => obj.Method (""));
    //  var instance = CreateInstance<DomainType4> (aspects, methodInfo);

    //  var input = "a";
    //  instance.Method (input);

    //  var arguments = aspects[0].Invocation.Context.Arguments;
    //  Assert.That (arguments, Has.Count.EqualTo (1));
    //  Assert.That (arguments[0], Is.EqualTo (input));
    //}

    //[Test]
    //public void CallAspect_ReturnValue_WithSet_ValueType ()
    //{
    //  var aspects = new[] { new ReturnValueTypeDomainAspectAttribute () };
    //  _fieldData.DelegateField = MemberInfoFromExpressionUtility.GetField (((DomainType5 obj) => obj.Delegate));
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType5 obj) => obj.Return100Method ());
    //  var instance = CreateInstance<DomainType5> (aspects, methodInfo);

    //  var result = instance.Return100Method ();

    //  Assert.That (result, Is.EqualTo (1));
    //}

    //[Test]
    //public void CallAspect_ReturnValue_WithSet_ReferenceType ()
    //{
    //  var aspects = new[] { new ReturnReferenceTypeDomainAspectAttribute () };
    //  _fieldData.DelegateField = MemberInfoFromExpressionUtility.GetField (((DomainType6 obj) => obj.Delegate));
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType6 obj) => obj.Method ());
    //  var instance = CreateInstance<DomainType6> (aspects, methodInfo);

    //  var result = instance.Method ();

    //  Assert.That (result, Is.InstanceOf<DomainType5>());
    //}

    //[Test]
    //public void CallAspect_ReturnValue_WithoutSet_ValueType ()
    //{
    //  var aspects = new[] { new DomainAspectAttribute () };
    //  _fieldData.DelegateField = MemberInfoFromExpressionUtility.GetField (((DomainType5 obj) => obj.Delegate));
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType5 obj) => obj.Return100Method ());
    //  var instance = CreateInstance<DomainType5> (aspects, methodInfo);

    //  var result = instance.Return100Method ();

    //  Assert.That (result, Is.EqualTo (0));
    //}

    //[Test]
    //public void CallAspect_ReturnValue_WithoutSet_ReferenceType ()
    //{
    //  var aspects = new[] { new DomainAspectAttribute () };
    //  _fieldData.DelegateField = MemberInfoFromExpressionUtility.GetField (((DomainType6 obj) => obj.Delegate));
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType6 obj) => obj.Method ());
    //  var instance = CreateInstance<DomainType6> (aspects, methodInfo);

    //  var result = instance.Method ();

    //  Assert.That (result, Is.EqualTo (null));
    //}

    //[Test]
    //public void CallAspect_Multiple ()
    //{
    //  var aspects = new[] { new ProceedingDomainAspectAttribute (), new ProceedingDomainAspectAttribute () };
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.Method ());
    //  var instance = CreateInstance<DomainType2> (aspects, methodInfo, new Action (() => { }));

    //  instance.Method ();

    //  Assert.That (aspects[0].OnInterceptCalled, Is.True);
    //  Assert.That (aspects[1].OnInterceptCalled, Is.True);
    //  Assert.That (aspects[1].Invocation, Is.InstanceOf<OuterInvocation> ());
    //}


    //[Test]
    //public void CallAspect_Multiple_MixedTypes ()
    //{
    //  var aspects = new AspectAttribute[] { new DomainAspectAttribute (), new DomainPropertyAspectAttribute () };
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.Method ());
    //  var instance = CreateInstance<DomainType2> (aspects, methodInfo);

    //  instance.Method ();
    //}

    //private T CreateInstance<T> (IEnumerable<AspectAttribute> aspects, MethodInfo methodInfo, Delegate @delegate = null)
    //    where T: DomainTypeBase
    //{
    //  var compileAspects = aspects.Select (x => new TypeArgsAspectDescriptor (x.GetType ())).Cast<IAspectDescriptor> ();
    //  var type = CreateType<T> (compileAspects, methodInfo);

    //  var instance = (T) Activator.CreateInstance (type);

    //  var aspectsField = _fieldData.InstanceAspectsField;
    //  aspectsField.SetValue (instance, aspects.ToArray());

    //  var methodInfoField = _fieldData.MethodInfoField;
    //  methodInfoField.SetValue (instance, methodInfo);

    //  var delegateField = _fieldData.DelegateField;
    //  //var copyMethodInfo = instance.GetType().GetMethod ("_m_" + methodInfo.Name + "_Copy", _bindingFlags);
    //  //var  = Delegate.CreateDelegate (methodInfo.GetDelegateType(), instance, copyMethodInfo);
    //  delegateField.SetValue (instance, @delegate);

    //  return instance;
    //}

    //private Type CreateType<T> (IEnumerable<IAspectDescriptor> aspects, MethodInfo methodInfo) where T: DomainTypeBase
    //{
    //  var type = AssembleType<T> (
    //      mutableType =>
    //      {
    //        var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

    //        //_patcher.AddMethodInterception(mutableMethod, )
    //        //_patcher.Patch (mutableMethod, _fieldData, aspects);
    //      });
    //  return type;
    //}

    //public class TypeArgsAspectDescriptor : IAspectDescriptor
    //{
    //  private readonly Type _type;

    //  public TypeArgsAspectDescriptor (Type type)
    //  {
    //    _type = type;
    //  }

    //  public int Priority
    //  {
    //    get { throw new NotImplementedException(); }
    //  }

    //  public AspectScope Scope
    //  {
    //    get { throw new NotImplementedException(); }
    //  }

    //  public Type AspectType
    //  {
    //    get { return _type; }
    //  }

    //  public ConstructorInfo ConstructorInfo
    //  {
    //    get { throw new NotImplementedException(); }
    //  }

    //  public IList<CustomAttributeTypedArgument> ConstructorArguments
    //  {
    //    get { throw new NotImplementedException(); }
    //  }

    //  public IList<CustomAttributeNamedArgument> NamedArguments
    //  {
    //    get { throw new NotImplementedException(); }
    //  }

    //  public bool Matches (MethodInfo method)
    //  {
    //    throw new NotImplementedException();
    //  }
    //}

    //public class DomainType2 : DomainTypeBase
    //{
    //  public virtual void Method () { }

    //  [CompilerGenerated]
    //  public virtual void set_Property () { }

    //  [CompilerGenerated]
    //  public virtual void get_Property () { }

    //  public virtual void MethodWithArgs (string arg) { }
    //}

    //public class DomainType3 : DomainTypeBase
    //{
    //  public bool MethodCalled { get; private set; }
    //  public virtual void Method () { MethodCalled = true; }
    //}

    //public class DomainType4 : DomainTypeBase
    //{
    //  public new Action<string> Delegate;

    //  public virtual void Method (string a) { }
    //}

    //public class DomainType5 : DomainTypeBase
    //{
    //  public new Func<int> Delegate;

    //  public virtual int Return100Method () { return 100; }
    //}

    //public class DomainType6 : DomainTypeBase
    //{
    //  public new Func<DomainType5> Delegate;

    //  public virtual DomainType5 Method () { return new DomainType5(); }
    //}

    //public class DomainAspectAttribute : MethodInterceptionAspectAttribute
    //{
    //  public bool OnInterceptCalled { get; private set; }

    //  public IInvocation Invocation { get; private set; }

    //  public override void OnIntercept (IInvocation invocation)
    //  {
    //    OnInterceptCalled = true;

    //    Invocation = invocation;
    //  }
    //}

    //public class DomainPropertyAspectAttribute : PropertyInterceptionAspectAttribute
    //{
    //  public bool OnGetInterceptCalled { get; private set; }
    //  public bool OnSetInterceptCalled { get; private set; }

    //  public IInvocation GetInvocation { get; private set; }
    //  public IInvocation SetInvocation { get; private set; }

    //  public override void OnInterceptGet (IInvocation invocation)
    //  {
    //    OnGetInterceptCalled = true;

    //    GetInvocation = invocation;
    //  }

    //  public override void OnInterceptSet (IInvocation invocation)
    //  {
    //    OnSetInterceptCalled = true;

    //    SetInvocation = invocation;
    //  }
    //}

    //public class ProceedingDomainAspectAttribute : DomainAspectAttribute
    //{
    //  public override void OnIntercept (IInvocation invocation)
    //  {
    //    base.OnIntercept (invocation);
    //    invocation.Proceed();
    //  }
    //}

    //public class ReturnValueTypeDomainAspectAttribute : MethodInterceptionAspectAttribute
    //{
    //  public override void OnIntercept (IInvocation invocation)
    //  {
    //    invocation.Context.ReturnValue = 1;
    //  }
    //}

    //public class ReturnReferenceTypeDomainAspectAttribute : MethodInterceptionAspectAttribute
    //{
    //  public override void OnIntercept (IInvocation invocation)
    //  {
    //    invocation.Context.ReturnValue = new DomainType5();
    //  }
    //}
  }
}