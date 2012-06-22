using System.Collections.Generic;

namespace ActiveAttributes.Core.Contexts.ArgumentCollection
{
  public interface IArgumentCollection : IEnumerable<object>
  {
    object this [int idx] { get; set; }
  }
}