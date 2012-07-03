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
// 
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Aspects
{
  [TestFixture]
  public class AspectAttributeTest
  {
    [Serializable]
    private class TestableAspectAttribute : MethodInterceptionAspectAttribute
    {
      public TestableAspectAttribute () {}

      protected TestableAspectAttribute (SerializationInfo info, StreamingContext context)
          : base (info, context) {}

      public override void OnIntercept (IInvocation invocation)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void Clone ()
    {
      var aspect = new TestableAspectAttribute { Scope = AspectScope.Instance };
      var copy = (MethodInterceptionAspectAttribute) aspect.Clone();

      Assert.That (copy.Scope, Is.EqualTo (AspectScope.Instance));
    }

    [Test]
    public void Serialize ()
    {
      var aspect = new TestableAspectAttribute
                   {
                       Scope = AspectScope.Instance,
                       Priority = 10
                   };
      var formatter = new BinaryFormatter();
      var memoryStream = new MemoryStream();

      formatter.Serialize (memoryStream, aspect);
      memoryStream.Position = 0;
      var copy = (MethodInterceptionAspectAttribute) formatter.Deserialize (memoryStream);

      Assert.That (copy.Scope, Is.EqualTo (aspect.Scope));
      Assert.That (copy.Priority, Is.EqualTo (aspect.Priority));
    }
  }
}