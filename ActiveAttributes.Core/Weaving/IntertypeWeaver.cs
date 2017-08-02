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
using ActiveAttributes.Model;
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Expressions;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (IntertypeWeaver))]
  public interface IIntertypeWeaver
  {
    void Import (Aspect aspect, JoinPoint joinPoint);
  }

  public class IntertypeWeaver : IIntertypeWeaver
  {
    private readonly IAspectStorageCache _aspectStorageCache;

    public IntertypeWeaver (IAspectStorageCache aspectStorageCache)
    {
      _aspectStorageCache = aspectStorageCache;
    }

    public void Import (Aspect aspect, JoinPoint joinPoint)
    {
      var mutableType = joinPoint.DeclaringType;
      var storage = _aspectStorageCache.GetOrAddStorage (aspect, joinPoint);
      foreach (var import in aspect.MemberImports)
      {
        var member = mutableType.GetMethod (import.Name);
        if (member == null)
        {
          if (import.IsRequired)
            throw new Exception (string.Format ("Member '{0}' not found.", import.Name));
          continue;
        }


        var import1 = import;
        var field = Expression.Field (storage.CreateStorageExpression (joinPoint.This), import.Field);
        mutableType.AddInstanceInitialization (
            ctx =>
            Expression.Assign (field, new NewDelegateExpression (import1.Type, joinPoint.This, member)));
      }
      //aspect.MemberImports.Single(x => x.)
    }
  }
}