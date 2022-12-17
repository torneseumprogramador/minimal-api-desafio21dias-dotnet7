using Microsoft.EntityFrameworkCore;
using MinimalApiDesafio;
using MinimalApiDesafio.Models;

namespace minimal_api_desafio.Infraestrutura.Database;

public class DbContexto : DbContext
{
    public DbContexto() {}
    public DbContexto(DbContextOptions<DbContexto> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conexao = Startup.StrConexao();
        optionsBuilder.UseMySql(conexao, ServerVersion.AutoDetect(conexao));
    }

    public virtual DbSet<Administrador> Administradores { get; set; } = default!;
    public virtual DbSet<Cliente> Clientes { get; set; } = default!;
} 