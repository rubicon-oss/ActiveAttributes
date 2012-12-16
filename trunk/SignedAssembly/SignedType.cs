using System;

namespace SignedAssembly
{
  public class SignedType {}

  public interface ISignedBase<T>
  {
    T GenericMethod (T argument);
  }

  public interface IValidInterface : ISignedBase<SignedType>
  {
    SignedType Property { get; set; }

    SignedType Method (SignedType argument);
  }
}
