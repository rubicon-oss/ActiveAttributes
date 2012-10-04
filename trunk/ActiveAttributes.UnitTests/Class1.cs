using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Ast;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  class Class1
  {
    public event EventHandler Event;
    [Test]
    public void name ()
    {
      var handler = new EventHandler (Method);
      AddHandler2 (handler);
    }

    private void Method (object sender, EventArgs eventArgs)
    {
    }

    private void AddHandler2 (EventHandler handler)
    {
      var typeArgs = handler.Method.GetParameters ().Select (x => x.ParameterType)
        .Concat (new[] { typeof (object[]) })
        .ToArray ();
      var delegateType = Expression.GetDelegateType (typeArgs);
      var parameters = handler.Method.GetParameters ().Select (x => Expression.Parameter (x.ParameterType, x.Name)).ToArray ();
      var lambda = Expression.Lambda (
          delegateType,
          Expression.NewArrayInit (typeof (object), parameters.Cast<Expression> ()),
          false,
          parameters);
      var del = lambda.Compile ();

      var action = new Action<object[]> (
          (args) => handler.DynamicInvoke (args));
    }

    private object Trigger (object[] args)
    {
      return _event.DynamicInvoke (args);
    }

    private void AddHandler (EventHandler handler)
    {
      _event = (EventHandler) Delegate.Combine (_event, handler);
    }

    private EventHandler _event;
  }
}
