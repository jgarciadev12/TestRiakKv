using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRiak.ViewModel
{
    public class EmpleadoVM
    {
        public int NumReg { get; set; }
        public string Nombre { get; set; }
        public DateTime Incorporacion { get; set; }
        public int Sueldo { get; set; }
        
        public ServicioVM Servicio { get; set; }


    }
}
