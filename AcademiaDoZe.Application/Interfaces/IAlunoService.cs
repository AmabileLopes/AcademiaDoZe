using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
namespace AcademiaDoZe.Application.Interfaces
{
    public interface IAlunoService
    {
        Task<AlunoDTO> ObterPorIdAsync(int id);
        Task<IEnumerable<AlunoDTO>> ObterTodosAsync();
        Task<AlunoDTO> AdicionarAsync(AlunoDTO alunoDto);
        Task<AlunoDTO> AtualizarAsync(AlunoDTO alunoDto);
        Task<bool> RemoverAsync(int id);
        Task<AlunoDTO> ObterPorCpfAsync(string cpf);
        Task<bool> CpfJaExisteAsync(string cpf, int? id = null);
        Task<bool> TrocarSenhaAsync(int id, string novaSenha);
    }
}
