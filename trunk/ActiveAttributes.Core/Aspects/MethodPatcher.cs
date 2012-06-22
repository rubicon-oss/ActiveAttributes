// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Aspects
{
  public class MethodPatcher
  {
    public void PatchMethod (MutableType mutableType, MutableMethodInfo mutableMethod, FieldInfo allMethodAspectsArrayField)
    {
      var copiedMethod = GetCopiedMethod (mutableType, mutableMethod);


    }

    private MutableMethodInfo GetCopiedMethod (MutableType mutableType, MutableMethodInfo mutableMethod)
    {
      return mutableType.AddMethod (
          "_m_" + mutableMethod.Name,
          MethodAttributes.Private,
          mutableMethod.ReturnType,
          ParameterDeclaration.CreateForEquivalentSignature (mutableMethod),
          ctx => ctx.GetCopiedMethodBody (mutableMethod, ctx.Parameters.Cast<Expression>()));
    }
    
  }
}