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
using System.Reflection;
using ActiveAttributes.Extensions;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Extensions
{
  [TestFixture]
  public class MethodInfoExtensionsTest
  {
    public class GetDelegateType
    {
      [Test]
      public void Action ()
      {
        var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SimpleMethod ()));

        var result = methodInfo.GetDelegateType ();

        Assert.That (result, Is.EqualTo (typeof (Action)));
      }

      [Test]
      public void ActionWithArgs ()
      {
        var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.ArgMethod ("a")));

        var result = methodInfo.GetDelegateType ();

        Assert.That (result, Is.EqualTo (typeof (Action<string>)));
      }

      [Test]
      public void Func ()
      {
        var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.ReturnMethod ()));

        var result = methodInfo.GetDelegateType ();

        Assert.That (result, Is.EqualTo (typeof (Func<string>)));
      }

      [Test]
      public void FuncWithArgs ()
      {
        var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.MixedMethod ("a", 1)));

        var result = methodInfo.GetDelegateType ();

        Assert.That (result, Is.EqualTo (typeof (Func<string, int, object>)));
      }

      class DomainType
      {
        public void SimpleMethod () { }
        public void ArgMethod (string a) { }
        public string ReturnMethod () { return default (string); }
        public object MixedMethod (string a, int b) { return default (object); }
      }
    }

    public class IsPropertyAccessor
    {
      [Test]
      public void AccessorMethod_ReturnsTrue ()
      {
        var accessorMethod = typeof (DomainType).GetMethod ("get_Property", BindingFlags.Instance | BindingFlags.Public);

        var result = accessorMethod.IsPropertyAccessor ();

        Assert.That (result, Is.True);
      }

      [Test]
      public void EventAccessor_ReturnsFalse ()
      {
        var accessorMethod = typeof (DomainType).GetMethod ("add_Event", BindingFlags.Instance | BindingFlags.Public);

        var result = accessorMethod.IsPropertyAccessor ();

        Assert.That (result, Is.False);
      }

      class DomainType
      {
        public string Property { get; set; }

        public event EventHandler Event;
      }
    }

    public class IsEventAccessor
    {
      [Test]
      public void AccessorMethod_ReturnsTrue ()
      {
        var accessorMethod = typeof (DomainType).GetMethod ("add_Event", BindingFlags.Instance | BindingFlags.Public);

        var result = accessorMethod.IsEventAccessor();

        Assert.That (result, Is.True);
      }

      [Test]
      public void PropertyAccessor_ReturnsFalse ()
      {
        var accessorMethod = typeof (DomainType).GetMethod ("get_Property", BindingFlags.Instance | BindingFlags.Public);

        var result = accessorMethod.IsEventAccessor();

        Assert.That (result, Is.False);
      }

      class DomainType
      {
        public string Property { get; set; }

        public event EventHandler Event;
      }
    }

    public class GetRelatedPropertyInfo
    {
      [Test]
      public void AccessorMethod_ReturnsPropertyInfo ()
      {
        var accessorMethod = typeof (DomainType).GetMethod ("get_Property", BindingFlags.Instance | BindingFlags.Public);
        var propertyInfo = MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.Property);

        var result = accessorMethod.GetRelatedPropertyInfo();

        Assert.That (result, Is.EqualTo (propertyInfo));
      }

      [Test]
      public void NormalMethod_ReturnsNull ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());
        var result = method.GetRelatedPropertyInfo();

        Assert.That (result, Is.Null);
      }

      class DomainType
      {
        public string Property { get; set; }

        public string Method () { return ""; }
      }
    }

    public class GetRelatedEventInfo
    {
      [Test]
      public void AccessorMethod_ReturnsPropertyInfo ()
      {
        var accessorMethod = typeof (DomainType).GetMethod ("add_Event", BindingFlags.Instance | BindingFlags.Public);
        var eventInfo = typeof (DomainType).GetEvent ("Event", BindingFlags.Instance | BindingFlags.Public);

        var result = accessorMethod.GetRelatedEventInfo ();

        Assert.That (result, Is.EqualTo (eventInfo));
      }

      [Test]
      public void NormalMethod_ReturnsNull ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method ());
        var result = method.GetRelatedEventInfo ();

        Assert.That (result, Is.Null);
      }

      class DomainType
      {
        public event EventHandler Event;

        public string Method () { return ""; }
      }
    }

    public class IsAction_IsFunc
    {
      [Test]
      public void IsAction ()
      {
        var action = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Action());
        var func = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Func());

        Assert.That (action.IsAction(), Is.True);
        Assert.That (func.IsAction(), Is.False);
      }

      [Test]
      public void IsFunc ()
      {
        var func = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Func());
        var action = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Action());

        Assert.That (func.IsFunc (), Is.True);
        Assert.That (action.IsFunc (), Is.False);
      }

      class DomainType
      {
        public void Action () { }
        public int Func () { return 1; }
      }
    }
  }
}