using System;
using System.Collections;
using System.Collections.Generic;

namespace ActiveAttributes.Core.Contexts.ArgumentCollection
{
  public class ActionArgumentCollection<TInstance, TA0> : IArgumentCollection, IReadOnlyArgumentCollection
  {
    private ActionInvocationContext<TInstance, TA0> _invocationContext;

    public ActionArgumentCollection (ActionInvocationContext<TInstance, TA0> invocationContext)
    {
      _invocationContext = invocationContext;
    }

    public object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return _invocationContext.Arg0;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: _invocationContext.Arg0 = (TA0) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }

    public IEnumerator<object> GetEnumerator ()
    {
      yield return _invocationContext.Arg0;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
  public class ActionArgumentCollection<TInstance, TA0, TA1> : IArgumentCollection, IReadOnlyArgumentCollection
  {
    private ActionInvocationContext<TInstance, TA0, TA1> _invocationContext;

    public ActionArgumentCollection (ActionInvocationContext<TInstance, TA0, TA1> invocationContext)
    {
      _invocationContext = invocationContext;
    }

    public object this[int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return _invocationContext.Arg0;
          case 1: return _invocationContext.Arg1;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: _invocationContext.Arg0 = (TA0) value; break;
          case 1: _invocationContext.Arg1 = (TA1) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }

    public IEnumerator<object> GetEnumerator ()
    {
      yield return _invocationContext.Arg0;
      yield return _invocationContext.Arg1;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }
  }
}