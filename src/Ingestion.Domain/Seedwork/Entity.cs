namespace JamesPChadwick.Ingestion.Domain.Seedwork
{
  using System.Collections.Generic;
  using MediatR;

  public abstract class Entity
  {
    private int? hashCode;

    private List<INotification>? events;

    public int Id { get; private set; }

    public bool IsTransient => Id == default;

    public IReadOnlyCollection<INotification>? Events => events?.AsReadOnly();

    public static bool operator ==(Entity left, Entity right)
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

    public static bool operator !=(Entity left, Entity right)
    {
      return !(left == right);
    }

    public override bool Equals(object? obj)
    {
      if (obj == null || obj is not Entity)
      {
        return false;
      }

      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      if (GetType() != obj.GetType())
      {
        return false;
      }

      Entity other = (Entity)obj;

      if (IsTransient || other.IsTransient)
      {
        return false;
      }
      else
      {
        return Id == other.Id;
      }
    }

    public override int GetHashCode()
    {
      if (!IsTransient)
      {
        if (!hashCode.HasValue)
        {
          hashCode = Id.GetHashCode() ^ 31;
        }

        return hashCode.Value;
      }
      else
      {
        return base.GetHashCode();
      }
    }

    public void AddEvent(INotification @event)
    {
      events ??= new List<INotification>();
      events.Add(@event);
    }

    public void RemoveEvent(INotification @event)
    {
      events?.Remove(@event);
    }

    public void ClearEvents()
    {
      events?.Clear();
    }
  }
}
