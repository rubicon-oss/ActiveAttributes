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
using System.Collections.Generic;
using System.Linq;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class CachingAspectTest : TestBase
  {
    private DomainType _instance;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

    }

    [Test]
    public void Cache ()
    {
      var type = AssembleType<DomainType> (TypeAssembler.Singleton.ModifyType);
      _instance = (DomainType) Activator.CreateInstance (type);
      SkipDeletion ();
      var input = "a";

      var result1 = _instance.Method (input);
      var result2 = _instance.Method (input);

      Assert.That (_instance.MethodExecutionCounter, Is.EqualTo (1));
      Assert.That (result1, Is.EqualTo (result2));
    }

    public class DomainType
    {
      public int MethodExecutionCounter { get; private set; }

      [CacheAspect]
      public virtual string Method (string arg)
      {
        MethodExecutionCounter++;
        return arg + "_computed";
      }
    }

    public class CacheAspectAttribute : MethodInterceptionAspectAttribute
    {
      private readonly ICache _cache;

      public CacheAspectAttribute ()
      {
        _cache = new SimpleCache();
      }

      public override void OnIntercept (IInvocation invocation)
      {
        var ctx = invocation.Context;
        var key = _cache.GenerateKey (ctx);
        if (!_cache.Contains (key))
        {
          invocation.Proceed ();
          _cache[key] = ctx.ReturnValue;
        }
        else
        {
          ctx.ReturnValue = _cache[key];
        }
      }
    }

    public interface ICache
    {
      object GenerateKey (IInvocationContext ctx);
      bool Contains (object key);
      object this [object key] { get; set; }
    }

    public class SimpleCache : ICache
    {
      private readonly IDictionary<object, object> _dictionary;

      public SimpleCache ()
      {
        _dictionary = new Dictionary<object, object>();
      }

      public object GenerateKey (IInvocationContext ctx)
      {
        return ctx.Arguments.Aggregate (ctx.MethodInfo.GetHashCode(), (current, arg) => current ^ arg.GetHashCode());
      }

      public object this [object key]
      {
        get { return _dictionary[key]; }
        set { _dictionary[key] = value; }
      }

      public bool Contains (object key)
      {
        return _dictionary.ContainsKey (key);
      }
    }
  }
}