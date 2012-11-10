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
using System.Reflection;
using ActiveAttributes.Core.AdviceInfo;
using ActiveAttributes.Core.Assembly.Storages;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.Construction;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public interface IAspectStorageService
  {
    IStorage GetOrAdd (Advice advice, MutableType mutableType);
  }

  public class AspectStorageService : IAspectStorageService
  {
    private readonly IFieldService _fieldService;
    private readonly IAspectInitializationExpressionHelper _initializationExpressionHelper;
    private readonly Dictionary<Tuple<IConstruction, AdviceScope>, IStorage> _fieldWrappers;

    private int _counter;

    public AspectStorageService (IFieldService fieldService, IAspectInitializationExpressionHelper initializationExpressionHelper)
    {
      ArgumentUtility.CheckNotNull ("fieldService", fieldService);
      ArgumentUtility.CheckNotNull ("initializationExpressionHelper", initializationExpressionHelper);

      _fieldService = fieldService;
      _initializationExpressionHelper = initializationExpressionHelper;
      _fieldWrappers = new Dictionary<Tuple<IConstruction, AdviceScope>, IStorage> ();
    }

    public IStorage GetOrAdd (Advice advice, MutableType mutableType)
    {
      ArgumentUtility.CheckNotNull ("advice", advice);
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);

      IStorage field;
      var tuple = Tuple.Create (advice.Construction, advice.Scope);
      if (!_fieldWrappers.TryGetValue (tuple, out field))
      {
        var attributes = FieldAttributes.Private | (advice.Scope == AdviceScope.Static ? FieldAttributes.Static : 0);
        field = _fieldService.AddField (mutableType, advice.DeclaringType, "aspect" + _counter++, attributes);
        var value = _initializationExpressionHelper.CreateInitExpression (advice.Construction);
        AddInitialization (mutableType, field, value);
        _fieldWrappers.Add (tuple, field);
      }

      return field;
    }

    private void AddInitialization (MutableType mutableType, IStorage field, Expression value)
    {
      var constructor = mutableType.AllMutableConstructors.Single ();
      constructor.SetBody (
          ctx =>
          {
            var assignExpression = Expression.Assign (field.GetStorageExpression (ctx.This), value);
            return Expression.Block (typeof(void), ctx.PreviousBody, assignExpression);
          });
    }
  }
}