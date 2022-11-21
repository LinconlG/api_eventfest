using API_EventFest.Mappers;
using API_EventFest.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_EventFest.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase {

        private readonly EventoMapper _eventoMapper;

        public EventoController(EventoMapper eventoMapper) {
            _eventoMapper = eventoMapper;
        }

        [HttpPost("criar")]
        public async Task<ActionResult> Post(
        [FromBody] Evento evento
) {
            try {

                await _eventoMapper.CriarEvento(evento);

                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }
    }
}
