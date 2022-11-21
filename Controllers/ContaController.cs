using API_EventFest.Models;
using Microsoft.AspNetCore.Mvc;
using API_EventFest.Mappers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_EventFest.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ContaController : ControllerBase {

        private readonly ContaMapper _contaMapper;

        public ContaController(ContaMapper contaMapper) { 
            _contaMapper = contaMapper;
        }
        
        [HttpPost("criar")]
        public async Task<ActionResult> Post(
            [FromBody] Conta conta
        ){
            try {

                await _contaMapper.CriarConta(conta);

                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

    }
}
