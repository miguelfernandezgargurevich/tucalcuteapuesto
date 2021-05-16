using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuCalcuTeApuesto.Models
{
    public class CabeceraModel
    {
        public int Id { get; set; }
        public int IdGrupo { get; set; }
        public int Value { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }

    }

    public class ListaCabecera
    {
        public List<CabeceraModel> Lista { get; set; }

    }

}
