using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using ActiveAttributes.Core.Contexts.ArgumentCollection;

namespace ActiveAttributes.Core.Contexts
{
  public abstract class FuncInvocationContextBase<TInstance, TR> : IInvocationContext, IReadOnlyInvocationContext, IArgumentCollection, IReadOnlyArgumentCollection
  {
    protected FuncInvocationContextBase (MethodInfo methodInfo, TInstance instance)
    {
      MethodInfo = methodInfo;
      Instance = instance;
    }

    public MethodInfo MethodInfo { get; private set; }
    public TInstance Instance { get; private set; }
    public TR ReturnValue { get; set; }

    public abstract int Count { get; }
    public abstract object this [int idx] { get; set; }

    object IInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
      set { ReturnValue = (TR) value; }
    }

    object IReadOnlyInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
    }

    object IInvocationContext.Instance
    {
      get { return Instance; }
    }

    object IReadOnlyInvocationContext.Instance
    {
      get { return Instance; }
    }

    IArgumentCollection IInvocationContext.Arguments
    {
      get { return this; }
    }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return this; }
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
}