using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Test
{
    public class AlunoDomainTests
    {
        // Padrão AAA (Arrange, Act, Assert)
        // Arrange (Organizar): Preparamos tudo que o teste precisa.
        private Logradouro GetValidLogradouro() => Logradouro.Criar(0,"12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        private Arquivo GetValidArquivo() => Arquivo.Criar(new byte[1]);
        [Fact] // [Fact] é um atributo que marca este método como um teste para o xUnit.
        public void CriarAluno_ComDadosValidos_DeveCriarObjeto() // Padrão de Nomenclatura: MetodoTestado_Cenario_ResultadoEsperado
        {
            // Arrange
            var nome = "João da Silva"; var cpf = "12345678901"; var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)); var telefone = "11999999999";
            var email = "joao@email.com"; var logradouro = GetValidLogradouro(); var numero = "123"; var complemento = "Apto 1"; var senha = "Senha@1"; var foto = GetValidArquivo();
            // Act
            var aluno = Aluno.Criar(0,nome, cpf, dataNascimento, telefone, email, logradouro, numero, complemento, senha, foto);
            // Assert
            Assert.NotNull(aluno);
        }
        [Fact]
        public void CriarAluno_CepInvalido_DeveLancarExcecao()
        {
            // Arrange
            var nome = "João da Silva"; var cpf = "123456"; var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)); var telefone = "11999999999";
            var email = "joao@email.com"; var logradouro = GetValidLogradouro(); var numero = "123"; var complemento = "Apto 1"; var senha = "Senha@123"; var foto = GetValidArquivo();
            // Act & Assert
            var ex = Assert.Throws<DomainException>(() =>
            Aluno.Criar(0,
            nome,
            cpf,
            dataNascimento,
            telefone,
            email,
            logradouro,
            numero,
            complemento,
            senha,
            foto
            ));
            Assert.Equal("CPF_DIGITOS", ex.Message);
        }
        [Fact]
        public void CriarAluno_ComNomeVazio_DeveLancarExcecao()
        {
            // Arrange
            var cpf = "12345678901"; var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)); var telefone = "11999999999";
            var email = "joao@email.com"; var logradouro = GetValidLogradouro(); var numero = "123"; var complemento = "Apto 1"; var senha = "Senha@123"; var foto = GetValidArquivo();
            // Act & Assert
            var ex = Assert.Throws<DomainException>(() =>
            Aluno.Criar(0,
            "",
            cpf,
            dataNascimento,
            telefone,
            email,
            logradouro,
            numero,
            complemento,
            senha,
            foto
            ));
            Assert.Equal("NOME_OBRIGATORIO", ex.Message);
        }

        [Fact]
        public void CriarAluno_VerificaNormalizacoes()
        {
            // Arrange
            var cpf = " 1234567  8901  "; var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)); var telefone = "    119999 99999    ";
            var email = "   joao@email.com "; var logradouro = GetValidLogradouro(); var numero = " 123"; var complemento = "Apto 1  "; var senha = "Senha1234@"; var foto = GetValidArquivo();
            // Act & Assert
            var aluno = Aluno.Criar(0,
            "João Silva",
            cpf,
            dataNascimento,
            telefone,
            email,
            logradouro,
            numero,
            complemento,
            senha,
            foto
            );
            Assert.Equal("João Silva", aluno.Nome);
            Assert.Equal("11999999999", aluno.Telefone);
            Assert.Equal("joao@email.com", aluno.Email);
            Assert.Equal("123", aluno.Numero);
            Assert.Equal("Senha1234@", aluno.Senha);
        }
    }

}
