using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ExpectativaMensal.Models;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExpectativaMensal.ViewModels
{
    class ExpectativaMensalViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ExpectativaMercado> _expectativas;

        public ObservableCollection<ExpectativaMercado> Expectativas
        {
            get => _expectativas;
            set
            {
                _expectativas = value;
                OnPropertyChanged();
            }
        }

        public ICommand GetExpectativasCommand { get; }

        public ExpectativaMensalViewModel()
        {
            Expectativas = new ObservableCollection<ExpectativaMercado>();
            GetExpectativasCommand = new RelayCommand(async () => await GetExpectativasAsync());
        }

        //Método que retorna as Expectativas dentro de um período selecionado pelo usuário
        public void ListExpectativasByDates(DateTime dtInicio, DateTime dtFinal)
        {
            try
            {
                ObservableCollection<ExpectativaMercado> auxiliar = new();

                MessageBox.Show($"data inicio: {dtInicio} data final: {dtFinal}");

                foreach (var expectativa in Expectativas)
                {
                    var d = Convert.ToDateTime(expectativa.Data);

                    if (d >= dtInicio && d <= dtFinal)
                    {
                        auxiliar.Add(expectativa);
                    }
                    else
                    {
                        continue;
                    }
                }

                Expectativas.Clear();

                foreach (var aux in auxiliar)
                {
                    Expectativas.Add(aux);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show($"'{dtInicio}' is not in the proper format.");
                MessageBox.Show($"'{dtFinal}' is not in the proper format.");
            }
        }

        //Formata o filtro para ser adicionado à requisição do método GetFilteredExpectativasAsync()
        public string addFilters(string uriText)
        {

            if(uriText != null)
            {
                return $"&%24filter=Indicador%20eq%20'{uriText}'&%24top=1000";
            }
            
            return "";
        }

        //Método para consumir os dados da API com filtros adicionados
        public async Task GetFilteredExpectativasAsync(string? uriText)
        {

            string filters = addFilters(uriText);

            using (HttpClient client = new HttpClient())
            {
                string url = $"https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais?%24format=json{filters}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    DeserializeJson(json);
                }
            }
        }

        //Método para consumir os dados da API
        public async Task GetExpectativasAsync()
        {

            using (HttpClient client = new HttpClient())
            {
                string url = $"https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais?%24format=json&%24top=1000";

                HttpResponseMessage response = await client.GetAsync(url);

                if(response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    DeserializeJson(json);
                }
            }
        }

        //Método para desserializar o json recebido e adicionar os dados a Collection Expectativa
        public void DeserializeJson(string json)
        {
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<Response>(json);

                Expectativas.Clear();

                foreach (var expectativa in apiResponse.Value)
                {
                    Expectativas.Add(expectativa);
                }
            }
            catch (JsonSerializationException ex)
            {
                // Tratar a exceção de desserialização
                Console.WriteLine($"Erro na desserialização: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Tratar outras exceções
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }

        //Método para formatar os dados de Expectativa de Mercado para o padrão CSV e Gravar os dados em um novo arquivo CSV
        public void ExportCsv(string fileName, ObservableCollection<ExpectativaMercado> expectativas)
        {
            StringBuilder csv = new StringBuilder();

            // cabeçalho
            csv.AppendLine("Indicador,Data,Data Referencia,Media,Mediana,Desvio Padrao,Minimo,Maximo,Numero Respondentes,Base Calculo");

            // corpo/células
            foreach (var expectativa in expectativas)
            {
                csv.AppendLine($"{expectativa.Indicador},{expectativa.Data},{expectativa.DataReferencia},{expectativa.Media},{expectativa.Mediana},{expectativa.DesvioPadrao},{expectativa.Minimo},{expectativa.Maximo},{expectativa.numeroRespondentes},{expectativa.baseCalculo}");
            }

            File.WriteAllText(fileName, csv.ToString());
            MessageBox.Show("Dados exportados com sucesso!", "Exportar CSV", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //Método que envia os dados para o Banco de Dados
        public async Task SaveOnDataBase()
        {
            using (AppDbContext context = new AppDbContext())
            {
                foreach(var expectativa in Expectativas)
                {
                    context.ExpectativaMercadoMensal.Add(expectativa);
                }

                await context.SaveChangesAsync();
            }
        }

        //EVENT Properties and Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public class RelayCommand : ICommand
        {
            private readonly Func<Task> _execute;

            public RelayCommand(Func<Task> execute)
            {
                _execute = execute;
            }

            public bool CanExecute(object parameter) => true;

            public async void Execute(object parameter)
            {
                await _execute();
            }

            public event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }
        }
    }
}
