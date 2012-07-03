// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

//internal class C
//{
//  private MethodInterceptionAspectAttribute[] _m_aspects;

//  private IInvocation _m_methodInfo;
//  private Func<string, int> _m__originalBody;
//  private MethodInterceptionAspectAttribute _m_aspect1;
//  private MethodInterceptionAspectAttribute _m_aspect2;

//  public C ()
//  {
//    _m_aspect1 = (MethodInterceptionAspectAttribute) new MyAspect ();
//    _m_aspect2 = (MethodInterceptionAspectAttribute) new MyAspect2 ();
//    _m_aspects = new[] { _m_aspect1, _m_aspect2 };

//    _m_methodInfo = methodof (M);
//    _m__originalBody = Dummy;
//  }

//  public virtual int M (string p1)
//  {
//    var argsAndReturnValue = new ArgsAndReturnValue<string, int> (p1);

//    var invocationForAspect1 = new FuncInvocationX<C, string, int> (this, argsAndReturnValue, _m__originalBody);
//    var invocationForAspect2 = new FuncInvocationX<C, string, int> (this, argsAndReturnValue, _m_aspect1, invocationForAspect1);
//    _m_aspect2.OnIntercept (invocationForAspect2);

//    return argsAndReturnValue.ReturnValue;
//  }

//  private int Dummy (string p1)
//  {
//    // Original body of M
//  }
//}

//public class FuncInvocationX<TInstance, TA0, TR>
//{
//  private TInstance _instance;
//  private readonly ArgsAndReturnValue<TA0, TR> _argsAndReturnValue;

//  private IProceedable _next;

//  public FuncInvocation (TInstance instance, ArgsAndReturnValue<TA0, TR> argsAndReturnValue, IProceedable next)
//  {
//    _instance = instance;
//    _argsAndReturnValue = argsAndReturnValue;
//    _next = next;
//  }

//  public IArgumentIndexer Arguments
//  {
//    get { return _argsAndReturnValue; }
//  }

//  public object ReturnValue
//  {
//    get { return _argsAndReturnValue.ReturnValue; }
//  }

//  public void Proceed ()
//  {
//    _next.Proceed (_argsAndReturnValue);
//  }
//}


//public class ActionInvocation : Invocation
//{
//  private readonly Action _action;
//  private readonly int _currentAspect;

//  public ActionInvocation (MethodInfo methodInfo, object instance, )
//      : base (innerInvocation, methodInfo, instance)
//  {
//    _action = action;
//  }

//  public override void Proceed ()
//  {
//    if (_currentAspect < _aspects.Length)
//      _aspects[_currentAspect++].OnIntercept (this);
//    else
//      _action();
//  }
//}





















//public class ActionInvocation : Invocation
//{
//  private readonly Action _action;

//  public ActionInvocation (IInvocation innerInvocation, MethodInfo methodInfo, object instance, Action action)
//      : base (innerInvocation, methodInfo, instance)
//  {
//    _action = action;
//  }

//  public override void Proceed ()
//  {
//    if (InnerInvocation == null)
//      _action();
//    else
//      InnerInvocation.Proceed();
//  }
//}

//public class ActionInvocation<TInstance> : Invocation<TInstance>
//{
//  private readonly Action _action;

//  public ActionInvocation (IInvocation innerInvocation, MethodInfo methodInfo, TInstance instance, Action action)
//      : base (innerInvocation, methodInfo, instance)
//  {
//    _action = action;
//  }

//  public override void Proceed ()
//  {
//    if (InnerInvocation == null)
//      _action();
//    else
//      InnerInvocation.Proceed();
//  }
//}

//public class ActionInvocation<TInstance, TA0> : Invocation<TInstance>
//{
//  private readonly Action<TA0> _action;

//  public ActionInvocation (IInvocation innerInvocation, MethodInfo methodInfo, TInstance instance, Action<TA0> action, TA0 arg0)
//      : base (innerInvocation, methodInfo, instance)
//  {
//    _action = action;
//    Arg0 = arg0;
//  }

//  public TA0 Arg0 { get; set; }

//  public override void Proceed ()
//  {
//    if (InnerInvocation == null)
//      _action (Arg0);
//    else
//      InnerInvocation.Proceed();
//  }
//}