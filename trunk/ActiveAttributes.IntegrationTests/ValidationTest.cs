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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveAttributes.Common.Validation;
using ActiveAttributes.Core.Assembly;
using NUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ValidationTest : TestBase
  {
    private DomainType _instance;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      var type = AssembleType<DomainType> (Assembler.Singleton.ModifyType);
      _instance = (DomainType) Activator.CreateInstance (type);
    }

    [Test]
    public void ArgumentNotNull ()
    {
      _instance.Method1 ("");
    }

    [Test, Ignore]
    public void ArgumentNullThrows ()
    {
      Assert.That (() => _instance.Method1 (null), Throws.TypeOf<ArgumentNullException> ());
    }

    [Test]
    public void ReturnNotNull ()
    {
      _instance.Method3 ();
    }

    [Test]
    public void ReturnNullThrows ()
    {
      _instance.Method2 ();
    }

    public class DomainType
    {
      public virtual void Method1 ([NotNull] string arg) { }

      [return: NotNull]
      public virtual string Method2 ()
      {
        return null;
      }

      [return: NotNull]
      public virtual string Method3 ()
      {
        return null;
      }
    } 
  }
}
