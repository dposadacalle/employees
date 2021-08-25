﻿using employees.Common.Models;
using employees.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;

namespace employees.Test.Helpers
{
    public class TestFactory
    {
        public static EntryEntity GetEntryEntity()
        {
            return new EntryEntity
            {
                ETag = "*",
                PartitionKey = "ENTRY",
                RowKey = Guid.NewGuid().ToString(),
                FechaEntrada = DateTime.UtcNow,
                FechaSalida = DateTime.UtcNow,
                Consolidado = false,
                Tipo = "1"
            };
        }
        public static DefaultHttpRequest CreateHttpRequest(Guid entryId, Entry entryRequest)
        {
            string request = JsonConvert.SerializeObject(entryRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{entryId}"
            };
        }

        public static DefaultHttpRequest DeleteHttpRequest(Guid entryId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{entryId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Entry entryRequest)
        {
            string request = JsonConvert.SerializeObject(entryRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Entry GetEntryRequest()
        {
            return new Entry
            {
                FechaEntrada = DateTime.UtcNow,
                FechaSalida = DateTime.UtcNow,
                Consolidado = false,
                Tipo = "1"
            };
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;

            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}
