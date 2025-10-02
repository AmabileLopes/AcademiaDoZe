using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class MatriculaRepository : BaseRepository<Matricula>, IMatriculaRepository
    {
        //Amabile Vitória Lopes Ouriques
        public MatriculaRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }
       

        public override async Task<Matricula> Adicionar(Matricula entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = _databaseType == DatabaseType.SqlServer
                ? $"INSERT INTO {TableName} (aluno_id, plano, data_inicio, data_fim, objetivo, restricao_medica, laudo_medico, obs_restricao) "
                + "OUTPUT INSERTED.id_matricula "
                + "VALUES (@Aluno, @Plano, @Data_inicio, @Data_fim, @Objetivo, @Restricao_medica, @Laudo_medico, @Obs_restricao);"
                : $"INSERT INTO {TableName} (aluno_id, plano, data_inicio, data_fim, objetivo, restricao_medica, laudo_medico, obs_restricao) "
                + "VALUES (@Aluno, @Plano, @Data_inicio, @Data_fim, @Objetivo, @Restricao_medica, @Laudo_medico, @Obs_restricao); "
                + "SELECT LAST_INSERT_ID();";
                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Aluno", entity.AlunoMatricula.Id, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Plano", (int)entity.Plano, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Data_inicio", entity.DataInicio, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Data_fim", entity.DataFim, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Objetivo", entity.Objetivo, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Restricao_medica", entity.RestricoesMedicas, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Laudo_medico", (object)entity.LaudoMedico?.Conteudo ?? DBNull.Value, DbType.Binary, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Obs_restricao", (object)entity.ObservacoesRestricoes ?? DBNull.Value, DbType.String, _databaseType));
                var id = await command.ExecuteScalarAsync();
                if (id != null && id != DBNull.Value)
                {
                    // Define o ID usando reflection
                    var idProperty = typeof(Entity).GetProperty("Id");
                    idProperty?.SetValue(entity, Convert.ToInt32(id));
                }
                return entity;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao adicionar matricula: {ex.Message}", ex); }
        }

        public override async Task<Matricula> Atualizar(Matricula entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"UPDATE {TableName} "
                + "SET aluno_id = @Aluno, "
                + "plano = @Plano, "
                + "data_inicio = @Data_inicio, "
                + "data_fim = @Data_fim, "
                + "objetivo = @Objetivo, "
                + "restricao_medica = @Restricao_medica, "
                + "laudo_medico = @Laudo_medico, "
                + "obs_restricao = @Obs_restricao "
                + "WHERE id_matricula = @Id";
                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Aluno", entity.AlunoMatricula.Id, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Plano", (int)entity.Plano, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Data_inicio", entity.DataInicio, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Data_fim", entity.DataFim, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Objetivo", entity.Objetivo, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Restricao_medica", entity.RestricoesMedicas, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Laudo_medico", (object)entity.LaudoMedico?.Conteudo ?? DBNull.Value, DbType.Binary, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Obs_restricao", (object)entity.ObservacoesRestricoes ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType)); // adicionado
                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Nenhuma matricula encontrado com o ID {entity.Id} para atualização.");
                }
                return entity;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao atualizar matrcula com ID {entity.Id}: {ex.Message}", ex);
            }
        }
        public async Task<IEnumerable<Matricula>> ObterPorAluno(int alunoId)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                
                string query = $"SELECT * FROM {TableName} WHERE aluno_id = @Aluno";
                await using var command = DbProvider.CreateCommand(query, connection);

                command.Parameters.Add(DbProvider.CreateParameter("@Aluno", alunoId, DbType.String, _databaseType));

                using var reader = await command.ExecuteReaderAsync();
                var matriculas = new List<Matricula>();
                while (await reader.ReadAsync())
                {
                    
                        matriculas.Add(await MapAsync(reader));
                    
                   
                }
                return matriculas;
            }
             catch (DbException ex) {throw new InvalidOperationException($"Erro ao obter matriculas do aluno com ID {alunoId}: {ex.Message}", ex); }

        }

        public async Task<IEnumerable<Matricula>> ObterAtivas(int idAluno = 0)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT * FROM {TableName} WHERE data_fim >= {(_databaseType == DatabaseType.SqlServer ? "GETDATE()" :
                "CURRENT_DATE()")} {(idAluno > 0 ? "AND aluno_id = @id" : "")} ";
                await using var command = DbProvider.CreateCommand(query, connection);
                if (idAluno > 0)
                {
                    command.Parameters.Add(DbProvider.CreateParameter("@id", idAluno, DbType.Int32, _databaseType));
                }
                using var reader = await command.ExecuteReaderAsync();
                var matriculas = new List<Matricula>();
                while (await reader.ReadAsync())
                {
                    matriculas.Add(await MapAsync(reader));
                }
                return matriculas;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao obter matrículas ativas: {ex.Message}", ex);
            }
        }
        
        public async Task<IEnumerable<Matricula>> ObterVencendoEmDias(int dias)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT * FROM {TableName} WHERE data_fim > @Hoje AND data_fim <= @Limite";

                await using var command = DbProvider.CreateCommand(query, connection);

                var hoje = DateTime.Today;
                var limite = hoje.AddDays(dias);

                var paramHoje = command.CreateParameter();
                paramHoje.ParameterName = "@Hoje";
                paramHoje.Value = hoje;
                command.Parameters.Add(paramHoje);

                var paramLimite = command.CreateParameter();
                paramLimite.ParameterName = "@Limite";
                paramLimite.Value = limite;
                command.Parameters.Add(paramLimite);

                using var reader = await command.ExecuteReaderAsync();

                var resultados = new List<Matricula>();
                while (await reader.ReadAsync())
                {
                    var matricula = await MapAsync(reader);
                    resultados.Add(matricula);
                }

                return resultados;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao obter matrículas vencendo em {dias} dias: {ex.Message}", ex);
            }
        }

        protected override async Task<Matricula> MapAsync (DbDataReader reader)
        {
            try
            {
                // Obtém o aluno de forma assíncrona
                var alunoId = Convert.ToInt32(reader["aluno_id"]);
                var alunoRepository = new AlunoRepository(_connectionString, _databaseType);
                var aluno = await alunoRepository.ObterPorId(alunoId) ?? throw new InvalidOperationException($"Aluno com ID {alunoId} não encontrado.");

                // Cria o objeto Matricula usando o método de fábrica
                var matricula = Matricula.Criar(
                     id: Convert.ToInt32(reader["id_matricula"]),
                    alunoMatricula: aluno,
                    plano: (EMatriculaPlano)Convert.ToInt32(reader["plano"]),
                    dataInicio: DateOnly.FromDateTime(Convert.ToDateTime(reader["data_inicio"])),
                    dataFim: DateOnly.FromDateTime(Convert.ToDateTime(reader["data_fim"])),
                    objetivo: reader["objetivo"].ToString(),
                    restricoesMedicas: (EMatriculaRestricoes)Convert.ToInt32(reader["restricao_medica"]),
                    laudoMedico: reader["laudo_medico"] is DBNull ? null : Arquivo.Criar((byte[])reader["laudo_medico"]),
                    observacoesRestricoes: reader["obs_restricao"]?.ToString() ?? string.Empty
                );
                // define o ID usando reflection
                var idProperty = typeof(Entity).GetProperty("Id");
                idProperty?.SetValue(matricula, Convert.ToInt32(reader["id_matricula"]));

                return matricula;
            }
            catch (DbException ex) { throw new InvalidOperationException($"Erro ao mapear dados da matricula: {ex.Message}", ex); }
        }
    }
}
