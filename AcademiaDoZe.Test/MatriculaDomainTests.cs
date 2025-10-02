using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Test;

public class MatriculaDomainTests
{
    private Logradouro GetValidLogradouro()
    => Logradouro.Criar(0, "12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");

    private Arquivo GetValidArquivo() => Arquivo.Criar(new byte[1]);

    private Aluno GetValidAluno() => Aluno.Criar(0,
            "Amabile Vitória Lopes",                  
            "111.111.111-11",                
            DateOnly.FromDateTime(DateTime.Today.AddYears(-18)), 
            "(11) 99999-9999",               
            "test@gmail.com",                
            GetValidLogradouro(),          
            "123",                           
            "Casa",                          
            "Senha@1234",                     
            GetValidArquivo()               
        );

    [Fact]
    public void CriarMatricula_Valido_NaoDeveLancarExcecao()
    {
        var aluno = GetValidAluno();
        var plano = EMatriculaPlano.Semestral;
        var dataInicio = DateOnly.FromDateTime(DateTime.Today);
        var dataFim = dataInicio.AddMonths(6);
        var objetivo = "Ganhar Massa";
        var restricoes = EMatriculaRestricoes.Diabetes;
        var laudoMedico = GetValidArquivo();
        var observacoes = "tempo limitado";

        var matricula = Matricula.Criar(0, aluno, plano, dataInicio, dataFim, objetivo, restricoes, laudoMedico, observacoes);

        // Assert
        Assert.NotNull(matricula);
    }

    [Fact]
    public void CriarMatricula_ComObjetivoVazio_DeveLancarExcecao()
    {
        // Arrange
        var aluno = GetValidAluno();
        var plano = EMatriculaPlano.Semestral;
        var dataInicio = DateOnly.FromDateTime(DateTime.Today); // válido
        var dataFim = dataInicio.AddMonths(6);
        var objetivo = ""; // inválido
        var restricoes = EMatriculaRestricoes.Alergias;
        var laudoMedico = GetValidArquivo();
        var observacoes = "tempo limitado";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Matricula.Criar(0,aluno, plano, dataInicio, dataFim, objetivo, restricoes, laudoMedico, observacoes)
        );

        Assert.Equal("OBJETIVO_OBRIGATORIO", exception.Message);
    }
}