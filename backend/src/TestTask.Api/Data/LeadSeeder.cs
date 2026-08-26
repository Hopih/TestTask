using Microsoft.EntityFrameworkCore;
using TestTask.Api.Domain;

namespace TestTask.Api.Data;

public static class LeadSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Leads.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        db.Leads.AddRange(
            new Lead
            {
                Id = Guid.NewGuid(),
                Name = "Анна Козлова",
                Phone = "+7 921 555-14-20",
                Comment = "Просит перезвонить после 18:00",
                Source = "Сайт",
                Status = LeadStatus.New,
                CreatedAt = now.AddHours(-6),
                UpdatedAt = now.AddHours(-6)
            },
            new Lead
            {
                Id = Guid.NewGuid(),
                Name = "Игорь Смирнов",
                Phone = "8 812 310-00-11",
                Comment = "Интересуется тарифом для команды из 8 человек",
                Source = "Телефон",
                Status = LeadStatus.InProgress,
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddHours(-2)
            },
            new Lead
            {
                Id = Guid.NewGuid(),
                Name = "Мария Белова",
                Phone = "+7 903 441-77-09",
                Comment = "Уже работает с конкурентом, сравнить условия",
                Source = "Telegram",
                Status = LeadStatus.Rejected,
                CreatedAt = now.AddDays(-3),
                UpdatedAt = now.AddDays(-2)
            });

        await db.SaveChangesAsync(cancellationToken);
    }
}
