namespace TestTask.Api.Domain;

public sealed class Lead
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public string? Comment { get; set; }
    public required string Source { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
