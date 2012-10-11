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

using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Assembly.Descriptors;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Assembly.Descriptors
{
  [TestFixture]
  public class TypeArgumentsDescriptorTest
  {
    public class Initialize
    {
      [Test]
      public void SetsInformation ()
      {
        var aspectType = typeof (AspectAttribute);
        var scope = AspectScope.Instance;
        var priority = 5;
        var descriptor = new TypeDescriptor (aspectType, scope, priority);

        Assert.That (descriptor.AspectType, Is.EqualTo (aspectType));
        Assert.That (descriptor.Scope, Is.EqualTo (scope));
        Assert.That (descriptor.Priority, Is.EqualTo (priority));
      }
    } 
  }
}