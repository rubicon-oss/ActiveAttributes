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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Invocations;
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

    private MethodInfo _createDelegate;
    private MethodInfo _onInterceptMethod;
    private MethodInfo _onInterceptGetMethod;
    private MethodInfo _onInterceptSetMethod;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _patcher = new MethodPatcher();

      _descriptor1 = MockRepository.GenerateMock<IAspectDescriptor> ();
      _descriptor2 = MockRepository.GenerateMock<IAspectDescriptor> ();

      _descriptor1.Stub (x => x.AspectType).Return (typeof (MethodInterceptionAspectAttribute));
      _descriptor2.Stub (x => x.AspectType).Return (typeof (MethodInterceptionAspectAttribute));

      _generator1 = MockRepository.GenerateMock<IAspectGenerator> ();
      _generator2 = MockRepository.GenerateMock<IAspectGenerator>();

      _generator1.Stub (x => x.Descriptor).Return (_descriptor1);
      _generator2.Stub (x => x.Descriptor).Return (_descriptor2);

      var fieldInfo1 = MemberInfoFromExpressionUtility.GetField ((DomainTypeBase obj) => obj.AspectField1);
      var fieldInfo2 = MemberInfoFromExpressionUtility.GetField ((DomainTypeBase obj) => obj.AspectField2);

      var fieldExpression1 = Expression.Field (new ThisExpression (typeof (DomainTypeBase)), fieldInfo1);
      var fieldExpression2 = Expression.Field (new ThisExpression (typeof (DomainTypeBase)), fieldInfo2);

      _generator1.Stub (x => x.GetStorageExpression (null)).IgnoreArguments().Return (fieldExpression1);
      _generator2.Stub (x => x.GetStorageExpression (null)).IgnoreArguments().Return (fieldExpression2);
      
      _oneGenerator = new[] { _generator1 };
      _twoGenerators = new[] { _generator1, _generator2 };

      _createDelegate = typeof (Delegate).GetMethod ("CreateDelegate", new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
      _onInterceptMethod = typeof (MethodInterceptionAspectAttribute).GetMethods().Where (x => x.Name == "OnIntercept").Single();
      _onInterceptGetMethod = typeof (PropertyInterceptionAspectAttribute).GetMethods().Where (x => x.Name == "OnInterceptGet").Single();
      _onInterceptSetMethod = typeof (PropertyInterceptionAspectAttribute).GetMethods().Where (x => x.Name == "OnInterceptSet").Single();
    }

    public class DomainTypeBase
    {
      public AspectAttribute AspectField1;
      public AspectAttribute AspectField2;
    }

    public class DomainType : DomainTypeBase
    {
      public MethodInfo MethodInfo;
      public Action Delegate;
      public virtual void Method () { }
    }

    [Test]
    public void ContainsInvocationContext ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // InvocationContext<TInstance, ...> ctx;
            var invocationContext = InvocationContext (mutableMethod);
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
    public void ContainsInvocationContextAssign ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // var ctx = GetInvocationContext();
            var invocationContext = InvocationContext (mutableMethod);
            var invocationContextCreate = InvocationContextCreate<DomainType> (mutableMethod, methodInfoField);
            var assign = Expression.Assign (invocationContext, invocationContextCreate);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, assign);
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
            var invocation = Invocation (mutableMethod, 1);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, invocation);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _oneGenerator, test);
    }

    [Test]
    public void ContainsInnermostInvocationCreate ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // new Invocation(ctx, originalMethodDelegate)
            var invocationContext = InvocationContext (mutableMethod);
            var invocationCreate = InnerInvocationCreate<DomainType> (mutableMethod, invocationContext, delegateField);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, invocationCreate);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _oneGenerator, test);
    }

    [Test]
    public void ContainsInnermostInvocationAssign ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // var invocation1 = GetInnermostInvocation();
            var invocationContext = InvocationContext (mutableMethod);
            var invocation = Invocation (mutableMethod, 1);
            var invocationCreate = InnerInvocationCreate<DomainType> (mutableMethod, invocationContext, delegateField);
            var expression = Expression.Assign (invocation, invocationCreate);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, expression);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _oneGenerator, test);
    }

    [Test]
    public void ContainsOuterInvocation ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // OuterInvocation invocation2;
            var invocation = Invocation (mutableMethod, 2);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, invocation);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _twoGenerators, test);
    }

    [Test]
    public void ContainsOuterInvocationCreate ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // new OuterInvocation(ctx, innerInvocationDelegate, innerInvocation);
            var invocationContext = InvocationContext (mutableMethod);
            var innerInvocation = Invocation (mutableMethod, 1);
            var outerInvocationCreate = OuterInvocationCreate<DomainType> (invocationContext, innerInvocation);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, outerInvocationCreate);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _twoGenerators, test);
    }

    [Test]
    public void ContainsOuterInvocationAssign ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // new OuterInvocation(ctx, innerInvocationDelegate, innerInvocation);
            var invocationContext = InvocationContext (mutableMethod);
            var innerInvocation = Invocation (mutableMethod, 1);
            var outerInvocation = Invocation (mutableMethod, 2);
            var outerInvocationCreate = OuterInvocationCreate<DomainType> (invocationContext, innerInvocation);
            var assign = Expression.Assign (outerInvocation, outerInvocationCreate);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, assign);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _twoGenerators, test);
    }

    [Test]
    public void ContainsOutermostAspectCall ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // outermostAspect.Intercept(outermostInvocation);
            var outerInvocation = Invocation (mutableMethod, 2);
            var outerAspectCall = OutermostAspectCall<DomainType> (_generator2, outerInvocation);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, outerAspectCall);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _twoGenerators, test);
    }

    [Test]
    public void ContainsPropertyAsReturnValue ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            var invocatonContext = InvocationContext (mutableMethod);
            var propertyAsReturnValue = PropertyAsReturnValue (invocatonContext);
            ExpressionTreeComparer2.CheckTreeContains (mutableMethod.Body, propertyAsReturnValue);
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      PatchAndTest<DomainType> (methodInfo, _twoGenerators, test);
    }



    public class DomainType2 : DomainTypeBase
    {
      public MethodInfo MethodInfo;
      public Action<string, int> Delegate;
      public virtual void MethodWithArgs (string a, int b) { }
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
    public void FullTreeTest ()
    {
      Action<MutableMethodInfo, FieldInfo, FieldInfo> test =
          (mutableMethod, methodInfoField, delegateField) =>
          {
            // ActionInvocationContext ctx = new ActionInvocationContext<TInstance, TA1, TA2> (this, methodInfo);
            var invocationContext = InvocationContext (mutableMethod);
            var invocationContextCreate = InvocationContextCreate<DomainType2> (mutableMethod, methodInfoField);
            var invocationContextAssign = Expression.Assign (invocationContext, invocationContextCreate);

            // ActionInvocation<TInstance, TA1, TA2> invocation1 = new ActionInvocation<TInstance, TA1, TA2> (ctx, methodDelegate);
            var innermostInvocation = Invocation (mutableMethod, 1);
            var innermostInvocationCreate = InnerInvocationCreate<DomainType2> (mutableMethod, invocationContext, delegateField);
            var innermostInvocationAssign = Expression.Assign (innermostInvocation, innermostInvocationCreate);

            // OuterInvocation invocation2 = new OuterInvocation(ctx, invocation1, aspect1);
            var outermostInvocation = Invocation (mutableMethod, 2);
            var outermostInvocationCreate = OuterInvocationCreate<DomainType2> (invocationContext, innermostInvocation);
            var outermostInvocationAssign = Expression.Assign (outermostInvocation, outermostInvocationCreate);

            // aspect2.OnIntercept
            var outermostAspectCall = OutermostAspectCall<DomainType2> (_generator2, outermostInvocation);

            // return ctx.ReturnValue;
            var propertyAsReturnValue = PropertyAsReturnValue (invocationContext);

            var block = Expression.Block (
                new[] { invocationContext, innermostInvocation, outermostInvocation }.Cast<ParameterExpression>(),
                invocationContextAssign,
                Expression.Block (
                    innermostInvocationAssign,
                    outermostInvocationAssign),
                outermostAspectCall,
                propertyAsReturnValue);

            ExpressionTreeComparer.CheckAreEqualTrees (mutableMethod.Body, Expression.Block(typeof(void), block));
          };

      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.MethodWithArgs ("a", 1)));
      PatchAndTest<DomainType2> (methodInfo, _twoGenerators, test);
    }





    private Expression OutermostAspectCall<T> (IAspectGenerator generator, Expression invocation)
    {
      var convertedAspect = Expression.Convert (generator.GetStorageExpression (ThisExpression<T>()), generator.Descriptor.AspectType);
      var call = Expression.Call (convertedAspect, _onInterceptMethod, new[] { invocation });
      return call;
    }

    private Expression ThisExpression<T> ()
    {
      return new ThisExpression (typeof (T));
    }

    private Expression OuterInvocationCreate<T> (Expression ctx, Expression innerInvocation)
    {
      var constructor = typeof (OuterInvocation).GetConstructors ().Single ();
      var innerInvocationDelegateType = Expression.Constant (typeof (Action<IInvocation>));
      var innerInvocationMethod = Expression.Constant (_generator2.Descriptor.AspectType.GetMethods ().Where (x => x.Name.EndsWith ("Intercept")).Single ());
      var innerInvocationDelegate = Expression.Call (
          null, _createDelegate, innerInvocationDelegateType, _generator1.GetStorageExpression (ThisExpression<T> ()), innerInvocationMethod);
      var create = Expression.New (constructor, ctx, Expression.Convert (innerInvocationDelegate, typeof (Action<IInvocation>)), innerInvocation);

      return create;
    }

    private Expression InnerInvocationCreate<T> (MutableMethodInfo mutableMethod, Expression invocationContext, FieldInfo delegateField)
    {
      var invocationType = new TypeProvider (mutableMethod).GetInvocationType ();
      var invocationCreate = Expression.New (
          invocationType.GetConstructors().Single(),
          invocationContext,
          Expression.Field (ThisExpression<T>(), delegateField));
      return invocationCreate;
    }

    private Expression InvocationContext (MutableMethodInfo mutableMethod)
    {
      var invocationContextType = new TypeProvider (mutableMethod).GetInvocationContextType();
      return Expression.Variable (invocationContextType, "ctx");
    }

    private NewExpression InvocationContextCreate<T> (MutableMethodInfo mutableMethod, FieldInfo methodInfoFieldInfo)
    {
      var invocationContextType = new TypeProvider(mutableMethod).GetInvocationContextType();
      var methodInfoField = GetField<T> (methodInfoFieldInfo);
      var thisExpression = ThisExpression<T>();
      var parameters = mutableMethod.GetParameters().Select (x => Expression.Parameter (x.ParameterType, x.Name)).Cast<Expression>();
      var invocationContextCreate = Expression.New (
          invocationContextType.GetConstructors().Single(),
          new[] { methodInfoField, thisExpression }.Concat (parameters));
      return invocationContextCreate;
    }

    private Expression Invocation (MutableMethodInfo mutableMethod, int index)
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
      return Expression.Field (ThisExpression<T>(), fieldInfo);
    }

    private Expression PropertyAsReturnValue (Expression ctx)
    {
      return Expression.Property (ctx, "ReturnValue");
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
  }
}