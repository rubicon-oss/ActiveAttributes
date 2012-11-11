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
using ActiveAttributes.Core.Assembly.Storages;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class FieldServiceTest
  {
    private MutableType _mutableType;
    private FieldService _service;

    [SetUp]
    public void SetUp ()
    {
      _mutableType = ObjectMother.GetMutableType();
      _service = new FieldService();
    }

    [Test]
    public void AddInstanceStorage ()
    {
      var type = ObjectMother2.GetDeclaringType();

      var result = _service.AddInstanceStorage (_mutableType, type, "field");

      Assert.That (result, Is.TypeOf<InstanceStorage>());
      var addedField = _mutableType.AddedFields.Single();
      Assert.That (addedField.Name, Is.StringStarting ("field"));
      Assert.That (addedField.FieldType, Is.EqualTo (type));
      Assert.That (addedField.Attributes, Is.EqualTo (FieldAttributes.Private));
      var storageField = PrivateInvoke.GetNonPublicField (result, "_field");
      Assert.That (storageField, Is.SameAs (addedField));
    }

    [Test]
    public void AddStaticStorage ()
    {
      var type = ObjectMother2.GetDeclaringType();

      var result = _service.AddStaticStorage (_mutableType, type, "field");

      Assert.That (result, Is.TypeOf<StaticStorage>());
      var addedField = _mutableType.AddedFields.Single();
      Assert.That (addedField.Name, Is.StringStarting ("field"));
      Assert.That (addedField.FieldType, Is.EqualTo (type));
      Assert.That (addedField.Attributes, Is.EqualTo (FieldAttributes.Private | FieldAttributes.Static));
      var storageField = PrivateInvoke.GetNonPublicField (result, "_field");
      Assert.That (storageField, Is.SameAs (addedField));
    }

    [Test]
    public void AddGlobalStorage ()
    {
      var type = ObjectMother2.GetDeclaringType ();

      var result = _service.AddGlobalStorage (type);

      Assert.That (result, Is.TypeOf<GlobalStorage> ());
      var fakeExpression = ObjectMother2.GetExpression();
      var storageExpression = result.GetStorageExpression (fakeExpression);
      Assert.That (storageExpression.Type, Is.EqualTo (type));
    }

    [Test]
    public void AddField_Instance ()
    {
      var result = _service.AddField (_mutableType, typeof (int), "field", FieldAttributes.Private);

      Assert.That (result, Is.TypeOf<InstanceStorage>());
      var addedField = _mutableType.AddedFields.Single();
      Assert.That (result.Field, Is.SameAs (addedField));
    }

    [Test]
    public void AddField_Static ()
    {
      var result = _service.AddField (_mutableType, typeof (int), "field", FieldAttributes.Static);

      Assert.That (result, Is.TypeOf<StaticStorage>());
    }
  }
}