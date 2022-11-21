using API_EventFest.Models.Enum;

namespace API_EventFest.Models {
    public class Evento {
        public int EventoID { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public string Endereco { get; set; }
        public Foto Foto { get; set; } = new Foto();
        public Classificacao Classificao { get; set; }
        public bool IsLivre { get; set; } // ***
        public decimal Preco { get; set; }
        public decimal Taxa { get; set; }
        public int QuantidadeIngressos { get; set; }
        public string Organizador { get; set; }
        public List<string> TiposIngresso { get; set; }
        public List<Ingresso> Ingressos { get; set; } = new List<Ingresso>();
    }
}
