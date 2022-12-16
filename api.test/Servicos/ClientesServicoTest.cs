

using Api.Test.Helpers;
using minimal_api_desafio.Infraestrutura.Database;
using MinimalApiDesafio.Models;
using MinimalApiDesafio.Servicos;

namespace Api.Test.Servicos;


[TestClass]
public class ClientesServicoTest
{
    [ClassInitialize]
    public static async Task ClassInit(TestContext testContext)
    {
        await Setup.ExecutaComandoSqlAsync("truncate table clientes");
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        await Setup.ExecutaComandoSqlAsync("truncate table clientes");
    }

    [TestMethod]
    public async Task TestaSalvarDadoNobanco()
    {
        var clientesServico = new ClientesServico(new DbContexto());

        var cliente = new Cliente()
        {
            Nome = "Usuario Teste",
            Telefone = "111111",
            Email = "Usuario@Teste.com",
        };

        await clientesServico.Salvar(cliente);
        
        Assert.AreEqual(1, cliente.Id);
    }
}