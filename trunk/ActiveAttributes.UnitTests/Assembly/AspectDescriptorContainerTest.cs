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
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using NUnit.Framework;
using Remotion.Collections;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectDescriptorContainerTest
  {
    [Test]
    public void Initialization ()
    {
      var instanceField = ObjectMother.GetFieldWrapper (typeof (AspectAttribute[]));
      var staticField = ObjectMother.GetFieldWrapper (typeof (AspectAttribute[]), FieldAttributes.Static);

      var instanceAspectDescriptor1 = ObjectMother.GetInstanceAspectDescriptor();
      var instanceAspectDescriptor2 = ObjectMother.GetInstanceAspectDescriptor();
      var staticAspectDescriptor1 = ObjectMother.GetStaticAspectDescriptor();
      var staticAspectDescriptor2 = ObjectMother.GetStaticAspectDescriptor();
      var aspectDescriptors = new[] { instanceAspectDescriptor1, instanceAspectDescriptor2, staticAspectDescriptor1, staticAspectDescriptor2 };

      var container = new AspectDescriptorContainer (aspectDescriptors, instanceField, staticField);

      Assert.That (container.InstanceField, Is.SameAs (instanceField));
      Assert.That (container.StaticField, Is.SameAs (staticField));
      Assert.That (container.InstanceAspects, Is.EquivalentTo (new[] { instanceAspectDescriptor1, instanceAspectDescriptor2 }));
      Assert.That (container.StaticAspects, Is.EquivalentTo (new[] { staticAspectDescriptor1, staticAspectDescriptor2 }));
      Assert.That (container.AspectStorageInfo[instanceAspectDescriptor1], Is.EqualTo (Tuple.Create (instanceField, 0)));
      Assert.That (container.AspectStorageInfo[instanceAspectDescriptor2], Is.EqualTo (Tuple.Create (instanceField, 1)));
      Assert.That (container.AspectStorageInfo[staticAspectDescriptor1], Is.EqualTo (Tuple.Create (staticField, 0)));
      Assert.That (container.AspectStorageInfo[staticAspectDescriptor2], Is.EqualTo (Tuple.Create (staticField, 1)));
    }
  }
}