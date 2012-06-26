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
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectPreparer
  {
    public FieldInfo PrepareAspects (
        MutableType mutableType, MutableMethodInfo methodInfo, IEnumerable<AspectAttribute> aspectAttributes)
    {
      var aspectAttributesAsCollection = aspectAttributes.ConvertToCollection();

      var staticAspectsAttributes = aspectAttributesAsCollection.Where (x => x.Scope == AspectScope.Static).ToList ();
      var staticAspectsArrayField = mutableType.AddField (typeof (AspectAttribute[]), "_s_aspects_for_" + methodInfo.Name, FieldAttributes.Static);
      var staticAspectsFlagField = mutableType.AddField (typeof (bool), "_s_aspects_for_" + methodInfo.Name + "_init", FieldAttributes.Static);
      var staticAspectsInitExpression = Expression.Assign (
          Expression.Field (null, staticAspectsArrayField),
          Expression.NewArrayInit (
              typeof (AspectAttribute),
              staticAspectsAttributes.Select (x => Expression.New (x.GetType())).Cast<Expression>()));

      var allMethodAspectsArrayField = mutableType.AddField (typeof (AspectAttribute[]), "_m_aspects_for_" + methodInfo.Name);
      
      // TODO: Bugfix for static aspect indexes when the same aspect is applied multiple times
      //var currentStaticAspectIndex = 0;
      //var initializers = aspectAttributes
      //    .Select (attribute => 
      //        attribute.Scope == AspectScope.Static
      //        ? (Expression)Expression.ArrayAccess (Expression.Field (null, staticAspectsArrayField), Expression.Constant (currentStaticAspectIndex++))
      //        : Expression.New (attribute.GetType())).ToList();

      var allMethodAspectsInitExpression = new Func<BodyContextBase, Expression> (
          ctx => Expression.Assign (
              Expression.Field (ctx.This, allMethodAspectsArrayField),
              Expression.NewArrayInit (
                  typeof (AspectAttribute),
                  aspectAttributesAsCollection.Select (x => GetInstanceAspectExpression (x, staticAspectsAttributes, staticAspectsArrayField)))));

      foreach (var constructor in mutableType.AllMutableConstructors)
      {
        constructor.SetBody (ctx =>
        {
          var previousBodyWithArguments = ctx.PreviousBody;

          return Expression.Block (
              staticAspectsInitExpression,
              allMethodAspectsInitExpression (ctx),
              previousBodyWithArguments);
        });
      }

      return allMethodAspectsArrayField;
    }

    private Expression GetInstanceAspectExpression (AspectAttribute aspect, IList<AspectAttribute> staticAspects, FieldInfo staticAspectsArrayField)
    {
      if (!staticAspects.Contains (aspect))
        return Expression.New (aspect.GetType());
      else
        return Expression.ArrayAccess (Expression.Field (null, staticAspectsArrayField), Expression.Constant (staticAspects.IndexOf (aspect)));
    }

    //TODO: Fix for aspects with ctor args and settable properties/fields
    //private Expression GetAttributeInitializationExpression (CustomAttributeData attributeData)
    //{
    //  var ctorCall = Expression.New (
    //      attributeData.Constructor, 
    //      attributeData.ConstructorArguments.Select (arg => Expression.Constant (arg.Value, arg.ArgumentType)).Cast<Expression>());
    //  var memberBindings =
    //      attributeData.NamedArguments
    //          .Select (arg => Expression.Bind (arg.MemberInfo, Expression.Constant (arg.TypedValue.Value, arg.TypedValue.ArgumentType)))
    //          .Cast<MemberBinding>();
    //  return Expression.MemberInit (ctorCall, memberBindings);
    //}
  }
}