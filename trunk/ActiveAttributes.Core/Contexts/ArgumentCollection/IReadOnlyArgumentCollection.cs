using System.Collections;
using System.Collections.Generic;

namespace ActiveAttributes.Core.Contexts.ArgumentCollection
{
  public interface IReadOnlyArgumentCollection : ICollection, IEnumerable<object>
  {
    object this [int idx] { get; }
  }
}