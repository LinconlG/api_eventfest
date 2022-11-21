using Microsoft.AspNetCore.Mvc;
using API_EventFest.Models;

namespace API_EventFest.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EventosController : ControllerBase {



        [HttpPost]
        public async Task<ActionResult<Evento>> Get() {

            var evento = new Evento();

            evento.Nome = "Stereo Pink Felguk";
            evento.Descricao = "CONFIRMADOS:\r\n\r\n@felguk, nosso headliner da noite vem com força total prometendo uma noite mágica para os amantes da música eletrônica. \r\n\r\nE junto a ele um time de peso!  @piratesnakemusic, @dommusicone, @diegobrunelli B2B @eduardotorrentes, @jessbenevides e @jotta.sounds .";
            evento.Classificao = 18;
            evento.Organizador = "Pink House";
            evento.Data = new DateTime(2022, 11, 18, 23, 00, 00);

            return Ok(evento);
        }
    }
}
