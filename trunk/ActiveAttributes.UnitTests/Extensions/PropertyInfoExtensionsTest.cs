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
using ActiveAttributes.Core.Extensions;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Extensions
{
  [TestFixture]
  public class PropertyInfoExtensionsTest
  {
    public class GetOverridenProperty
    {
      [Test]
      public void Normal ()
      {
        var propertyInfo = typeof (DerivedType).GetProperties().Single (x => x.Name == "VirtualProperty");
        var expected = typeof (BaseType).GetProperties().Single (x => x.Name == "VirtualProperty");

        Assert.That (expected, Is.Not.Null);
        Assert.That (propertyInfo.GetBaseProperty(), Is.EqualTo (expected));
      }

      [Test]
      public void ReturnsNullForBaseProperty ()
      {
        var propertyInfo = typeof (DerivedType).GetProperties ().Single (x => x.Name == "DeclaredProperty");

        Assert.That (propertyInfo.GetBaseProperty(), Is.Null);
      }

      [Test]
      public void PartialAccessors ()
      {
        var propertyInfo = typeof (DerivedType).GetProperties().Single (x => x.Name == "PartialProperty");
        var expected = typeof (BaseType).GetProperties().Single (x => x.Name == "PartialProperty");

        Assert.That (propertyInfo.GetBaseProperty(), Is.EqualTo (expected));
      }

      [Test]
      public void RespectsNewAttribute ()
      {
        var propertyInfo = typeof (DerivedType).GetProperties ().Single (x => x.Name == "NewProperty");

        Assert.That (propertyInfo.GetBaseProperty (), Is.Null);
      }

      class BaseType
      {
        public virtual string VirtualProperty { get; protected set; }
        public string NewProperty { get; protected set; }
        public virtual string PartialProperty { set { } }
      }

      class DerivedType : BaseType
      {
        public override string VirtualProperty { get; protected set; }
        public new string NewProperty { get; protected set; }
        public string DeclaredProperty { get; protected set; }
        public override string PartialProperty { set { } }
      }
    }

    public class IsIndexer
    {
      [Test]
      public void IndexerPropertyInfo_ReturnsTrue ()
      {
        var property = typeof (DomainType).GetProperty ("Item");

        Assert.That (property.IsIndexer(), Is.True);
      }

      [Test]
      public void PropertyInfo_ReturnsFalse ()
      {
        var property = typeof (DomainType).GetProperty ("Property");

        Assert.That (property.IsIndexer (), Is.False);
      }

      class DomainType
      {
        public int this [int index] { set { } }

        public int Property { get; set; }
      }
    }
  }
}