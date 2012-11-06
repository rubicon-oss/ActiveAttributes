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
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Invocations
{
  [TestFixture]
  public class IndexerSetInvocationTest
  {
    [Test]
    public void Initialization ()
    {
      var contextStub = MockRepository.GenerateStub<IndexerSetInvocationContext<object, int, int>> (null, null, null, null, null);
      var flag = false;
      var action = new Action<int, int> ((i, j) => flag = true);
      IInvocation invocation = new IndexerSetInvocation<object, int, int> (contextStub, action);

      invocation.Proceed ();

      Assert.That (invocation.Context, Is.SameAs (contextStub));
      Assert.That (flag, Is.True);
    }
  }
}