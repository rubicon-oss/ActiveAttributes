﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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

using ActiveAttributes.Core.Utilities;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Utilities
{
  [TestFixture]
  public class CircularDependencyExceptionTest
  {
    [Test]
    public void Normal ()
    {
      object[][] dependencies = new[]
                                {
                                    new object[] { 1, 2, 3 },
                                    new object[] { 2, 3, 4 }
                                };

      var exception = new CircularDependencyException<object> (dependencies);

      var expected = "Circular dependencies detected:\r\n" +
                     "1 -> 2 -> 3\r\n" +
                     "2 -> 3 -> 4";
      var actual = exception.Message;

      Assert.That (actual, Is.EqualTo (expected));
    }
  }
}