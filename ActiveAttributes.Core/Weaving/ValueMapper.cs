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
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (ValueMapper))]
  public interface IValueMapper
  {
    Expression GetIndexMapping (Expression context, int index);
    Expression GetTypeMapping (Expression context, Type type);
    Expression GetReturnMapping (Expression context);
  }

  public class ValueMapper : IValueMapper
  {
    public Expression GetIndexMapping (Expression context, int index)
    {
      var field = context.Type.GetField ("Arg" + index);
      return Expression.Field (context, field);
    }

    public Expression GetTypeMapping (Expression context, Type type)
    {
      var field = context.Type.GetFields().Single (x => type.IsAssignableFrom (x.FieldType));
      return Expression.Field (context, field);
    }

    public Expression GetReturnMapping (Expression context)
    {
      var field = context.Type.GetField ("TypedReturnValue");
      return Expression.Field (context, field);
    }
  }
}