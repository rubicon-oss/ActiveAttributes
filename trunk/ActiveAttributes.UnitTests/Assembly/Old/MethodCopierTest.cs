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
using System.Reflection;
using ActiveAttributes.Core.Assembly;
using NUnit.Framework;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly.Old
{
  [TestFixture]
  public class MethodCopierTest : TestBase
  {
    private MethodCopier _copier;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _copier = new MethodCopier();
    }

    [Test]
    public void CopyMethod_Name ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (1));
      Action<MutableMethodInfo, MutableMethodInfo> test =
          (baseMethod, copiedMethod) => Assert.That (copiedMethod.Name, Is.EqualTo ("_m_" + methodInfo.Name + "_Copy"));

      Copy<DomainType> (methodInfo, test);
    }

    [Test]
    public void CopyMethod_Private ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (1));
      Action<MutableMethodInfo, MutableMethodInfo> test =
          (baseMethod, copiedMethod) => Assert.That (copiedMethod.Attributes & MethodAttributes.Private, Is.EqualTo (MethodAttributes.Private));

      Copy<DomainType> (methodInfo, test);
    }

    [Test]
    public void CopyMethod_Arguments ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (1));
      Action<MutableMethodInfo, MutableMethodInfo> test =
          (baseMethod, copiedMethod) =>
          {
            var zipped = copiedMethod.ParameterExpressions.Zip (baseMethod.ParameterExpressions, (copyArg, baseArg) => new { copyArg, baseArg });
            foreach (var argPair in zipped)
              ExpressionTreeComparer.CheckAreEqualTrees (argPair.copyArg, argPair.baseArg);
          };

      Copy<DomainType> (methodInfo, test);
    }

    [Test]
    public void CopyMethod_ReturnType ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (1));
      Action<MutableMethodInfo, MutableMethodInfo> test =
          (baseMethod, copiedMethod) => Assert.That (copiedMethod.ReturnType, Is.EqualTo (methodInfo.ReturnType));

      Copy<DomainType> (methodInfo, test);
    }

    [Test]
    public void CopyMethod_Body ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (1));
      Action<MutableMethodInfo, MutableMethodInfo> test =
          (baseMethod, copiedMethod) => ExpressionTreeComparer.CheckAreEqualTrees (copiedMethod.Body, baseMethod.Body);

      Copy<DomainType> (methodInfo, test);
    }

    private void Copy<T> (MethodInfo methodInfo, Action<MutableMethodInfo, MutableMethodInfo> test)
    {
      AssembleType<T> (
          mutableType =>
          {
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

            var copiedMethod = _copier.GetCopy (mutableMethod);

            test (mutableMethod, copiedMethod);
          });
    }

    public class DomainType
    {
      public int Method (int i) { return i; }
    }
  }
}