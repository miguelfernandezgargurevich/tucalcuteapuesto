//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TuCalcuTeApuesto
{
    using System;
    using System.Collections.Generic;
    
    public partial class UsuariosJugadas
    {
        public int CodPrograma { get; set; }
        public int CodUsuario { get; set; }
        public int CodProgramaDetalle { get; set; }
        public string DesJugada { get; set; }
        public string Cuota { get; set; }
    
        public virtual Programas Programas { get; set; }
        public virtual ProgramasDetalle ProgramasDetalle { get; set; }
        public virtual Usuarios Usuarios { get; set; }
    }
}
