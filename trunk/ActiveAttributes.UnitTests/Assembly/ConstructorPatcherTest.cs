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
using ActiveAttributes.Core.Assembly.Configuration;
using JetBrains.Annotations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using ActiveAttributes.Core.Extensions;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{


  [TestFixture]
  public class ConstructorPatcherTest : TestBase
  {
    private ICollection<IAspectGenerator> _emptyAspects;
    private ICollection<IAspectGenerator> _instanceAspect;

    [SetUp]
    public override void SetUp ()
    {
      var generator1 = MockRepository.GenerateMock<IAspectGenerator>();
      var generator2 = MockRepository.GenerateMock<IAspectGenerator>();

      generator1.Expect (x => x.GetInitExpression()).Return (Expression.Constant (null, typeof (AspectAttribute)));
      generator2.Expect (x => x.GetInitExpression()).Return (Expression.Constant (null, typeof (AspectAttribute)));

      var descriptor1 = MockRepository.GenerateMock<IAspectDescriptor>();
      var descriptor2 = MockRepository.GenerateMock<IAspectDescriptor>();

      descriptor1.Expect (x => x.Scope).Return (AspectScope.Instance);
      descriptor2.Expect (x => x.Scope).Return (AspectScope.Static);

      generator1.Expect (x => x.Descriptor).Return (descriptor1);
      generator2.Expect (x => x.Descriptor).Return (descriptor2);

      _emptyAspects = new IAspectGenerator[0];
      _instanceAspect = new[] { generator1 };
    }

    public class DomainClassBase
    {
      public PropertyInfo PropertyInfo;
      public EventInfo EventInfo;
      public MethodInfo MethodInfo;

      public static AspectAttribute[] InstanceAspects;
      public static AspectAttribute[] StaticAspects;
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

    public class DomainClass3 : DomainClassBase
    {
      public Action<EventHandler> Delegate; 
      public event EventHandler Event;
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

      PatchAndTest (methodInfo, methodInfo, _emptyAspects, test);
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

      PatchAndTest (methodInfo, methodInfo, _emptyAspects, test);
    }

    [Test]
    public void AssignPropertyInfo ()
    {
      var methodInfo = typeof (DomainClass2).GetMethods ().Single (x => x.Name == "set_Property");
      Action<IEnumerable<MutableConstructorInfo>> test = constructors =>
      {
        var propertyInfoAssign = PropertyInfoAssign (methodInfo);
        var constructor = constructors.Single ();
        ExpressionTreeComparer2.CheckTreeContains (constructor.Body, propertyInfoAssign);
      };

      PatchAndTest (methodInfo, methodInfo, _emptyAspects, test);
    }

    [Test]
    public void EmptyPropertyInfo ()
    {
      var methodInfo = typeof (DomainClass3).GetMethods ().Single (x => x.Name == "add_Event");
      Action<IEnumerable<MutableConstructorInfo>> test = constructors =>
      {
        var constructor = constructors.Single ();
        var propertyInfoAssign = Field (constructor.UnderlyingSystemConstructorInfo.DeclaringType, "PropertyInfo");
        Assert.That (() => ExpressionTreeComparer2.CheckTreeContains (constructor.Body, propertyInfoAssign), Throws.Exception);
      };

      PatchAndTest (methodInfo, methodInfo, _emptyAspects, test);
    }

    [Test]
    public void AssignEventInfo ()
    {
      var methodInfo = typeof (DomainClass3).GetMethods ().Single (x => x.Name == "add_Event");
      Action<IEnumerable<MutableConstructorInfo>> test = constructors =>
      {
        var eventInfoAssign = EventInfoAssign (methodInfo);
        var constructor = constructors.Single ();
        ExpressionTreeComparer2.CheckTreeContains (constructor.Body, eventInfoAssign);
      };

      PatchAndTest (methodInfo, methodInfo, _emptyAspects, test);
    }

    [Test]
    public void EmptyEventInfo ()
    {
      var methodInfo = typeof (DomainClass2).GetMethods ().Single (x => x.Name == "set_Property");
      Action<IEnumerable<MutableConstructorInfo>> test = constructors =>
      {
        var constructor = constructors.Single ();
        var eventInfoAssign = Field (constructor.UnderlyingSystemConstructorInfo.DeclaringType, "EventInfo");
        Assert.That (() => ExpressionTreeComparer2.CheckTreeContains (constructor.Body, eventInfoAssign), Throws.Exception);
      };

      PatchAndTest (methodInfo, methodInfo, _emptyAspects, test);
    }

    [Test]
    public void AssignInstanceAspects ()
    {
      var methodInfo = typeof (DomainClass2).GetMethods ().Single (x => x.Name == "set_Property");
      Action<IEnumerable<MutableConstructorInfo>> test = constructors =>
      {
        var constructor = constructors.Single ();
        var instanceAspectsAssign = InstanceAspectsAssign (_instanceAspect);
        ExpressionTreeComparer2.CheckTreeContains (constructor.Body, instanceAspectsAssign);
      };

      PatchAndTest (methodInfo, methodInfo, _instanceAspect, test);
    }

    private Expression StaticAspectsAssign (IEnumerable<IAspectGenerator> aspects)
    {
      var fieldInfo = typeof (DomainClassBase).GetFields().Single (x => x.Name == "StaticAspects");
      var field = Expression.Field (null, fieldInfo);
      var initExpression = Expression.NewArrayInit (typeof (AspectAttribute), aspects.Select (x => x.GetInitExpression()));
      var assignExpression = Expression.Assign (field, initExpression);
      var assignIfNullExpression = Expression.IfThen (Expression.Equal (field, Expression.Constant (null)), assignExpression);
      return assignIfNullExpression;
    }

    private Expression InstanceAspectsAssign (IEnumerable<IAspectGenerator> aspects)
    {
      var fieldInfo = typeof (DomainClassBase).GetFields ().Single (x => x.Name == "InstanceAspects");
      var field = Expression.Field (null, fieldInfo);
      var initExpression = Expression.NewArrayInit (typeof (AspectAttribute), aspects.Select (x => x.GetInitExpression()));
      var assignExpression = Expression.Assign (field, initExpression);
      return assignExpression;
    }

    private Expression EventInfoAssign (MethodInfo methodInfo)
    {
      var eventInfo = methodInfo.GetRelatedEventInfo ();
      var declaringType = methodInfo.DeclaringType;
      var eventInfoField = Field (declaringType, "EventInfo");
      var getEventMethod = typeof (Type).GetMethod ("GetEvent", new[] { typeof (string), typeof (BindingFlags) });
      var getEvent = Expression.Call (
          Expression.Constant (declaringType, typeof (Type)),
          getEventMethod,
          Expression.Constant (eventInfo.Name),
          Expression.Constant (BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic));
      return Expression.Assign (eventInfoField, getEvent);
    }

    private Expression PropertyInfoAssign (MethodInfo methodInfo)
    {
      var propertyInfo = methodInfo.GetRelatedPropertyInfo ();
      var declaringType = methodInfo.DeclaringType;
      var propertyInfoField = Field (declaringType, "PropertyInfo");
      var getPropertyMethod = typeof (Type).GetMethod ("GetProperty", new[] { typeof (string), typeof (BindingFlags) });
      var getProperty = Expression.Call (
          Expression.Constant (declaringType, typeof (Type)),
          getPropertyMethod,
          Expression.Constant (propertyInfo.Name),
          Expression.Constant (BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic));
      return Expression.Assign (propertyInfoField, getProperty);
    }

    private Expression DelegateAssign (MethodInfo methodInfo)
    {
      var declaringType = methodInfo.DeclaringType;
      var delegateField = Field (declaringType, "Delegate");
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
      var methodInfoField = Field (declaringType, "MethodInfo");
      return Expression.Assign (methodInfoField, Expression.Constant (methodInfo, typeof(MethodInfo)));
    }

    private Expression Field (Type declaringType, string fieldName)
    {
      var fieldInfo = declaringType.GetFields ().Single (x => x.Name == fieldName);
      return Expression.Field (ThisExpression (declaringType), fieldInfo);
    }

    private Expression ThisExpression (Type type)
    {
      return new ThisExpression (type);
    }

    private void PatchAndTest (
        MethodInfo methodInfo, MethodInfo copyInfo, ICollection<IAspectGenerator> aspects, Action<IEnumerable<MutableConstructorInfo>> test)
    {
      var declaringType = methodInfo.DeclaringType;
      var patcher = new ConstructorPatcher();
      AssembleType (
          declaringType,
          new Action<MutableType>[]
          {
              mutableType =>
              {
                var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);
                var mutableCopy = mutableType.GetOrAddMutableMethod (copyInfo);
                var propertyInfoField = declaringType.GetFields().Single (x => x.Name == "PropertyInfo");
                var eventInfoField = declaringType.GetFields().Single (x => x.Name == "EventInfo");
                var methodInfoField = declaringType.GetFields().Single (x => x.Name == "MethodInfo");
                var delegateField = declaringType.GetFields().Single (x => x.Name == "Delegate");

                patcher.AddReflectionAndDelegateInitialization (
                    mutableMethod, propertyInfoField, eventInfoField, methodInfoField, delegateField, mutableCopy);

                var instanceField = declaringType.GetFields (BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                    .Single (x => x.Name == "InstanceAspects");
                var staticField = declaringType.GetFields (BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                    .Single (x => x.Name == "StaticAspects");
                var instanceAccessor = MockRepository.GenerateMock<IArrayAccessor>();
                var staticAccessor = MockRepository.GenerateMock<IArrayAccessor>();
                instanceAccessor.Expect (x => x.GetAccessExpression (null)).IgnoreArguments().Return (Expression.Field (null, instanceField));
                staticAccessor.Expect (x => x.GetAccessExpression (null)).IgnoreArguments().Return (Expression.Field (null, staticField));

                var instanceAspects = aspects.Where (x => x.Descriptor.Scope == AspectScope.Instance);
                var staticAspects = aspects.Where (x => x.Descriptor.Scope == AspectScope.Static);

                patcher.AddAspectInitialization (mutableType, staticAccessor, instanceAccessor, staticAspects, instanceAspects);

                test (mutableType.AllMutableConstructors);
              }
          });
    }
  }
}