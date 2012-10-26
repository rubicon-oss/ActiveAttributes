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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Extensions;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Assembly
{
  public interface IAspectExpressionGenerator
  {
    Expression GetInitExpression (IAspectDescriptor aspectDescriptor);
  }

  internal class AspectExpressionGenerator : IAspectExpressionGenerator
  {
    private static MemberBinding GetMemberBindingExpression (ICustomAttributeNamedArgument namedArgument)
    {
      var constantExpression = ConvertTypedArgumentToExpression (namedArgument);
      var bindingExpression = Expression.Bind (namedArgument.MemberInfo, constantExpression);
      return bindingExpression;
    }

    private static Expression ConvertTypedArgumentToExpression (ICustomAttributeNamedArgument typedArgument)
    {
      return typedArgument.ConvertTo (CreateElement, CreateArray);
    }

    private static Expression CreateElement (Type type, object obj)
    {
      // TODO Should not be necessary with TypePipe custom attribute data - if it still is, fix in TypePipe
      if (type.IsEnum)
        obj = Enum.ToObject (type, obj);

      return Expression.Constant (obj, type);
    }

    private static Expression CreateArray (Type type, IEnumerable<Expression> objs)
    {
      return Expression.NewArrayInit (type, objs);
    }

    public Expression GetInitExpression (IAspectDescriptor aspectDescriptor)
    {

      var constructorInfo = aspectDescriptor.ConstructorInfo;
      var constructorArguments = constructorInfo.GetParameters ().Select (x => x.ParameterType).Zip (
          aspectDescriptor.ConstructorArguments, (type, value) => Expression.Constant (value, type)).Cast<Expression> ();
      var createExpression = Expression.New (constructorInfo, constructorArguments.ToArray ());

      var memberBindingExpressions = aspectDescriptor.NamedArguments.Select (GetMemberBindingExpression);

      var initExpression = Expression.MemberInit (createExpression, memberBindingExpressions);

      return initExpression;
    }
  }

  public interface IExpressionGeneratorContainer
  {
    ReadOnlyCollection<IAspectDescriptor> InstanceAspects { get; }
    ReadOnlyCollection<IAspectDescriptor> StaticAspects { get; }
    ReadOnlyDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> AspectStorageInfo { get; }

    IndexExpression GetStorageExpression (IAspectDescriptor aspectDescriptor, Expression thisExpression);

    Expression GetInstanceAspectsInitExpression ();
    Expression GetStaticAspectsAssignExpression ();
  }

  public class ExpressionGeneratorContainer : IExpressionGeneratorContainer
  {
    private readonly ReadOnlyCollection<IAspectDescriptor> _instanceAspects;
    private readonly ReadOnlyCollection<IAspectDescriptor> _staticAspects;
    private readonly ReadOnlyDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> _aspectStorageInfo;

    public ExpressionGeneratorContainer (IEnumerable<IAspectDescriptor> aspectDescriptors, IFieldWrapper instanceField, IFieldWrapper staticField)
    {
      ArgumentUtility.CheckNotNull ("aspectDescriptors", aspectDescriptors);
      ArgumentUtility.CheckNotNull ("instanceField", instanceField);
      ArgumentUtility.CheckNotNull ("staticField", staticField);

      var aspectDescriptorsAsCollection = aspectDescriptors.ConvertToCollection();
      _instanceAspects = aspectDescriptorsAsCollection.Where (x => x.Scope == Scope.Instance).ToList().AsReadOnly();
      _staticAspects = aspectDescriptorsAsCollection.Where (x => x.Scope == Scope.Static).ToList().AsReadOnly();

      var instanceStorageInfo = _instanceAspects.Select ((x, i) => new { Descriptor = x, Info = Tuple.Create (instanceField, i) });
      var staticStorageInfo = _staticAspects.Select ((x, i) => new { Descriptor = x, Info = Tuple.Create (staticField, i) });
      _aspectStorageInfo = instanceStorageInfo.Concat (staticStorageInfo).ToDictionary (x => x.Descriptor, x => x.Info).AsReadOnly();
    }

    public ReadOnlyCollection<IAspectDescriptor> InstanceAspects
    {
      get { return _instanceAspects; }
    }

    public ReadOnlyCollection<IAspectDescriptor> StaticAspects
    {
      get { return _staticAspects; }
    }

    public ReadOnlyDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> AspectStorageInfo
    {
      get { return _aspectStorageInfo; }
    }

    public IndexExpression GetStorageExpression (IAspectDescriptor aspectDescriptor, Expression thisExpression)
    {
      ArgumentUtility.CheckNotNull ("aspectDescriptor", aspectDescriptor);
      ArgumentUtility.CheckNotNull ("thisExpression", thisExpression);

      var storageInfo = AspectStorageInfo[aspectDescriptor];
      var array = storageInfo.Item1.GetAccessExpression (thisExpression);
      var index = Expression.Constant (storageInfo.Item2);
      return Expression.ArrayAccess (array, index);
    }

    public Expression GetInstanceAspectsInitExpression ()
    {
      //return Expression.NewArrayInit(typeof(AspectAttribute), )
      throw new Exception();
    }

    public Expression GetStaticAspectsAssignExpression ()
    {
      throw new NotImplementedException();
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