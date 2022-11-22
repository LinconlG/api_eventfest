using Microsoft.AspNetCore.Mvc;

namespace API_EventFest.Models {
    public class Foto {
        public int FotoID { get; set; }
        public string Url { get; set; }
        public byte[] arquivo { get; set; }
        public FileContentResult foto { get; set; }

        public Foto() { }

        public Foto(string imagePath, int? fotoid = null, string url = null) {

            FotoID = fotoid == null ? 0 : (int)fotoid;
            Url = url;
            if (File.Exists(imagePath)) {
                arquivo = File.ReadAllBytes(imagePath);
                //criar context pra fazer o request aqui e a variavel FOTO receber o actionResult
            }
        }
    }

}
