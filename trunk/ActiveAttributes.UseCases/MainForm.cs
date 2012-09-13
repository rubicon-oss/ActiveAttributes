//Sample license text.
using System;
using System.Threading;
using System.Windows.Forms;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Assembly.Configuration.Rules;
using ActiveAttributes.UseCases.Aspects;

namespace ActiveAttributes.UseCases
{
  public partial class MainForm : Form
  {
    public MainForm ()
    {
      InitializeComponent();
    }

    [CatchExceptionAspect]
    protected virtual void AddLine (string line)
    {
      listBox1.Items.Add (line);
      listBox1.SelectedIndex = listBox1.Items.Count - 1;
      listBox1.SelectedIndex = -1;
    }

    [InvokeAspect]
    protected virtual void AddLineAspected (string line)
    {
      listBox1.Items.Add (line);
      listBox1.SelectedIndex = listBox1.Items.Count - 1;
      listBox1.SelectedIndex = -1;
    }

    [CacheAspect]
    public virtual DateTime GetTimeCachedByFiveSeconds ()
    {
      return DateTime.Now;
    }


    private void button1_Click (object sender, EventArgs e)
    {
      ThreadPool.QueueUserWorkItem (s => AddLine (DateTime.Now.ToString ("HH:mm:ss.fff")));
    }

    private void button2_Click (object sender, EventArgs e)
    {
      ThreadPool.QueueUserWorkItem (s => AddLineAspected (DateTime.Now.ToString ("HH:mm:ss.fff")));
    }

    private void button3_Click (object sender, EventArgs e)
    {
      var cached = GetTimeCachedByFiveSeconds();
      AddLine (cached.ToString());
    }

    private void button4_Click (object sender, EventArgs e)
    {
      Text = DateTime.Now.ToString();
    }

    private void button6_Click (object sender, EventArgs e)
    {
      AspectConfiguration.Singleton.Rules.Clear ();
      AspectConfiguration.Singleton.Rules.Add (new TypeOrderRule ("", typeof (OrderedAspect2Attribute), typeof (OrderedAspect1Attribute)));
      var obj = ObjectFactory.Create<DomainClass> ();
      obj.Method ();
    }

    private void button5_Click (object sender, EventArgs e)
    {
      AspectConfiguration.Singleton.Rules.Clear ();
      AspectConfiguration.Singleton.Rules.Add (new TypeOrderRule ("", typeof (OrderedAspect1Attribute), typeof (OrderedAspect2Attribute)));
      var obj = ObjectFactory.Create<DomainClass> ();
      obj.Method ();
    }

    public class DomainClass
    {
      [OrderedAspect1]
      [OrderedAspect2]
      public virtual void Method ()
      {
      }
    }
  }
}