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
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Utilities;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Configuration2.AspectDescriptorProviders
{
  [TestFixture]
  public class AspectDescriptorProviderUtilityTest
  {
    [Test]
    public void GetAspects ()
    {
      var memberSequence = new[] { typeof (DomainType) };

      var result = AspectDescriptorProviderUtility.GetDescriptors (memberSequence[0], memberSequence).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.Some.Matches<IAspectDescriptor> (desc => desc.Type == typeof (InheritingAspectAttribute)));
    }

    [Test]
    public void GetAspects_Inherit ()
    {
      var memberSequence = new[] { typeof (InheritDerivedType), typeof (InheritBaseType) };

      var result = AspectDescriptorProviderUtility.GetDescriptors (memberSequence[0], memberSequence).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.Matches<IAspectDescriptor> (desc => desc.Type == typeof (InheritingAspectAttribute)));
    }

    [Test]
    public void GetAspects_AllowMultiple ()
    {
      var memberSequence = new[] { typeof (MultipleDerivedType), typeof (MultipleBaseType) };

      var result = AspectDescriptorProviderUtility.GetDescriptors (memberSequence[0], memberSequence).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.Matches<IAspectDescriptor> (desc => desc.Type == typeof (DisallowMultipleAspectAttribute)));
    }

    [NonAspect]
    [InheritingAspect]
    class DomainType {}

    [InheritingAspect]
    [NotInheritingAspect]
    class InheritBaseType { }

    class InheritDerivedType : InheritBaseType { }

    [DisallowMultipleAspectAttribute]
    class MultipleBaseType { }

    [DisallowMultipleAspectAttribute]
    class MultipleDerivedType : MultipleBaseType { }

    class NonAspectAttribute : Attribute {}

    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    class InheritingAspectAttribute : AspectAttribute {}

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    class NotInheritingAspectAttribute : AspectAttribute {}

    [AttributeUsage (AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    class DisallowMultipleAspectAttribute : AspectAttribute {}
  }
}