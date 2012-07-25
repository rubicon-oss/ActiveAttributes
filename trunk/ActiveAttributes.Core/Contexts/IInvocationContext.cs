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
// 

using System;
using System.Reflection;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Contexts
{
  /// <summary>
  ///   Provides information about the interception.
  /// </summary>
  public interface IInvocationContext
  {
    /// <summary>
    ///   The <see cref = "MethodInfo" /> of the intercepted action (i.e., constructor, method, property).
    /// </summary>
    MethodInfo MethodInfo { get; }

    /// <summary>
    ///   The <code>this</code> object whose action is intercepted.
    /// </summary>
    object Instance { get; }

    /// <summary>
    ///   The collection of arguments passed to the intercepted action.
    /// </summary>
    IArgumentCollection Arguments { get; }

    /// <summary>
    ///   The return value of the intercepted method. It holds the return value of the intercepted action after calling
    ///   <see cref = "IInvocation.Proceed" />.
    /// </summary>
    object ReturnValue { get; set; }
  }
}