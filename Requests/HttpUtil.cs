using System.Text.Json;

namespace API_EventFest.Requests {
    internal static class HttpUtil {

        internal async static Task<T> GetBody<T>(this HttpResponseMessage response) {
            var rawBody = await response.Content.ReadAsStringAsync();
            if (rawBody == "") {
                return default;
            }
            return JsonSerializer.Deserialize<T>(rawBody, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }
    }
}
