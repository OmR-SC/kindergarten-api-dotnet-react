using KindergartenAPI.Models;
using KindergartenAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using KindergartenAPI.DTOs.Personas;
using FluentValidation;



namespace KindergartenAPI.Routes
{
    public static class PersonaRoutes
{
    public static WebApplication MapPersonaRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/personas").WithTags("Personas");

        group.MapGet("/", async (KindergartenContext db,IMapper mapper) =>
        {
            var personas = await db.Personas
                .Include(p => p.Nino_Autorizados)
                    .ThenInclude(na => na.MatriculaNavigation)
                .ToListAsync();

            var dto = mapper.Map<List<PersonaReadDto>>(personas);
            return Results.Ok(dto);

        });

        group.MapGet("/{cedula}", async (string cedula, KindergartenContext db,IMapper mapper) =>
        {
            var persona = await db.Personas
                .Include(p => p.Nino_Autorizados)
                    .ThenInclude(na => na.MatriculaNavigation)
                .FirstOrDefaultAsync(p => p.Cedula == cedula);

            if (persona is null)
                    return Results.NotFound();

            var dto = mapper.Map<PersonaReadDto>(persona);
        
            return Results.Ok(dto);
        });

        group.MapPost("/", async (PersonaCreateDto dto, IValidator<PersonaCreateDto> validator, KindergartenContext db,IMapper mapper) =>
        {

            var validationResult  = await validator.ValidateAsync(dto);
            if (!validationResult .IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                return Results.ValidationProblem(errors);
            }

            var persona = mapper.Map<Persona>(dto);
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            var readDto = mapper.Map<PersonaReadDto>(persona);
            return Results.Created($"/api/personas/{persona.Cedula}", readDto);
        });

        group.MapPut("/{cedula}", async (string cedula, PersonaUpdateDto dto, IValidator<PersonaUpdateDto> validator, KindergartenContext db, IMapper mapper) =>
        {

            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                return Results.ValidationProblem(errors);
            }

            var persona = await db.Personas.FindAsync(cedula);
            if (persona is null)
                return Results.NotFound();

            mapper.Map(dto, persona);
            await db.SaveChangesAsync();
            return Results.NoContent();

        });

        group.MapDelete("/{cedula}", async (string cedula, KindergartenContext db) =>
        {
            var persona = await db.Personas.FindAsync(cedula);
            if (persona is null)
                return Results.NotFound();
            
            db.Personas.Remove(persona);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return app;

    }
}
}