// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  public class MethodCopier
  {
    public MutableMethodInfo GetCopy (MutableMethodInfo mutableMethod)
    {
      var mutableType = (MutableType) mutableMethod.DeclaringType;

      var copiedMethod = mutableType.AddMethod (
          "_m_" + mutableMethod.Name + "_Copy",
          MethodAttributes.Private,
          mutableMethod.ReturnType,
          ParameterDeclaration.CreateForEquivalentSignature (mutableMethod),
          ctx => ctx.GetCopiedMethodBody (mutableMethod, ctx.Parameters.Cast<Expression> ()));

      return copiedMethod;
    }
  }
}