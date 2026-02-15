using BookHistoryApi.Api.Middleware;
using BookHistoryApi.Application.Services;
using BookHistoryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => ConfigureSqlite(options));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddExceptionHandler<ExcepitonMiddleware>();
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerGen(options => SetDescription(options));

var app = builder.Build();

await SeedDatabase();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();



void ConfigureSqlite(DbContextOptionsBuilder options)
{
    options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection"));
}

void SetDescription(SwaggerGenOptions options)
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "BookHistoryApi",
        Description = "ASP.NET Core Web API that manages books and books change history"
    });
}

async Task SeedDatabase()
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await ApplicationDbContextSeed.SeedAsync(db);
}