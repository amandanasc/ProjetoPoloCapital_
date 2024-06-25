using ExpectativaMensal.Models;
using ExpectativaMensal.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExpectativaMensal.Views
{
    public partial class ExpectativaMercadoView : Window
    {
        private readonly ExpectativaMensalViewModel _viewModel;

        //Ao iniciar o componente, adiciona as informações do _viewModel ao DataContext e carrega os dados na tela
        public ExpectativaMercadoView()
        {
            InitializeComponent();
            _viewModel = new ExpectativaMensalViewModel();
            DataContext = _viewModel;

            Loaded += MainWindow_Loaded;
        }

        //Método que chama e recebe a função de acesso ao _viewModel
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GetAllExpectativas();
        }

        //Método que acessa a função que recebe os dados da API no _viewModel na forma de tarefa (task)
        private async Task GetAllExpectativas()
        {
            try
            {
                // Chama o método que recebe as Expectativas de Mercado de forma assíncrona do _viewModel
                await _viewModel.GetExpectativasAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }
        }

        //Método para receber os dados Filtrados por Indicador Selecionado do _viewModel
        private async Task GetExpectativasByIndicador(string uriText)
        {
            try
            {
                // Chama o método que recebe as Expectativas de Mercado de forma assíncrona do _viewModel
                await _viewModel.GetFilteredExpectativasAsync(uriText);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }
        }

        //Método que recebe o valor clicado no Combobox/Select
        private void SelectIndicador_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem indicador = (ComboBoxItem)SelectIndicador.SelectedItem;

            //transforma o valor clicado em string
            string indicadorSelecionado = indicador.Content.ToString();

            //formata o valor para o formato que será usado na requisição à API
            string uriText = FormatStringUri(indicadorSelecionado);

            if (uriText == null)
            {
                GetAllExpectativas();
            }

            GetExpectativasByIndicador(uriText);
        }
        
        //Método para formatar o filtro para o padrão de requisição exigido pela API
        private string? FormatStringUri(string uriText)
        {
            if(uriText == "Todos")
            {
                return null;
            }
            return uriText.Replace(" ", "%20");
        }


        //private void btnDate_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show("Você clicou em Data");
        //}


        //Método para exportação dos dados para CSV
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "export.csv"
            };

            if(saveFileDialog.ShowDialog() == true)
            {
                ExportarParaCsv(saveFileDialog.FileName, _viewModel.Expectativas);
            }
        }

        //Método para formatar os dados de Expectativa de Mercado para o padrão CSV e Gravar os dados em um novo arquivo CSV
        private void ExportarParaCsv(string fileName, ObservableCollection<ExpectativaMercado> expectativas)
        {
            StringBuilder csv = new StringBuilder();

            // cabeçalho
            csv.AppendLine("Indicador,Data,Data Referencia,Media,Mediana,Desvio Padrao,Minimo,Maximo,Numero Respondentes,Base Calculo");
            
            // corpo/células
            foreach(var expectativa in expectativas)
            {
                csv.AppendLine($"{expectativa.Indicador},{expectativa.Data},{expectativa.DataReferencia},{expectativa.Media},{expectativa.Mediana},{expectativa.DesvioPadrao},{expectativa.Minimo},{expectativa.Maximo},{expectativa.numeroRespondentes},{expectativa.baseCalculo}");
            }

            File.WriteAllText(fileName, csv.ToString());
            MessageBox.Show("Dados exportados com sucesso!", "Exportar CSV", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnGraphic_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Você clicou em Gráfico");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Você clicou em Salvar");
        }
    }
}
