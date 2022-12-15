using Microsoft.EntityFrameworkCore;
using MinimalApiDesafio;
using MinimalApiDesafio.Models;

namespace minimal_api_desafio.Infraestrutura.Database;

public class DbContextoTeste : DbContext
{
    public DbContextoTeste() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conexao = Startup.StrConexao();
        optionsBuilder.UseMySql(conexao, ServerVersion.AutoDetect(conexao));
    }

    public DbSet<Cliente> Clientes { get; set; } = default!;
} 