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
using ActiveAttributes.Aspects;
using ActiveAttributes.Aspects.Pointcuts;
using ActiveAttributes.Weaving;
using ActiveAttributes.Weaving.Context;
using ActiveAttributes.Weaving.Invocation;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using System.Linq;
using Remotion.Development.UnitTesting.Reflection;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ScopeTest
  {
    private static Dictionary<object, object> dict = new Dictionary<object, object>();


    [Test]
    public void Execution1 ()
    {
      var instance = ObjectFactory.Create<DomainType> ();
      instance.Method1 (1);
      var res = instance.Method2();
      Assert.That (res, Is.EqualTo (1));
    }

    public class DomainType
    {
      [DomainAspect]
      public virtual void Method1 (int i)
      {
      }

      [DomainAspect]
      public virtual int Method2 ()
      {
        return 0;
      }
    }

    public class DomainAspect : AspectAttributeBase
    {
      private int _saved;

      public DomainAspect ()
          : base (AspectScope.Singleton) {}

      [Advice (AdviceExecution.Around)]
      [MethodExecutionPointcut]
      public void Around (IContext context)
      {
        var method = context.MemberInfo as MethodInfo;
        if (method.ReturnType == typeof (void))
          _saved = (int) context.Arguments[0];
        else
          context.ReturnValue = _saved;
      }
    }
  }
}