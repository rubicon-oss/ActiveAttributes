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
using System.Threading;
using System.Windows.Forms;
using ActiveAttributes.Core;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.Common.UnitTests
{
  [TestFixture]
  public class InvokeAspectAttributeTest
  {
    [Test]
    public void ProceedsIfSameThread ()
    {
      Action<DomainControl> threadOperation =
          control =>
          {
            Assertion.IsTrue (Application.OpenForms.Count == 1);
            var form = Application.OpenForms[0];

            form.BeginInvoke (new Action (control.Method));
          };

      var result = WasInvokeRequired (threadOperation);

      Assert.That (result, Is.False);
    }

    [Test]
    public void ProceedsIfOtherThread ()
    {
      Action<DomainControl> threadOperation = control => control.Method ();

      var result = WasInvokeRequired (threadOperation);

      Assert.That (result, Is.False);
    }

    private bool WasInvokeRequired (Action<DomainControl> threadOperation)
    {
      var control = ObjectFactory.Create<DomainControl> ();
      var form = new Form ();
      form.Controls.Add (control);
      var thread = new Thread (() => threadOperation (control));
      form.Shown += (s, e) => thread.Start ();
      Application.Run (form);

      return control.WasInvokeRequired;
    }

    public class DomainControl : Control
    {
      public bool WasInvokeRequired { get; private set; }

      [InvokeAspect]
      public virtual void Method ()
      {
        WasInvokeRequired = InvokeRequired;
        var form = (Form) Parent;
        form.Close();
      }
    }
  }
}