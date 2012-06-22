using System.Collections.Generic;

namespace ActiveAttributes.Core.Contexts.ArgumentCollection
{
  public interface IReadOnlyArgumentCollection : IEnumerable<object>
  {
    object this [int idx] { get; }
  }
}