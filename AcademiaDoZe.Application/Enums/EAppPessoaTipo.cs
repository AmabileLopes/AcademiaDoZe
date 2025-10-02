using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Application.Enums
{
        public enum EAppPessoaTipo
        {
            [Display(Name = "Colaborador")]
            Colaborador = 0,
            [Display(Name = "Aluno")]
            Aluno = 1
        }
}
