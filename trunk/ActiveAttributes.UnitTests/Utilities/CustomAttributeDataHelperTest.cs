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
using System.Runtime.CompilerServices;
using ActiveAttributes.Aspects2;
using ActiveAttributes.Extensions;
using ActiveAttributes.UnitTests.Extensions;
using ActiveAttributes.Utilities;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Utilities
{
  [TestFixture]
  public class CustomAttributeDataHelperTest
  {
    private CustomAttributeDataHelper _helper;

    [SetUp]
    public void SetUp ()
    {
      _helper = new CustomAttributeDataHelper();
    }

    [Test]
    public void IsInheriting ()
    {
      var customAttributeData1 = ObjectMother.GetCustomAttributeData (typeof (InheritingAttribute));
      var customAttributeData2 = ObjectMother.GetCustomAttributeData (typeof (NotInheritingAttribute));

      Assert.That (_helper.IsInheriting (customAttributeData1), Is.True);
      Assert.That (_helper.IsInheriting (customAttributeData2), Is.False);
    }

    [Test]
    public void AllowsMultiple ()
    {
      var customAttributeData1 = ObjectMother.GetCustomAttributeData (typeof (AllowMultipleAttribute));
      var customAttributeData2 = ObjectMother.GetCustomAttributeData (typeof (DisallowMultipleAttribute));

      Assert.That (_helper.AllowsMultiple (customAttributeData1), Is.True);
      Assert.That (_helper.AllowsMultiple (customAttributeData2), Is.False);
    }

    [Test]
    public void IsAspectAttribute ()
    {
      var customAttributeData1 = ObjectMother.GetCustomAttributeData (typeof (DomainAttribute));
      var customAttributeData2 = ObjectMother.GetCustomAttributeData (typeof (CompilerGeneratedAttribute));

      Assert.That (_helper.IsAspectAttribute (customAttributeData1), Is.True);
      Assert.That (_helper.IsAspectAttribute (customAttributeData2), Is.False);
    }

    [Test]
    public void Resolution ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<ICustomAttributeDataHelper>();

      Assert.That (instance, Is.TypeOf<CustomAttributeDataHelper>());
    }

    class DomainAttribute : AspectAttributeBase
    {
      public string[] Field;

      public DomainAttribute () { }

      public DomainAttribute (string constructorArgument)
      {
        ConstructorArgument = constructorArgument;
      }

      public string ConstructorArgument { get; set; }
    }

    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    class InheritingAttribute : Attribute {}

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    class NotInheritingAttribute : Attribute {}

    [AttributeUsage (AttributeTargets.All, AllowMultiple = true)]
    class AllowMultipleAttribute : Attribute {}

    [AttributeUsage (AttributeTargets.All, AllowMultiple = false)]
    class DisallowMultipleAttribute : Attribute {}
  }
}