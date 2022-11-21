using System.ComponentModel.DataAnnotations;

namespace API_EventFest.Models {
    public class Localizacao {

        [Range(-90.0, 90.0, ErrorMessage = "A latitude deve respeitar o intervalo de (-90.0 -> 90.0)")]
        public decimal Latitude { get; set; }
        [Range(-180.0, 180.0, ErrorMessage = "A longitude deve respeitar o intervalo de (-180.0 -> 180.0)")]
        public decimal Longitude { get; set; }
    }
}
