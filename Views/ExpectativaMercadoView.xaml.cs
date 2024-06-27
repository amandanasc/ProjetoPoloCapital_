using ExpectativaMensal.Models;
using ExpectativaMensal.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public string dtInicial;
        public string dtFinal;
        public bool isDataInicialNotEmpty = false;

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

        //Método que chama a função que recebe os dados da API no _viewModel
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

            string indicadorSelecionado = indicador.Content.ToString();

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

        private void dtDataInicial(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            DateTime date = (DateTime)datePicker.SelectedDate;
            string dateFormated = $"{date:yyyy-MM-dd}";
            this.dtInicial = dateFormated;

            this.isDataInicialNotEmpty = true;
        }

        private void dtDataFinal(object sender, SelectionChangedEventArgs e)
        {
            DateTime date = (DateTime)dpDataFinal.SelectedDate;
            string dateFormated = $"{date:yyyy-MM-dd}";
            this.dtFinal = dateFormated;

            //chamar a função do view model passando a data inicial e final
            _viewModel.ListarExpectativasPorDatas(this.dtInicial, this.dtFinal);
        }

        //Método para exportação dos dados para CSV
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "export.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _viewModel.ExportarParaCsv(saveFileDialog.FileName, _viewModel.Expectativas);
            }
        }

        private void btnGraphic_Click(object sender, RoutedEventArgs e)
        {
            List<double> data = new();

            for (int i = 0; i < _viewModel.Expectativas.Count; i++)
            {
                data.Add((double)_viewModel.Expectativas[i].Media);
            }

            GraficoView graphView = new(data);
            //graphView.Show();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Você clicou em Salvar");
        }
    }
}
