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
using System.Linq.Expressions;
using System.Reflection;
using ActiveAttributes.Core.Extensions;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Extensions
{
  [TestFixture]
  public class PropertyInfoExtensionsTest
  {
    [Test]
    public void GetBaseDefinition ()
    {
      var expected = typeof (B).GetProperties ().Where (x => x.Name == "PropertyA").Single ();
      var input = typeof (C).GetProperties ().Where (x => x.Name == "PropertyA").Single ();
      var actual = input.GetOverridenProperty ();

      Assert.That (actual, Is.EqualTo (expected));
    }


    [Test]
    public void GetBaseDefinition2 ()
    {
      var expected = typeof (A).GetProperties ().Where (x => x.Name == "PropertyB").Single ();
      var input = typeof (C).GetProperties ().Where (x => x.Name == "PropertyB").Single ();
      var actual = input.GetOverridenProperty ();

      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void GetBaseDefinition3 ()
    {
      var expected = default (PropertyInfo);
      var input = typeof (A).GetProperties ().Where (x => x.Name == "PropertyA").Single ();
      var actual = input.GetOverridenProperty ();

      Assert.That (actual, Is.EqualTo (expected));
    }

    //[Test]
    public void GetBaseDefinition4 ()
    {
      var expected = typeof (A).GetProperties ().Where (x => x.Name == "PropertyC").Single ();
      var input = typeof (C).GetProperties ().Where (x => x.Name == "PropertyC").Single ();
      var actual = input.GetOverridenProperty ();

      Assert.That (actual, Is.EqualTo (expected));
    }

    //[Test]
    public void GetBaseDefinition5 ()
    {
      var expected = typeof (A).GetProperties ().Where (x => x.Name == "PropertyD").Single ();
      var input = typeof (C).GetProperties ().Where (x => x.Name == "PropertyD").Single ();
      var actual = input.GetOverridenProperty ();

      Assert.That (actual, Is.EqualTo (expected));
    }



    public class A
    {
      public virtual string PropertyA { get; set; }
      public virtual string PropertyB { get; set; }

      public virtual string PropertyC { get { return ""; } }
      public virtual string PropertyD { set { } }

      public virtual string GetString ()
      {
        return "";
      }
    }

    public class B : A
    {
      public override string PropertyA
      {
        get { return base.PropertyA; }
        set { base.PropertyA = value; }
      }
    }

    public class C : B
    {
      public override string PropertyA
      {
        get { return base.PropertyA; }
        set { base.PropertyA = value; }
      }

      public override string PropertyB
      {
        get { return base.PropertyB; }
        set { base.PropertyB = value; }
      }

      public override string PropertyC
      {
        get { return base.PropertyC; }
      }

      public override string PropertyD
      {
        set { base.PropertyD = value; }
      }
    }
  }
}