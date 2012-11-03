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
using ActiveAttributes.Core.Attributes.AdviceInfo;
using ActiveAttributes.Core.Attributes.Pointcuts;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Infrastructure.Pointcuts;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Discovery
{
  [TestFixture]
  public class CustomAttributeProviderToAdviceConverterTest
  {
    private CustomAttributeProviderToAdviceConverter _converter;

    [SetUp]
    public void SetUp ()
    {
      _converter = new CustomAttributeProviderToAdviceConverter();
    }

    [Test]
    [AdviceExecution (AdviceExecution.Around)]
    [AdviceScope (AdviceScope.Instance)]
    [AdviceName ("Name")]
    [AdviceRole ("Role")]
    [AdvicePriority (10)]
    public void AdviceInfo ()
    {
      var method = MethodInfo.GetCurrentMethod();
      CheckAdviceValue (method, x => x.Execution, AdviceExecution.Around);
      CheckAdviceValue (method, x => x.Scope, AdviceScope.Instance);
      CheckAdviceValue (method, x => x.Name, "Name");
      CheckAdviceValue (method, x => x.Role, "Role");
      CheckAdviceValue (method, x => x.Priority, 10);
    }

    [Test]
    [TypePointcut (typeof (string))]
    [MemberNamePointcut ("MemberName")]
    public void Pointcuts ()
    {
      var method = MethodInfo.GetCurrentMethod();
      var result = _converter.GetAdvice (method);

      var pointcuts = result.Pointcuts.ToList();
      Assert.That (pointcuts, Has.Count.EqualTo (2));
      Assert.That (pointcuts, Has.Some.TypeOf<TypePointcut>().With.Property ("Type").EqualTo (typeof (string)));
      Assert.That (pointcuts, Has.Some.TypeOf<MemberNamePointcut>().With.Property ("MemberName").EqualTo ("MemberName"));
    }

    private void CheckAdviceValue<T> (ICustomAttributeProvider method, Func<Advice, T> selector, T value)
    {
      var result = _converter.GetAdvice (method);
      Assert.That (selector (result), Is.EqualTo (value));
    }

  }
}