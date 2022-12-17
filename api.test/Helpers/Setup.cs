using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MinimalApiDesafio;
using minimal_api_desafio.Infraestrutura.Database;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiDesafio.Infraestrutura.Interfaces;
using MinimalApiDesafio.Models;
using Api.Test.Mock;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using MinimalApiDesafio.Servicos;
using MinimalApiDesafio.Routes;
using Castle.Core.Configuration;
using Microsoft.Extensions.Hosting;

namespace Api.Test.Helpers;

public class Setup
{
    public const string PORT = "5001";
    public static TestContext testContext = default!;
    public static IHost http = default!;
    public static HttpClient client = default!;

    public static async Task ExecutaComandoSqlAsync(string sql)
    {
        await new DbContexto().Database.ExecuteSqlRawAsync(sql);
    }

    public static async Task<int> ExecutaEntityCountAsync(int id, string nome)
    {
        return await new DbContexto().Clientes.Where(c => c.Id == id && c.Nome == nome).CountAsync();
    }
    
    public static async Task FakeClienteAsync()
    {
        await new DbContexto().Database.ExecuteSqlRawAsync("""
        insert clientes(Nome, Telefone, Email, DataCriacao)
        values('Danilo', '(11)11111-1111', 'email@teste.com', '2022-12-15 06:09:00')
        """);
    }

    public static void ClassInit(TestContext testContext)
    {
        Setup.testContext = testContext;

        Setup.http = Host.CreateDefaultBuilder(null).ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");
            webBuilder.UseStartup<StartupFake>();
        }).Build();
        
        Setup.http.RunAsync();

        Setup.client = new HttpClient();
    }

    public static void ClassCleanup()
    {
        Setup.http.Dispose();
    }
}


public class StartupFake
{

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddDbContext<DbContexto>(options =>
        {
            string conexao = "Server=localhost;Database=desafio21dias_dotnet7_test;Uid=root;Pwd=root";
            options.UseMySql(conexao, ServerVersion.AutoDetect(conexao));
        });

        services.AddScoped<IBancoDeDadosServico<Cliente>, ClientesServico>();
        services.AddScoped<ILogin<Administrador>, AdministradoresServico>();

        TokenServico.Secret = "1763823b32b32323027332g4k23n23kjew2323h23n232332";
        var key = Encoding.ASCII.GetBytes(TokenServico.Secret);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("administrador", policy => policy.RequireClaim("administrador"));
            options.AddPolicy("editor", policy => policy.RequireClaim("editor"));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            HomeRoute.MapRoutes(endpoints);
            AdministradoresRoute.MapRoutes(endpoints);
            ClientesRoute.MapRoutes(endpoints);
        });
    }
}