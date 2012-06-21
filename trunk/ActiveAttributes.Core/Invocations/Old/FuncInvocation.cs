//using System;
//using System.Reflection;
//using ActiveAttributes.Core.Aspects;

//namespace ActiveAttributes.Core.Invocations
//{
//  public abstract class FuncInvocationBase<TInstance, TResult> : Invocation<TInstance>, IInvocationInfo<TInstance, TResult>
//  {
//    protected FuncInvocationBase (IInvocation innerInvocation, MethodInfo methodInfo, TInstance instance)
//        : base (innerInvocation, methodInfo, instance) {}

//    #region IInvocationInfo<TInstance,TResult> Members

//    public new TResult ReturnValue
//    {
//      get { return (TResult) base.ReturnValue; }
//      set { base.ReturnValue = value; }
//    }

//    #endregion
//  }

//  public class FuncInvocation<TInstance, TResult> : FuncInvocationBase<TInstance, TResult>
//  {
//    private readonly Func<TResult> _func;

//    public FuncInvocation (IInvocation innerInvocation, MethodInfo methodInfo, TInstance instance, Func<TResult> func)
//        : base (innerInvocation, methodInfo, instance)
//    {
//      _func = func;
//    }

//    public override void Proceed ()
//    {
//      if (InnerInvocation == null)
//        ReturnValue = _func();
//      else
//        InnerInvocation.Proceed();
//    }
//  }

//  public class FuncInvocation<TInstance, TA0, TResult> : FuncInvocationBase<TInstance, TResult>
//  {
//    private readonly Func<TA0, TResult> _func;

//    public FuncInvocation (MethodInterceptionAspectAttribute aspect, 
//      IInvocation innerInvocation, MethodInfo methodInfo, TInstance instance, Func<TA0, TResult> func, TA0 arg0)
//        : base (innerInvocation, methodInfo, instance)
//    {
//      _func = func;
//      Arg0 = arg0;
//    }

//    public TA0 Arg0 { get; set; }

//    public override void Proceed ()
//    {
//      if (InnerInvocation == null)
//        ReturnValue = _func (Arg0);
//      else
//        InnerInvocation.Proceed();
//        //aspect.OnIntercept (InnerInvocation);
//    }
//  }
//}