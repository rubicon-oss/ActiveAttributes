using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClassLibrary2
{
  [TestFixture]
  class MyClass
  {
    [Test]
    public void name ()
    {
      var assembly = typeof (MyClass).Assembly;
      var assemblyName = assembly.GetName();

      Assert.That (assemblyName.KeyPair, Is.Null);
      Assert.That (assemblyName.GetPublicKey(), Is.Empty);
    }
  }
}
