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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Extensions;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Extensions
{
  [TestFixture]
  public class MutableMethodInfoExtensionsTest
  {
    [Test]
    public void GetDelegateType_SimpleMethod ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SimpleMethod ()));
      var result = methodInfo.GetDelegateType ();

      Assert.That (result, Is.EqualTo (typeof (Action)));
    }

    [Test]
    public void GetDelegateType_ArgMethod ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.ArgMethod ("a")));
      var result = methodInfo.GetDelegateType ();

      Assert.That (result, Is.EqualTo (typeof (Action<string>)));
    }

    [Test]
    public void GetDelegateType_ReturnMethod ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.ReturnMethod ()));
      var result = methodInfo.GetDelegateType ();

      Assert.That (result, Is.EqualTo (typeof (Func<string>)));
    }

    [Test]
    public void GetDelegateType_MixedMethod ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.MixedMethod ("a", 1)));
      var result = methodInfo.GetDelegateType ();

      Assert.That (result, Is.EqualTo (typeof (Func<string, int, object>)));
    }

    public class DomainType
    {
      public void SimpleMethod () { }
      public void ArgMethod (string a) { }
      public string ReturnMethod () { return default (string); }
      public object MixedMethod (string a, int b) { return default (object); }
    }
  }
}