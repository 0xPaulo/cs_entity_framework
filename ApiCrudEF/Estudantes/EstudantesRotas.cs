using ApiCrudEF.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiCrudEF.Estudantes
{
    public static class EstudantesRotas
    {
        // this é como um extends
        public static void AddRotasEstudantes(this WebApplication app)

        {
            var estudantesRotas = app.MapGroup("estudantes");
            estudantesRotas.MapGet("", async (AppDBContext context) =>
            {
                var estudantes = await context.estudantes
                .Where(estudante => estudante.Ativo)
                .Select(estudante => new EstudanteDto(estudante.Id, estudante.Nome))
                .ToListAsync();
                return Results.Ok(estudantes);
            });

            estudantesRotas.MapPost("", async (AddEstudanteRequest request, AppDBContext context) =>
            {
                var jaExiste = await context.estudantes.AnyAsync(i => i.Nome == request.Nome);
                if (jaExiste)
                {
                    return Results.Conflict("Ja existe");
                }
                var novoEstudante = new Estudante(request.Nome);
                await context.AddAsync(novoEstudante);
                await context.SaveChangesAsync();
                var estudanteRetorno = new EstudanteDto(novoEstudante.Id, novoEstudante.Nome);
                return Results.Ok(estudanteRetorno);

            });
            estudantesRotas.MapPatch("{id}", async (Guid id, UpdateEstudanteResquest resquest, AppDBContext context) =>
            {
                var estudanteEncontrado = await context.estudantes.SingleOrDefaultAsync(estudante => estudante.Id == id);
                if (estudanteEncontrado == null)
                {
                    return Results.NotFound();
                }
                estudanteEncontrado.atualizarNome(resquest.Nome);
                await context.SaveChangesAsync();
                return Results.Ok(new EstudanteDto(estudanteEncontrado.Id, estudanteEncontrado.Nome));
            });

            estudantesRotas.MapDelete("{id}", async (Guid id, AppDBContext context) =>
            {
                var estudanteEncontrado = await context.estudantes.SingleOrDefaultAsync(estudante => estudante.Id == id);
                if (estudanteEncontrado == null)
                {
                    return Results.NotFound();
                }
                estudanteEncontrado.desativar();
                await context.SaveChangesAsync();
                return Results.Ok();
            });
        }

    }
}