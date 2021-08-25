using System;

namespace employees.Common.Models
{
    public class Entry
    {
        public int IdEmpleo { get; set; } 
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public string Tipo { get; set; }
        public bool Consolidado { get; set; }
    }
}
