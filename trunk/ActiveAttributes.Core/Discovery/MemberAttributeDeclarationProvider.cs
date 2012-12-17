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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Extensions;
using ActiveAttributes.Model;

namespace ActiveAttributes.Discovery
{
  public class MemberAttributeDeclarationProvider : IMethodLevelDeclarationProvider
  {
    private readonly IAttributeDeclarationProvider _attributeDeclarationProvider;

    public MemberAttributeDeclarationProvider (IAttributeDeclarationProvider attributeDeclarationProvider)
    {
      _attributeDeclarationProvider = attributeDeclarationProvider;
    }

    public IEnumerable<Aspect> GetDeclarations (MethodInfo method)
    {
      var declarations = _attributeDeclarationProvider.GetDeclaration (method).ToList();

      var property = method.GetRelatedPropertyInfo();
      if (property != null)
        declarations.AddRange (_attributeDeclarationProvider.GetDeclaration (property));

      var event_ = method.GetRelatedEventInfo();
      if (event_ != null)
        declarations.AddRange (_attributeDeclarationProvider.GetDeclaration (event_));

      return declarations;
    }
  }
}