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
using Remotion.Utilities;

namespace ActiveAttributes.Core.AdviceInfo
{
  public enum AdviceExecution
  {
    Undefined,
    Before,
    After,
    Around
  }

  public sealed class AdviceExecutionAttribute : AdviceAttribute
  {
    private readonly AdviceExecution _execution;

    public AdviceExecutionAttribute (AdviceExecution execution)
    {
      Assertion.IsTrue (execution != AdviceExecution.Undefined);

      _execution = execution;
    }

    public AdviceExecution Execution
    {
      get { return _execution; }
    }
  }
}