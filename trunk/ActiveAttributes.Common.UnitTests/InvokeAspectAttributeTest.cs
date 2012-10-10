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
      Action<DomainControl> operation =
          control =>
          {
            Assertion.IsTrue (Application.OpenForms.Count == 1);
            var form = Application.OpenForms[0];

            form.BeginInvoke (new Action (control.Method));
          };

      var result = WasInvokeRequired (operation);

      Assert.That (result, Is.False);
    }

    [Test]
    public void ProceedsIfOtherThread ()
    {
      Action<DomainControl> operation = control => control.Method();

      var result = WasInvokeRequired (operation);

      Assert.That (result, Is.False);
    }

    private bool WasInvokeRequired (Action<DomainControl> otherThreadOperation)
    {
      var control = ObjectFactory.Create<DomainControl> ();
      var form = new Form ();
      form.Controls.Add (control);
      var thread = new Thread (() => otherThreadOperation (control));
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