using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuCalcuTeApuesto.Models
{
    public class EquiposModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }
        public string Imagen { get; set; }

        public string IsSelected { get; set; }
    }

    public class ListaEquipos
    {
        public List<EquiposModel> Lista { get; set; }

    }

}
