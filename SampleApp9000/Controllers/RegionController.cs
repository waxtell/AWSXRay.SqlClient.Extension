using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SampleApp9000.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegionController : ControllerBase
    {
        private readonly ILogger<RegionController> _logger;

        public RegionController(ILogger<RegionController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Please note this is not to demonstrate best practices, but rather
        /// to demonstrate basic functionality
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Region> Get()
        {
            using var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            return connection.Query<Region>("SELECT RegionID, RegionDescription FROM [Northwind].[dbo].[Region]");
        }
    }
}
