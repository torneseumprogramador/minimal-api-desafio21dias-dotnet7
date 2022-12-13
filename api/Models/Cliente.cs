namespace MinimalApiDesafio.Models;

public record Cliente
{
    public Cliente()
    {
        this.DataCriacao = DateTime.Now;
    }
    
    public int Id { get;set; }
    public string? Nome { get;set; }
    public string? Telefone { get;set; }
    public string? Email { get;set; }
    public DateTime DataCriacao { get;set; }
}