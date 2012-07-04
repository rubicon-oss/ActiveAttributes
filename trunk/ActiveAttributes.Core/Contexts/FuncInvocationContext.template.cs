using System;
using System.Reflection;

namespace ActiveAttributes.Core.Contexts
{
  // @begin-template first=0 template=0 generate=0..15 suppressTemplate=true
  // @replace ", TA<n> arg<n>"
  // @replace ", TA<n>"
  public class FuncInvocationContext<TInstance, TA0, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0)
        : base(methodInfo, instance)
    {
      // @begin-repeat
      // @replace-one "<n>"
      Arg0 = arg0;
      // @end-repeat
    }

    // @begin-repeat
    // @replace-one "<n>"
    public TA0 Arg0 { get; set; }
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
        switch (idx)
        {
          // @begin-repeat
          // @replace-one "<n>"
          case 0: return Arg0;
          // @end-repeat
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          // @begin-repeat
          // @replace-one "<n>"
          case 0: Arg0 = (TA0) value; break;
          // @end-repeat
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  // @end-template
}
