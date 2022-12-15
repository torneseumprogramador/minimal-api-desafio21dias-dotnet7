using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MinimalApiDesafio;
using minimal_api_desafio.Infraestrutura.Database;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using MinimalApiDesafio.Infraestrutura.Interfaces;
using MinimalApiDesafio.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Test.Helpers;

public class Setup
{
    public const string PORT = "5001";
    public static TestContext testContext = default!;
    public static WebApplicationFactory<Startup> http = default!;
    public static HttpClient client = default!;

    public virtual async Task ExecutaComandoSqlAsync(string sql)
    {
        await new DbContexto().Database.ExecuteSqlRawAsync(sql);
    }

    public virtual void ExecutaComandoSql(string sql)
    {
        new DbContexto().Database.ExecuteSqlRaw(sql);
    }

    public virtual async Task<int> ExecutaEntityCountAsync(int id, string nome)
    {
        return await new DbContexto().Clientes.Where(c => c.Id == id && c.Nome == nome).CountAsync();
    }

    public virtual int ExecutaEntityCount(int id, string nome)
    {
        return new DbContexto().Clientes.Where(c => c.Id == id && c.Nome == nome).Count();
    }
    
    public static async Task FakeCliente()
    {
        await new DbContexto().Database.ExecuteSqlRawAsync("""
        insert clientes(Nome, Telefone, Email, DataCriacao)
        values('Danilo', '(11)11111-1111', 'email@teste.com', '2022-12-15 06:09:00')
        """);
    }

    public static void ClassInit(TestContext testContext)
    {
        Setup.testContext = testContext;
        Setup.http = new WebApplicationFactory<Startup>();

        Setup.http = Setup.http.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IBancoDeDadosServico<Cliente>, MockClientesServico>();
                
                /*
                // Caso queira deixar o teste com conexão diferente
                var conexao = "Server=localhost;Database=desafio21dias_dotnet7_test;Uid=root;Pwd=root"
                services.AddDbContext<DbContexto>(options =>
                {
                    options.UseMySql(conexao, ServerVersion.AutoDetect(conexao));
                });
                */

            });
        });

        Setup.client = Setup.http.CreateClient();
    }

    public static void ClassCleanup()
    {
        Setup.http.Dispose();
    }
}

public class MockClientesServico : IBancoDeDadosServico<Cliente>
{
    public Task<Cliente?> BuscaPorId(int id)
    {
        Cliente? cli = null;
        if(id != 1) return Task.FromResult(cli);

        cli = new Cliente(){ Id = 1, Nome = "Sheila", Email = "she@gmail.com", Telefone = "(00)00000-0001" };
        return Task.FromResult(cli ?? null);
    }

    public Task Excluir(Cliente objeto)
    {
        return Task.FromResult(() => {});
    }

    public Task ExcluirPorId(int id)
    {
        return Task.FromResult(() => {});
    }

    public Task Salvar(Cliente objeto)
    {
        objeto.Id = 1;
        return Task.FromResult(() => {});
    }

    public Task<List<Cliente>> Todos()
    {
        var list = new List<Cliente>();
        list.Add(new Cliente(){ Id = 1, Nome = "Lana", Email = "lana@gmail.com", Telefone = "(00)00000-0000"});
        list.Add(new Cliente(){ Id = 2, Nome = "Sheila", Email = "she@gmail.com", Telefone = "(00)00000-0001" });
        return Task.FromResult(list);
    }
}