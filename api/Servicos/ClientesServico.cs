using minimal_api_desafio.Infraestrutura.Database;
using MinimalApiDesafio.Infraestrutura.Interfaces;
using Microsoft.EntityFrameworkCore;
using MinimalApiDesafio.Models;

namespace MinimalApiDesafio.Servicos;

public class ClientesServico : IBancoDeDadosServico<Cliente>
{
    public ClientesServico() {}

    public ClientesServico(DbContexto dbContexto)
    {
        this.dbContexto = dbContexto;
    }

    private DbContexto dbContexto = default!;

    public virtual async Task Salvar(Cliente cliente)
    {
        if(cliente.Id == 0)
            this.dbContexto.Clientes.Add(cliente);
        else
            this.dbContexto.Clientes.Update(cliente);
        
        var ret = this.dbContexto.SaveChanges();
        if(ret != 1) throw new Exception("Não foi possivel salvar o dado no banco");
        await Task.FromResult(ret);
    }

    public async Task Update(Cliente clientePara, object clienteDe)
    {
        if(clientePara.Id == 0)
            throw new Exception("Id de cliente é obrigatório");
        
        foreach(var propDe in clienteDe.GetType().GetProperties())
        {
            var propPara = clientePara.GetType().GetProperty(propDe.Name);
            if(propPara is not null)
            {
                propPara.SetValue(clientePara, propDe.GetValue(clienteDe));
            }
        }

        this.dbContexto.Clientes.Update(clientePara);
        await this.dbContexto.SaveChangesAsync();
    }

    public async Task ExcluirPorId(int id)
    {
        var cliente = await this.dbContexto.Clientes.Where(c => c.Id == id).FirstAsync();
        if(cliente is not null)
        {
            await Excluir(cliente);
        }
    }

    public async Task Excluir(Cliente cliente)
    {
        this.dbContexto.Clientes.Remove(cliente);
        await this.dbContexto.SaveChangesAsync();
    }

    public async Task<Cliente?> BuscaPorId(int id)
    {
        Cliente? cliente = await Task.FromResult(this.dbContexto.Clientes.Where(c => c.Id == id).FirstOrDefault());
        return cliente;
    }

    public async Task<List<Cliente>> Todos()
    {
        // return await this.dbContexto.Clientes.ToListAsync();
        return await Task.FromResult(this.dbContexto.Clientes.ToList());
    }
}