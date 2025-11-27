using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using Azure;
using CommunityToolkit.Mvvm.Input;
using ZstdSharp.Unsafe;
namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    [QueryProperty(nameof(MatriculaId), "Id")]
    public partial class MatriculaViewModel : BaseViewModel
    {

        // Lista de opções disponíveis para o CheckBox

        private List<TipoMatriculaOption> _OpcoesTipo;
        public List<TipoMatriculaOption> OpcoesTipo
        {
            get => _OpcoesTipo;
            set => SetProperty(ref _OpcoesTipo, value);
        }

        public void InicializaTipoRestricoes()
        {
            OpcoesTipo = new List<TipoMatriculaOption>();
            foreach (EAppMatriculaRestricoes p in Enum.GetValues(typeof(EAppMatriculaRestricoes)))
            {
                bool selecionado = false;
                if (Matricula.RestricoesMedicas.HasFlag(p))
                {
                    selecionado = true;
                }

                OpcoesTipo.Add(new TipoMatriculaOption()
                {
                    Nome = p.GetDisplayName(),
                    Valor = p,
                    IsSelecionado = selecionado
                });
            }
        }

        public void AtualizarRestricoesMatricula()
        {
            EAppMatriculaRestricoes restricoes = default(EAppMatriculaRestricoes);
            foreach (TipoMatriculaOption x in OpcoesTipo)
            {
                if (x.IsSelecionado)
                    restricoes |= x.Valor;
            }

            Matricula.RestricoesMedicas = restricoes;
        }
    }
    // Classe auxiliar para binding
    public class TipoMatriculaOption
    {
        public EAppMatriculaRestricoes Valor { get; set; }
        public string Nome { get; set; }
        public bool IsSelecionado { get; set; }
    }
}
