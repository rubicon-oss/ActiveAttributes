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
using ActiveAttributes.Assembly;
using ActiveAttributes.Weaving.Storage;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class StorageServiceTest
  {
    private MutableType _mutableType;
    private StorageService _service;

    [SetUp]
    public void SetUp ()
    {
      _mutableType = ObjectMother.GetMutableType();
      _service = new StorageService();
    }

    [Test]
    public void AddInstanceStorage ()
    {
      var type = ObjectMother.GetDeclaringType();

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
      var type = ObjectMother.GetDeclaringType();

      var result = _service.AddStaticStorage (_mutableType, type, "field");

      Assert.That (result, Is.TypeOf<StaticStorage>());
      var addedField = _mutableType.AddedFields.Single();
      Assert.That (addedField.Name, Is.StringStarting ("field"));
      Assert.That (addedField.FieldType, Is.EqualTo (type));
      Assert.That (addedField.Attributes, Is.EqualTo (FieldAttributes.Private | FieldAttributes.Static));
      var storageField = PrivateInvoke.GetNonPublicField (result, "_field");
      Assert.That (storageField, Is.SameAs (addedField));
    }

    //[Test]
    //public void AddGlobalStorage ()
    //{
    //  var type = ObjectMother.GetDeclaringType ();

    //  var result = _service.AddGlobalStorage (type);

    //  Assert.That (result, Is.TypeOf<DictionaryStorage> ());
    //  var fakeExpression = ObjectMother.GetExpression();
    //  var storageExpression = result.CreateStorageExpression (fakeExpression);
    //  Assert.That (storageExpression.Type, Is.EqualTo (type));
    //}

    [Test]
    public void Resolution ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<IStorageService>();

      Assert.That (instance, Is.TypeOf<StorageService>());
    }
  }
}