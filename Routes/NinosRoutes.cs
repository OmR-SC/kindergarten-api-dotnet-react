using KindergartenAPI.Data;
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
            group.MapGet("/", async (KindergartenContext db) =>
            {

                var ninos = await db.Ninos
                   .Include(n => n.CedulaPagadorNavigation)
                   .Include(a => a.Ingredientes)
                   .Include(n => n.Asistencia)
                   .ToListAsync();
                    
                    return Results.Ok(ninos);
            });
            // GET /ninos/{id}
            group.MapGet("/{matricula:int}", async (int matricula, KindergartenContext db) =>
            {

                // await db.Ninos.FindAsync(matricula) is Nino nino
                //     ? Results.Ok(nino)
                //     : Results.NotFound();

                var nino = await db.Ninos
                    .Include(n => n.CedulaPagadorNavigation)
                    .Include(n => n.Ingredientes)
                    .Include(n => n.Asistencia)
                    .FirstOrDefaultAsync(n => n.Matricula == matricula);

               return nino is not null ?
                    Results.Ok(nino) :
                    Results.NotFound();
                    

                // return await db.Ninos
                //     .Include(n => n.Matricula)
                //     .Include(n => n.Alergias)
                //         .ThenInclude(a => a.Ingrediente)
                //     .Include(n => n.Asistencias)
                //     .FirstOrDefaultAsync(n => n.Matricula == matricula);
            });

            group.MapPost("/", async (Nino nino, KindergartenContext db) =>
            {
                db.Ninos.Add(nino);
                return await db.SaveChangesAsync() > 0
                    ? Results.Created($"/ninos/{nino.Matricula}", nino)
                    : Results.BadRequest("Error al crear el niño.");
            });

            group.MapPut("/{matricula:int}", async (int matricula, Nino input, KindergartenContext db) =>
            {
                if (matricula != input.Matricula)
                {
                    return Results.BadRequest("La matrícula no coincide.");
                }

                var existingNino = await db.Ninos.FindAsync(matricula);
                if (existingNino == null)
                {
                    return Results.NotFound();
                }

                existingNino.Nombre = input.Nombre;
                existingNino.FechaNacimiento = input.FechaNacimiento;
                existingNino.FechaIngreso = input.FechaIngreso;
                existingNino.FechaBaja = input.FechaBaja;
                existingNino.CedulaPagador = input.CedulaPagador;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            group.MapDelete("/{matricula:int}", async (int matricula, KindergartenContext db) =>
            {
                var nino = await db.Ninos.FindAsync(matricula);
                if (nino == null)
                {
                    return Results.NotFound();
                }

                db.Ninos.Remove(nino);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            return app;
        }
    }

}

    //     public static void MapNinosAutorizadosRoutes(this IEndpointRouteBuilder app)
    //     {
    //         app.MapGet("/ninos/{matricula}/autorizados", async (int matricula, KindergartenContext db) =>
    //         {
    //             return await db.NinoAutorizados
    //                 .Where(na => na.Matricula == matricula)
    //                 .Include(na => na.Persona)
    //                 .ToListAsync();
    //         });
    //     }
    // }