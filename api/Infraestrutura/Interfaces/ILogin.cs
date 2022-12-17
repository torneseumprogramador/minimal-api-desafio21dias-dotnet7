namespace MinimalApiDesafio.Infraestrutura.Interfaces;

public interface ILogin<T> : IBancoDeDadosServico<T> 
{
    Task<T?> LoginAsync(string email, string senha);
}