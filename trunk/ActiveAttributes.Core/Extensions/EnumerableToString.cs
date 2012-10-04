using System;
using System.Collections.Generic;
using System.Text;

internal static partial class EnumerableExtensions
{
  public static string ToString<T> (this IEnumerable<T> collection)
  {
    return ToString (collection, t => t.ToString (), ", ");
  }

  public static string ToString<T> (this IEnumerable<T> collection, string separator)
  {
    return ToString (collection, t => t.ToString (), separator);
  }

  public static string ToString<T> (this IEnumerable<T> collection, Func<T, string> stringElement, string separator)
  {
    var sb = new StringBuilder ();
    foreach (var item in collection)
    {
      sb.Append (stringElement (item));
      sb.Append (separator);
    }
    return sb.ToString (0, Math.Max (0, sb.Length - separator.Length));
  }
}