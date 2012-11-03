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

namespace ActiveAttributes.Core.Infrastructure.Orderings
{
  public class AdviceNameOrdering : AdviceOrderingBase
  {
    private readonly string _beforeName;
    private readonly string _afterName;

    public AdviceNameOrdering (string beforeName, string afterName, string source)
        : base (ArgumentUtility.CheckNotNullOrEmpty ("source", source))
    {
      ArgumentUtility.CheckNotNullOrEmpty ("beforeName", beforeName);
      ArgumentUtility.CheckNotNullOrEmpty ("afterName", afterName);

      _beforeName = beforeName;
      _afterName = afterName;
    }

    public string BeforeName
    {
      get { return _beforeName; }
    }

    public string AfterName
    {
      get { return _afterName; }
    }
  }
}