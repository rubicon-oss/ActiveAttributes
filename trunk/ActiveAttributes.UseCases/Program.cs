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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Configuration2.Configurators;

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
      var configurator = new AppConfigConfigurator();
      configurator.Initialize (new ActiveAttributesConfiguration());

      Application.EnableVisualStyles ();
      Application.SetCompatibleTextRenderingDefault (false);

      var threadingForm = ObjectFactory.Create<MainForm>();
      Application.Run (threadingForm);
    }
  }
}
