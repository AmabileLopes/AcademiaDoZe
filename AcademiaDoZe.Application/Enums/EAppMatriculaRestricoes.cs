using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace AcademiaDoZe.Application.Enums
{
    [Flags]
    public enum EAppMatriculaRestricoes
    {
        [Display(Name = "Nenhuma Restrição")]
        None = 0,
        [Display(Name = "Diabetes")]
        Diabetes = 1,
        [Display(Name = "Pressão Alta")]
        PressaoAlta = 2,
        [Display(Name = "Labirintite")]
        Labirintite = 4,
        [Display(Name = "Alergias")]
        Alergias = 8,
        [Display(Name = "Problemas Respiratórios")]
        ProblemasRespiratorios = 16,
        [Display(Name = "Remédio Contínuo")]
        RemedioContinuo = 32
    }
}
