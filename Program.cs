var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

MapRoutes(app);

app.Run();


#region Rotas utilizando Minimal API

void MapRoutes(WebApplication app)
{
    app.MapGet("/", () => new {Mensagem = "Bem vindo a API"});
    app.MapGet("/recebe-parametro", (HttpRequest request, HttpResponse response, string? nome) => 
    {
        response.StatusCode = 201;

        nome = $""" 
        Alterando parametro recebido {nome}
        """;

        var objetoDeRetono = new {
            ParametroPassado = nome,
            Mensagem = "Muito bem alunos passamos um parametro por querystring"
        };

        return objetoDeRetono;
    });
}


#endregion
