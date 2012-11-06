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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Copies a method.
  /// </summary>
  [ConcreteImplementation (typeof (MethodCopyService))]
  public interface IMethodCopyService
  {
    /// <summary>
    /// Copies the original body of a method to another method.
    /// </summary>
    /// <param name="mutableMethod">The original method.</param>
    /// <returns>A copy of the original method.</returns>
    MutableMethodInfo GetCopy (MutableMethodInfo mutableMethod);
  }

  /// <summary>
  /// Copies a method including signature and body.
  /// </summary>
  public class MethodCopyService : IMethodCopyService
  {
    public MutableMethodInfo GetCopy (MutableMethodInfo mutableMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      Assertion.IsTrue (mutableMethod.DeclaringType != null);

      var declaringType = (MutableType) mutableMethod.DeclaringType;
      var copiedMethod = declaringType.AddMethod (
          "_m_" + mutableMethod.Name + "_Copy",
          MethodAttributes.Private,
          mutableMethod.ReturnType,
          ParameterDeclaration.CreateForEquivalentSignature (mutableMethod),
          ctx => ctx.GetCopiedMethodBody (mutableMethod, ctx.Parameters.Cast<Expression>()));

      return copiedMethod;
    }
  }
}