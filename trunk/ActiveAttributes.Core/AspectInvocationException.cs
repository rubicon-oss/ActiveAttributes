using System;
using System.Runtime.Serialization;

namespace ActiveAttributes.Core
{
  /// <summary>
  /// Exception that is thrown when an aspect invocation throws an exception.
  /// </summary>
  [Serializable]
  public class AspectInvocationException : Exception
  {
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public AspectInvocationException () {}

    public AspectInvocationException (string message)
        : base (message) {}

    public AspectInvocationException (string message, Exception inner)
        : base (message, inner) {}

    protected AspectInvocationException (
        SerializationInfo info,
        StreamingContext context)
        : base (info, context) {}
  }
}