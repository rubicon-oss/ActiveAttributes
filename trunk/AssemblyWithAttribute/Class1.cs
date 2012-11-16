using System;
using ActiveAttributes.Aspects;
using AssemblyWithAttribute;

[assembly: DomainAspect]

namespace AssemblyWithAttribute
{
  public class DomainAspectAttribute : AspectAttributeBase {}
}