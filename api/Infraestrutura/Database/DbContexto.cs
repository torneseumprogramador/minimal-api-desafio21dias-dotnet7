using Microsoft.EntityFrameworkCore;
using MinimalApiDesafio.Models;

namespace minimal_api_desafio.Infraestrutura.Database;

public class DbContexto : DbContext
{
    public DbContexto(DbContextOptions<DbContexto> options) : base(options) { }
    public DbSet<Cliente> Clientes { get; set; } = default!;
} 