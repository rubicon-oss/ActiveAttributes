using System;
using System.Reflection;
using Remotion.Logging;

namespace ActiveAttributes.Core.Contexts
{
  // @begin-template first=1 template=1 generate=0..15 suppressTemplate=true
  // @replace ", TA<n> arg<n>"
  // @replace ", TA<n>"
  public class FuncInvocationContext<TInstance, TA1, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (IInvocationContext));

    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA1 arg1)
        : base(methodInfo, instance)
    {
      // @begin-repeat
      // @replace-one "<n>"
      Arg1 = arg1;
      // @end-repeat
    }

    // @begin-repeat
    // @replace-one "<n>"
    public TA1 Arg1 { get; set; }
    // @end-repeat

    public override int Count
    {
      // @replace-one "return <n>"
      get { return 1; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx + 1)
        {
          // @begin-repeat
          // @replace-one "<n>"
          case 1: return Arg1;
          // @end-repeat
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx + 1)
        {
          // @begin-repeat
          // @replace-one "<n>"
          case 1: Arg1 = (TA1) value; s_log.DebugFormat ("Set 'Arg1' of method '{0}' to '{1}'.", MethodInfo, value); break;
          // @end-repeat
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  // @end-template
}
