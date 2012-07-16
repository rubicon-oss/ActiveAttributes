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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration;
using NUnit.Framework;
using Remotion.Utilities;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectGeneratorFactoryTest
  {
    private IAspectDescriptor _descriptor;
    private IAspectDescriptor[] _enumerable;

    private FieldInfo _fieldInfo;

    private AspectGeneratorFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _descriptor = MockRepository.GenerateMock<IAspectDescriptor>();
      _enumerable = new IAspectDescriptor[] { _descriptor };

      _fieldInfo = MemberInfoFromExpressionUtility.GetField (() => _fieldInfo);

      _factory = new AspectGeneratorFactory();
    }

    [Test]
    public void RespectsInstanceScope ()
    {
      var scope = AspectScope.Instance;
      _descriptor.Expect (x => x.Scope).Return (scope);

      var result = _factory.GetAspectGenerators (_enumerable, scope, _fieldInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.TypeOf<InstanceAspectGenerator> ());
    }

    [Test]
    public void RespectsStaticScope ()
    {
      var scope = AspectScope.Static;
      _descriptor.Expect (x => x.Scope).Return (scope);

      var result = _factory.GetAspectGenerators (_enumerable, scope, _fieldInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.TypeOf<StaticAspectGenerator> ());
    }

    [Test]
    public void RespectsUnrelatedDueScope ()
    {
      _descriptor.Expect (x => x.Scope).Return (AspectScope.Static);

      var result = _factory.GetAspectGenerators (_enumerable, AspectScope.Instance, _fieldInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (0));
    }
  }
}