using Microsoft.AspNetCore.Mvc;
using API_EventFest.Requests;

namespace API_EventFest.Models {
    public class Foto {
        
        public int FotoID { get; set; }
        public string Url { get; set; }
        public byte[] arquivo { get; set; }

        public Foto() { }

        public Foto(string imagePath, int? fotoid = null, string url = null) {

            FotoID = fotoid == null ? 0 : (int)fotoid;
            Url = url;
            if (File.Exists(imagePath)) {
                arquivo = File.ReadAllBytes(imagePath);
            }
        }
    }

}
