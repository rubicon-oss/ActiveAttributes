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
using ActiveAttributes.Aspects;
using ActiveAttributes.Assembly;
using ActiveAttributes.Extensions;
using ActiveAttributes.Interception.Invocations;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class PropertyTest : TypeAssemblerIntegrationTestBase
  {
    [Ignore]
    [Test]
    public void Proceed ()
    {
      var assembleType = AssembleType<DomainType> (ObjectFactory.Assembler.ModifyType);
      var instance = assembleType.CreateInstance<DomainType> ();

      instance.Property = "";
      Assert.That (PropertyAspectAttribute.LastAction, Is.EqualTo ("Set"));

      Dev.Null = instance.Property;
      Assert.That (PropertyAspectAttribute.LastAction, Is.EqualTo ("Set"));
    }

    public class DomainType
    {
      [PropertyAspect]
      public string Property { get; set; }
    }

    public class PropertyAspectAttribute : PropertyInterceptionAspectAttributeBase
    {
      public static string LastAction { get; private set; }

      public override void OnInterceptGet (IInvocation invocation)
      {
        LastAction = "Get";
      }

      public override void OnInterceptSet (IInvocation invocation)
      {
        LastAction = "Set";
      }
    }
  }
}