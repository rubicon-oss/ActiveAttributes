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
using ActiveAttributes.Aspects2;
using ActiveAttributes.Weaving.Construction;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.UnitTests.Declaration.Construction
{
  [TestFixture]
  public class AttributeConstructionTest
  {
    [Test]
    [DomainAspect ("ctor", ApplyToTypeName = "named")]
    public void Initialization ()
    {
      var method = MethodInfo.GetCurrentMethod();
      var customAttributeData = GetCustomAttributeData<DomainAspectAttribute> (method);
      var constructor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainAspectAttribute(""));

      var result = new AttributeConstruction (customAttributeData);

      Assert.That (result.ConstructorInfo, Is.EqualTo (constructor));
      Assert.That (result.ConstructorArguments, Has.Count.EqualTo (1));
      Assert.That (result.NamedArguments, Has.Count.EqualTo (1));
    }

    private ICustomAttributeData GetCustomAttributeData<T> (MethodBase method)
    {
      return TypePipeCustomAttributeData.GetCustomAttributes (method).Single (x => x.Constructor.DeclaringType == typeof (T));
    }

    class DomainAspectAttribute : AspectAttributeBase
    {
      public DomainAspectAttribute (string arg) {}
    }
  }
}