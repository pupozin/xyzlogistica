using Microsoft.AspNetCore.Mvc;
using ZyxLogistics.Api.Database;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TestController(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpGet]
        public IActionResult Test()
        {
            using var connection = _connectionFactory.CreateConnection();

            connection.Open();

            return Ok("Banco conectado!");
        }
    }
}
