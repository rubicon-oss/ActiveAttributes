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
using System.Diagnostics;
using System.Reflection;
using System.Text;
using ActiveAttributes.Advices;
using ActiveAttributes.Discovery.Construction;
using ActiveAttributes.Pointcuts;

namespace ActiveAttributes.Discovery
{
  public interface IAdviceBuilder
  {
    IAdviceBuilder SetConstruction (IConstruction construction);
    IAdviceBuilder SetMethod (MethodInfo method);

    IAdviceBuilder SetName (string name);
    IAdviceBuilder SetRole (string role);
    IAdviceBuilder SetPriority (int priority);
    IAdviceBuilder SetExecution (AdviceExecution execution);
    IAdviceBuilder SetScope (AdviceScope scope);
    IAdviceBuilder AddPointcut (IPointcut pointcut);

    IAdviceBuilder Copy ();

    Advice Build ();
  }

  [DebuggerDisplay ("{ToDebugString()}")]
  public class AdviceBuilder : IAdviceBuilder
  {
    private IConstruction _construction;
    private MethodInfo _method;
    
    private string _role;
    private string _name;
    private int _priority;
    private AdviceExecution _execution;
    private AdviceScope _scope;

    private readonly List<IPointcut> _pointcuts = new List<IPointcut>();

    public IAdviceBuilder SetConstruction (IConstruction construction)
    {
      if (_construction != null
          && construction.GetType() == typeof (TypeConstruction)
          && _construction.GetType() == typeof (CustomAttributeDataConstruction))
      {
        var message = "Construction can not be overwritten if existing construction is more meaningful.";
        throw new InvalidOperationException (message);
      }

      _construction = construction;
      return this;
    }

    public IAdviceBuilder SetMethod (MethodInfo method)
    {
      if (_method != null && !_method.Equals (default (MethodInfo)))
        throw new Exception();

      _method = method;
      return this;
    }

    public IAdviceBuilder SetName (string name)
    {
      _name = name;
      return this;
    }

    public IAdviceBuilder SetRole (string role)
    {
      _role = role;
      return this;
    }

    public IAdviceBuilder SetPriority (int priority)
    {
      _priority = priority;
      return this;
    }

    public IAdviceBuilder SetExecution (AdviceExecution execution)
    {
      _execution = execution;
      return this;
    }

    public IAdviceBuilder SetScope (AdviceScope scope)
    {
      _scope = scope;
      return this;
    }

    public IAdviceBuilder AddPointcut (IPointcut pointcut)
    {
      _pointcuts.RemoveAll (x => x.GetType() == pointcut.GetType());
      _pointcuts.Add (pointcut);
      return this;
    }

    public IAdviceBuilder Copy ()
    {
      var copy = new AdviceBuilder()
          .SetConstruction (_construction)
          .SetMethod (_method)
          .SetName (_name)
          .SetRole (_role)
          .SetExecution (_execution)
          .SetScope (_scope)
          .SetPriority (_priority);

      foreach (var pointcut in _pointcuts)
        copy.AddPointcut (pointcut);

      return copy;
    }

    public Advice Build ()
    {
      EnsureWasSet ("construction",_construction);
      EnsureWasSet ("method", _method);
      EnsureWasSet ("execution", _execution);
      EnsureWasSet ("scope", _scope);

      return new Advice (_construction, _method, _name, _role, _execution, _scope, _priority, _pointcuts);
    }

    private void EnsureWasSet<T> (string memberName, T value)
    {
      if (value == null || value.Equals (default(T)))
      {
        var message = string.Format ("Cannot build advice without having set its {0}.", memberName);
        throw new InvalidOperationException (message);
      }
    }

    private string ToDebugString ()
    {
      return new StringBuilder()
          .Append ("Construction: ").Append (_construction != null ? _construction.ConstructorInfo.DeclaringType.Name : "Unspecified").Append (" ")
          .Append ("Method: ").Append (_method.Name).Append (" ")
          .Append ("Execution: ").Append (_execution).Append (" ")
          .Append ("Scope: ").Append (_scope)
          .ToString();
    }
  }
}