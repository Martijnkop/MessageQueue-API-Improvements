namespace Social.Domain.Models.Base;

public abstract record Entity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
}
