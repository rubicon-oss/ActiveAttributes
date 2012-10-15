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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Utilities;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Utilities
{
  [TestFixture]
  public class DescriptorProviderTest
  {
    [Test]
    public void GetAspects ()
    {
      var memberSequence = new[] { typeof (BaseType) };

      var result = DescriptorProvider.GetDescriptors (memberSequence[0], memberSequence).ToArray();

      Assert.That (result, Has.Length.EqualTo (2));
      Assert.That (result, Has.Some.Matches<IDescriptor> (desc => desc.AspectType == typeof (InheritingAspectAttribute)));
      Assert.That (result, Has.Some.Matches<IDescriptor> (desc => desc.AspectType == typeof (NotInheritingAspectAttribute)));
    }

    [Test]
    public void GetAspects_Derived ()
    {
      var memberSequence = new[] { typeof (DerivedType), typeof (BaseType) };

      var result = DescriptorProvider.GetDescriptors (memberSequence[0], memberSequence).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.Matches<IDescriptor> (desc => desc.AspectType == typeof (InheritingAspectAttribute)));
    }

    [InheritingAspect]
    [NotInheritingAspect]
    [NonAspect]
    private class BaseType {}

    private class DerivedType : BaseType {}

    private class NonAspectAttribute : Attribute {}

    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    private class InheritingAspectAttribute : AspectAttribute {}

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    private class NotInheritingAspectAttribute : AspectAttribute {}
  }
}