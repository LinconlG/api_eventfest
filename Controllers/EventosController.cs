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
        [FromForm] FileUpload foto) {
            try {
                await _eventoMapper.UploadFoto(foto.file);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        [HttpPatch("editar")]
        public async Task<ActionResult> PatchEvento(
        [FromBody] Evento evento) {
            await _eventoMapper.EditarEvento(evento);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<Evento>>> GetEventos(
        [FromQuery(Name = "eventoid")] int? eventoid) {
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
        [FromQuery(Name = "eventoid")] int? eventoid = null,
        [FromQuery(Name = "fotoid")] int? fotoid = null) {

            try {
                string imagepath = await _eventoMapper.FindFotoPath(eventoid, fotoid);
                byte[] foto = System.IO.File.ReadAllBytes(imagepath);
                return File(foto, "image/jpg");
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
