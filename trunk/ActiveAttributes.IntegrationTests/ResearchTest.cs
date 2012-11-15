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
using Remotion.Development.UnitTesting.Reflection;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ResearchTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void Proceed ()
    {
      var assembleType = AssembleType<DomainType> (
          mt =>
          {
            var methodCopyService = new MethodCopyService();
            var method = typeof (DomainType).GetMethod ("add_MyEvent");
            var mutableMethod = mt.GetOrAddMutableMethod (method);
            var copy = methodCopyService.GetCopy (mutableMethod);

            Console.WriteLine();
            Console.WriteLine();
          });
      var instance = assembleType.CreateInstance<DomainType>();
      SkipDeletion();
    }

    public class DomainType
    {
      public virtual event EventHandler MyEvent;
    }
  }
}