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
using ActiveAttributes.Advices;
using ActiveAttributes.Aspects;
using ActiveAttributes.Declaration;
using ActiveAttributes.Declaration.Construction;
using ActiveAttributes.Pointcuts;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Rhino.Mocks;
using System.Linq;

namespace ActiveAttributes.UnitTests.Declaration
{
  [TestFixture]
  public class CustomAttributeDataTransformTest
  {
    [Test]
    public void UpdateAdviceBuilders ()
    {
      var adviceBuilderMock1 = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      var adviceBuilderMock2 = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      var adviceBuilderMocks = new[] { adviceBuilderMock1, adviceBuilderMock2 };

      var namedArguments =
          new[]
          {
              ObjectMother.GetCustomAttributeNamedArgument ("AdvicePriority", 4),
              ObjectMother.GetCustomAttributeNamedArgument ("AdviceExecution", AdviceExecution.Around),
              ObjectMother.GetCustomAttributeNamedArgument ("AdviceScope", AdviceScope.Static),
              ObjectMother.GetCustomAttributeNamedArgument ("AdviceName", "name"),
              ObjectMother.GetCustomAttributeNamedArgument ("AdviceRole", "role"),
              //
              ObjectMother.GetCustomAttributeNamedArgument ("ApplyToType", typeof (string)),
              ObjectMother.GetCustomAttributeNamedArgument ("ApplyToTypeName", "typeName"),
              ObjectMother.GetCustomAttributeNamedArgument ("ApplyToNamespace", "namespace"),
              //
              ObjectMother.GetCustomAttributeNamedArgument ("MemberNameFilter", "memberName"),
              ObjectMother.GetCustomAttributeNamedArgument ("MemberReturnTypeFilter", typeof (int)),
              ObjectMother.GetCustomAttributeNamedArgument ("MemberArgumentTypesFilter", new[] { typeof (string) }),
              ObjectMother.GetCustomAttributeNamedArgument ("MemberVisibilityFilter", Visibility.Family),
              ObjectMother.GetCustomAttributeNamedArgument ("MemberCustomAttributeFilter", typeof(Attribute)),
          };
      var customAttributeData = ObjectMother.GetCustomAttributeData (typeof(AspectAttributeBase), namedArguments);

      adviceBuilderMock1.Expect (x => x.SetConstruction (Arg<AttributeConstruction>.Is.TypeOf)).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.SetName ("name")).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.SetRole ("role")).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.SetExecution (AdviceExecution.Around)).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.SetScope (AdviceScope.Static)).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.SetPriority (4)).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.AddPointcut (null)).IgnoreArguments ().Return (adviceBuilderMock1).Repeat.Any ();
      adviceBuilderMock2.Expect (x => x.SetConstruction (Arg<AttributeConstruction>.Is.TypeOf)).Return (adviceBuilderMock2);
      adviceBuilderMock2.Expect (x => x.SetName ("name")).Return (adviceBuilderMock2);
      adviceBuilderMock2.Expect (x => x.SetRole ("role")).Return (adviceBuilderMock2);
      adviceBuilderMock2.Expect (x => x.SetExecution (AdviceExecution.Around)).Return (adviceBuilderMock2);
      adviceBuilderMock2.Expect (x => x.SetScope (AdviceScope.Static)).Return (adviceBuilderMock2);
      adviceBuilderMock2.Expect (x => x.SetPriority (4)).Return (adviceBuilderMock2);
      adviceBuilderMock2.Expect (x => x.AddPointcut (null)).IgnoreArguments ().Return (adviceBuilderMock2).Repeat.Any ();

      var result = new CustomAttributeDataTransform ().UpdateAdviceBuilders (customAttributeData, adviceBuilderMocks);

      var construction1 = adviceBuilderMock1.GetArgumentsForCallsMadeOn (x => x.SetConstruction (null))[0][0];
      var construction2 = adviceBuilderMock2.GetArgumentsForCallsMadeOn (x => x.SetConstruction (null))[0][0];
      var pointcuts = adviceBuilderMock1.GetArgumentsForCallsMadeOn (x => x.AddPointcut (null)).Select (x => (IPointcut) x[0]);
      adviceBuilderMock1.VerifyAllExpectations ();
      adviceBuilderMock2.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (adviceBuilderMocks));
      Assert.That (construction1, Is.SameAs (construction2));

      Assert.That (pointcuts, Has.Some.TypeOf<TypePointcut>().With.Property ("Type").EqualTo (typeof (string)));
      Assert.That (pointcuts, Has.Some.TypeOf<TypeNamePointcut>().With.Property ("TypeName").EqualTo ("typeName"));
      Assert.That (pointcuts, Has.Some.TypeOf<NamespacePointcut>().With.Property ("Namespace").EqualTo ("namespace"));

      Assert.That (pointcuts, Has.Some.TypeOf<MemberNamePointcut>().With.Property ("MemberName").EqualTo ("memberName"));
      Assert.That (pointcuts, Has.Some.TypeOf<ReturnTypePointcut>().With.Property ("ReturnType").EqualTo (typeof (int)));
      Assert.That (pointcuts, Has.Some.TypeOf<ArgumentTypesPointcut>().With.Property ("ArgumentTypes").EqualTo (new[] { typeof (string) }));
      Assert.That (pointcuts, Has.Some.TypeOf<VisibilityPointcut>().With.Property ("Visibility").EqualTo (Visibility.Family));
      Assert.That (pointcuts, Has.Some.TypeOf<CustomAttributePointcut>().With.Property ("CustomAttributeType").EqualTo (typeof (Attribute)));
    }

    [Test]
    public void Resolution ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<ICustomAttributeDataTransform>();

      Assert.That (instance, Is.TypeOf<CustomAttributeDataTransform>());
    }
  }
}