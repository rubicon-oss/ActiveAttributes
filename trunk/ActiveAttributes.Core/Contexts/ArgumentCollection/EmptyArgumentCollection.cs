using System;
using System.Collections;
using System.Collections.Generic;

namespace ActiveAttributes.Core.Contexts.ArgumentCollection
{
  public class EmptyArgumentCollection : IArgumentCollection, IReadOnlyArgumentCollection
  {
    public IEnumerator<object> GetEnumerator ()
    {
      yield break;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public object this [int idx]
    {
      get { throw new IndexOutOfRangeException("idx"); }
      set { throw new IndexOutOfRangeException("idx"); }
    }

    public void CopyTo (Array array, int index)
    {
      throw new NotImplementedException();
    }

    public int Count
    {
      get { return 0; }
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }
  }
}