using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using employees.Common.Models;
using employees.Functions.Entities;
using employees.Common.Responses;

namespace employees.Functions.Functions
{
    public static class EmployeeApi
    {
        /*
            Daniel Posada
            Date: 22/08/2021
            Methodo: POST
            Description: 
               - Endpoint for create a new Entry with post
        */
        [FunctionName("CreateEntry")]
        public static async Task<IActionResult> CreateEntry( 
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "employee")] HttpRequest req,
            [Table("entry", Connection = "AzureWebJobsStorage")] CloudTable entryTable, 
            ILogger log)
        {
            log.LogInformation("Receive a new Entry for him Employee.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Entry entry = JsonConvert.DeserializeObject<Entry>(requestBody);

            EntryEntity entryEntity = new EntryEntity
            {
                IdEmpleo = entry.IdEmpleo,
                ETag = "*",
                FechaEntrada = DateTime.UtcNow,
                FechaSalida = DateTime.UtcNow,
                Tipo = entry.Tipo,
                Consolidado = false,
                PartitionKey = "ENTRY",
                RowKey = Guid.NewGuid().ToString()
            };

            TableOperation addOperation = TableOperation.Insert(entryEntity);
            await entryTable.ExecuteAsync(addOperation);

            string message = "New Entry stored in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSucess = true, 
                Message = message, 
                Result = entryEntity
            });

        }

    }
}
