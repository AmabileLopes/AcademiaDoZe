using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class MatriculaListViewModel : BaseViewModel
    {
        public ObservableCollection<string> FilterTypes { get; } = new() { "Id", "CPF", "Dias para vencimento" };
        private readonly IMatriculaService _matriculaService;
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private string _selectedFilterType = "CPF";
        public string SelectedFilterType
        {
            get => _selectedFilterType;
            set => SetProperty(ref _selectedFilterType, value);
        }
        private ObservableCollection<MatriculaDTO> _matriculas = new();
        public ObservableCollection<MatriculaDTO> Matriculas
        {
            get => _matriculas;
            set => SetProperty(ref _matriculas, value);
        }

        private MatriculaDTO? _selectedMatricula;
        public MatriculaDTO? SelectedMatricula
        {
            get => _selectedMatricula;
            set => SetProperty(ref _selectedMatricula, value);
        }
        public MatriculaListViewModel(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
            Title = "Matrículas";
        }

        [RelayCommand]
        private async Task AddMatriculaAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("matricula");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao navegar para tela de matricula: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task EditMatriculaAsync(MatriculaDTO matricula)
        {
            try
            {
                if (matricula == null)
                    return;
                await Shell.Current.GoToAsync($"matricula?Id={matricula.Id}");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao navegar para tela de edição: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadMatriculasAsync();
        }

        [RelayCommand]
        private async Task SearchMatriculasAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                // Limpa a lista atual

                await MainThread.InvokeOnMainThreadAsync(() =>

                {
                    Matriculas.Clear();
                });
                IEnumerable<MatriculaDTO> resultados = Enumerable.Empty<MatriculaDTO>();
                // Busca os colaboradores de acordo com o filtro
                if (string.IsNullOrWhiteSpace(SearchText))

                {
                    resultados = await _matriculaService.ObterTodasAsync() ?? Enumerable.Empty<MatriculaDTO>();
                }
                else if (SelectedFilterType == "Id" && int.TryParse(SearchText, out int id))
                {
                    var matricula = await _matriculaService.ObterPorIdAsync(id);

                    if (matricula != null)

                        resultados = new[] { matricula };
                }
                else if (SelectedFilterType == "CPF")
                {
                    var matricula = await _matriculaService.ObterPorAlunoCpfAsync(SearchText);

                    if (matricula != null)

                        resultados = new[] { matricula };
                }
                else if (SelectedFilterType == "Dias para vencimento" && int.TryParse(SearchText, out int dias))
                {
                    var matriculas = await _matriculaService.ObterVencendoEmDiasAsync(dias);
                    if (matriculas != null)
                        resultados = matriculas;
                }
                // Atualiza a coleção na thread principal

                await MainThread.InvokeOnMainThreadAsync(() =>

                {
                    foreach (var item in resultados)
                    {
                        Matriculas.Add(item);
                    }
                    OnPropertyChanged(nameof(Matriculas));
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao buscar colaboradores: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadMatriculasAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                // Limpa a lista atual antes de carregar novos dados
                await MainThread.InvokeOnMainThreadAsync(() =>

                {
                    Matriculas.Clear();
                    OnPropertyChanged(nameof(Matriculas));
                });
                var matriculaList = await _matriculaService.ObterTodasAsync();
                if (matriculaList != null)
                {
                    // Garantir que a atualização da UI aconteça na thread principal

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        foreach (var matricula in matriculaList)
                        {
                            Matriculas.Add(matricula);
                        }
                        OnPropertyChanged(nameof(Matriculas));
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar colaboradores: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task DeleteMatriculaAsync(MatriculaDTO matricula)
        {
            if (matricula == null)
                return;
            bool confirm = await Shell.Current.DisplayAlert(
            "Confirmar Exclusão",

            $"Deseja realmente excluir a matrícula ?",
            "Sim", "Não");
            if (!confirm)
                return;
            try
            {
                IsBusy = true;
                bool success = await _matriculaService.RemoverAsync(matricula.Id);
                if (success)
                {
                    Matriculas.Remove(matricula);
                    await Shell.Current.DisplayAlert("Sucesso", "Matrícula excluída com sucesso!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Não foi possível excluir a matrícula.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao excluir a matrícula: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }


    }
}

