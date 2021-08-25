using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

using employees.Functions.Functions;
using employees.Functions.Entities;
using employees.Test.Helpers;
using employees.Common.Models;

namespace employees.Test.Tests
{
    public class EntryApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateEntry_Should_Return_200()
        {
            // Arrenge
            MockCloudTableEntrys mockEntries = new MockCloudTableEntrys(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Entry entryRequest = TestFactory.GetEntryRequest(); 
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(entryRequest);

            // Act
            IActionResult response = await EmployeeApi.CreateEntry(request, mockEntries, logger);

            // Assert
            OkObjectResult result = (OkObjectResult) response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
