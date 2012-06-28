// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Extensions
{
  public static class MethodBaseExtensions
  {

    // TODO TEST
    public static bool IsCompilerGenerated (this MethodBase methodInfo)
    {
      var customAttributes = methodInfo.GetCustomAttributes (typeof (CompilerGeneratedAttribute), true);
      return customAttributes.Length == 1;
    }
  }
}