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
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Infrastructure.Construction;
using ActiveAttributes.Core.Pointcuts;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
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

  public class AdviceBuilder : IAdviceBuilder
  {
    private IConstruction _construction;
    private MethodInfo _method;
    
    private string _role;
    private string _name;
    private int _priority;
    private AdviceExecution _execution;
    private AdviceScope _scope;
    
    private readonly IList<IPointcut> _pointcuts = new List<IPointcut>();

    public IAdviceBuilder SetConstruction (IConstruction construction)
    {
      if (_construction != null
          && (construction.GetType() != typeof (CustomAttributeDataConstruction)
              || _construction.GetType() == typeof (CustomAttributeDataConstruction)))
        throw new Exception ("TODO");

      _construction = construction;
      return this;
    }

    public IAdviceBuilder SetMethod (MethodInfo method)
    {
      EnsureWasNotSet (_method);

      _method = method;
      return this;
    }

    public IAdviceBuilder SetName (string name)
    {
      EnsureWasNotSet (_name);

      _name = name;
      return this;
    }

    public IAdviceBuilder SetRole (string role)
    {
      EnsureWasNotSet (_role);

      _role = role;
      return this;
    }

    public IAdviceBuilder SetPriority (int priority)
    {
      EnsureWasNotSet (_priority);

      _priority = priority;
      return this;
    }

    public IAdviceBuilder SetExecution (AdviceExecution execution)
    {
      EnsureWasNotSet (_execution);

      _execution = execution;
      return this;
    }

    public IAdviceBuilder SetScope (AdviceScope scope)
    {
      EnsureWasNotSet (_scope);

      _scope = scope;
      return this;
    }

    public IAdviceBuilder AddPointcut (IPointcut pointcut)
    {
      if (_pointcuts.Any (x => x.GetType() == pointcut.GetType()))
        throw new Exception ("TODO");

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
      EnsureWasSet (_construction);
      EnsureWasSet (_method);
      EnsureWasSet (_execution);
      EnsureWasSet (_scope);

      return new Advice (_construction, _method, _name, _role, _execution, _scope, _priority, _pointcuts);
    }

    private void EnsureWasNotSet<T> (T value)
    {
      if (value != null && !value.Equals (default (T)))
        throw new Exception();
    }

    private void EnsureWasSet<T> (T value)
    {
      if (value == null || value.Equals (default(T)))
        throw new Exception ("TODO");
    }
  }
}