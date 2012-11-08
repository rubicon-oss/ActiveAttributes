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

using ActiveAttributes.Core;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Pointcuts;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [Ignore]
  [TestFixture]
  public class ContextExposureTest
  {
    private DomainType _instance;

    [SetUp]
    public void SetUp ()
    {
      DomainAspect.Instance = null;
      DomainAspect.Argument = null;
      _instance = ObjectFactory.Create<DomainType> ();
    }

    [Test]
    public void TypePointcut ()
    {
      _instance.Method1 ();

      Assert.That (DomainAspect.Instance, Is.SameAs (_instance));
    }

    [Test]
    public void ArgumentPointcut ()
    {
      _instance.Method2 ("test");

      Assert.That (DomainAspect.Argument, Is.EqualTo ("test"));
    }

    [Test]
    public void ArgumentPointcut_Out ()
    {
      var result = _instance.Method3 ("test");

      Assert.That (result, Is.EqualTo ("advice"));
    }

    public class DomainType
    {
      public void Method1 () { }
      public void Method2 (string abc) { }
      public string Method3 (string abc) {return abc; }
    }

    public class DomainAspect : IAspect
    {
      public static string Muh;
      public static DomainType Instance { get; set; }

      [TypePointcut (typeof (DomainType))]
      public void Method1Advice (DomainType instance)
      {
        Instance = instance;
      }

      public static string Argument { get; set; }

      [ArgumentTypePointcut (typeof (string))]
      public void Method2Advice (string argument)
      {
        Argument = argument;
      }

      [ArgumentTypePointcut (typeof (string))]
      public void Method3Advice (out string argument)
      {
        argument = "advice";
      }
    }
  }
}