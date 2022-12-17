using Microsoft.AspNetCore.Mvc;
using MinimalApiDesafio.ModelViews;
using MinimalApiDesafio.DTOs;
using MinimalApiDesafio.Models;
using Microsoft.OpenApi.Models;
using minimal_api_desafio.Infraestrutura.Database;
using Microsoft.EntityFrameworkCore;
using MinimalApiDesafio.Infraestrutura.Interfaces;
using MinimalApiDesafio.Servicos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using MinimalApiDesafio.Routes;

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
        services.AddEndpointsApiExplorer();

        var conexao = StrConexao(Configuration);
        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(conexao, ServerVersion.AutoDetect(conexao));
        });

        services.AddScoped<IBancoDeDadosServico<Cliente>, ClientesServico>();
        services.AddScoped<ILogin<Administrador>, AdministradoresServico>();

        TokenServico.Secret = Configuration?["SecretJwt"] ?? "";
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

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Minimal API",
                Description = "Torne-se um programador API Minima",
                Contact = new OpenApiContact { Name = "Danilo Aparecido", Email = "suporte@torneseumprogramador.com.br" },
                License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Insira o token JWT como no exemplo: Bearer {SEU_TOKEN}",
                Name = "Authorization",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
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