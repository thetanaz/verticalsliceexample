namespace TravelInspiration.API.Shared.Domain.Entities;

public class Stop :AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Uri? ImageUri { get; set; }
    public Itenerary? Itenerary { get; set; }

    public IteneraryId {
        get;
        set;
    }
    }
}