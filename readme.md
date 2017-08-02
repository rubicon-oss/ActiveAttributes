# Project Description
Create attributes that execute code when their target members are called.

# Sample
```c#
public class TraceAttribute : ActiveAttribute
{
  public override void OnIntercept (IInvocation invocation)
  {
    Console.WriteLine ("Calling " + invocation.Method.Name + " (" 
            + string.Join (", ", invocation.Arguments.Select (arg => arg.Value)) + ")");

    invocation.Proceed();

    Console.WriteLine ("Exiting " + invocation.Method.Name);
  }
}
```

(Disclaimer: The API is subject to change.)

# Description
ActiveAttributes is an [AOP](http://en.wikipedia.org/wiki/Aspect-oriented_programming) framework implemented in C# that provides facilities to intercept method calls. **CustomAttributes** derived from abstract classes **MethodInterceptionAspectAttribute**, or **PropertyInterceptionAspectAttribute** provide predefined [join points](http://en.wikipedia.org/wiki/Join_point) and can be applied to methods, or properties respectively:

```c#
public class DomainClass
{
  [MethodAspect]
  public virtual void MyMethod(string arg)
  {
    // some code
  }

  [PropertyAspect]
  public virtual string MyProperty { get; set; }
}

public class MethodAspectAttribute : MethodInterceptionAspectAttribute
{
  public override void OnIntercept (IInvocation invocation)
  {
  }
}

public class PropertyAspectAttribute : PropertyInterceptionAspectAttribute
{
  public override void OnInterceptGet (IInvocation invocation)
  {
  }

  public override void OnInterceptSet (IInvocation invocation)
  {
  }
}
```

An object of type **IInvocation** is passed for each join point. It includes members for proceeding the intercepted method, and get/set context information:
```c#
// Proceeding an intercepted method
invocation.Proceed();

// Get the argument list
var args = invocation.Context.Arguments;

// Override the first argument
invocation.Context.Arguments[0] = "new string argument";

// Get the methodInfo of the intercepted method
var methodName = invocation.Context.MethodInfo;

// Get the instance related to the intercepted method
var instance = invocation.Instance;
```

Also, ActiveAttributes provides a basic [pointcut](http://en.wikipedia.org/wiki/Pointcut) language, enabling a developer to pointcut multiple target members based on their namespace, type, and signature:
```c#
// pointcuts all targets within MyNamespace
[DomainAspect(IfType = "MyNamespace.*")]

// pointcuts all targets within MyType
[DomainAspect(IfType = typeof(MyType)]

// pointcuts all void methods without arguments
[DomainAspect(IfSignature = "void *()")]

// pointcuts all methods starting with 'Get'
[DomainAspect(IfSignature = "* Get*(*)")]
```
