using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.UseCases
{
  public partial class ThreadingForm : Form
  {
    private readonly Thread _thread;

    public ThreadingForm ()
    {
      InitializeComponent ();
    }

    [InvokeAspect]
    protected virtual void AddLineAspected (string line)
    {
      listBox1.Items.Add (line);
      listBox1.SelectedIndex = listBox1.Items.Count - 1;
      listBox1.SelectedIndex = -1;
    }

    protected virtual void AddLine (string line)
    {
      listBox1.Items.Add (line);
      listBox1.SelectedIndex = listBox1.Items.Count - 1;
      listBox1.SelectedIndex = -1;
    }

    private void button1_Click (object sender, EventArgs e)
    {
      ThreadPool.QueueUserWorkItem (s => AddLine (DateTime.Now.ToString("HH:mm:ss.fff")));
    }

    private void button2_Click (object sender, EventArgs e)
    {
      ThreadPool.QueueUserWorkItem (s => AddLineAspected (DateTime.Now.ToString ("HH:mm:ss.fff")));
    }
  }

  public class InvokeAspectAttribute : MethodInterceptionAspectAttribute
  {
    public override void OnIntercept (IInvocation invocation)
    {
      var form = (Form) invocation.Context.Instance;
      form.Invoke (new Action (invocation.Proceed));
    }
  }
}
