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
using ActiveAttributes.Advices;
using ActiveAttributes.Infrastructure.Ordering;
using ActiveAttributes.Ordering;
using ActiveAttributes.Ordering.Providers;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.ServiceLocation;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace ActiveAttributes.UnitTests.Ordering
{
  [TestFixture]
  public class AdviceDependencyProviderTest
  {
    private AdviceDependencyProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new AdviceDependencyProvider (Enumerable.Empty<IAdviceOrderingProvider>());
    }

    [Test]
    public void GetDependencies ()
    {
      var advice1 = ObjectMother.GetAdvice (name: "1");
      var advice2 = ObjectMother.GetAdvice (name: "2");
      var advice3 = ObjectMother.GetAdvice (name: "3");

      var orderingProviderMock1 = MockRepository.GenerateStrictMock<IAdviceOrderingProvider>();
      var orderingProviderMock2 = MockRepository.GenerateStrictMock<IAdviceOrderingProvider>();
      var orderingMock1 = MockRepository.GenerateStrictMock<IOrdering>();
      var orderingMock2 = MockRepository.GenerateStrictMock<IOrdering>();
      var orderingMock3 = MockRepository.GenerateStrictMock<IOrdering>();

      orderingProviderMock1.Expect (x => x.GetOrderings()).Return (new[] { orderingMock1, orderingMock2 }.AsOneTime());
      orderingProviderMock2.Expect (x => x.GetOrderings()).Return (new[] { orderingMock3 }.AsOneTime());

      var provider = new AdviceDependencyProvider (new[] { orderingProviderMock1, orderingProviderMock2 });

      orderingMock1.Expect (x => x.DependVisit (provider, advice1, advice2)).Return (false);
      orderingMock1.Expect (x => x.DependVisit (provider, advice2, advice1)).Return (true);
      orderingMock1.Expect (x => x.DependVisit (provider, advice2, advice3)).Return (false);
      orderingMock1.Expect (x => x.DependVisit (provider, advice3, advice2)).Return (false);
      orderingMock1.Expect (x => x.DependVisit (provider, advice1, advice3)).Return (false);
      orderingMock1.Expect (x => x.DependVisit (provider, advice3, advice1)).Return (false);

      orderingMock2.Expect (x => x.DependVisit (provider, advice1, advice2)).Return (false);
      orderingMock2.Expect (x => x.DependVisit (provider, advice2, advice1)).Return (false);
      orderingMock2.Expect (x => x.DependVisit (provider, advice2, advice3)).Return (false);
      orderingMock2.Expect (x => x.DependVisit (provider, advice3, advice2)).Return (false);
      orderingMock2.Expect (x => x.DependVisit (provider, advice1, advice3)).Return (true);
      orderingMock2.Expect (x => x.DependVisit (provider, advice3, advice1)).Return (false);

      orderingMock3.Expect (x => x.DependVisit (provider, advice1, advice2)).Return (false);
      orderingMock3.Expect (x => x.DependVisit (provider, advice2, advice1)).Return (false);
      orderingMock3.Expect (x => x.DependVisit (provider, advice2, advice3)).Return (true);
      orderingMock3.Expect (x => x.DependVisit (provider, advice3, advice2)).Return (false);
      orderingMock3.Expect (x => x.DependVisit (provider, advice1, advice3)).Return (false);
      orderingMock3.Expect (x => x.DependVisit (provider, advice3, advice1)).Return (false);

      var result = provider.GetDependencies (new[] { advice1, advice2, advice3 }).ToArray();

      orderingProviderMock1.VerifyAllExpectations();
      orderingProviderMock2.VerifyAllExpectations();
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
      var advice1 = ObjectMother.GetAdvice (name: "1");
      var advice2 = ObjectMother.GetAdvice (name: "2");

      var orderingProviderMock = MockRepository.GenerateStrictMock<IAdviceOrderingProvider> ();
      var orderingMock1 = MockRepository.GenerateStrictMock<IOrdering>();
      var orderingMock2 = MockRepository.GenerateStrictMock<IOrdering>();
      orderingProviderMock.Expect (x => x.GetOrderings()).Return (new[] { orderingMock1, orderingMock2 });

      var provider = new AdviceDependencyProvider (new[] { orderingProviderMock });

      orderingMock1.Expect (x => x.DependVisit (provider, advice1, advice2)).Return (true);
      orderingMock1.Expect (x => x.DependVisit (provider, advice2, advice1)).Return (false);

      orderingMock2.Expect (x => x.DependVisit (provider, advice1, advice2)).Return (true);
      orderingMock2.Expect (x => x.DependVisit (provider, advice2, advice1)).Return (false);

      var result = provider.GetDependencies (new[] { advice1, advice2 }).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Test]
    public void VisitType ()
    {
      CheckDepends (_provider.DependsType, new AspectTypeOrdering (typeof (A), typeof (int), "s"), GetAdvice (typeof (A)), GetAdvice (typeof (int)));
      CheckDepends (_provider.DependsType, new AspectTypeOrdering (typeof (A), typeof (int), "s"), GetAdvice (typeof (B)), GetAdvice (typeof (int)));
      CheckDepends (_provider.DependsType, new AspectTypeOrdering (typeof (int), typeof (A), "s"), GetAdvice (typeof (int)), GetAdvice (typeof (B)));
      CheckDependsNot (_provider.DependsType, new AspectTypeOrdering (typeof (int), typeof (B), "s"), GetAdvice (typeof (int)), GetAdvice (typeof (A)));
    }

    [Test]
    public void VisitRole ()
    {
      CheckDepends (_provider.DependsRole, new AspectRoleOrdering ("A", "B", "s"), GetAdvice (role: "A"), GetAdvice (role: "B"));
      CheckDepends (_provider.DependsRole, new AspectRoleOrdering ("A*", "B", "s"), GetAdvice (role: "Atest"), GetAdvice (role: "B"));
      CheckDepends (_provider.DependsRole, new AspectRoleOrdering ("A", "B*", "s"), GetAdvice (role: "A"), GetAdvice (role: "Btest"));
      CheckDependsNot (_provider.DependsRole, new AspectRoleOrdering ("A", "B", "s"), GetAdvice (role: "A"), GetAdvice (role: "C"));
    }

    [Test]
    public void Resolution ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<IAdviceDependencyProvider>();

      Assert.That (instance, Is.TypeOf<AdviceDependencyProvider>());
    }

    private void CheckDepends<T> (Func<Advice, Advice, T, bool> dependsFunc, T ordering, Advice advice1, Advice advice2)
        where T : IOrdering
    {
      Assert.That (dependsFunc (advice1, advice2, ordering), Is.True);
    }

    private void CheckDependsNot<T> (Func<Advice, Advice, T, bool> dependsFunc, T ordering, Advice advice1, Advice advice2)
        where T : IOrdering
    {
      Assert.That (dependsFunc (advice1, advice2, ordering), Is.False);
    }

    private Advice GetAdvice (Type declaringType = null, string role = null)
    {
      var method = ObjectMother.GetMethodInfo (declaringType: declaringType);

      return ObjectMother.GetAdvice (method: method, role: role);
    }

    class A {}
    class B : A {}
  }
}