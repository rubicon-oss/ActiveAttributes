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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using ActiveAttributes.Aspects;
using ActiveAttributes.Aspects.Ordering;
using ActiveAttributes.Aspects.Pointcuts;
using ActiveAttributes.Model.Ordering;
using ActiveAttributes.Weaving;
using ActiveAttributes.Weaving.Context;
using ActiveAttributes.Weaving.Invocation;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using System.Linq;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Expressions;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ImportTest
  {
    public event EventHandler Event;

    [Test]
    public void name ()
    {
      var field = typeof (ImportTest).GetFields (BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.NonPublic);
      var fieldInfo = field.FirstOrDefault();
      Assert.That (fieldInfo.Name, Is.EqualTo ("Event"));
      var event_ = typeof (ImportTest).GetEvents ().Single ();
      var type = event_.EventHandlerType;
      var method = type.GetMethod ("Invoke");
      var parameters = method.GetParameters().Select (x => x.ParameterType);
      var returnType = method.ReturnType;
      //var combine = typeof (Delegate).GetMethod ("Combine", new[] { typeof (Delegate), typeof (Delegate) });
      //Expression.Lambda<Action> (
      //    Expression.Block(
      //    Expression.Call (null,
      //    combine,
      //        Expression.Field (Expression.Constant(this), fieldInfo), 
      //        new NewDelegateExpression(event_.EventHandlerType, Expression.Constant(this), NormalizingMemberInfoFromExpressionUtility.GetMethod((ImportTest obj) => obj.Handler(null, null))))
      //        )).Compile()();
    }

    private void Handler (object sender, EventArgs args)
    {
      
    }

    [Test]
    public void Execution1 ()
    {
      var instance = ObjectFactory.Create<DomainType> ();
      ObjectFactory.Save();

      Assert.That (instance.Method1(), Is.EqualTo (2));
      Assert.That (instance.Method2(), Is.EqualTo (0));
    }

    [DomainAspect]
    public class DomainType
    {
      public virtual int Method1 ()
      {
        return 0;
      }

      public virtual int Method2 ()
      {
        return 0;
      }

      public virtual int Method3()
      {
        return 2;
      }
    }

    [AspectRoleOrdering (Position.Before, StandardRoles.ExceptionHandling)]
    public class DomainAspect : AspectAttributeBase
    {
      [ImportMember ("Method3", true)]
      public Func<int> Method3; 

      public DomainAspect ()
          : base (AspectScope.PerObject, StandardRoles.Caching) {}

      [Advice (AdviceExecution.Around)]
      [MethodExecutionPointcut (Name = "Method1")]
      public void Around (IContext context)
      {
        context.ReturnValue = Method3();
      }
    }
  }
}