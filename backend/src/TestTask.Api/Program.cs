using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TestTask.Api.Data;
using TestTask.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddOpenApi();
builder.Services.AddScoped<LeadService>();

var connectionString = BuildConnectionString(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var frontendOrigin = builder.Configuration["Cors:FrontendOrigin"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    await LeadSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Frontend");
app.MapControllers();
app.Run();

static string BuildConnectionString(IConfiguration config)
{
    var raw = config.GetConnectionString("Postgres")
              ?? throw new InvalidOperationException("Не задана строка подключения ConnectionStrings:Postgres.");

    var cs = new NpgsqlConnectionStringBuilder(raw);
    var password = config["Postgres:Password"]
                   ?? Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

    if (!string.IsNullOrWhiteSpace(password))
    {
        cs.Password = password;
    }

    if (string.IsNullOrWhiteSpace(cs.Password))
    {
        throw new InvalidOperationException(
            "Пароль PostgreSQL не задан. Задайте переменную окружения POSTGRES_PASSWORD.");
    }

    return cs.ConnectionString;
}
