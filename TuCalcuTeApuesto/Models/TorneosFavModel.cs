using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuCalcuTeApuesto.Models
{
    public class TorneosFavModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public string NombreCorto { get; set; }
        public bool IsChecked { get; set; }
        public string Imagen { get; set; }

        public string IsSelected { get; set; }
    }

    public class ListaTorneossFav
    {
        public List<TorneosFavModel> Lista { get; set; }

    }

}
