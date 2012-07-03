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
using System.Runtime.Serialization;

namespace ActiveAttributes.Core
{
  /// <summary>
  /// Exception that is thrown when an aspect invocation throws an exception.
  /// </summary>
  [Serializable]
  public class AspectInvocationException : Exception
  {
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public AspectInvocationException () {}

    public AspectInvocationException (string message)
        : base (message) {}

    public AspectInvocationException (string message, Exception inner)
        : base (message, inner) {}

    protected AspectInvocationException (
        SerializationInfo info,
        StreamingContext context)
        : base (info, context) {}
  }
}