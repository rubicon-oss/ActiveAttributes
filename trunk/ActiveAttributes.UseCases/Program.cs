using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ActiveAttributes.Core;

namespace ActiveAttributes.UseCases
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main ()
    {
      Application.EnableVisualStyles ();
      Application.SetCompatibleTextRenderingDefault (false);

      var threadingForm = ObjectFactory.Create<ThreadingForm>();
      Application.Run (threadingForm);
    }
  }
}
