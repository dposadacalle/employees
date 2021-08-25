using employees.Common.Models;
using employees.Common.Responses;
using employees.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

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
        [FunctionName(nameof(CreateEntry))]
        public static async Task<IActionResult> CreateEntry(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "entry")] HttpRequest req,
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

        /*
            Daniel Posada
            Date: 23/08/2021
            Method: PUT
            Description: 
               - Endpoint: PUT for update a Entry
        */
        [FunctionName(nameof(UpdateEntry))]
        public static async Task<IActionResult> UpdateEntry(
             [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "entry/{id}")] HttpRequest req,
             [Table("entry", Connection = "AzureWebJobsStorage")] CloudTable entryTable,
             string id,
             ILogger log)
        {
            log.LogInformation($"Update for entry {id}, received.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Entry entry = JsonConvert.DeserializeObject<Entry>(requestBody);

            // Validate Entry Id
            TableOperation findOperation = TableOperation.Retrieve<EntryEntity>("ENTRY", id);
            TableResult findResult = await entryTable.ExecuteAsync(findOperation);
            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucess = false,
                    Message = "Entry not found."
                });
            }

            EntryEntity entryEntity = (EntryEntity)findResult.Result;
            if (!string.IsNullOrEmpty(entry.Tipo))
            {
                entryEntity.Tipo = entry.Tipo;
            }

            TableOperation addOperation = TableOperation.Replace(entryEntity);
            await entryTable.ExecuteAsync(addOperation);

            string message = $"Entry: {id} updated in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSucess = true,
                Message = message,
                Result = entryEntity
            });

        }

        /*
            Daniel Posada
            Date: 23/08/2021
            Methodo: GET
            Description: 
               - Endpoint for get all the Entrys 
        */
        [FunctionName(nameof(GetAllEntry))]
        public static async Task<IActionResult> GetAllEntry(

            // Inject through HttpRequest
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "entry")] HttpRequest req,
            // Inject through CloudTable 
            [Table("entry", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            // Inject through ILogger
            ILogger log)
        {
            log.LogInformation("Get all entrys received.");

            TableQuery<EntryEntity> query = new TableQuery<EntryEntity>();
            TableQuerySegment<EntryEntity> entrys = await todoTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieve all entrys.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSucess = true,
                Message = message,
                Result = entrys
            });

        }

        /*
            Daniel Posada
            Date: 23/08/2021
            Methodo: GET
            Description: 
               - Endpoint forget a Entry for the Id
        */
        [FunctionName(nameof(GetEntryById))]
        public static IActionResult GetEntryById(

            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "entry/{id}")] HttpRequest req,
            [Table("entry", "TODO", "{id}", Connection = "AzureWebJobsStorage")] EntryEntity entryEntity,
            string id,
            ILogger log)
        {
            log.LogInformation($"Get entry by id: {id} received.");


            if (entryEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucess = false,
                    Message = "Entry not found."
                });
            }

            string message = $"Todo {entryEntity.RowKey}, received.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSucess = true,
                Message = message,
                Result = entryEntity
            });

        }

        /*
            Daniel Posada
            Date: 23/08/2021
            Methodo: DELETE
            Description: 
               - Endpoint for delete a Entry for the ID
        */
        [FunctionName(nameof(DeleteEntryById))]
        public static async Task<IActionResult> DeleteEntryById(

             [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "entry/{id}")] HttpRequest req,
             [Table("entry", "ENTRY", "{id}", Connection = "AzureWebJobsStorage")] EntryEntity entryEntity,
             [Table("entry", Connection = "AzureWebJobsStorage")] CloudTable entryTable,
             string id,
             ILogger log)
        {
            log.LogInformation($"Delete entry: {id} received.");

            if (entryEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucess = false,
                    Message = "Entry not found."
                });
            }

            await entryTable.ExecuteAsync(TableOperation.Delete(entryEntity));

            string message = $"Entry {entryEntity.RowKey}, deleted.";
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
