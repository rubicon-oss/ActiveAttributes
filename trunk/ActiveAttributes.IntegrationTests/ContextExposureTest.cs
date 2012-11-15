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

using ActiveAttributes.Advices;
using ActiveAttributes.Assembly;
using ActiveAttributes.Extensions;
using ActiveAttributes.Interception.Invocations;
using ActiveAttributes.Pointcuts;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [Ignore]
  [TestFixture]
  public class ContextExposureTest : TypeAssemblerIntegrationTestBase
  {
    private DomainType _instance;

    [SetUp]
    public void SetUp ()
    {
      DomainAspect.Instance = null;
      DomainAspect.Argument = null;

      var assembleType = AssembleType<DomainType> (ObjectFactory.Assembler.ModifyType);
      _instance = assembleType.CreateInstance<DomainType> ();
    }

    [Test]
    public void TypePointcut ()
    {
      _instance.Method1 ();

      Assert.That (DomainAspect.Instance, Is.SameAs (_instance));
    }

    [Test]
    public void Argument ()
    {
      _instance.Method2 ("test");

      Assert.That (DomainAspect.Argument, Is.EqualTo ("test"));
    }

    [Test]
    public void RefArgument ()
    {
      var result = _instance.Method3 (1);

      Assert.That (result, Is.EqualTo ("7"));
    }

    public class DomainType
    {
      public virtual void Method1 () {}
      public virtual void Method2 (string abc) {}
      public virtual string Method3 (int arg) { return arg.ToString(); }
    }

    [AdviceInfo (Execution = AdviceExecution.Around, Scope = AdviceScope.Static)]
    [TypePointcut (typeof (DomainType))]
    public class DomainAspect : IAspect
    {
      public static DomainType Instance { get; set; }
      public static string Argument { get; set; }

      ////[TypePointcut (typeof (DomainType))]
      [MemberNamePointcut ("Method1")]
      public void Advice1 (DomainType instance)
      {
        Instance = instance;
      }


      //[ArgumentTypePointcut (typeof (string))]
      [MemberNamePointcut ("Method2")]
      public void Advice2 (string argument)
      {
        Argument = argument;
      }

      //[ArgumentTypePointcut (typeof (string))]
      [MemberNamePointcut ("Method3")]
      public void Advice3 (IInvocation invocation, ref int argument)
      {
        argument = argument + 6;
        invocation.Proceed();
      }
    }
  }
}