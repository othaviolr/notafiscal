using Microsoft.EntityFrameworkCore;
using ServiceFaturamento.Domain.Repositories;
using ServiceFaturamento.Infrastructure.Data;
using ServiceFaturamento.Infrastructure.Data.Repositories;
using ServiceFaturamento.Infrastructure.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=postgres-faturamento;Database=faturamento_db;Username=postgres;Password=postgres123";

builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();

builder.Services.AddHttpClient<EstoqueApiClient>();
builder.Services.AddScoped<EstoqueApiClient>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();