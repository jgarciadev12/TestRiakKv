using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRiak.Entity;

namespace TestRiak.ViewModel
{
    public class HabitacionVM
    {
        public int Numero { get; set; }
        public int? superficie { get; set; }
        public string bar { get; set; }
        public string terraza { get; set; }
        public string puedesupletoria { get; set; }
        
        public PrecioVM Tipo { get; set; }
        public List<LimpiezaVM> LimpiezaList { get; set; }



    }
}
