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
// 

using System;
using ActiveAttributes.UnitTests.Assembly;

//[assembly: AspectOrdering.IgnorePresets()]
[assembly: AspectOrdering.IgnorePresets (typeof (LoggingAspect2), typeof (LoggingAspect3))]

[assembly: AspectOrdering.ByRole (StandardRoles.Security, StandardRoles.Logging, StandardRoles.Any)]
[assembly: AspectOrdering.ByType (typeof(LoggingAspect1), typeof(LoggingAspect2))]

namespace ActiveAttributes.UnitTests.Assembly
{

  [AspectOrdering.Preset(OrderingType.Require, OrderingPosition.Before, StandardRoles.Any)]
  public class LoggingAspect1
  {
  }

  //[AspectOrdering.Preset(OrderingType.Conflict, OrderingPosition.Before, StandardRoles.Any)]
  public class LoggingAspect2
  {
  }

  public class LoggingAspect3
  {
  }

  //[AspectOrdering.Preset(OrderingType.Conflict, OrderingPosition.Before, StandardRoles.Any)]
  public class TraceAspect
  {
  }

  [AttributeUsage(AttributeTargets.Assembly)]
  public class ApplyAspectTo : Attribute
  {
    public ApplyAspectTo (Type appliedType, Type aspectType, params object[] arguments)
    {
    }
    public ApplyAspectTo (Type appliedType, Type aspectType, object argumentsAsAnonymousType)
    {
    }
  }

  public class StandardRoles
  {
    public const string Any = "*";
    public const string Security = "*Security*";
    public const string Logging = "*Logging*";
    public const string Transaction = "*Transaction*";
    public const string Caching = "*Caching*";
    public const string Threading = "*Threading*";
  }

  public enum OrderingType
  {
    Require
  }

  public enum OrderingPosition
  {
    Before,
    After
  }

  public class AspectOrdering
  {
    [AttributeUsage(AttributeTargets.Class)]
    public class Preset : Attribute
    {
      public Preset (OrderingType type, OrderingPosition position, Type type2)
      {
      }
      public Preset (OrderingType type, OrderingPosition position, string role)
      {
      }
    }

    [AttributeUsage (AttributeTargets.Assembly)]
    public class ByType : Attribute
    {
      public ByType (params Type[] types)
      {
      }
      public ByType (bool ignoreAspectPresets, params Type[] types)
      {
      }
    }

    [AttributeUsage (AttributeTargets.Assembly)]
    public class ByRole : Attribute
    {
      public ByRole (params string[] roles)
      {
      }
      public ByRole (bool ignoreAspectPresets, params string[] roles)
      {
      }
    }

    [AttributeUsage (AttributeTargets.Assembly)]
    public class IgnorePresets : Attribute
    {
      public IgnorePresets ()
      {
      }

      public IgnorePresets (params Type[] aspectType)
      {
        
      }
    }
  }

  public class AspectRole : Attribute
  {
    public AspectRole (string role)
    { 
    }
  }

  public class AspectEffect : Attribute
  {
    public AspectEffect (string effect)
    {
    }
  }
}