// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;

namespace ActiveAttributes.Core.Extensions
{
  public static class MethodInfoExtensions
  {
    public static Type GetDelegateType (this MethodInfo methodInfo, Type instanceType = null)
    {
      var instanceType2 = instanceType != null
                              ? new[] { instanceType }
                              : new Type[0];

      var parameterTypes = methodInfo.GetParameters ().Select (x => x.ParameterType);
      var returnType = new[] { methodInfo.ReturnType };

      var delegateTypes = instanceType2.Concat (parameterTypes).Concat (returnType).ToArray ();
      return Expression.GetDelegateType (delegateTypes);
    }

    public static Type GetDelegateType (this MethodInfo methodInfo)
    {
      var parameterTypes = methodInfo.GetParameters ().Select (x => x.ParameterType);
      var returnType = new[] { methodInfo.ReturnType };

      var delegateTypes = parameterTypes.Concat (returnType).ToArray ();
      return Expression.GetDelegateType (delegateTypes);
    } 
  }
}