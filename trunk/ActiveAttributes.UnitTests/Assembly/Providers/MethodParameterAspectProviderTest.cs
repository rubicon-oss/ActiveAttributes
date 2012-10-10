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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Assembly.Providers;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace ActiveAttributes.UnitTests.Assembly.Providers
{
  [TestFixture]
  public class MethodParameterAspectProviderTest
  {
    private MethodParameterAspectProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new MethodParameterAspectProvider();
    }

    [Test]
    public void Normal ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => Method (""));

      var result = _provider.GetAspects (method).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.Matches<IAspectDescriptor> (x => x.AspectType == typeof (AppliedAspectAttribute)));
    }

    [Test]
    public void DistinctPerMember ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => Method ("", ""));

      var result = _provider.GetAspects (method).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.Matches<IAspectDescriptor> (x => x.AspectType == typeof (AppliedAspectAttribute)));
    }

    private void Method ([ParameterAspect] string arg) { }

    private void Method ([ParameterAspect] string arg1, [ParameterAspect] string arg2) { }

    [ApplyAspect (typeof (AppliedAspectAttribute))]
    private class ParameterAspectAttribute : AspectAttribute { }

    private class AppliedAspectAttribute : AspectAttribute { }
  }
}