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
using ActiveAttributes.Core.AdviceInfo;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.Construction;
using ActiveAttributes.Core.Pointcuts;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace ActiveAttributes.UnitTests.Discovery
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
              ObjectMother2.GetCustomAttributeNamedArgument ("Priority", 4),
              ObjectMother2.GetCustomAttributeNamedArgument ("Execution", AdviceExecution.Around),
              ObjectMother2.GetCustomAttributeNamedArgument ("Scope", AdviceScope.Static),
              ObjectMother2.GetCustomAttributeNamedArgument ("Name", "name"),
              ObjectMother2.GetCustomAttributeNamedArgument ("Role", "role"),
              //
              ObjectMother2.GetCustomAttributeNamedArgument ("ApplyToType", typeof (string)),
              ObjectMother2.GetCustomAttributeNamedArgument ("ApplyToTypeName", "typeName"),
              ObjectMother2.GetCustomAttributeNamedArgument ("ApplyToNamespace", "namespace"),
              //
              ObjectMother2.GetCustomAttributeNamedArgument ("MemberNameFilter", "memberName"),
              ObjectMother2.GetCustomAttributeNamedArgument ("MemberReturnTypeFilter", typeof (int)),
              ObjectMother2.GetCustomAttributeNamedArgument ("MemberArgumentFilter", typeof (string)),
              ObjectMother2.GetCustomAttributeNamedArgument ("MemberVisibilityFilter", Visibility.Family),
              ObjectMother2.GetCustomAttributeNamedArgument ("MemberCustomAttributeFilter", typeof(Attribute)),
          };
      var customAttributeData = ObjectMother2.GetCustomAttributeData (typeof(AspectAttributeBase), namedArguments);

      adviceBuilderMock1.Expect (x => x.SetConstruction (Arg<CustomAttributeDataConstruction>.Is.TypeOf)).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.SetName ("name")).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.SetRole ("role")).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.SetExecution (AdviceExecution.Around)).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.SetScope (AdviceScope.Static)).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.SetPriority (4)).Return (adviceBuilderMock1);
      adviceBuilderMock1.Expect (x => x.AddPointcut (null)).IgnoreArguments ().Return (adviceBuilderMock1).Repeat.Any ();
      adviceBuilderMock2.Expect (x => x.SetConstruction (Arg<CustomAttributeDataConstruction>.Is.TypeOf)).Return (adviceBuilderMock2);
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
      Assert.That (pointcuts, Has.Some.TypeOf<ArgumentTypePointcut>().With.Property ("ArgumentType").EqualTo (typeof (string)));
      Assert.That (pointcuts, Has.Some.TypeOf<VisibilityPointcut>().With.Property ("Visibility").EqualTo (Visibility.Family));
      Assert.That (pointcuts, Has.Some.TypeOf<CustomAttributePointcut>().With.Property ("CustomAttributeType").EqualTo (typeof (Attribute)));

    }
  }
}