
namespace MinimalApiDesafio.DTOs;

public record AdministradorLogadoDTO
{
    public required string Email { get;set; }
    public required string Permissao { get;set; }
    public required string Token { get;set; }
    
}