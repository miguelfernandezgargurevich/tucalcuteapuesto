using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuCalcuTeApuesto.Models
{
    public class FileModel
    {
        public string Codigo { get; set; }
        public string Extension { get; set; }
        public string Name { get; set; }
        public string NameShow { get; set; }
        public string Length { get; set; }
        public string CreationTimeUtc { get; set; }
        public string Path { get; set; }

    }
}
