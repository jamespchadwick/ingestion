namespace JamesPChadwick.Ingestion.Domain.Seedwork
{
  using System.Collections.Generic;
  using System.Linq;

  public abstract class ValueObject
  {
    public static bool operator ==(ValueObject left, ValueObject right)
    {
      if (Equals(left, null))
      {
        return Equals(right, null);
      }
      else
      {
        return left.Equals(right);
      }
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
      return !(left == right);
    }

    public override bool Equals(object? obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }

      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      ValueObject other = (ValueObject)obj;
      IEnumerator<object> thisValues = GetAtomicValues().GetEnumerator();
      IEnumerator<object> othervalues = other.GetAtomicValues().GetEnumerator();

      while (thisValues.MoveNext() && othervalues.MoveNext())
      {
        if (thisValues.Current is null ^ othervalues.Current is null)
        {
          return false;
        }

        if (thisValues.Current != null && !thisValues.Current.Equals(othervalues.Current))
        {
          return false;
        }
      }

      return !thisValues.MoveNext() && !othervalues.MoveNext();
    }

    public override int GetHashCode()
    {
      return GetAtomicValues()
             .Select(x => x != null ? x.GetHashCode() : 0)
             .Aggregate((x, y) => x ^ y);
    }

    protected abstract IEnumerable<object> GetAtomicValues();
  }
}
