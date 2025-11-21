using Microsoft.EntityFrameworkCore;
using FerreteriaAPI.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // ✅ AGREGAR ESTO

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS para React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Entity Framework con MySQL 
builder.Services.AddDbContext<FerreteriaContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Debug: Listar endpoints
    app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
    {
        var endpoints = endpointSources.SelectMany(es => es.Endpoints);
        return Results.Ok(endpoints.Select(e => e.DisplayName));
    });
}

app.UseCors("AllowReact");

// Solo usar HTTPS redirection en producción
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

// Mapear controllers
app.MapControllers();

// Endpoint raíz para verificar que la API está funcionando
app.MapGet("/", () => "🚀 Ferreteria API está funcionando correctamente!");

app.Run();