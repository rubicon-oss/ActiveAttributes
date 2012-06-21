using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;

using Microsoft.Scripting.Ast;

using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectInitializer
  {
    public Tuple<MutableMethodInfo, MutableFieldInfo> IntroduceInitialization (
        MutableType mutableType, MutableMethodInfo methodInfo, IEnumerable<AspectAttribute> aspectAttributes)
    {
      var staticAspectsAttributes = aspectAttributes.Where (x => x.Scope == AspectScope.Static).ToList();
      var staticAspectsArrayField = mutableType.AddField (typeof (AspectAttribute[]), "_s_aspects_for_" + methodInfo.Name, FieldAttributes.Static);
      var staticAspectsFlagField = mutableType.AddField (typeof (bool), "_s_aspects_for_" + methodInfo.Name + "_init", FieldAttributes.Static);
      var staticAspectsInitExpression = Expression.Assign (
          Expression.Field (null, staticAspectsArrayField),
          Expression.NewArrayInit (
              typeof (AspectAttribute),
              staticAspectsAttributes.Select (x => Expression.New (x.GetType())).Cast<Expression>()));

      var allMethodAspectsArrayField = mutableType.AddField (typeof (AspectAttribute[]), "_m_aspects_for_" + methodInfo.Name);
      var allMethodAspectsInitExpression = new Func<BodyContextBase, Expression> (
          ctx => Expression.Assign (
              Expression.Field (ctx.This, allMethodAspectsArrayField),
              Expression.NewArrayInit (
                  typeof (AspectAttribute),
                  aspectAttributes.Select (x => GetInstanceAspectExpression (x, staticAspectsAttributes, staticAspectsArrayField)))));

      foreach (var constructor in mutableType.AllMutableConstructors)
      {
        constructor.SetBody (ctx =>
                             Expression.Block (
                                 staticAspectsInitExpression,
                                 allMethodAspectsInitExpression (ctx),
                                 ctx.GetPreviousBodyWithArguments())
            );
      }

      return new Tuple<MutableMethodInfo, MutableFieldInfo> (methodInfo, allMethodAspectsArrayField);
    }

    private Expression GetInstanceAspectExpression (AspectAttribute aspect, IList<AspectAttribute> staticAspects, FieldInfo staticAspectsArrayField)
    {
      if (!staticAspects.Contains (aspect))
        return Expression.New (aspect.GetType());
      else
        return Expression.ArrayAccess (Expression.Field (null, staticAspectsArrayField), Expression.Constant (staticAspects.IndexOf (aspect)));
    }
  }
}