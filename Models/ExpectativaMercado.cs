using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpectativaMensal.Models
{
    class ExpectativaMercado { 
        public string? Indicador { get; set; }
        public string? Data { get; set; }
        public string? DataReferencia { get; set; }
        public double? Media { get; set; }
        public double? Mediana { get; set; }
        public double? DesvioPadrao { get; set; }
        public double? Minimo { get; set; }
        public double? Maximo { get; set; }
        public Int32? numeroRespondentes { get; set; }
        public Int32? baseCalculo { get; set; }
    }
}
