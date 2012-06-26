using System.Collections;
using System.Collections.Generic;

namespace ActiveAttributes.Core.Contexts.ArgumentCollection
{
  public interface IArgumentCollection : ICollection, IEnumerable<object>
  {
    object this [int idx] { get; set; }
  }
}