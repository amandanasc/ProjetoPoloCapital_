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


        public void setOrder(string order)
        {
            if(order == "order")
            {
                this.order = "&%24orderby=Data";
            }

            return;
        }

        public void setFilterIndicador(string indicador)
        {
            if(indicador != null)
            {
                this.indicador = $"&%24filter={indicador}"; //&%24filter=IPCA
            }

            return;
        }

        public async Task GetExpectativasAsync()
        {
            using(HttpClient client = new HttpClient())
            {
                string url = $"https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais?%24format=json&%24top=10{this.indicador}{this.order}";

                HttpResponseMessage response = await client.GetAsync(url);

                if(response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

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
