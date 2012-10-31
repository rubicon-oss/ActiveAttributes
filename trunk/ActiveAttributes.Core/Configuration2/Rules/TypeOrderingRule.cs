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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Old;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Configuration2.Rules
{
  /// <summary>
  /// A <see cref="IAspectOrderingRule"/> based on the type of an aspect.
  /// </summary>
  public class TypeOrderingRule : AspectOrderingRuleBase
  {
    private readonly Type _type1;
    private readonly Type _type2;

    public TypeOrderingRule (string source, Type type1, Type type2)
        : base (source)
    {
      ArgumentUtility.CheckNotNull ("type1", type1);
      ArgumentUtility.CheckNotNull ("type2", type2);

      _type1 = type1;
      _type2 = type2;
    }

    public override int Compare (IAspectDescriptor x, IAspectDescriptor y)
    {
      var type1 = x.Type;
      var type2 = y.Type;

      if (_type1.IsAssignableFrom (type1) && _type2.IsAssignableFrom (type2))
        return -1;
      if (_type1.IsAssignableFrom (type2) && _type2.IsAssignableFrom (type1))
        return 1;

      return 0;
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals (null, obj))
        return false;
      if (ReferenceEquals (this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals ((TypeOrderingRule) obj);
    }

    public override int GetHashCode ()
    {
      unchecked
      {
        return ((_type1 != null ? _type1.GetHashCode() : 0) * 397) ^ (_type2 != null ? _type2.GetHashCode() : 0);
      }
    }

    public override string ToString ()
    {
      return ToString (_type1.Name, _type2.Name);
    }

    private bool Equals (TypeOrderingRule other)
    {
      return _type1 == other._type1 && _type2 == other._type2;
    }
  }
}