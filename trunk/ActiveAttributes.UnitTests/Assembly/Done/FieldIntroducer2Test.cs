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
using System.Reflection;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.FieldWrapper;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.UnitTests.Assembly.Done
{
  [TestFixture]
  public class FieldIntroducer2Test
  {
    private MutableType _mutableType;
    private FieldIntroducer2 _introducer;

    [SetUp]
    public void SetUp ()
    {
      _mutableType = ObjectMother.GetMutableType();
      _introducer = new FieldIntroducer2();
    }

    [Test]
    public void AddField ()
    {
      var result = _introducer.AddField (_mutableType, typeof (int), "field", FieldAttributes.Private);

      Assert.That (result, Is.TypeOf<InstanceFieldWrapper>());
      var addedField = _mutableType.AddedFields.Single();
      Assert.That (result.Field, Is.SameAs (addedField));
    }

    [Test]
    public void AddField_Static ()
    {
      var result = _introducer.AddField (_mutableType, typeof (int), "field", FieldAttributes.Static);

      Assert.That (result, Is.TypeOf<StaticFieldWrapper>());
    }
  }
}