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
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Configuration2.Rules;
using ActiveAttributes.UseCases.Aspects;
using Remotion.ServiceLocation;

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
      var activeAttributesConfiguration = SafeServiceLocator.Current.GetInstance<IActiveAttributesConfiguration>();
      activeAttributesConfiguration.AspectOrderingRules.Clear();
      activeAttributesConfiguration.AspectOrderingRules.Add (
          new TypeOrdering ("", typeof (OrderedAspect2Attribute), typeof (OrderedAspect1Attribute)));
      var obj = ObjectFactory.Create<DomainClass>();
      obj.Method();
    }

    private void button5_Click (object sender, EventArgs e)
    {
      var activeAttributesConfiguration = SafeServiceLocator.Current.GetInstance<IActiveAttributesConfiguration>();
      activeAttributesConfiguration.AspectOrderingRules.Clear();
      activeAttributesConfiguration.AspectOrderingRules.Add (
          new TypeOrdering ("", typeof (OrderedAspect1Attribute), typeof (OrderedAspect2Attribute)));
      var obj = ObjectFactory.Create<DomainClass>();
      obj.Method();
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