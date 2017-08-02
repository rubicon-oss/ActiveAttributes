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

using System.Linq;
using ActiveAttributes.Utilities;
using NUnit.Framework;
using Remotion.Collections;

namespace ActiveAttributes.UnitTests.Utilities
{
  [TestFixture]
  public class TopologicalSortTest
  {
    private readonly object _obj1;
    private readonly object _obj2;
    private readonly object _obj3;

    private readonly object[] _enumerable;

    public TopologicalSortTest ()
    {
      _obj1 = 1;
      _obj2 = 2;
      _obj3 = 3;
      _enumerable = new[] { _obj1, _obj2, _obj3 };
    }

    [Test]
    public void Normal ()
    {
      var dependencies = new[]
                         {
                             Tuple.Create (_obj3, _obj2),
                             Tuple.Create (_obj2, _obj1)
                         };

      var expected = new[] { _obj3, _obj2, _obj1 };
      var actual = _enumerable.TopologicalSort (dependencies);

      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void ThrowsExceptionForCircularDependencies ()
    {
      var dependencies = new[]
                         {
                             Tuple.Create (_obj3, _obj2),
                             Tuple.Create (_obj2, _obj1),
                             Tuple.Create (_obj1, _obj3)
                         };

      Assert.That (
          () => _enumerable.TopologicalSort (dependencies),
          Throws.Exception.Matches<CircularDependencyException> (
              exception =>
              {
                var expected = new[] { _obj1, _obj2, _obj3 };
                var actual = exception.Cycles.Single();
                Assert.That (actual, Is.EquivalentTo (expected));
                return true;
              }));
    }

    [Test]
    public void ThrowsExceptionForUndefinedOrder ()
    {
      Assert.That (
          () => _enumerable.TopologicalSort (new Tuple<object, object>[0], throwIfOrderIsUndefined: true),
          Throws.Exception.Matches<UndefinedOrderException> (
              expection =>
              {
                Assert.That (expection.Items, Is.EquivalentTo (_enumerable));
                return true;
              }));
    }
  }
}