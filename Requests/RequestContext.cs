using Microsoft.AspNetCore.Mvc;

namespace API_EventFest.Requests {
    public class RequestContext : RequestBase {

        public RequestContext(IConfiguration configuration) : base(configuration) { }

        public async Task<ActionResult> GetEventoFoto(int? eventoid = null, int? fotoid = null, string imagepath = null) {

            var parametros = new Dictionary<string, object> {
                {"eventoid", eventoid },
                {"fotoid", fotoid },
                {"imagepath", imagepath }
            };

            var request = Get(_apiEventFestUrl + "/v1/evento/EventoFoto", parametros);
            var response = await _httpClient.SendAsync(request);

            return await response.GetBody<ActionResult>();
        }
    }
}
