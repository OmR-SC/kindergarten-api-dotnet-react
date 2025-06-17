using KindergartenAPI.Models;
using KindergartenAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;



namespace KindergartenAPI.Routes
{
    public static class PersonaRoutes
{
    public static WebApplication MapPersonaRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/personas").WithTags("Personas");

        group.MapGet("/", async (KindergartenContext db) =>
        {
            var personas = await db.Personas
                .Include(p => p.Nino_Autorizados)
                    .ThenInclude(na => na.MatriculaNavigation)
                .ToListAsync();
            return Results.Ok(personas);

        });

        group.MapGet("/{cedula}", async (string cedula, KindergartenContext db) =>
        {
            var persona = await db.Personas
                .Include(p => p.Nino_Autorizados)
                    .ThenInclude(na => na.MatriculaNavigation)
                .FirstOrDefaultAsync(p => p.Cedula == cedula);

            return persona is not null ? Results.Ok(persona) : Results.NotFound();
        });

        group.MapPost("/", async (Persona persona, KindergartenContext db) =>
        {
            db.Personas.Add(persona);
            await db.SaveChangesAsync();
            return Results.Created($"/api/personas/{persona.Cedula}", persona);
        });

        group.MapPut("/{cedula}", async (string cedula, Persona updatedPersona, KindergartenContext db) =>
        {
            var persona = await db.Personas.FindAsync(cedula);
            if (persona is null)
            {
                return Results.NotFound();
            }

            persona.Nombre = updatedPersona.Nombre;
            persona.Telefono = updatedPersona.Telefono;
            persona.Direccion = updatedPersona.Direccion;
            persona.CuentaCorriente = updatedPersona.CuentaCorriente;

            await db.SaveChangesAsync();
            return Results.NoContent();

        });

        group.MapDelete("/{cedula}", async (string cedula, KindergartenContext db) =>
        {
            var persona = await db.Personas.FindAsync(cedula);
            if (persona is null)
            {
                return Results.NotFound();
            }

            db.Personas.Remove(persona);
            await db.SaveChangesAsync();
            return Results.NoContent();


        });

        return app;

    }
}
}