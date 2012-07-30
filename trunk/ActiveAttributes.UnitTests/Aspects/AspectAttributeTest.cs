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
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Aspects
{
  [TestFixture]
  public class AspectAttributeTest
  {
    //[Serializable]
    //private class TestableAspectAttribute : MethodInterceptionAspectAttribute
    //{
    //  public TestableAspectAttribute () {}

    //  protected TestableAspectAttribute (SerializationInfo info, StreamingContext context)
    //      : base (info, context) {}

    //  public override void OnIntercept (IInvocation invocation)
    //  {
    //    throw new NotImplementedException();
    //  }
    //}

    //[Test]
    //public void Clone ()
    //{
    //  var aspect = new TestableAspectAttribute { Scope = AspectScope.Instance };
    //  var copy = (MethodInterceptionAspectAttribute) aspect.Clone();

    //  Assert.That (copy.Scope, Is.EqualTo (AspectScope.Instance));
    //}

    //[Test]
    //public void Serialize ()
    //{
    //  var aspect = new TestableAspectAttribute
    //               {
    //                   Scope = AspectScope.Instance,
    //                   Priority = 10
    //               };
    //  var formatter = new BinaryFormatter();
    //  var memoryStream = new MemoryStream();

    //  formatter.Serialize (memoryStream, aspect);
    //  memoryStream.Position = 0;
    //  var copy = (MethodInterceptionAspectAttribute) formatter.Deserialize (memoryStream);

    //  Assert.That (copy.Scope, Is.EqualTo (aspect.Scope));
    //  Assert.That (copy.Priority, Is.EqualTo (aspect.Priority));
    //}

    [Test]
    public void RequiresType_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method1());
      var aspect = new TestableAspectAttribute { RequiresType = typeof (RequireTypeMatchingClass) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void RequiresType_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { RequiresType = typeof (int) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void RequiresType_MatchingSub ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { RequiresType = typeof (object) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void RequiresMethodName_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method2 ());
      var aspect = new TestableAspectAttribute { RequiresMethodName = "Method2" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void RequiresMethodName_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { RequiresMethodName = "Method2" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void RequiresMethodName_MatchingWildcard ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method2 ());
      var aspect = new TestableAspectAttribute { RequiresMethodName = "*2" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }


    [Test]
    public void RequiresNamespace_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method2 ());
      var aspect = new TestableAspectAttribute { RequiresNamespace = "ActiveAttributes.UnitTests.Aspects" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void RequiresNamespace_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { RequiresNamespace = "ActiveAttributes.UnitTests2.Aspects" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void RequiresNamespace_MatchingWildcard ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method2 ());
      var aspect = new TestableAspectAttribute { RequiresNamespace = "ActiveAttributes.UnitTests.*" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void RequiresVisibility_PublicMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.PublicMethod ());
      var aspect = new TestableAspectAttribute { RequiresVisibility = Visibility.Public };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void RequiresVisibility_PublicNotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.InternalMethod ());
      var aspect = new TestableAspectAttribute { RequiresVisibility = Visibility.Public };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void RequiresAttribute_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { RequiresMarkers = new[] { typeof (CompilerGeneratedAttribute) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void RequiresAttribute_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method2 ());
      var aspect = new TestableAspectAttribute { RequiresMarkers = new[] { typeof (CompilerGeneratedAttribute) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void RequiresAttribute_MatchingMultiple ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { RequiresMarkers = new[] { typeof (CompilerGeneratedAttribute), typeof (MethodImplAttribute) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void RequiresAttribute_NotMatchingMultiple ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { RequiresMarkers = new[] { typeof (CompilerGeneratedAttribute), typeof (ObsoleteAttribute) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void RequiresReturnType_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.IntegerMethod ());
      var aspect = new TestableAspectAttribute { RequiresReturnType = typeof (int) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void RequiresReturnType_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.IntegerMethod ());
      var aspect = new TestableAspectAttribute { RequiresReturnType = typeof (double) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void RequiresReturnType_MatchingSub ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.IntegerMethod ());
      var aspect = new TestableAspectAttribute { RequiresReturnType = typeof (object) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void RequiresArguments_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.MethodWithArg (1, ""));
      var aspect = new TestableAspectAttribute { RequiresArguments = new[] { typeof (int), typeof (string) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void RequiresArguments_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((RequireTypeMatchingClass obj) => obj.MethodWithArg (1, ""));
      var aspect = new TestableAspectAttribute { RequiresArguments = new[] { typeof (int), typeof (int) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    public class RequireTypeMatchingClass
    {
      [CompilerGenerated]
      [MethodImpl]
      public void Method1 ()
      {
      }

      public void Method2 ()
      {
      }

      public void PublicMethod ()
      {
      }

      protected void ProtectedMethod ()
      {
      }

      private void PrivateMethod ()
      {
      }

      internal void InternalMethod ()
      {
      }

      public int IntegerMethod ()
      {
        return 1;
      }

      public void MethodWithArg (int arg1, string arg2)
      {
      }
    }

    public class TestableAspectAttribute : AspectAttribute
    {
    }
  }
}