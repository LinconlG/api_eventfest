using API_EventFest.Mappers;
using API_EventFest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Tracing;

namespace API_EventFest.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EventosController : ControllerBase {

        private readonly EventoMapper _eventoMapper;


        public EventosController(EventoMapper eventoMapper) {
            _eventoMapper = eventoMapper;
        }

        [HttpPost("criar")]
        public async Task<ActionResult> PostEvento(
        [FromForm] Evento evento) {
            try {



                await _eventoMapper.CriarEvento(evento);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("uploadFoto")]
        public async Task<ActionResult> PostImagem(
        [FromForm] Foto foto) {
            try {
                await _eventoMapper.UploadFoto(foto);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Evento>>> GetEventos(
        [FromQuery (Name = "eventoid")] int? eventoid) {
            try {
                var eventos = await _eventoMapper.FindEventos(eventoid);

                return Ok(eventos);
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        [HttpGet("EventoFoto")]
        public async Task<ActionResult> GetEventoFoto(
            [FromQuery] int eventoid) {

            try {
                var filepath = await _eventoMapper.FindFotoPath(eventoid);

                if (System.IO.File.Exists(filepath)) {
                    byte[] b = System.IO.File.ReadAllBytes(filepath);
                    return File(b, "image/jpg");
                }
                return null;
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        [HttpDelete("deletar")]
        public async Task<ActionResult> DeleteEvento(
        [FromQuery(Name = "eventoid")] int eventoid) {
            try {
                await _eventoMapper.DeletarEvento(eventoid);

                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }


        }
    }
}
