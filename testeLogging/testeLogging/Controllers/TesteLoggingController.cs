using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace testeLogging.Controllers
{
    [Route("Teste")]    
    public class TesteLoggingController : Controller
    {
        private readonly ILogger _logger;
        public TesteLoggingController(ILogger<TesteLoggingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("TesteLogging")]
        public async Task<IActionResult> TesteLogging()
        {
            _logger.LogInformation("Método chamado com sucesso!!!");
            _logger.LogDebug("Método chamado com sucesso!!!");
            _logger.LogWarning("Método chamado com sucesso!!!");
            _logger.LogError("Método chamado com sucesso!!!");
            _logger.LogCritical("Método chamado com sucesso!!!");

            return Ok();
        }

        [HttpGet("OutroTeste")]
        public void teste()
        {

        }
    }
}