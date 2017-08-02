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
using ActiveAttributes.Weaving;
using ActiveAttributes.Weaving.Construction;
using ActiveAttributes.Weaving.Invocation;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.MutableReflection;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class Research
  {
    public class MyClass
    {
      public virtual void Muh (string kuh)
      {
        
      }
    }
    [Test]
    public void name ()
    {
      var type = MutableTypeObjectMother.CreateForExisting (typeof (MyClass));
      var method = type.AllMutableMethods.Single();
      method.SetBody (ctx => ctx.GetBaseCall(method, ctx.Parameters.Cast<Expression>()));
    }

    [Test]
    public void name2 ()
    {
      var arg1 = "";
      var pars = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => Method (out arg1, ref arg1, arg1)).GetParameters();
      var par1 = pars[0];
      var par2 = pars[1];
      var par3 = pars[2];

      Assert.That (par1.IsOut, Is.True);
      Assert.That (par2.IsOut, Is.False);
      Assert.That (par1.ParameterType, Is.EqualTo (typeof (string).MakeByRefType()));
      Assert.That (par2.ParameterType, Is.EqualTo (typeof (string).MakeByRefType()));

      Assert.That (par1.IsRef(), Is.False);
      Assert.That (par2.IsRef(), Is.True);
      Assert.That (par3.IsRef(), Is.False);
    }

    private void Method (out string arg1, ref string arg2, string arg3)
    {
      arg1 = "";
    }

    class Rewriter : ExpressionVisitor
    {
      private readonly BlockExpression _around;

      public Rewriter (BlockExpression around)
      {
        _around = around;
      }


      protected override CatchBlock VisitCatchBlock (CatchBlock node)
      {
        var parameterExpression = node.Variable;
        var expression = node.Filter;
        var blockExpression = Visit(_around);
        var catchBlock = node.Update (parameterExpression, expression, blockExpression);
        return base.VisitCatchBlock (catchBlock);
      }
    }
  }

  public static class ParameterInfoExtensions
  {
    public static bool IsRef (this ParameterInfo parameterInfo)
    {
      return !parameterInfo.IsOut && parameterInfo.ParameterType.IsByRef;
    }
  }
}