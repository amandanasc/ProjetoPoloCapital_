using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpectativaMensal.Models
{
    class Response
    {
        public string? Descricao;
        public ExpectativaMercado[] Value { get; set; }
    }
}
