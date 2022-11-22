namespace API_EventFest.Models {
    public class Foto {
        public int FotoID { get; set; }
        public string Url { get; set; }
        public IFormFile arquivo { get; set; }
    }

}
