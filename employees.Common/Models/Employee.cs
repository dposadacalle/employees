using System;

namespace employees.Common.Models
{
    internal class Employee
    {
        public int Id { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public char Tipo { get; set; }
        public bool Consolidado { get; set; }

    }
}
