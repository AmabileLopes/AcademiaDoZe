using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
namespace AcademiaDoZe.Application.Enums
{
    public enum EAppColaboradorVinculo
    {
        [Display(Name = "CLT")]
        CLT = 0,
        [Display(Name = "Estagiário")]
        Estagio = 1
    }
}