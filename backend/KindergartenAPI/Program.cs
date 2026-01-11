using FluentValidation;
using FluentValidation.AspNetCore;
using KindergartenAPI.Data;
using KindergartenAPI.Routes;
using KindergartenAPI.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Definir el nombre de la política CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Configurar CORS para permitir el frontend en localhost:3039 (puerto Vite)
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3039", "http://localhost:3000", "http://localhost")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Registrando EF Core con SQL Server
builder.Services.AddDbContext<KindergartenContext>(options =>
{
    if (builder.Environment.IsEnvironment("Test"))
    {
        // Usar InMemory para tests
        options.UseInMemoryDatabase("TestDb");
    }
    else
    {
        // SQL Server en los demás entornos
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

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

// Agregando AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly); // Registrando AutoMapper

// Agregando FluentValidation
builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<Program>();

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

app.UseCors(MyAllowSpecificOrigins);

app.MapNinosRoutes();
app.MapPersonaRoutes();

if (!builder.Environment.IsEnvironment("Test"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<KindergartenContext>();

            context.Database.EnsureCreated();

            Console.WriteLine("--> Base de datos creada/verificada correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Error creando la DB: {ex.Message}");
        }
    }
}


app.Run();

/*
var port = Environment.GetEnvironmentVariable("ASPNETCORE_PORT") ?? "5214";
app.Urls.Clear();
app.Urls.Add($"http://*:{port}");
*/
public partial class Program { }
