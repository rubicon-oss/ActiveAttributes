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
using System.Collections.ObjectModel;
using System.Linq;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Configuration2;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public class ExpressionGeneratorFactory : IExpressionGeneratorFactory
  {
    public IEnumerable<IExpressionGenerator> GetExpressionGenerators (
        IArrayAccessor instanceAccessor, IArrayAccessor staticAccessor, IEnumerable<IAspectDescriptor> descriptors)
    {
      ArgumentUtility.CheckNotNull ("instanceAccessor", instanceAccessor);
      ArgumentUtility.CheckNotNull ("staticAccessor", staticAccessor);
      ArgumentUtility.CheckNotNull ("descriptors", descriptors);
      Assertion.IsTrue (!instanceAccessor.IsStatic);
      Assertion.IsTrue (staticAccessor.IsStatic);

      var aspectsAsCollection = descriptors.ConvertToCollection();
      var instanceGenerators = GetExpressionGenerators (instanceAccessor, aspectsAsCollection, Scope.Instance);
      var staticGenerators = GetExpressionGenerators (staticAccessor, aspectsAsCollection, Scope.Static);

      return instanceGenerators.Concat (staticGenerators);
    }

    private IEnumerable<IExpressionGenerator> GetExpressionGenerators (IArrayAccessor accessor, IEnumerable<IAspectDescriptor> aspects, Scope scope)
    {
      return aspects.Where (x => x.Scope == scope).Select ((x, i) => new ExpressionGenerator (accessor, i, x)).Cast<IExpressionGenerator>();
    }
  }

  //public class ExpressionGeneratorContainer
  //{
  //  private readonly ReadOnlyCollection<IAspectDescriptor> _instanceAspects;
  //  private readonly ReadOnlyCollection<IAspectDescriptor> _staticAspects;
  //  private readonly ReadOnlyDictionary<IAspectDescriptor, Tuple<IArrayAccessor, int>> _aspectStorageInfo;

  //  public ExpressionGeneratorContainer (IEnumerable<IAspectDescriptor> aspects, IArrayAccessor instanceAccessor, IArrayAccessor staticAccessor)
  //  {
  //    var aspectCollection = aspects.ConvertToCollection();
  //    _instanceAspects = aspectCollection.Where (d => d.Scope == Scope.Instance).ToList ().AsReadOnly ();
  //    _staticAspects = aspectCollection.Where (d => d.Scope == Scope.Static).ToList ().AsReadOnly();

  //    _aspectStorageInfo = 
  //        _instanceAspects.Select ((d, i) => new { Descriptor = d, Tuple = Tuple.Create (instanceAccessor, i) })
  //        .Concat (_staticAspects.Select ((d, i) => new { Descriptor = d, Tuple = Tuple.Create (staticAccessor, i) }))
  //        .ToDictionary (t => t.Descriptor, t => t.Tuple)
  //        .AsReadOnly();
  //  }
    
  //  public ReadOnlyCollection<IAspectDescriptor> InstanceAspects
  //  {
  //    get { return _instanceAspects; }
  //  }

  //  public ReadOnlyCollection<IAspectDescriptor> StaticAspects
  //  {
  //    get { return _staticAspects; }
  //  }

  //  public ReadOnlyDictionary<IAspectDescriptor, Tuple<IArrayAccessor, int>> AspectStorageInfo
  //  {
  //    get { return _aspectStorageInfo; }
  //  }

  //  public Expression GetStorageExpression (IAspectDescriptor descriptor, Expression thisExpression)
  //  {
  //    var aspectTuple = _aspectStorageInfo[descriptor];
  //    return GetStorageExpression (thisExpression, aspectTuple.Item1, aspectTuple.Item2);
  //  }

  //  private Expression GetStorageExpression (Expression thisExpression, IArrayAccessor arrayAccessor, int index)
  //  {
  //    // TODO Inline ExpressionGenerator.GetStorageExpression here
  //    throw new NotImplementedException();
  //  }

  //  public Expression GetNewInstanceArrayInitExpression ()
  //  {
  //    // TODO Inline ExpressionGenerator.GetInitExpression and a part of ConstructorPatcher here
  //    return Expression.NewArrayInit (...);
  //  }

  //  public Expression GetNewStaticArrayInitExpression ()
  //  {
  //    return Expression.NewArrayInit (...);
  //  }
  //}
}