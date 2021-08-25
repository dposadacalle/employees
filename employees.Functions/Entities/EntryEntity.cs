using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace employees.Functions.Entities
{
    public class EntryEntity : TableEntity
    {
        public int IdEmpleo { get; set; } 
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public string Tipo { get; set; }
        public bool Consolidado { get; set; }
    }
}
