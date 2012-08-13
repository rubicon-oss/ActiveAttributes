//Sample license text.
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using ActiveAttributes.UseCases.Aspects;

namespace ActiveAttributes.UseCases
{
  public partial class MainForm : Form, INotifyPropertyChanged
  {
    public MainForm ()
    {
      InitializeComponent ();

      PropertyChanged += (s, e) => AddLine ("Property " + e.PropertyName + " changed");
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
    public virtual DateTime GetTime (DateTime dateTime5)
    {
      return DateTime.Now;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    //[NotifyPropertyChangedAspect]
    public override string Text
    {
      get { return base.Text; }
      set { base.Text = value; }
    }

    private void button1_Click (object sender, EventArgs e)
    {
      ThreadPool.QueueUserWorkItem (s => AddLine (DateTime.Now.ToString("HH:mm:ss.fff")));
    }

    private void button2_Click (object sender, EventArgs e)
    {
      ThreadPool.QueueUserWorkItem (s => AddLineAspected (DateTime.Now.ToString ("HH:mm:ss.fff")));
    }

    private void button3_Click (object sender, EventArgs e)
    {
      var today = DateTime.Today;
      var now = DateTime.Now;
      var todayHM = today.Add (new TimeSpan (now.Hour, now.Minute, now.Second / 5));
      var cached = GetTime (todayHM);
      AddLine (cached.ToString());
    }

    private void button4_Click (object sender, EventArgs e)
    {
      Text = DateTime.Now.ToString();
    }
  }
}
