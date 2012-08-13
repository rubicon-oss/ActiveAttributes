//Sample license text.

using System;
using System.ComponentModel;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.UseCases.Aspects
{
  public class NotifyPropertyChangedAspectAttribute : PropertyInterceptionAspectAttribute
  {
    public override void OnInterceptGet (IPropertyInvocation invocation)
    {
      invocation.Proceed();
    }

    public override void OnInterceptSet (IPropertyInvocation invocation)
    {
      var instance = invocation.Context.Instance;
      var declaringType = instance.GetType();
      var fieldInfo = declaringType.GetField ("NotifyPropertyChanged");
      var notifyEvent = (MulticastDelegate) fieldInfo.GetValue (instance);
      var eventArgs = new PropertyChangedEventArgs (invocation.Context.PropertyInfo.Name);
      notifyEvent.DynamicInvoke (instance, eventArgs);
    }
  }
}