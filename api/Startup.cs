using Microsoft.AspNetCore.Mvc;
using MinimalApiDesafio.ModelViews;
using MinimalApiDesafio.DTOs;
using MinimalApiDesafio.Models;
using Microsoft.OpenApi.Models;
using minimal_api_desafio.Infraestrutura.Database;
using Microsoft.EntityFrameworkCore;
using MinimalApiDesafio.Infraestrutura.Interfaces;
using MinimalApiDesafio.Servicos;

namespace MinimalApiDesafio;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration? Configuration { get;set; }
    public static string? StrConexao(IConfiguration? Configuration = null)
    {
        // === Dá prioridade ao app settings ===
        // if(Configuration is not null)
        // {
        //     return Configuration?.GetConnectionString("Conexao");
        // }

        // return Environment.GetEnvironmentVariable("DATABASE_URL_MINIMAL_API");

        // === Dá prioridade a variavel de ambiente ===
        string? conexao = Environment.GetEnvironmentVariable("DATABASE_URL_MINIMAL_API");
        if(conexao is null)
        {
            conexao = Configuration?.GetConnectionString("Conexao");
        }

        return conexao;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minimal API Desafio", Version = "v1" });
        });

        services.AddEndpointsApiExplorer();

        var conexao = StrConexao(Configuration);
        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(conexao, ServerVersion.AutoDetect(conexao));
        });

        services.AddScoped<IBancoDeDadosServico<Cliente>, ClientesServico>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API Desafio v1"));
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            MapRoutes(endpoints);
            MapRoutesClientes(endpoints);
        });
    }

    
    #region Rotas utilizando Minimal API

    public void MapRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => new {Mensagem = "Bem vindo a API"})
            .Produces<dynamic>(StatusCodes.Status200OK)
            .WithName("Home")
            .WithTags("Testes");

        app.MapGet("/recebe-parametro", (string? nome) => 
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


    public void MapRoutesClientes(IEndpointRouteBuilder app)
    {
        app.MapGet("/clientes", async ([FromServices] IBancoDeDadosServico<Cliente> clientesServico) => 
        {
            var clientes = await clientesServico.Todos();
            return Results.Ok(clientes);
        })
        .Produces<List<Cliente>>(StatusCodes.Status200OK)
        .WithName("GetClientes")
        .WithTags("Clientes");

        app.MapPost("/clientes", async ([FromServices] IBancoDeDadosServico<Cliente> clientesServico, [FromBody] ClienteDTO clienteDTO) => 
        {
            var cliente = new Cliente
            {
                Nome = clienteDTO.Nome,
                Telefone = clienteDTO.Telefone,
                Email = clienteDTO.Email,
            };
            await clientesServico.Salvar(cliente);

            return Results.Created($"/cliente/{cliente.Id}", cliente);
        })
        .Produces<Cliente>(StatusCodes.Status201Created)
        .Produces<Error>(StatusCodes.Status400BadRequest)
        .WithName("PostClientes")
        .WithTags("Clientes");

        app.MapPut("/clientes/{id}", async ([FromServices] IBancoDeDadosServico<Cliente> clientesServico, [FromRoute] int id, [FromBody] ClienteDTO clienteDTO) => 
        {
            if (string.IsNullOrEmpty(clienteDTO.Nome))
            {
                return Results.BadRequest(new Error
                {
                    Codigo = 123432,
                    Mensagem = $"O Nome é obrigatório"
                });
            }

            var clienteDb = await clientesServico.BuscaPorId(id);
            if(clienteDb is null)
            {
                return Results.NotFound(new Error 
                { 
                    Codigo = 423, 
                    Mensagem = $"Cliente não encontrado com o id {id}" 
                });
            }

            await clientesServico.Update(clienteDb, clienteDTO);

            return Results.Ok(clienteDb);
        })
        .Produces<Cliente>(StatusCodes.Status200OK)
        .Produces<Error>(StatusCodes.Status404NotFound)
        .Produces<Error>(StatusCodes.Status400BadRequest)
        .WithName("PutClientes")
        .WithTags("Clientes");

        app.MapPatch("/clientes/{id}", async ([FromServices] IBancoDeDadosServico<Cliente> clientesServico, [FromRoute] int id, [FromBody] ClienteNomeDTO clienteNomeDTO) => 
        {
            var clienteDb = await clientesServico.BuscaPorId(id);
            if(clienteDb is null)
            {
                return Results.NotFound(new Error 
                { 
                    Codigo = 2345, 
                    Mensagem = $"Cliente não encontrado com o id {id}" 
                });
            }

            clienteDb.Nome = clienteNomeDTO.Nome;
            await clientesServico.Salvar(clienteDb);

            return Results.Ok(clienteDb);
        })
        .Produces<Cliente>(StatusCodes.Status200OK)
        .Produces<Error>(StatusCodes.Status404NotFound)
        .Produces<Error>(StatusCodes.Status400BadRequest)
        .WithName("PatchClientes")
        .WithTags("Clientes");

        app.MapDelete("/clientes/{id}", async ([FromServices] IBancoDeDadosServico<Cliente> clientesServico, [FromRoute] int id) => 
        {
            var clienteDb = await clientesServico.BuscaPorId(id);
            if(clienteDb is null)
            {
                return Results.NotFound(new Error 
                { 
                    Codigo = 22345, 
                    Mensagem = $"Cliente não encontrado com o id {id}" 
                });
            }

            await clientesServico.Excluir(clienteDb);

            return Results.NoContent();
        })
        .Produces<Cliente>(StatusCodes.Status204NoContent)
        .Produces<Error>(StatusCodes.Status404NotFound)
        .WithName("DeleteClientes")
        .WithTags("Clientes");


        app.MapGet("/clientes/{id}", async ([FromServices] IBancoDeDadosServico<Cliente> clientesServico, [FromRoute] int id) => 
        {
            var clienteDb = await clientesServico.BuscaPorId(id);
            if(clienteDb is null)
            {
                return Results.NotFound(new Error 
                { 
                    Codigo = 21345, 
                    Mensagem = $"Cliente não encontrado com o id {id}" 
                });
            }

            return Results.Ok(clienteDb);
        })
        .Produces<Cliente>(StatusCodes.Status204NoContent)
        .Produces<Error>(StatusCodes.Status404NotFound)
        .WithName("GetClientesPorId")
        .WithTags("Clientes");
    }

    #endregion
}