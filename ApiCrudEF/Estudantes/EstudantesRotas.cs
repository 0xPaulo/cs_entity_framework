namespace ApiCrudEF.Estudantes
{
    public static class EstudantesRotas
    {
        public static void AddRotasEstudantes(this WebApplication app)
        {
            app.MapGet("hello-world", () => "hello world");
        }

    }
}