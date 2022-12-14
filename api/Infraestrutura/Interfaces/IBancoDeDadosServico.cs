namespace MinimalApiDesafio.Infraestrutura.Interfaces;

public interface IBancoDeDadosServico<T>
{
    Task Salvar(T objeto);
    Task Update(T objetoDe, object paraPara);
    Task Excluir(T objeto);
    Task ExcluirPorId(int id);
    Task<T?> BuscaPorId(int id);
    Task<List<T>> Todos();
}