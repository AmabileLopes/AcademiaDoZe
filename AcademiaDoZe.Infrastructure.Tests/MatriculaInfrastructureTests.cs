using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Infrastructure.Tests;
using System.Linq;

namespace AcademiaDoZe.Infrastructure.Tests;

public class MatriculaInfrastructureTests : TestBase
{ //Amabile Vitória Lopes Ouriques

    [Fact]
    public async Task Matricula_Adicionar()

    {// com base em logradouroID, acessar logradourorepository e obter o logradouro

       var alunoRepo = new AlunoRepository(ConnectionString, DatabaseType);
			var aluno = await alunoRepo.ObterPorCpf("12345678901");
			Assert.NotNull(aluno);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

      

        var matricula = Matricula.Criar(0,
            aluno,
            EMatriculaPlano.Semestral,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            "Emagrecer",
            EMatriculaRestricoes.Alergias,
            arquivo,
            "Sem observações"
        );

        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var matriculaInserida = await repoMatricula.Adicionar(matricula);

        Assert.NotNull(matriculaInserida);
        Assert.True(matriculaInserida.Id > 0);
    }

    [Fact]
    public async Task Matricula_ObterPorAluno_Atualizar()
    {
        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);

        var aluno = await repoAluno.ObterPorCpf("12345678901");
        Assert.NotNull(aluno);

        var repoMatricula2 = new MatriculaRepository(ConnectionString, DatabaseType);
        
        var matriculas = (await repoMatricula2.ObterPorAluno(aluno!.Id)).ToList();
        Assert.NotEmpty(matriculas);

        var matricula = matriculas.First(); // pega a primeira matrícula

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });
        var matriculaAtualizada = Matricula.Criar(0,
            aluno,
            EMatriculaPlano.Anual,
            new DateOnly(2020, 05, 20),
            new DateOnly(2020, 05, 20).AddMonths(12),
            "Hipertrofia",
            EMatriculaRestricoes.Alergias,
            arquivo,
            "Observação atualizada"
        );

        typeof(Entity).GetProperty("Id")?.SetValue(matriculaAtualizada, matricula.Id);

        var repoMatricula3 = new MatriculaRepository(ConnectionString, DatabaseType);
        var resultado = await repoMatricula3.Atualizar(matriculaAtualizada);
        Assert.NotNull(resultado);
        Assert.Equal("Hipertrofia", resultado.Objetivo);
    }

    [Fact]
    public async Task Matricula_ObterPorAluno_Remover_ObterPorId()
    {
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);

        var aluno = await repoAluno.ObterPorCpf("12345678901");
        Assert.NotNull(aluno);


        var matricula = (await repoMatricula.ObterPorAluno(aluno!.Id)).FirstOrDefault();
        Assert.NotNull (matricula);

        // Remover
        var repoMatricula2 = new MatriculaRepository(ConnectionString, DatabaseType);
        var resultadoRemocao = await repoMatricula2.Remover(matricula.Id);

        Assert.True(resultadoRemocao);

        // Verificar se foi removida
        var repoMatricula3 = new MatriculaRepository(ConnectionString, DatabaseType);
        var matriculaRemovida = await repoMatricula3.ObterPorId(matricula.Id);
        Assert.Null(matriculaRemovida);
    }

    [Fact]
    public async Task Matricula_ObterTodos()
    {
        // ObterTodos

        var repoMatriculaTodos = new MatriculaRepository(ConnectionString, DatabaseType);

        var resultado = await repoMatriculaTodos.ObterTodos();
        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task Matricula_ObterAtivas()
    {
        var repo = new MatriculaRepository(ConnectionString, DatabaseType);
        var ativas = await repo.ObterAtivas();
        Assert.NotNull(ativas);
        Assert.All(ativas, m => Assert.True(m.DataFim >= DateOnly.FromDateTime(DateTime.Today)));
    }

    [Fact]
    public async Task Matricula_ObterPorId()
    {
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);

        var aluno = await repoAluno.ObterPorCpf("12345678901");
        Assert.NotNull(aluno);


        var repoMatricula2 = new MatriculaRepository(ConnectionString, DatabaseType);

        var matriculas = (await repoMatricula2.ObterPorAluno(aluno!.Id)).ToList();
        Assert.NotEmpty(matriculas);

        var matricula = matriculas.First(); // pega a primeira matrícula


        var repoMatricula3 = new MatriculaRepository(ConnectionString, DatabaseType);

        var matriculaPorId = await repoMatricula3.ObterPorId(matricula.Id);
        Assert.NotNull(matriculaPorId);
        Assert.Equal(matricula.Id, matriculaPorId.Id);
    }
}