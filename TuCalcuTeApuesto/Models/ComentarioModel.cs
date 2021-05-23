using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuCalcuTeApuesto.Models
{
    public class ComentarioModel
    {
        public string Mensaje { get; set; }
        public Nullable<int> Puntaje { get; set; }
        public Nullable<int> CodUsuario { get; set; }
    }
}
