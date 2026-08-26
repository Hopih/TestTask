using Microsoft.EntityFrameworkCore;
using TestTask.Api.Contracts;
using TestTask.Api.Data;
using TestTask.Api.Domain;

namespace TestTask.Api.Services;

public sealed class LeadService(AppDbContext db)
{
    public async Task<IReadOnlyList<LeadDto>> ListAsync(LeadStatus? status, CancellationToken cancellationToken)
    {
        var query = db.Leads.AsNoTracking();
        if (status is not null)
        {
            query = query.Where(x => x.Status == status);
        }

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return items.Select(ToDto).ToList();
    }

    public async Task<LeadDto> CreateAsync(CreateLeadRequest request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var lead = new Lead
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Phone = NormalizePhone(request.Phone),
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            Source = request.Source.Trim(),
            Status = LeadStatus.New,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Leads.Add(lead);
        await db.SaveChangesAsync(cancellationToken);
        return ToDto(lead);
    }

    public async Task<LeadDto?> UpdateStatusAsync(Guid id, LeadStatus status, CancellationToken cancellationToken)
    {
        var lead = await db.Leads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (lead is null)
        {
            return null;
        }

        lead.Status = status;
        lead.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return ToDto(lead);
    }

    private static string NormalizePhone(string phone) =>
        string.Join(" ", phone.Split(' ', StringSplitOptions.RemoveEmptyEntries)).Trim();

    private static LeadDto ToDto(Lead lead) => new(
        lead.Id,
        lead.Name,
        lead.Phone,
        lead.Comment,
        lead.Source,
        lead.Status,
        lead.CreatedAt,
        lead.UpdatedAt);
}
