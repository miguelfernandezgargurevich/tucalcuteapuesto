using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TuCalcuTeApuesto.Models
{
    public class ListaModel
    {
        public List<ModeloModel> Lista { get; set; }
        public List<CabeceraModel> ListaCabeceras { get; set; }
        public List<TorneoModel> ListaTorneos { get; set; }
        public List<int> ListaCabecerasFav { get; set; }
        public List<int> ListaCabecerasMin { get; set; }
        public List<FileModel> ListaFiles { get; set; }
        public FileModel Archivo { get; set; }


    }
}
