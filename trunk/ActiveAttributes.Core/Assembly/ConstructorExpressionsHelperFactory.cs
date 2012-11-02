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
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  [ConcreteImplementation (typeof (ConstructorExpressionsHelperFactory))]
  public interface IConstructorExpressionsHelperFactory
  {
    IConstructorExpressionsHelper CreateConstructorExpressionHelper (BodyContextBase bodyContextBase);
  }

  public class ConstructorExpressionsHelperFactory : IConstructorExpressionsHelperFactory
  {
    private readonly AspectInitExpressionHelper _aspectInitExpressionHelper;

    public ConstructorExpressionsHelperFactory (AspectInitExpressionHelper aspectInitExpressionHelper)
    {
      ArgumentUtility.CheckNotNull ("aspectInitExpressionHelper", aspectInitExpressionHelper);

      _aspectInitExpressionHelper = aspectInitExpressionHelper;
    }

    public IConstructorExpressionsHelper CreateConstructorExpressionHelper (BodyContextBase bodyContextBase)
    {
      ArgumentUtility.CheckNotNull ("bodyContextBase", bodyContextBase);

      return new ConstructorExpressionsHelper (_aspectInitExpressionHelper, bodyContextBase);
    }
  }
}