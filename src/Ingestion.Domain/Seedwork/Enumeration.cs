namespace JamesPChadwick.Ingestion.Domain.Seedwork
{
  using System;
  using Ardalis.SmartEnum;

  public abstract class Enumeration<TEnum> : Enumeration<TEnum, string>
    where TEnum : SmartEnum<TEnum, string>
  {
    public Enumeration(string name)
      : base(name, name)
    {
    }
  }

  public abstract class Enumeration<TEnum, TValue> : SmartEnum<TEnum, TValue>
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
  {
    public Enumeration(string name, TValue value)
      : base(name, value)
    {
    }
  }
}
