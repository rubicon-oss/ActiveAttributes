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
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using ActiveAttributes.Annotations.Ordering;
using ActiveAttributes.Aspects;
using ActiveAttributes.Infrastructure;
using ActiveAttributes.Infrastructure.Ordering;
using ActiveAttributes.Weaving;
using ActiveAttributes.Weaving.Invocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class EventTest
  {
    public class UnsignedTypeHelper
    {
      private readonly Type _type;
      private readonly MethodInfo _method;
      private readonly Type _interface;
      private readonly Type _genericType;
      private readonly FieldInfo _field;

      public UnsignedTypeHelper()
      {
        var assemblyName = "UnsignedTypeHelper";

        var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName (assemblyName), AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule (assemblyName + ".dll");

        var typeBuilder = moduleBuilder.DefineType ("UnsignedType");
        var methodBuilder = typeBuilder.DefineMethod ("Method", MethodAttributes.Static | MethodAttributes.Public, typeof (void), Type.EmptyTypes);
        var ilgen = methodBuilder.GetILGenerator();
        ilgen.Emit (OpCodes.Ret);
        typeBuilder.DefineField ("Field", typeof (int), FieldAttributes.Public | FieldAttributes.Static);
        _type = typeBuilder.CreateType();
        _method = _type.GetMethods (BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public).Single();
        _field = _type.GetFields (BindingFlags.Static | BindingFlags.Public).Single();

        typeBuilder = moduleBuilder.DefineType ("UnsignedInterface", TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract);
        _interface = typeBuilder.CreateType();

        typeBuilder = moduleBuilder.DefineType ("UnsignedGenericType", TypeAttributes.Public);
        typeBuilder.DefineGenericParameters ("T");
        _genericType = typeBuilder.CreateType();
      }

      public Type Type
      {
        get { return _type; }
      }

      public MethodInfo Method
      {
        get { return _method; }
      }

      public Type Interface
      {
        get { return _interface; }
      }

      public Type GenericType
      {
        get { return _genericType; }
      }

      public FieldInfo Field
      {
        get { return _field; }
      }
    }

    [Test]
    public void name ()
    {
      var helper = new UnsignedTypeHelper();

      var type = helper.Type;
      Assert.That (type.Assembly.GetName().GetPublicKey(), Is.Empty);

    }

    [Test]
    public void Execution1 ()
    {
      var instance = ObjectFactory.Create<DomainType> ();
      ObjectFactory.Save();

      instance.Event += InstanceOnEvent;
      instance.Raise ();
      instance.Event -= InstanceOnEvent;
    }

    private void InstanceOnEvent (object sender, EventArgs eventArgs)
    {
        
    }

    public class DomainType
    {
    [DomainAspect]
      public virtual event EventHandler Event;

      public void Raise ()
      {
        Event (this, null);
      }
    }

    [AspectRoleOrdering (Position.Before, StandardRoles.ExceptionHandling)]
    public class DomainAspect : EventInterceptionAttributeBase
    {
      public DomainAspect ()
          : base (AspectScope.PerType, StandardRoles.Caching) {}

      public override void OnInvoke (IInvocation invocation)
      {
        invocation.Proceed ();
      }

      public override void OnAdd (IInvocation invocation)
      {
        invocation.Proceed();
      }

      public override void OnRemove (IInvocation invocation)
      {
        invocation.Proceed();
      }
    }
  }
}