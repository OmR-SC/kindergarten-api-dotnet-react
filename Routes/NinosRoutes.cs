using AutoMapper;
using KindergartenAPI.Data;
using KindergartenAPI.DTOs.Ninos;
using KindergartenAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KindergartenAPI.Routes
{
    public static class NinosRoutes
    {
        public static WebApplication MapNinosRoutes(this WebApplication app)
        {

            var group = app.MapGroup("/api/v1/ninos");
            group.WithTags("Ninos");

            // GET /ninos
            group.MapGet("/", async (KindergartenContext db,IMapper mapper) =>
            {

                var ninos = await db.Ninos
                   .Include(n => n.CedulaPagadorNavigation)
                   .Include(a => a.Ingredientes)
                   .Include(n => n.Asistencia)
                   .ToListAsync();
                    
                var dtos = mapper.Map<List<NinoReadDto>>(ninos);
                return Results.Ok(dtos);
            });
            // GET /ninos/{id}
            group.MapGet("/{matricula:int}", async (int matricula, KindergartenContext db,IMapper mapper) =>
            {

                var nino = await db.Ninos
                    .Include(n => n.CedulaPagadorNavigation)
                    .Include(n => n.Ingredientes)
                    .Include(n => n.Asistencia)
                    .FirstOrDefaultAsync(n => n.Matricula == matricula);

                if (nino is null)
                    return Results.NotFound();

                return nino is not null
                    ? Results.Ok(mapper.Map<NinoReadDto>(nino))
                    : Results.NotFound();
                    
            });

            group.MapPost("/", async (NinoCreateDto dto, KindergartenContext db,IMapper mapper) =>
            {
                var nino = mapper.Map<Nino>(dto);
                db.Ninos.Add(nino);

                await db.SaveChangesAsync();

                var readDto = mapper.Map<NinoReadDto>(nino);
                return Results.Created($"/api/v1/ninos/{nino.Matricula}", readDto);
            
            });

            group.MapPut("/{matricula:int}", async (int matricula, NinoUpdateDto dto, KindergartenContext db, IMapper mapper) =>
            {

                var existingNino = await db.Ninos.FindAsync(matricula);
                if (existingNino is null)
                    return Results.NotFound();
                
                mapper.Map(dto, existingNino);                
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            group.MapDelete("/{matricula:int}", async (int matricula, KindergartenContext db) =>
            {
                var nino = await db.Ninos.FindAsync(matricula);
                if (nino is null)
                    return Results.NotFound();

                db.Ninos.Remove(nino);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            return app;
        }
    }

}