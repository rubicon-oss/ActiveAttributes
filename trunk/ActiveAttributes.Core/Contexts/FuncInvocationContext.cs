using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ActiveAttributes.Core.Contexts.ArgumentCollection;

namespace ActiveAttributes.Core.Contexts
{
  public abstract class InvocationContext : IInvocationContext, IReadOnlyInvocationContext, IArgumentCollection, IReadOnlyArgumentCollection
  {
    protected InvocationContext (MethodInfo methodInfo)
    {
      MethodInfo = methodInfo;
    }

    public MethodInfo MethodInfo { get; private set; }

    public abstract IArgumentCollection Arguments { get; }
    public abstract object Instance { get; }
    public abstract object ReturnValue { get; set; }
    //public abstract IEnumerator<object> GetEnumerator ();
    public abstract int Count { get; }
    public abstract object this[int idx] { get; set; }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return (IReadOnlyArgumentCollection) Arguments; }
    }

    public IEnumerator<object> GetEnumerator ()
    {
      for (var i = 0; i < Count; i++)
        yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void CopyTo (Array array, int index)
    {
      for (var i = 0; i < Count; i++)
        array.SetValue (this[i], i);
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }

    object IReadOnlyArgumentCollection.this [int idx]
    {
      get { return this[idx]; }
    }
  }

  public class FuncInvocationContext<TInstance, TR> : IInvocationContext, IReadOnlyInvocationContext, IArgumentCollection, IReadOnlyArgumentCollection
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance)
    {
      MethodInfo = methodInfo;
      Instance = instance;
    }

    public MethodInfo MethodInfo { get; private set; }

    public TInstance Instance { get; private set; }

    object IReadOnlyInvocationContext.Instance
    {
      get { return Instance; }
    }

    object IInvocationContext.Instance
    {
      get { return Instance; }
    }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return this; }
    }

    IArgumentCollection IInvocationContext.Arguments
    {
      get { return this; }
    }

    public TR ReturnValue { get; set; }

    object IReadOnlyInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
    }

    object IInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
      set { ReturnValue = (TR) value; }
    }

    public IEnumerator<object> GetEnumerator ()
    {
      yield break;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    public void CopyTo (Array array, int index)
    {
      throw new NotImplementedException ();
    }

    public int Count
    {
      get { return 0; }
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException (); }
    }

    public bool IsSynchronized
    {
      get { throw new NotImplementedException (); }
    }

    public object this [int idx]
    {
      get { throw new IndexOutOfRangeException ("idx"); }
      set { throw new IndexOutOfRangeException ("idx"); }
    }
  }

  // TODO: Remove FuncArgumentCollection, implement IArgumentCollection explicitly on FuncInvocationContext, return 
  public class FuncInvocationContext<TInstance, TA0, TR> : IInvocationContext, IReadOnlyInvocationContext, IArgumentCollection, IReadOnlyArgumentCollection
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0)
    {
      MethodInfo = methodInfo;
      Instance = instance;
      Arg0 = arg0;
    }

    public MethodInfo MethodInfo { get; private set; }

    public TInstance Instance { get; private set; }

    object IReadOnlyInvocationContext.Instance
    {
      get { return Instance; }
    }

    object IInvocationContext.Instance
    {
      get { return Instance; }
    }

    public TA0 Arg0 { get; set; }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return this; }
    }

    IArgumentCollection IInvocationContext.Arguments
    {
      get { return this; }
    }

    public TR ReturnValue { get; set; }

    object IReadOnlyInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
    }

    object IInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
      set { ReturnValue = (TR) value; }
    }

    public IEnumerator<object> GetEnumerator ()
    {
      yield return Arg0;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void CopyTo (Array array, int index)
    {
      throw new NotImplementedException();
    }

    public int Count
    {
      get { return 1; }
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }

    public object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
}