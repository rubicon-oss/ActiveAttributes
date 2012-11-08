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
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Ordering;
using NUnit.Framework;
using Remotion.Collections;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Ordering
{
  [TestFixture]
  public class AdviceDependencyProviderTest
  {
    private AdviceDependencyProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new AdviceDependencyProvider (Enumerable.Empty<IAdviceOrdering>());
    }

    [Test]
    public void GetDependencies ()
    {
      var advice1 = ObjectMother2.GetAdvice (name: "1");
      var advice2 = ObjectMother2.GetAdvice (name: "2");
      var advice3 = ObjectMother2.GetAdvice (name: "3");

      var orderingMock1 = MockRepository.GenerateStrictMock<IAdviceOrdering>();
      var orderingMock2 = MockRepository.GenerateStrictMock<IAdviceOrdering>();
      var orderingMock3 = MockRepository.GenerateStrictMock<IAdviceOrdering>();

      var provider = new AdviceDependencyProvider (new[] { orderingMock1, orderingMock2, orderingMock3 });

      orderingMock1.Expect (x => x.Depends (provider, advice1, advice2)).Return (false);
      orderingMock1.Expect (x => x.Depends (provider, advice2, advice1)).Return (true);
      orderingMock1.Expect (x => x.Depends (provider, advice2, advice3)).Return (false);
      orderingMock1.Expect (x => x.Depends (provider, advice3, advice2)).Return (false);
      orderingMock1.Expect (x => x.Depends (provider, advice1, advice3)).Return (false);
      orderingMock1.Expect (x => x.Depends (provider, advice3, advice1)).Return (false);

      orderingMock2.Expect (x => x.Depends (provider, advice1, advice2)).Return (false);
      orderingMock2.Expect (x => x.Depends (provider, advice2, advice1)).Return (false);
      orderingMock2.Expect (x => x.Depends (provider, advice2, advice3)).Return (false);
      orderingMock2.Expect (x => x.Depends (provider, advice3, advice2)).Return (false);
      orderingMock2.Expect (x => x.Depends (provider, advice1, advice3)).Return (true);
      orderingMock2.Expect (x => x.Depends (provider, advice3, advice1)).Return (false);

      orderingMock3.Expect (x => x.Depends (provider, advice1, advice2)).Return (false);
      orderingMock3.Expect (x => x.Depends (provider, advice2, advice1)).Return (false);
      orderingMock3.Expect (x => x.Depends (provider, advice2, advice3)).Return (true);
      orderingMock3.Expect (x => x.Depends (provider, advice3, advice2)).Return (false);
      orderingMock3.Expect (x => x.Depends (provider, advice1, advice3)).Return (false);
      orderingMock3.Expect (x => x.Depends (provider, advice3, advice1)).Return (false);

      var result = provider.GetDependencies (new[] { advice1, advice2, advice3 }).ToArray();

      orderingMock1.VerifyAllExpectations();
      orderingMock2.VerifyAllExpectations();
      orderingMock3.VerifyAllExpectations();
      var dependency1 = Tuple.Create (advice2, advice1);
      var dependency2 = Tuple.Create (advice1, advice3);
      var dependency3 = Tuple.Create (advice2, advice3);
      Assert.That (result, Is.EquivalentTo (new[] { dependency1, dependency2, dependency3 }));
    }

    [Test]
    public void GetDependencies_HashSet ()
    {
      var advice1 = ObjectMother2.GetAdvice (name: "1");
      var advice2 = ObjectMother2.GetAdvice (name: "2");

      var orderingMock1 = MockRepository.GenerateStrictMock<IAdviceOrdering>();
      var orderingMock2 = MockRepository.GenerateStrictMock<IAdviceOrdering>();

      var provider = new AdviceDependencyProvider (new[] { orderingMock1, orderingMock2 });

      orderingMock1.Expect (x => x.Depends (provider, advice1, advice2)).Return (true);
      orderingMock1.Expect (x => x.Depends (provider, advice2, advice1)).Return (false);

      orderingMock2.Expect (x => x.Depends (provider, advice1, advice2)).Return (true);
      orderingMock2.Expect (x => x.Depends (provider, advice2, advice1)).Return (false);

      var result = provider.GetDependencies (new[] { advice1, advice2 }).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Test]
    public void VisitType ()
    {
      CheckDepends (_provider.DependsType, new AdviceTypeOrdering (typeof (A), typeof (int), "s"), GetAdvice (typeof (A)), GetAdvice (typeof (int)));
      CheckDepends (_provider.DependsType, new AdviceTypeOrdering (typeof (A), typeof (int), "s"), GetAdvice (typeof (B)), GetAdvice (typeof (int)));
      CheckDepends (_provider.DependsType, new AdviceTypeOrdering (typeof (int), typeof (A), "s"), GetAdvice (typeof (int)), GetAdvice (typeof (B)));
      CheckDependsNot (_provider.DependsType, new AdviceTypeOrdering (typeof (int), typeof (B), "s"), GetAdvice (typeof (int)), GetAdvice (typeof (A)));
    }

    [Test]
    public void VisitRole ()
    {
      CheckDepends (_provider.DependsRole, new AdviceRoleOrdering ("A", "B", "s"), GetAdvice (role: "A"), GetAdvice (role: "B"));
      CheckDepends (_provider.DependsRole, new AdviceRoleOrdering ("A*", "B", "s"), GetAdvice (role: "Atest"), GetAdvice (role: "B"));
      CheckDepends (_provider.DependsRole, new AdviceRoleOrdering ("A", "B*", "s"), GetAdvice (role: "A"), GetAdvice (role: "Btest"));
      CheckDependsNot (_provider.DependsRole, new AdviceRoleOrdering ("A", "B", "s"), GetAdvice (role: "A"), GetAdvice (role: "C"));
    }

    private void CheckDepends<T> (Func<Advice, Advice, T, bool> dependsFunc, T ordering, Advice advice1, Advice advice2)
        where T : IAdviceOrdering
    {
      Assert.That (dependsFunc (advice1, advice2, ordering), Is.True);
    }

    private void CheckDependsNot<T> (Func<Advice, Advice, T, bool> dependsFunc, T ordering, Advice advice1, Advice advice2)
        where T : IAdviceOrdering
    {
      Assert.That (dependsFunc (advice1, advice2, ordering), Is.False);
    }

    private Advice GetAdvice (Type declaringType = null, string role = null)
    {
      var method = ObjectMother2.GetMethodInfo (declaringType: declaringType);

      return ObjectMother2.GetAdvice (method: method, role: role);
    }

    class A {}
    class B : A {}
  }
}