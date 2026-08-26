using System.ComponentModel.DataAnnotations;
using TestTask.Api.Domain;

namespace TestTask.Api.Contracts;

public sealed class CreateLeadRequest
{
    [Required, StringLength(120, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(30, MinimumLength = 5)]
    [RegularExpression(@"^[+\d][\d\s()\-]{4,29}$", ErrorMessage = "Укажите телефон в свободном формате, начиная с + или цифры.")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Comment { get; set; }

    [Required, StringLength(80, MinimumLength = 1)]
    public string Source { get; set; } = string.Empty;
}

public sealed class UpdateLeadStatusRequest
{
    [Required]
    public LeadStatus Status { get; set; }
}

public sealed record LeadDto(
    Guid Id,
    string Name,
    string Phone,
    string? Comment,
    string Source,
    LeadStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
