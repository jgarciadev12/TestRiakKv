using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRiak.ViewModel
{
    public class ServicioVM
    {
        public int CodS { get; set; }
        public string Descripcion { get; set; }
        public int costeinterno { get; set; }

        public EmpleadoVM NumReg { get; set; }
    }
}
