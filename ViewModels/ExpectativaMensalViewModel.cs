using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpectativaMensal.Models;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

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

        public string order;
        public string indicador;

        public ICommand GetExpectativasCommand { get; }

        public ExpectativaMensalViewModel()
        {
            Expectativas = new ObservableCollection<ExpectativaMercado>();
            GetExpectativasCommand = new RelayCommand(async () => await GetExpectativasAsync());
        }

        public string addFilters(string filtro, string uriText)
        {

            if(filtro != null && filtro != "")
            {
                if(filtro == "indicador")
                {
                    return $"&%24filter={uriText}&%24top=50";
                } 

                if(filtro ==  "ordenar")
                {
                    return $"&%24orderby={uriText}&%24top=50";
                }
            }

            return "";
        }

        public async Task GetFilteredExpectativasAsync(string? filter, string? uriText)
        {

            string filters = addFilters(filter, uriText);

            using (HttpClient client = new HttpClient())
            {
                string url = $"https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais?%24format=json{filters}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    DesserializarJson(json);
                }
            }
        }

        public async Task GetExpectativasAsync()
        {

            using (HttpClient client = new HttpClient())
            {
                string url = $"https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais?%24format=json&%24top=50";

                HttpResponseMessage response = await client.GetAsync(url);

                if(response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    DesserializarJson(json);
                }
            }
        }

        public void DesserializarJson(string json)
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
                // Tratar a exceção de desserialização aqui
                Console.WriteLine($"Erro na desserialização: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Tratar outras exceções aqui
                Console.WriteLine($"Erro: {ex.Message}");
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
