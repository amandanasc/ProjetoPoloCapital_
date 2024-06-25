using ExpectativaMensal.ViewModels;
using System;
using System.Collections.Generic;
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
        public ExpectativaMercadoView()
        {
            InitializeComponent();
            _viewModel = new ExpectativaMensalViewModel();
            DataContext = _viewModel;

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.GetExpectativasAsync(); // Chamar o método assíncrono do ViewModel
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }
        }

        private async void MainWindow_Loaded(string? filter, string? uriText)
        {
            try
            {
                await _viewModel.GetFilteredExpectativasAsync(filter, uriText); // Chamar o método assíncrono do ViewModel
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }
        }

        private void SelectIndicador_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem indicador = (ComboBoxItem)SelectIndicador.SelectedItem;
            string indicadorSelecionado = indicador.Content.ToString();

            string indicadorTextoFormatado = FormatStringUri(indicadorSelecionado);

            //MessageBox.Show(indicadorTextoFormatado);

            MainWindow_Loaded("indicador", indicadorTextoFormatado);
        }

        private string FormatStringUri(string uriText)
        {
            return uriText.Replace(" ", "%20");
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Você clicou em Order");
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Você clicou em Exportar");
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
