using KindergartenAPI.Data;
using KindergartenAPI.Routes;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Registrando EF Core con SQL Server
builder.Services.AddDbContext<KindergartenContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregando Swagger/OpenAPI para documentación
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Guarderia API",
        Version = "v1",
        Description = "API para gestión de guardería"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                // Para generar el JSON de Swagger
    app.UseSwaggerUI(c =>            // Interfaz web de Swagger
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Guarderia API v1");
        c.RoutePrefix = "";          // Swagger UI está en la raíz (http://localhost:5000/)
    });
}

app.MapNinosRoutes();
app.MapPersonaRoutes();

app.Run();
