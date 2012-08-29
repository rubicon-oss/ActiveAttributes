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
using ActiveAttributes.Core.Extensions;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Extensions
{
  [TestFixture]
  public class MemberInfoExtensionsTest
  {
    [Test]
    public void EqualsBaseDefinition_DirectOverride ()
    {
      var first = MemberInfoFromExpressionUtility.GetMethod (((B obj) => obj.Method1 ()));
      var second = MemberInfoFromExpressionUtility.GetMethod (((C obj) => obj.Method1 ()));

      var result = first.EqualsBaseDefinition (second);
      Assert.That (result, Is.True);
    }

    [Test]
    public void EqualsBaseDefinition_DoubleOverride ()
    {
      var first = MemberInfoFromExpressionUtility.GetMethod (((A obj) => obj.Method1 ()));
      var second = MemberInfoFromExpressionUtility.GetMethod (((C obj) => obj.Method1 ()));

      var result = first.EqualsBaseDefinition (second);
      Assert.That (result, Is.True);
    }

    [Test]
    public void EqualsBaseDefinition_IndirectOverride ()
    {
      var first = MemberInfoFromExpressionUtility.GetMethod (((A obj) => obj.Method2 ()));
      var second = MemberInfoFromExpressionUtility.GetMethod (((C obj) => obj.Method2 ()));

      var result = first.EqualsBaseDefinition (second);
      Assert.That (result, Is.True);
    }

    [Test]
    public void EqualsBaseDefinition_NewSlot ()
    {
      var first = MemberInfoFromExpressionUtility.GetMethod (((A obj) => obj.Method3 ()));
      var second = MemberInfoFromExpressionUtility.GetMethod (((C obj) => obj.Method3 ()));

      var result = first.EqualsBaseDefinition (second);
      Assert.That (result, Is.False);
    }

    [Test]
    public void EqualsBaseDefinition_NotRelated ()
    {
      var first = MemberInfoFromExpressionUtility.GetMethod (((A obj) => obj.Method4 ()));
      var second = MemberInfoFromExpressionUtility.GetMethod (((A obj) => obj.Method2 ()));

      var result = first.EqualsBaseDefinition (second);
      Assert.That (result, Is.False);
    }

    [Test]
    public void EqualsBaseDefinition_DirectInherit ()
    {
      var first = typeof (A);
      var second = typeof (B);

      var result = first.EqualsBaseDefinition (second);

      Assert.That (result, Is.True);
    }

    [Test]
    public void EqualsBaseDefinition_IndirectInherit ()
    {
      var first = typeof (C);
      var second = typeof (B);

      var result = first.EqualsBaseDefinition (second);

      Assert.That (result, Is.True);
    }

    [Test]
    public void EqualsBaseDefinition_ExcludeObject ()
    {
      var first = typeof (A);
      var second = typeof (D);

      var result = first.EqualsBaseDefinition (second);

      Assert.That (result, Is.False);
    }

    public class A
    {
      public virtual void Method4 () { }
      public virtual void Method2 () { }
      public void Method3 () { }
      public virtual void Method1 () { }
    }

    public class B : A
    {
      public override void Method4 () { }
      public override void Method1 () { }
    }

    public class C : B
    {
      public override void Method2 () { }
      public new void Method3 () { }
      public override void Method1 () { }
    }

    public class D
    {
    }
  }
}