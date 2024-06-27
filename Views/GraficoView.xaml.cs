using LiveCharts.Wpf;
using LiveCharts;
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
using System.Collections.ObjectModel;
using ExpectativaMensal.Models;
using OxyPlot;
using OxyPlot.Axes;
using AxisPosition = OxyPlot.Axes.AxisPosition;

namespace ExpectativaMensal.Views
{
    public partial class GraficoView : Window
    {
        public ChartValues<double> Values { get; set; }

        public GraficoView(int length, IEnumerable<double> min, IEnumerable<double> max, IEnumerable<double> desvioPadrao, IEnumerable<double> baseCalculo)
        {
            InitializeComponent();

            List<double> data = new List<double>();

            for (var i = 0; i < length; i++)
            {
                double minX = baseCalculo.ElementAt(i) - desvioPadrao.ElementAt(i);
                double maxX = baseCalculo.ElementAt(i) + desvioPadrao.ElementAt(i);
                double minY = min.ElementAt(i);
                double maxY = max.ElementAt(i);

                double slope = (maxY - minY) / (maxX - minX);
                double interception = minY - slope * minX;

                data.Add(interception * slope);
            }

            Values = new ChartValues<double>(data);

            DataContext = this;
        }
    }
}
