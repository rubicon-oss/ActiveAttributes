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
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Infrastructure;
using Remotion.Collections;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  [ConcreteImplementation (typeof (MethodExpressionHelperFactory))]
  public interface IMethodExpressionHelperFactory
  {
    IMethodExpressionHelper CreateMethodExpressionHelper (
        MutableMethodInfo method,
        BodyContextBase context,
        IDictionary<Advice, IFieldWrapper> adviceDictionary);
  }

  public class MethodExpressionHelperFactory : IMethodExpressionHelperFactory
  {
    private readonly InvocationExpressionHelper _invocationExpressionHelper;

    public MethodExpressionHelperFactory (InvocationExpressionHelper invocationExpressionHelper)
    {
      ArgumentUtility.CheckNotNull ("invocationExpressionHelper", invocationExpressionHelper);

      _invocationExpressionHelper = invocationExpressionHelper;
    }

    public IMethodExpressionHelper CreateMethodExpressionHelper (MutableMethodInfo method, BodyContextBase context, IDictionary<Advice, IFieldWrapper> adviceDictionary)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("adviceDictionary", adviceDictionary);

      return new MethodExpressionHelper (context, adviceDictionary, _invocationExpressionHelper);
    }
  }
}