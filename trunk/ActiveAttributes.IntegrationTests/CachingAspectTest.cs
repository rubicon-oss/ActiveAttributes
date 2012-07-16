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
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;

using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class CachingAspectTest
  {
    [Test]
    public void Cache ()
    {
      var input = "a";

      var instance = ObjectFactory.Create<DomainType>();
      var result1 = instance.Method (input);
      var result2 = instance.Method (input);

      Assert.AreEqual (instance.MethodExecutionCounter, 1);
      Assert.AreEqual (result1, result2);

      // TODO: Move to utility
      //var assemblyFileName = instance.GetType ().Module.ScopeName;
      //((AssemblyBuilder) instance.GetType().Assembly).Save (assemblyFileName);
    }

    public class DomainType
    {
      public int MethodExecutionCounter { get; private set; }

      [CacheAspect("test")]
      [CacheAspect("test")]
      public virtual string Method (string arg)
      {
        MethodExecutionCounter++;
        return arg + "_computed";
      }
    }

    public class CacheAspectAttribute : MethodInterceptionAspectAttribute
    {
      public string Arg { get; set; }
      private object _key;
      private object _value;

      public CacheAspectAttribute (string arg)
      {
        Arg = arg;
      }

      public override void OnIntercept (IInvocation invocation)
      {
        var context = invocation.Context;
        if (_key != context.Arguments[0])
        {
          invocation.Proceed();
          _key = context.Arguments[0];
          _value = context.ReturnValue;
        }
        else
          context.ReturnValue = _value;
      }
    }
  }
}