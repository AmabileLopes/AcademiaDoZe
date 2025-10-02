using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;
//Amabile Vitória Lopes Ouriques
public class AlunoInfrastructureTests : TestBase
{
    [Fact]
    public async Task Aluno_LogradouroPorId_CpfJaExiste_Adicionar()
    {
        var logradouroId = 5;
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        Logradouro? logradouro = await repoLogradouro.ObterPorId(logradouroId);

        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });
        var _cpf = "12346678959";

        var repoAlunoCpf = new AlunoRepository(ConnectionString, DatabaseType);
        var cpfExistente = await repoAlunoCpf.CpfJaExiste(_cpf);
        Assert.False(cpfExistente, "CPF já existe no banco de dados.");

        var aluno = Aluno.Criar(1,
            "Aluno Teste",
            _cpf,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "complemento casa",
            "Senha@123",
            arquivo
        );

        var repoAlunoAdicionar = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoInserido = await repoAlunoAdicionar.Adicionar(aluno);
        Assert.NotNull(alunoInserido);
        Assert.True(alunoInserido.Id > 0);
    }

    [Fact]
    public async Task Aluno_ObterPorCpf_Atualizar()
    {
        var _cpf = "12346678955";
        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        var repoAlunoObterPorCpf = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoExistente = await repoAlunoObterPorCpf.ObterPorCpf(_cpf);
        Assert.NotNull(alunoExistente);

        // criar novo colaborador com os mesmos dados, editando o que quiser
        var alunoAtualizado = Aluno.Criar(0,
            "Aluno Atualizado",
            alunoExistente.Cpf,
            alunoExistente.DataNascimento,
            alunoExistente.Telefone,
            alunoExistente.Email,
            alunoExistente.Endereco,
            alunoExistente.Numero,
            alunoExistente.Complemento,
            alunoExistente.Senha,
            arquivo
        );
        // Usar reflexão para definir o ID

        var idProperty = typeof(Entity).GetProperty("Id");
        idProperty?.SetValue(alunoAtualizado, alunoExistente.Id);
       
        // Teste de Atualização
        var repoAlunoAtualizar = new AlunoRepository(ConnectionString, DatabaseType);
        var resultadoAtualizacao = await repoAlunoAtualizar.Atualizar(alunoAtualizado);

        Assert.NotNull(resultadoAtualizacao);
        Assert.Equal("Aluno Atualizado", resultadoAtualizacao.Nome);
    }

    [Fact]
    public async Task Aluno_ObterPorCpf_TrocarSenha()
    {
        var _cpf = "12346678951";
        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });
        var repoAlunoObterPorCpf = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoExistente = await repoAlunoObterPorCpf.ObterPorCpf(_cpf);
        Assert.NotNull(alunoExistente);
        // Trocar Senha 
        
        var novaSenha = "NovaSenha123";
        var repoAlunoTrocarSenha = new AlunoRepository(ConnectionString, DatabaseType);
        var resultadoTroca = await repoAlunoTrocarSenha.TrocarSenha(alunoExistente.Id, novaSenha);
        Assert.True(resultadoTroca);

        var repoAlunoObterPorId = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoAtualizado = await repoAlunoObterPorId.ObterPorId(alunoExistente.Id);
        Assert.NotNull(alunoAtualizado);
        Assert.Equal(novaSenha, alunoAtualizado.Senha);
    }

    [Fact]
    public async Task Aluno_Remover_ObterPorId()
    {
        var _cpf = "12346678955";
        var repoAlunoObterPorCpf = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoExistente = await repoAlunoObterPorCpf.ObterPorCpf(_cpf);
        Assert.NotNull(alunoExistente);

        var RepoAlunoRemover = new AlunoRepository(ConnectionString, DatabaseType);
        var resultadoRemover = await RepoAlunoRemover.Remover(alunoExistente.Id);
        Assert.True(resultadoRemover);

        var RepoAlunoObterPorId = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoRemovido = await RepoAlunoObterPorId.ObterPorId(alunoExistente.Id);
        Assert.Null(alunoRemovido);
    }

    [Fact]
    public async Task Aluno_ObterTodos()
    {
        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var resultado = await repoAluno.ObterTodos();
        Assert.NotNull(resultado);
    }
}