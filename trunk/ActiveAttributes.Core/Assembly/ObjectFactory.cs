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
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;

namespace ActiveAttributes.Assembly
{
  public static class ObjectFactory
  {
    private static readonly Remotion.TypePipe.ObjectFactory s_objectFactory;

    static ObjectFactory ()
    {
      var assembler = SafeServiceLocator.Current.GetInstance<IAssembler>();
      var typeModifier = SafeServiceLocator.Current.GetInstance<ITypeModifier>();
      var typeAssembler = new TypeAssembler (new[] { assembler }, typeModifier);
      var constructorFinder = new ConstructorFinder();
      var delegateFactory = new DelegateFactory();
      var typeCache = new TypeCache (typeAssembler, constructorFinder, delegateFactory);
      s_objectFactory = new Remotion.TypePipe.ObjectFactory (typeCache);
    }

    public static T Create<T> () where T : class
    {
      return s_objectFactory.CreateObject<T>();
    }
  }
}