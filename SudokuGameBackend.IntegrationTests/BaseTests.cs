using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.IntegrationTests
{
    public class BaseTests
    {
        public readonly TestServer _server;

        public BaseTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.Development.json");
                })
                .UseStartup<Startup>());
        }
    }
}
