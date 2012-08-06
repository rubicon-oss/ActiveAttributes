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
using ActiveAttributes.Core.Configuration;
using JetBrains.Annotations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using ActiveAttributes.Core.Extensions;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class ConstructorPatcherTest : TestBase
  {
    private ConstructorPatcher _patcher;
    private MethodInfo _methodInfo;
    private MethodInfo _copiedMethodInfo;



    [SetUp]
    public override void SetUp ()
    {
      _patcher = new ConstructorPatcher();
      _methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      _copiedMethodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method_Copy ()));
      DomainTypeBase.StaticAspects = null;
    }

    public class DomainClassBase
    {
      public PropertyInfo PropertyInfo;
      public EventInfo EventInfo;
      public MethodInfo MethodInfo;
    }

    public class DomainClass : DomainClassBase
    {
      public Action Delegate; 
      public void Method () { }
    }

    public class DomainClass2 : DomainClassBase
    {
      public Action<string> Delegate;
      public string Property { get; set; }
    }

    [Test]
    public void AssignMethodInfo ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method ());
      Action<IEnumerable<MutableConstructorInfo>> test = constructors =>
      {
        var methodInfoAssign = MethodInfoAssign (methodInfo);
        var constructor = constructors.Single ();
        ExpressionTreeComparer2.CheckTreeContains (constructor.Body, methodInfoAssign);
      };

      PatchAndTest (methodInfo, methodInfo, test);
    }

    [Test]
    public void AssignDelegate ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method ());
      Action<IEnumerable<MutableConstructorInfo>> test = constructors =>
      {
        var delegateAssign = DelegateAssign (methodInfo);
        var constructor = constructors.Single ();
        ExpressionTreeComparer2.CheckTreeContains (constructor.Body, delegateAssign);
      };

      PatchAndTest (methodInfo, methodInfo, test);
    }

    [Test]
    public void AssignPropertyInfo ()
    {
      var methodInfo = typeof (DomainClass2).GetMethods().Single (x => x.Name == "set_Property");
      Action<IEnumerable<MutableConstructorInfo>> test = constructors =>
      {
        var propertyInfoAssign = PropertyInfoAssign (methodInfo);
        var constructor = constructors.Single ();
        ExpressionTreeComparer2.CheckTreeContains (constructor.Body, propertyInfoAssign);
      };

      PatchAndTest (methodInfo, methodInfo, test);
    }

    private Expression PropertyInfoAssign (MethodInfo methodInfo)
    {
      var propertyInfo = methodInfo.GetRelatedPropertyInfo ();
      if (propertyInfo == null)
        return Expression.Empty();

      var declaringType = methodInfo.DeclaringType;
      var propertyInfoField = GetField (declaringType, "PropertyInfo");
      var getPropertyMethd = typeof (Type).GetMethod ("GetProperty", new[] { typeof (string), typeof (BindingFlags) });
      var getProperty = Expression.Call (
          Expression.Constant (declaringType, typeof (Type)),
          getPropertyMethd,
          Expression.Constant (propertyInfo.Name),
          Expression.Constant (BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic));
      return Expression.Assign (propertyInfoField, getProperty);
    }

    private Expression DelegateAssign (MethodInfo methodInfo)
    {
      var declaringType = methodInfo.DeclaringType;
      var delegateField = GetField (declaringType, "Delegate");
      var createDelegateMethodInfo = typeof (Delegate).GetMethod ("CreateDelegate", new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
      var parameterTypes = methodInfo.GetParameters().Select (x => x.ParameterType).Concat (new[] { methodInfo.ReturnType }).ToArray();
      var delegateType = Expression.GetDelegateType (parameterTypes);
      var createExpression = Expression.Call (
          null,
          createDelegateMethodInfo,
          Expression.Constant (delegateType),
          ThisExpression (declaringType),
          Expression.Constant (methodInfo, typeof (MethodInfo)));
      return Expression.Assign (delegateField, Expression.Convert(createExpression, delegateType));
    }

    private Expression MethodInfoAssign (MethodInfo methodInfo)
    {
      var declaringType = methodInfo.DeclaringType;
      var methodInfoField = GetField (declaringType, "MethodInfo");
      return Expression.Assign (methodInfoField, Expression.Constant (methodInfo, typeof(MethodInfo)));
    }

    private Expression GetField (Type declaringType, string fieldName)
    {
      var fieldInfo = declaringType.GetFields ().Single (x => x.Name == fieldName);
      return Expression.Field (ThisExpression (declaringType), fieldInfo);
    }

    private Expression ThisExpression (Type type)
    {
      return new ThisExpression (type);
    }

    private void PatchAndTest (
        MethodInfo methodInfo, MethodInfo copyInfo, Action<IEnumerable<MutableConstructorInfo>> test)
    {
      var declaringType = methodInfo.DeclaringType;
      var patcher = new ConstructorPatcher();
      AssembleType (
          declaringType,
          mutableType =>
          {
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);
            var mutableCopy = mutableType.GetOrAddMutableMethod (copyInfo);
            var propertyInfoField = declaringType.GetFields().Where (x => x.Name == "PropertyInfo").Single();
            var eventInfoField = declaringType.GetFields().Where (x => x.Name == "EventInfo").Single();
            var methodInfoField = declaringType.GetFields().Where (x => x.Name == "MethodInfo").Single();
            var delegateField = declaringType.GetFields().Where (x => x.Name == "Delegate").Single();

            patcher.AddReflectionAndDelegateInitialization (
                mutableMethod, propertyInfoField, eventInfoField, methodInfoField, delegateField, mutableCopy);

            test (mutableType.AllMutableConstructors);
          });
    }



    [Test]
    public void Init_MethodInfo ()
    {
      var instance = CreateInstance<DomainType> (new AspectDescriptor[0], _methodInfo, _copiedMethodInfo);

      Assert.That (instance.MethodInfo, Is.EqualTo (_methodInfo));
    }

    [Test]
    public void Init_Delegate ()
    {
      var instance = CreateInstance<DomainType> (new AspectDescriptor[0], _methodInfo, _copiedMethodInfo);

      Assert.That (instance.Delegate, Is.EqualTo (new Action (instance.Method_Copy)));
    }

    [Test]
    public void Init_InstanceAspects_Empty ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SingleInstanceAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (instance.InstanceAspects, Is.Not.Null);
    }

    [Test]
    public void Init_InstanceAspects_Single ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SingleInstanceAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (instance.InstanceAspects, Has.Length.EqualTo (1));
      Assert.That (instance.InstanceAspects, Has.All.InstanceOf<DomainAspectAttribute> ());
    }

    [Test]
    public void Init_InstanceAspects_Multiple ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.MultiInstanceAspectsMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (instance.InstanceAspects, Has.Length.EqualTo (2));
      Assert.That (instance.InstanceAspects, Has.All.InstanceOf<DomainAspectAttribute> ());
      Assert.That (instance.InstanceAspects[0], Is.Not.SameAs (instance.InstanceAspects[1]));
    }

    [Test]
    public void Init_StaticAspects_Empty ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SingleStaticAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (DomainTypeBase.StaticAspects, Is.Not.Null);
    }

    [Test]
    public void Init_StaticAspects_Single ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SingleStaticAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (DomainTypeBase.StaticAspects, Has.Length.EqualTo (1));
      Assert.That (DomainTypeBase.StaticAspects, Has.All.InstanceOf<DomainAspectAttribute> ());
      Assert.That (DomainTypeBase.StaticAspects[0], Is.SameAs (instance.InstanceAspects[0]));
    }

    [Test]
    public void Init_StaticAspects_Multiple ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.MultiStaticAspectsMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (DomainTypeBase.StaticAspects, Has.Length.EqualTo (2));
      Assert.That (DomainTypeBase.StaticAspects, Has.All.InstanceOf<DomainAspectAttribute> ());
      Assert.That (DomainTypeBase.StaticAspects[0], Is.SameAs (instance.InstanceAspects[0]));
      Assert.That (DomainTypeBase.StaticAspects[1], Is.SameAs (instance.InstanceAspects[1]));
    }

    [Test]
    public void Init_StaticAspects_OnlyOnce ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SingleStaticAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);
      var before = DomainTypeBase.StaticAspects;
      CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);
      var after = DomainTypeBase.StaticAspects;

      Assert.That (after, Is.SameAs (before));
    }

    [Test]
    public void Init_Aspects_CtorElementArguments ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.CtorElementArgAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      var ctorArgAspect = (CtorArgsDomainAspectAttribute) instance.InstanceAspects[0];
      Assert.That (ctorArgAspect.ElementArg, Is.EqualTo ("a"));
    }

    [Test]
    public void Init_Aspects_NamedElementArguments ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.NamedElementArgAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      var namedArgAspect = (NamedArgsDomainAspectAttribute) instance.InstanceAspects[0];
      Assert.That (namedArgAspect.ElementArg, Is.EqualTo ("a"));
      Assert.That (namedArgAspect.Priority, Is.EqualTo (10));
    }

    [Test]
    public void Init_Aspects_CtorArrayArguments ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.CtorArrayArgAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      var ctorArgAspect = (CtorArgsDomainAspectAttribute) instance.InstanceAspects[0];
      Assert.That (ctorArgAspect.ArrayArg, Is.EqualTo (new[] { "a" }));
    }

    [Test]
    public void Init_Aspects_NamedArrayArguments ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.NamedArrayArgAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      var namedArgAspect = (NamedArgsDomainAspectAttribute) instance.InstanceAspects[0];
      Assert.That (namedArgAspect.ArrayArg, Is.EqualTo (new[] { "a" }));
    }

    [Test]
    public void Patch_MultipleConstructors ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.Method ()));
      var ctorArgs = new object[] { "a" };
      var instance = CreateInstance<DomainType2> (new AspectDescriptor[0], methodInfo, methodInfo, ctorArgs);
      var instance2 = CreateInstance<DomainType2> (new AspectDescriptor[0], methodInfo, methodInfo);

      Assert.That (instance.MethodInfo, Is.Not.Null);
      Assert.That (instance.Delegate, Is.Not.Null);
      Assert.That (DomainType2.StaticAspects, Is.Not.Null);
      Assert.That (instance.InstanceAspects, Is.Not.Null);

      Assert.That (instance2.MethodInfo, Is.Not.Null);
      Assert.That (instance2.Delegate, Is.Not.Null);
      Assert.That (DomainType2.StaticAspects, Is.Not.Null);
      Assert.That (instance2.InstanceAspects, Is.Not.Null);
    }

    [Test]
    public void name ()
    {
      SkipDeletion();
      var methodInfo = typeof (DomainType3).GetMethods().Single (x => x.Name == "set_Property");
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var isntance = CreateInstance<DomainType3> (compileTimeAspects, methodInfo, methodInfo);

      Assert.That (isntance.PropertyInfo, Is.Not.Null);
    }

    private T CreateInstance<T> (IEnumerable<IAspectDescriptor> aspects, MethodInfo methodInfo, MethodInfo copiedMethod, params object[] args)
      where T: DomainTypeBase
    {
      var propertyInfoField = typeof (T).GetFields().Single (x => x.Name == "PropertyInfo");
      var eventInfoField = typeof (T).GetFields().Single (x => x.Name == "EventInfo");
      var methodInfoField = typeof (T).GetFields().Single (x => x.Name == "MethodInfo");
      var delegateField = typeof (T).GetFields().Single (x => x.Name == "Delegate");
      var staticAspectsField = typeof (DomainTypeBase).GetFields (BindingFlags.Static | BindingFlags.Public).Single();
      var instanceAspectsField = typeof (T).GetFields().Single (x => x.Name == "InstanceAspects");
      var fieldData = new FieldIntroducer.Data
                      {
                          MethodInfoField = methodInfoField,
                          DelegateField = delegateField,
                          StaticAspectsField = staticAspectsField,
                          InstanceAspectsField = instanceAspectsField
                      };

      var type = AssembleType<T> (
          mutableType =>
          {
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);
            var mutableCopy = mutableType.GetOrAddMutableMethod (copiedMethod);

            _patcher.AddReflectionAndDelegateInitialization (
                mutableMethod, propertyInfoField, eventInfoField, methodInfoField, delegateField, mutableCopy);
            //_patcher.AddAspectInitialization(mutableMethod, )
          });

      return (T) Activator.CreateInstance (type, args);
    }

    private IAspectDescriptor[] GetCompileTimeAspects (MethodInfo methodInfo)
    {
      var customAttributeData = CustomAttributeData.GetCustomAttributes (methodInfo);
      var compileTimeAspects = customAttributeData
          .Where (x => typeof (AspectAttribute).IsAssignableFrom (x.Constructor.DeclaringType))
          .Select (x => new AspectDescriptor (x)).ToArray();
      return compileTimeAspects;
    }

    public abstract class DomainTypeBase
    {
      public PropertyInfo PropertyInfo;
      public EventInfo EventInfo;
      public MethodInfo MethodInfo;
      public static AspectAttribute[] StaticAspects;
      public AspectAttribute[] InstanceAspects;
    }

    public class DomainType : DomainTypeBase
    {
      public Action Delegate;

      public virtual void Method () { }

      public virtual void Method_Copy () { }

      [DomainAspect (Scope = AspectScope.Instance)]
      public virtual void SingleInstanceAspectMethod () { }

      [DomainAspect (Scope = AspectScope.Instance)]
      [DomainAspect (Scope = AspectScope.Instance)]
      public virtual void MultiInstanceAspectsMethod () { }

      [DomainAspect (Scope = AspectScope.Static)]
      public virtual void SingleStaticAspectMethod () { }

      [DomainAspect (Scope = AspectScope.Static)]
      [DomainAspect (Scope = AspectScope.Static)]
      public virtual void MultiStaticAspectsMethod () { }

      [CtorArgsDomainAspect ("a")]
      public virtual void CtorElementArgAspectMethod () { }

      [NamedArgsDomainAspect (ElementArg = "a", Priority = 10)]
      public virtual void NamedElementArgAspectMethod () { }

      [CtorArgsDomainAspect (new[] { "a" })]
      public virtual void CtorArrayArgAspectMethod () { }

      [NamedArgsDomainAspect (ArrayArg = new[] { "a" })]
      public virtual void NamedArrayArgAspectMethod () { }
    }

    [UsedImplicitly]
    public class DomainType2 : DomainTypeBase
    {
      public Action Delegate;

      public DomainType2 ()
      {
      }
      public DomainType2 (string arg)
      {
      }

      public virtual void Method () { }
    }

    public class DomainType3 : DomainTypeBase
    {
      public Action<string> Delegate;

      [DomainAspect]
      public virtual string Property { get; set; }
    }

    public class DomainAspectAttribute : AspectAttribute
    {
    }

    public class CtorArgsDomainAspectAttribute : AspectAttribute
    {
      public string ElementArg { get; set; }
      public string[] ArrayArg { get; set; }

      public CtorArgsDomainAspectAttribute (string elementArg)
      {
        ElementArg = elementArg;
      }

      public CtorArgsDomainAspectAttribute (string[] arrayArg)
      {
        ArrayArg = arrayArg;
      }
    }

    public class NamedArgsDomainAspectAttribute : AspectAttribute
    {
      public string ElementArg { get; set; }
      public string[] ArrayArg { get; set; }

      public NamedArgsDomainAspectAttribute ()
      {
      }
    }
  }
}