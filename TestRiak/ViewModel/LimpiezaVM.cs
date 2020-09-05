using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestRiak.ViewModel
{
    public class LimpiezaVM
    {
        public EmpleadoVM NumReg { get; set; }
        public HabitacionVM Numero { get; set; }

        public DateTime Fecha { get; set; }

    }
}
