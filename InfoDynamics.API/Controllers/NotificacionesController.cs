using InfoDynamics.Aplicacion.servicios.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Employees.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionesController : ControllerBase
    {
        private readonly NotificacionHrsServicio _notificacionHoras;
        private readonly IConfiguration _configuration;

        public NotificacionesController(
            NotificacionHrsServicio notificacionHorasService,
            IConfiguration configuration)
        {
            _notificacionHoras = notificacionHorasService;
            _configuration = configuration;
        }

        [HttpPost("recordatorio-horas")]
        public async Task<IActionResult> EnviarRecordatorioHoras(
            [FromHeader(Name = "X-Cron-Api-Key")] string? apiKey)
        {
            var expectedApiKey = _configuration["CronJobs:ApiKey"];

            if (string.IsNullOrWhiteSpace(expectedApiKey))
            {
                return StatusCode(500, new
                {
                    mensaje = "CronJobs:ApiKey no está configurada."
                });
            }

            if (apiKey != expectedApiKey)
            {
                return Unauthorized(new
                {
                    mensaje = "Acceso denegado."
                });
            }

            var enviados = await _notificacionHoras.EnviarRecordatorioHorasAsync();

            return Ok(new
            {
                mensaje = "Recordatorio de horas ejecutado.",
                correosEnviados = enviados
            });
        }
    }
}