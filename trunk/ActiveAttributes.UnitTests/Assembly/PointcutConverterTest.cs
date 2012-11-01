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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.Attributes.Aspects;
using ActiveAttributes.Core.Infrastructure.Pointcuts;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class PointcutConverterTest
  {
    [Test]
    public void Converts ()
    {
      Check<TypePointcut> (ObjectMother2.GetAspectAttribute (type: typeof (string)), "Type", typeof (string));
      Check<MemberNamePointcut> (ObjectMother2.GetAspectAttribute (memberName: "Method"), "MemberName", "Method");
    }

    private void Check<TPointcut> (AspectBaseAttribute aspectAttribute, string property, object expected)
    {
      var result = new PointcutConverter().GetPointcuts (aspectAttribute).Single();
      Assert.That (result, Is.TypeOf<TPointcut>().And.Property (property).EqualTo (expected));
    }
  }
}