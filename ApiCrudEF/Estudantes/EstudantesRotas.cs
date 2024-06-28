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
            estudantesRotas.MapGet("", async (AppDBContext context, CancellationToken ct) =>
            {
                var estudantes = await context.estudantes
                .Where(estudante => estudante.Ativo)
                .Select(estudante => new EstudanteDto(estudante.Id, estudante.Nome))
                .ToListAsync(ct);
                return Results.Ok(estudantes);
            });

            estudantesRotas.MapPost("", async (AddEstudanteRequest request, AppDBContext context, CancellationToken ct) =>
            {
                var jaExiste = await context.estudantes.AnyAsync(i => i.Nome == request.Nome, ct);
                if (jaExiste)
                {
                    return Results.Conflict("Ja existe");
                }
                var novoEstudante = new Estudante(request.Nome);
                await context.AddAsync(novoEstudante, ct);
                await context.SaveChangesAsync(ct);
                var estudanteRetorno = new EstudanteDto(novoEstudante.Id, novoEstudante.Nome);
                return Results.Ok(estudanteRetorno);

            });
            estudantesRotas.MapPatch("{id}", async (Guid id, UpdateEstudanteResquest resquest, AppDBContext context, CancellationToken ct) =>
            {
                var estudanteEncontrado = await context.estudantes.SingleOrDefaultAsync(estudante => estudante.Id == id, ct);
                if (estudanteEncontrado == null)
                {
                    return Results.NotFound();
                }
                estudanteEncontrado.atualizarNome(resquest.Nome);
                await context.SaveChangesAsync(ct);
                return Results.Ok(new EstudanteDto(estudanteEncontrado.Id, estudanteEncontrado.Nome));
            });

            estudantesRotas.MapDelete("{id}", async (Guid id, AppDBContext context, CancellationToken ct) =>
            {
                var estudanteEncontrado = await context.estudantes.SingleOrDefaultAsync(estudante => estudante.Id == id, ct);
                if (estudanteEncontrado == null)
                {
                    return Results.NotFound();
                }
                estudanteEncontrado.desativar();
                await context.SaveChangesAsync(ct);
                return Results.Ok();
            });
        }

    }
}