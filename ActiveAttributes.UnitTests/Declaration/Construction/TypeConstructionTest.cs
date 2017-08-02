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
using ActiveAttributes.Weaving.Construction;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace ActiveAttributes.UnitTests.Declaration.Construction
{
  [TestFixture]
  public class TypeConstructionTest
  {
    [Test]
    public void Initialization ()
    {
      var constructor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainAspect());

      var result = new TypeConstruction (typeof (DomainAspect));

      Assert.That (result.ConstructorInfo, Is.EqualTo (constructor));
      Assert.That (result.ConstructorArguments, Is.Empty);
      Assert.That (result.NamedArguments, Is.Empty);
    }

    [Test]
    public void ThrowsForNoDefaultConstructor ()
    {
      var aspectType = typeof (DomainAspectWithoutDefaultConstructor);
      var message = aspectType.Name + " must provide a default constructor.";
      Assert.That (() => new TypeConstruction (aspectType), Throws.ArgumentException.With.Message.EqualTo (message));
    }

    class DomainAspect : IAspect {}

    class DomainAspectWithoutDefaultConstructor : IAspect
    {
      public DomainAspectWithoutDefaultConstructor (string arg) {}
    }
  }
}