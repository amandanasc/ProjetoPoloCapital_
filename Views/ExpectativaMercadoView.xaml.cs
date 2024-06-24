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
    }
}
