using Microsoft.AspNetCore.Authorization;

namespace MinimalApiDesafio.Routes;

internal struct HomeRoute
{
    internal static void MapRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", [AllowAnonymous] () => new {Mensagem = "Bem vindo a API"})
            .Produces<dynamic>(StatusCodes.Status200OK)
            .WithName("Home")
            .WithTags("Testes");

        app.MapGet("/recebe-parametro", [AllowAnonymous] (string? nome) => 
        {
            if(string.IsNullOrEmpty(nome))
            {
                return Results.BadRequest(new {
                    Mesagem = "Olha você não mandou uma informação importante, o nome é obrigatório"
                });
            }

            nome = $""" 
            Alterando parametro recebido {nome}
            """;

            var objetoDeRetono = new {
                ParametroPassado = nome,
                Mensagem = "Muito bem alunos passamos um parametro por querystring"
            };

            return Results.Created($"/recebe-parametro/{objetoDeRetono.ParametroPassado}", objetoDeRetono);
        })
        .Produces<dynamic>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithName("TesteRebeParametro")
        .WithTags("Testes");
    }
}