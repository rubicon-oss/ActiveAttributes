using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SignedAssembly;
using UnsignedAssembly;

namespace Test
{
  [TestFixture]
  public class TestClass
  {
    private interface ISignableValidator
    {
      bool IsSignable (Type type);
    }

    public class SignableValidator : ISignableValidator
    {
      public bool IsSignable (Type type)
      {
        if (IsNotSigned (type))
          return false;
          
        return true;
      }

      //private bool IsNotSignable (Type t)
      //{
      //  if (t is MutableType && IsNotSigned (((MutableType) t).UnderlyingType))
      //    return true;

      //  if (t.GetGenericArguments().Any (IsNotSigned))
      //    return true;

      //  if (t.GetInterfaces ().Any (IsNotSignable))
      //    return true;

      //  if (t.GetProperties ().Any (IsNotSigned))
      //    return true;

      //  ////if (t.BaseType != null && !IsSignable (t.BaseType))
      //  ////  return false;

      //  //return true;
      //  return false;
      //}

      private bool IsNotSigned (Type type)
      {
        return !type.Assembly.GetName().GetPublicKey().Any();
      }

      private bool IsNotSigned (PropertyInfo pi)
      {
        return IsNotSigned (pi.PropertyType);
      }
    }

    [Test]
    public void name ()
    {
      var validator = new SignableValidator();

      Assert.That (validator.IsSignable (typeof (SignedType)), Is.True);
      Assert.That (validator.IsSignable (typeof (UnsignedType)), Is.False);
      Assert.That (validator.IsSignable (typeof (IValidInterface)), Is.True);
      Assert.That (validator.IsSignable (typeof (IInvalidInterfaceGeneric)), Is.False);
      Assert.That (validator.IsSignable (typeof (IInvalidInterfaceProperty)), Is.False);
      Assert.That (validator.IsSignable (typeof (IInvalidInterfaceReturnType)), Is.False);
      Assert.That (validator.IsSignable (typeof (IInvalidInterfaceParameterType)), Is.False);
    }

    public interface IValidInterface : IBase<SignedType>
    {
      SignedType Property { get; set; }

      SignedType Method (SignedType argument);
    }

    public interface IBase<T>
    {
      T GenericMethod (T argument);
    }

    public interface IInvalidInterfaceGeneric : IBase<UnsignedType> {}

    public interface IInvalidInterfaceProperty
    {
      UnsignedType Property { get; set; }
    }

    public interface IInvalidInterfaceReturnType
    {
      UnsignedType Method (int argument);
    }

    public interface IInvalidInterfaceParameterType
    {
      int Method (UnsignedType argument);
    }

    public class SignedBaseType : SignedType
    {
       
    }

    //public class UnsignedBaseType : 
  }
}
