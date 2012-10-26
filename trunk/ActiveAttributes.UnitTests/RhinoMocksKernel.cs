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
using Ninject.MockingKernel;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests
{
  public class RhinoMocksKernel : MockingKernel
  {
    private readonly bool _autoReplay;
    private readonly MockRepository _mockRepository = new MockRepository();

    public RhinoMocksKernel (bool autoReplay = true)
    {
      _autoReplay = autoReplay;
      _mockRepository = new MockRepository();

      Bind<MockRepository>().ToConstant (_mockRepository);

      Load (new[] { new RhinoMocksModule() });
    }

    public IDisposable Ordered ()
    {
      return _mockRepository.Ordered();
    }

    public IDisposable Unordered ()
    {
      return _mockRepository.Unordered();
    }

    public void VerifyAll ()
    {
      _mockRepository.VerifyAll();
    }

    public RhinoMocksKernel ReplayAll ()
    {
      _mockRepository.ReplayAll();
      return this;
    }

    private MockRepository TryGetRepository (object obj)
    {
      try
      {
        return obj.GetMockRepository();
      }
      catch (Exception)
      {
        return null;
      }
    }

    public RhinoMocksKernel With<T> (T obj)
    {
      Rebind<T>().ToConstant (obj);
      if (TryGetRepository (obj) == _mockRepository && _autoReplay)
        _mockRepository.Replay (obj);
      return this;
    }

    public RhinoMocksKernel WithStrict<T> (out T mock)
    {
      mock = _mockRepository.StrictMock<T>();
      return With (mock);
    }

    public RhinoMocksKernel WithStrict<T> ()
    {
      T mock;
      return WithStrict (out mock);
    }

    public RhinoMocksKernel WithDynamic<T> (out T mock) where T : class
    {
      mock = _mockRepository.DynamicMock<T>();
      return With (mock);
    }

    public RhinoMocksKernel WithDynamic<T> () where T : class
    {
      T mock;
      return WithDynamic (out mock);
    }

    public RhinoMocksKernel WithPartial<T> (out T mock, params object[] constructorArguments)
        where T : class
    {
      mock = _mockRepository.PartialMock<T> (constructorArguments);
      return With (mock);
    }

    public RhinoMocksKernel WithPartial<T> (params object[] constructorArguments) where T : class
    {
      T mock;
      return WithPartial (out mock, constructorArguments);
    }

    public RhinoMocksKernel WithStub<T> (out T stub)
    {
      stub = _mockRepository.Stub<T>();
      return With (stub);
    }

    public RhinoMocksKernel WithStub<T> (Action<T> stubbing)
    {
      T stub;
      WithStub (out stub);
      stubbing (stub);
      return this;
    }
  }
}