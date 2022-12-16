

using Api.Test.Helpers;
using minimal_api_desafio.Infraestrutura.Database;
using MinimalApiDesafio.Models;
using MinimalApiDesafio.Servicos;
using Moq;

namespace Api.Test.Servicos;


[TestClass]
public class ClientesServicoMockTest
{
    [TestMethod]
    public async Task TestaSalvarDadoNobanco()
    {
        var mockClientesServico = new Mock<ClientesServico>();

        var cliente = new Cliente()
        {
            Nome = "Usuario Teste",
            Telefone = "111111",
            Email = "Usuario@Teste.com",
        };

        var clienteMock = cliente;
        clienteMock.Id = 1;

        mockClientesServico.Setup(s => s.Salvar(cliente)).Returns(Task.FromResult(clienteMock));
        await mockClientesServico.Object.Salvar(cliente);
        
        Assert.AreEqual(1, cliente.Id);
    }
}