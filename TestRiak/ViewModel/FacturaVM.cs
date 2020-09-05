using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TestRiak.ViewModel
{
    public class FacturaVM
    {
        public  int CodigoF { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime Salida { get; set; }
        
        public ClienteVM Cliente { get; set; }
        public HabitacionVM Numero { get; set; }
        public FormaPagoVM Forma { get; set; }

        public decimal Total { get; set; }


    }
}
