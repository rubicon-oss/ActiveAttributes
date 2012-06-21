using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectInitializer
  {
    public Tuple<MutableMethodInfo, MutableFieldInfo> IntroduceInitialization (
        MutableType mutableType, MutableMethodInfo methodInfo, IEnumerable<AspectAttribute> aspectAttributes, Expression thisExpression)
    {
      var staticAspectsAttributes = aspectAttributes.Where (x => x.Scope == AspectScope.Static).ToList();
      var staticAspectsField = mutableType.AddField (typeof (AspectAttribute[]), "_s_aspects_for_" + methodInfo.Name, FieldAttributes.Static);
      var staticAspectsFlagField = mutableType.AddField (typeof (bool), "_s_aspects_for_" + methodInfo.Name + "_init", FieldAttributes.Static);
      var staticAspectsInitExpression = Expression.Assign (
          Expression.Field (null, staticAspectsField),
          Expression.NewArrayInit (
              typeof (AspectAttribute),
              staticAspectsAttributes.Select (x => Expression.New (x.GetType())).Cast<Expression>()));

      var allMethodAspectsField = mutableType.AddField (typeof (AspectAttribute[]), "_m_aspects_for_" + methodInfo.Name);
      //var allMethodAspectsInitExpression

      return null;
    }
  }
}