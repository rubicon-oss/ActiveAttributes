using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Configuration.Configurators;

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
      var configurator = new AppConfigAspectConfigurator();
      configurator.Initialize (new AspectConfiguration());

      Application.EnableVisualStyles ();
      Application.SetCompatibleTextRenderingDefault (false);

      var threadingForm = ObjectFactory.Create<MainForm>();
      Application.Run (threadingForm);
    }
  }
}
