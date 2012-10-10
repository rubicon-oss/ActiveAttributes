using System.Threading;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Common
{
  /// <summary>
  /// Executes the method call on asynchronously.
  /// </summary>
  public class HandleAsyncAspectAttribute : MethodInterceptionAspectAttribute
  {
    public override void OnIntercept (IInvocation invocation)
    {
      ThreadPool.QueueUserWorkItem (state => invocation.Proceed());
    }
  }
}