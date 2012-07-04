//------------------------------------------------------------------------------
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if 
// the code is regenerated.
//
//------------------------------------------------------------------------------
using System;
using System.Reflection;

namespace ActiveAttributes.Core.Contexts
{
  public class FuncInvocationContext<TInstance, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance)
        : base(methodInfo, instance)
    {
    }


    public override int Count
    {
      get { return 0; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }

    public override int Count
    {
      get { return 2; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }

    public override int Count
    {
      get { return 3; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }

    public override int Count
    {
      get { return 4; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }

    public override int Count
    {
      get { return 5; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TA5, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
      Arg5 = arg5;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }
    public TA5 Arg5 { get; set; }

    public override int Count
    {
      get { return 6; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          case 5: return Arg5;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          case 5: Arg5 = (TA5) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TA5, TA6, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
      Arg5 = arg5;
      Arg6 = arg6;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }
    public TA5 Arg5 { get; set; }
    public TA6 Arg6 { get; set; }

    public override int Count
    {
      get { return 7; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          case 5: return Arg5;
          case 6: return Arg6;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          case 5: Arg5 = (TA5) value; break;
          case 6: Arg6 = (TA6) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6, TA7 arg7)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
      Arg5 = arg5;
      Arg6 = arg6;
      Arg7 = arg7;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }
    public TA5 Arg5 { get; set; }
    public TA6 Arg6 { get; set; }
    public TA7 Arg7 { get; set; }

    public override int Count
    {
      get { return 8; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          case 5: return Arg5;
          case 6: return Arg6;
          case 7: return Arg7;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          case 5: Arg5 = (TA5) value; break;
          case 6: Arg6 = (TA6) value; break;
          case 7: Arg7 = (TA7) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6, TA7 arg7, TA8 arg8)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
      Arg5 = arg5;
      Arg6 = arg6;
      Arg7 = arg7;
      Arg8 = arg8;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }
    public TA5 Arg5 { get; set; }
    public TA6 Arg6 { get; set; }
    public TA7 Arg7 { get; set; }
    public TA8 Arg8 { get; set; }

    public override int Count
    {
      get { return 9; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          case 5: return Arg5;
          case 6: return Arg6;
          case 7: return Arg7;
          case 8: return Arg8;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          case 5: Arg5 = (TA5) value; break;
          case 6: Arg6 = (TA6) value; break;
          case 7: Arg7 = (TA7) value; break;
          case 8: Arg8 = (TA8) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TA9, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6, TA7 arg7, TA8 arg8, TA9 arg9)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
      Arg5 = arg5;
      Arg6 = arg6;
      Arg7 = arg7;
      Arg8 = arg8;
      Arg9 = arg9;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }
    public TA5 Arg5 { get; set; }
    public TA6 Arg6 { get; set; }
    public TA7 Arg7 { get; set; }
    public TA8 Arg8 { get; set; }
    public TA9 Arg9 { get; set; }

    public override int Count
    {
      get { return 10; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          case 5: return Arg5;
          case 6: return Arg6;
          case 7: return Arg7;
          case 8: return Arg8;
          case 9: return Arg9;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          case 5: Arg5 = (TA5) value; break;
          case 6: Arg6 = (TA6) value; break;
          case 7: Arg7 = (TA7) value; break;
          case 8: Arg8 = (TA8) value; break;
          case 9: Arg9 = (TA9) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TA9, TA10, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6, TA7 arg7, TA8 arg8, TA9 arg9, TA10 arg10)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
      Arg5 = arg5;
      Arg6 = arg6;
      Arg7 = arg7;
      Arg8 = arg8;
      Arg9 = arg9;
      Arg10 = arg10;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }
    public TA5 Arg5 { get; set; }
    public TA6 Arg6 { get; set; }
    public TA7 Arg7 { get; set; }
    public TA8 Arg8 { get; set; }
    public TA9 Arg9 { get; set; }
    public TA10 Arg10 { get; set; }

    public override int Count
    {
      get { return 11; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          case 5: return Arg5;
          case 6: return Arg6;
          case 7: return Arg7;
          case 8: return Arg8;
          case 9: return Arg9;
          case 10: return Arg10;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          case 5: Arg5 = (TA5) value; break;
          case 6: Arg6 = (TA6) value; break;
          case 7: Arg7 = (TA7) value; break;
          case 8: Arg8 = (TA8) value; break;
          case 9: Arg9 = (TA9) value; break;
          case 10: Arg10 = (TA10) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TA9, TA10, TA11, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6, TA7 arg7, TA8 arg8, TA9 arg9, TA10 arg10, TA11 arg11)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
      Arg5 = arg5;
      Arg6 = arg6;
      Arg7 = arg7;
      Arg8 = arg8;
      Arg9 = arg9;
      Arg10 = arg10;
      Arg11 = arg11;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }
    public TA5 Arg5 { get; set; }
    public TA6 Arg6 { get; set; }
    public TA7 Arg7 { get; set; }
    public TA8 Arg8 { get; set; }
    public TA9 Arg9 { get; set; }
    public TA10 Arg10 { get; set; }
    public TA11 Arg11 { get; set; }

    public override int Count
    {
      get { return 12; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          case 5: return Arg5;
          case 6: return Arg6;
          case 7: return Arg7;
          case 8: return Arg8;
          case 9: return Arg9;
          case 10: return Arg10;
          case 11: return Arg11;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          case 5: Arg5 = (TA5) value; break;
          case 6: Arg6 = (TA6) value; break;
          case 7: Arg7 = (TA7) value; break;
          case 8: Arg8 = (TA8) value; break;
          case 9: Arg9 = (TA9) value; break;
          case 10: Arg10 = (TA10) value; break;
          case 11: Arg11 = (TA11) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TA9, TA10, TA11, TA12, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6, TA7 arg7, TA8 arg8, TA9 arg9, TA10 arg10, TA11 arg11, TA12 arg12)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
      Arg5 = arg5;
      Arg6 = arg6;
      Arg7 = arg7;
      Arg8 = arg8;
      Arg9 = arg9;
      Arg10 = arg10;
      Arg11 = arg11;
      Arg12 = arg12;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }
    public TA5 Arg5 { get; set; }
    public TA6 Arg6 { get; set; }
    public TA7 Arg7 { get; set; }
    public TA8 Arg8 { get; set; }
    public TA9 Arg9 { get; set; }
    public TA10 Arg10 { get; set; }
    public TA11 Arg11 { get; set; }
    public TA12 Arg12 { get; set; }

    public override int Count
    {
      get { return 13; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          case 5: return Arg5;
          case 6: return Arg6;
          case 7: return Arg7;
          case 8: return Arg8;
          case 9: return Arg9;
          case 10: return Arg10;
          case 11: return Arg11;
          case 12: return Arg12;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          case 5: Arg5 = (TA5) value; break;
          case 6: Arg6 = (TA6) value; break;
          case 7: Arg7 = (TA7) value; break;
          case 8: Arg8 = (TA8) value; break;
          case 9: Arg9 = (TA9) value; break;
          case 10: Arg10 = (TA10) value; break;
          case 11: Arg11 = (TA11) value; break;
          case 12: Arg12 = (TA12) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TA9, TA10, TA11, TA12, TA13, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6, TA7 arg7, TA8 arg8, TA9 arg9, TA10 arg10, TA11 arg11, TA12 arg12, TA13 arg13)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
      Arg5 = arg5;
      Arg6 = arg6;
      Arg7 = arg7;
      Arg8 = arg8;
      Arg9 = arg9;
      Arg10 = arg10;
      Arg11 = arg11;
      Arg12 = arg12;
      Arg13 = arg13;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }
    public TA5 Arg5 { get; set; }
    public TA6 Arg6 { get; set; }
    public TA7 Arg7 { get; set; }
    public TA8 Arg8 { get; set; }
    public TA9 Arg9 { get; set; }
    public TA10 Arg10 { get; set; }
    public TA11 Arg11 { get; set; }
    public TA12 Arg12 { get; set; }
    public TA13 Arg13 { get; set; }

    public override int Count
    {
      get { return 14; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          case 5: return Arg5;
          case 6: return Arg6;
          case 7: return Arg7;
          case 8: return Arg8;
          case 9: return Arg9;
          case 10: return Arg10;
          case 11: return Arg11;
          case 12: return Arg12;
          case 13: return Arg13;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          case 5: Arg5 = (TA5) value; break;
          case 6: Arg6 = (TA6) value; break;
          case 7: Arg7 = (TA7) value; break;
          case 8: Arg8 = (TA8) value; break;
          case 9: Arg9 = (TA9) value; break;
          case 10: Arg10 = (TA10) value; break;
          case 11: Arg11 = (TA11) value; break;
          case 12: Arg12 = (TA12) value; break;
          case 13: Arg13 = (TA13) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
  public class FuncInvocationContext<TInstance, TA0, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TA9, TA10, TA11, TA12, TA13, TA14, TR> : FuncInvocationContextBase<TInstance, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4, TA5 arg5, TA6 arg6, TA7 arg7, TA8 arg8, TA9 arg9, TA10 arg10, TA11 arg11, TA12 arg12, TA13 arg13, TA14 arg14)
        : base(methodInfo, instance)
    {
      Arg0 = arg0;
      Arg1 = arg1;
      Arg2 = arg2;
      Arg3 = arg3;
      Arg4 = arg4;
      Arg5 = arg5;
      Arg6 = arg6;
      Arg7 = arg7;
      Arg8 = arg8;
      Arg9 = arg9;
      Arg10 = arg10;
      Arg11 = arg11;
      Arg12 = arg12;
      Arg13 = arg13;
      Arg14 = arg14;
    }

    public TA0 Arg0 { get; set; }
    public TA1 Arg1 { get; set; }
    public TA2 Arg2 { get; set; }
    public TA3 Arg3 { get; set; }
    public TA4 Arg4 { get; set; }
    public TA5 Arg5 { get; set; }
    public TA6 Arg6 { get; set; }
    public TA7 Arg7 { get; set; }
    public TA8 Arg8 { get; set; }
    public TA9 Arg9 { get; set; }
    public TA10 Arg10 { get; set; }
    public TA11 Arg11 { get; set; }
    public TA12 Arg12 { get; set; }
    public TA13 Arg13 { get; set; }
    public TA14 Arg14 { get; set; }

    public override int Count
    {
      get { return 15; }
    }

    public override object this [int idx]
    {
      get
      {
        switch (idx)
        {
          case 0: return Arg0;
          case 1: return Arg1;
          case 2: return Arg2;
          case 3: return Arg3;
          case 4: return Arg4;
          case 5: return Arg5;
          case 6: return Arg6;
          case 7: return Arg7;
          case 8: return Arg8;
          case 9: return Arg9;
          case 10: return Arg10;
          case 11: return Arg11;
          case 12: return Arg12;
          case 13: return Arg13;
          case 14: return Arg14;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
      set
      {
        switch (idx)
        {
          case 0: Arg0 = (TA0) value; break;
          case 1: Arg1 = (TA1) value; break;
          case 2: Arg2 = (TA2) value; break;
          case 3: Arg3 = (TA3) value; break;
          case 4: Arg4 = (TA4) value; break;
          case 5: Arg5 = (TA5) value; break;
          case 6: Arg6 = (TA6) value; break;
          case 7: Arg7 = (TA7) value; break;
          case 8: Arg8 = (TA8) value; break;
          case 9: Arg9 = (TA9) value; break;
          case 10: Arg10 = (TA10) value; break;
          case 11: Arg11 = (TA11) value; break;
          case 12: Arg12 = (TA12) value; break;
          case 13: Arg13 = (TA13) value; break;
          case 14: Arg14 = (TA14) value; break;
          default: throw new IndexOutOfRangeException ("idx");
        }
      }
    }
  }
}
