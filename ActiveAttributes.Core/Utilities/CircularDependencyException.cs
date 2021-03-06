﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ActiveAttributes.Extensions;
using JetBrains.Annotations;

namespace ActiveAttributes.Utilities
{
  /// <summary>
  /// Exception that is thrown when sorting a set of items is not possible due to a circular dependency.
  /// </summary>
  public class CircularDependencyException : Exception
  {
    private const string c_message = "Circular dependencies detected:\r\n";

    public CircularDependencyException (IEnumerable<IEnumerable> cycles)
    {
      Cycles = cycles;
    }

    [UsedImplicitly]
    protected CircularDependencyException (SerializationInfo info, StreamingContext context)
      : base (info, context) { }

    public IEnumerable<IEnumerable> Cycles { get; private set; }

    public override string Message
    {
      get { return c_message + Cycles.ToString (x => Enumerable.Cast<object>(x).ToString (" -> "), "\r\n"); }
    }
  }
}