using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using CommunityToolkit.Mvvm.Input;
using ZstdSharp.Unsafe;
namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    [QueryProperty(nameof(MatriculaId), "Id")]
    public partial class MatriculaViewModel : BaseViewModel
    {
        private readonly IMatriculaService _matriculaService;
        private readonly IAlunoService _alunoService;
        public IEnumerable<EAppMatriculaPlano> MatriculaPlanos { get; } = Enum.GetValues(typeof(EAppMatriculaPlano)).Cast<EAppMatriculaPlano>();
        private MatriculaDTO _matricula = new()
        {
            AlunoMatricula = new AlunoDTO
            {
                Nome = string.Empty,
                Cpf = string.Empty,
                Telefone = string.Empty,
                DataNascimento = default,
                Numero = string.Empty,
                Endereco = new LogradouroDTO { Cep = string.Empty, Nome = string.Empty, Bairro = string.Empty, Cidade = string.Empty, Estado = string.Empty, Pais = string.Empty }
            },
            Plano = EAppMatriculaPlano.Anual,
            DataInicio = DateOnly.FromDateTime(DateTime.Now),
            DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(12)),
            Objetivo = string.Empty,
            RestricoesMedicas = EAppMatriculaRestricoes.None,
            ObservacoesRestricoes = string.Empty,
            LaudoMedico = null
        };
        public MatriculaDTO Matricula
        {
            get => _matricula;
            set => SetProperty(ref _matricula, value);
        }
        private int _matriculaId;
        public int MatriculaId
        {
            get => _matriculaId;
            set => SetProperty(ref _matriculaId, value);
        }
        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }
        public MatriculaViewModel(IMatriculaService matriculaService, IAlunoService alunoService)
        {
            _matriculaService = matriculaService;
            _alunoService = alunoService;
            Title = "Detalhes da Matrícula";
        }
        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
        public async Task InitializeAsync()
        {
            if (MatriculaId > 0)
            {
                IsEditMode = true;
                Title = "Editar Matricula";
                await LoadMatriculaAsync();
            }
            else
            {
                IsEditMode = false;
                Title = "Nova Matricula";
            }
            InicializaTipoRestricoes();
        }
        [RelayCommand]
        public async Task LoadMatriculaAsync()
        {
            if (MatriculaId <= 0)
                return;
            try
            {
                IsBusy = true;
                var matriculaData = await _matriculaService.ObterPorIdAsync(MatriculaId);

                if (matriculaData != null)
                {
                    Matricula = matriculaData;
                }

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar matrícula: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        public async Task SaveMatriculaAsync()
        {
            if (IsBusy)
                return;
            if (!ValidateMatricula(Matricula))
                return;
            try
            {
                IsBusy = true;
                // Verifica se o CEP existe antes de continuar

                var alunoData = await _alunoService.ObterPorIdAsync(Matricula.AlunoMatricula.Id);
                if (alunoData == null)
                {
                    await Shell.Current.DisplayAlert("Erro", "O aluno informado não existe. O cadastro não pode continuar.", "OK");
                    return;
                }
                Matricula.AlunoMatricula = alunoData;
                AtualizarRestricoesMatricula();
                if (IsEditMode)
                {
                    await _matriculaService.AtualizarAsync(Matricula);

                    await Shell.Current.DisplayAlert("Sucesso", "Matricula atualizada com sucesso!", "OK");

                }
                else
                {
                    await _matriculaService.AdicionarAsync(Matricula);

                    await Shell.Current.DisplayAlert("Sucesso", "Matricula criada com sucesso!", "OK");

                }
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao salvar matricula: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        public async Task SearchByCpfAsync()
        {
            if (string.IsNullOrWhiteSpace(Matricula.AlunoMatricula.Cpf))
                return;
            try
            {
                IsBusy = true;
                var matriculaData = await _matriculaService.ObterPorAlunoCpfAsync(Matricula.AlunoMatricula.Cpf);

                if (matriculaData != null)

                {
                    Matricula = matriculaData;
                    IsEditMode = true;
                    await Shell.Current.DisplayAlert("Aviso", "Matricula já cadastrado! Dados carregados para edição.", "OK");
                }
                else
                {
                    var alunoData = await _alunoService.ObterPorCpfAsync(Matricula.AlunoMatricula.Cpf);

                    if (alunoData != null)
                    {
                        Matricula.AlunoMatricula = alunoData;
                        await Shell.Current.DisplayAlert("Aviso", "Aluno encontrado! Preencha os dados da matrícula.", "OK");
                        OnPropertyChanged(nameof(Matricula));
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Aviso", "CPF não encontrado.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao buscar CPF: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SelecionarFotoAsync()
        {
            try
            {
                string escolha = await Shell.Current.DisplayActionSheet("Origem do arquivo", "Cancelar", null, "Galeria", "Câmera");
                FileResult? result = null;
                if (escolha == "Galeria")

                {
                    result = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Selecione um arquivo",
                        FileTypes = FilePickerFileType.Images
                    });
                }
                else if (escolha == "Câmera")
                {
                    if (MediaPicker.Default.IsCaptureSupported)
                    {
                        result = await MediaPicker.Default.CapturePhotoAsync();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erro", "Captura de foto não suportada neste dispositivo.", "OK");
                        return;
                    }
                }
                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    Matricula.LaudoMedico = new ArquivoDTO { Conteudo = ms.ToArray() };
                    OnPropertyChanged(nameof(Matricula));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao selecionar imagem: {ex.Message}", "OK");
            }
        }
        private static bool ValidateMatricula(MatriculaDTO matricula)
        {
            const string validationTitle = "Validação";
            if (matricula.AlunoMatricula == null)
            {
                Shell.Current.DisplayAlert(validationTitle, "Aluno é obrigatório.", "OK");
                return false;
            }
            if (string.IsNullOrWhiteSpace(matricula.Objetivo))
            {
                Shell.Current.DisplayAlert(validationTitle, "O Objetivo é obrigatório", "OK");
                return false;
            }
            int idade =CalcularIdade(matricula.AlunoMatricula.DataNascimento);
            if (idade >= 12 && idade <= 16)
            {
                if (matricula.LaudoMedico == null)
                {
                    Shell.Current.DisplayAlert(validationTitle, "Laudo médico é obrigatória.", "OK");
                    return false;
                }
            }
            if (matricula.RestricoesMedicas > 0 && string.IsNullOrWhiteSpace(matricula.ObservacoesRestricoes))
            {
                Shell.Current.DisplayAlert(validationTitle, "Observações das restrições é obrigatório", "OK");
                return false;
            }
            return true;
        }
        public static int CalcularIdade(DateOnly dataNascimento)
        {
            var hoje = DateOnly.FromDateTime(DateTime.Today);
            int idade = hoje.Year - dataNascimento.Year;

            if (dataNascimento > hoje.AddYears(-idade))
            {
                idade--; // ainda não fez aniversário este ano
            }

            return idade;
        }

    }
}
