using ExpectativaMensal.Models;
using ExpectativaMensal.ViewModels;
using Microsoft.VisualBasic;
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
using static ExpectativaMensal.ViewModels.ExpectativaMensalViewModel;

namespace ExpectativaMensal.Views
{
    public partial class ExpectativaMercadoView : Window
    {
        private readonly ExpectativaMensalViewModel _viewModel;

        public DateTime dtInicial;
        public DateTime dtFinal;
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

        //Formata e atribui a data selecionada à variável local
        private void SelectInitialDate(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            DateTime date = (DateTime)datePicker.SelectedDate;
            this.dtInicial = date;

            this.isDataInicialNotEmpty = true;
        }

        //Atribui a data selecionada à variável local e chama o método de Listagem
        private void SelectFinalDate(object sender, SelectionChangedEventArgs e)
        {
            DateTime date = (DateTime)dpDataFinal.SelectedDate;
            this.dtFinal = date;

            //chamar a função do view model passando a data inicial e final
            _viewModel.ListExpectativasByDates(this.dtInicial, this.dtFinal);
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
                _viewModel.ExportCsv(saveFileDialog.FileName, _viewModel.Expectativas);
            }
        }

        //Chama a View para criação do Gráfico
        private void btnGraphic_Click(object sender, RoutedEventArgs e)
        {
            List<double> min = new List<double>();
            List<double> max = new List<double>();
            List<double> desvioPadrao = new List<double>();
            List<double> baseCalculo = new List<double>();

            for (int i = 0; i < _viewModel.Expectativas.Count; i++)
            {
                min.Add((double)_viewModel.Expectativas[i].Minimo);
                max.Add((double)_viewModel.Expectativas[i].Maximo);
                desvioPadrao.Add((double)_viewModel.Expectativas[i].DesvioPadrao);
                baseCalculo.Add((double)_viewModel.Expectativas[i].DesvioPadrao);
            }

            GraficoView graphView = new(_viewModel.Expectativas.Count, min, max, desvioPadrao, baseCalculo);
            graphView.Show();
        }

        //Chama o método que salva os dados no Banco de Dados
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.SaveOnDataBase();
                MessageBox.Show("Salvo com Sucesso!");
            }
            catch 
            {
                MessageBox.Show("Erro ao salvar informações no Banco de Dados");
            }
        }
    }
}
