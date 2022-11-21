namespace API_EventFest.Models {
    public class Ingresso {
        public int IngressoID { get; set; }
        public string TipoIngresso { get; set; } //ter opções pre definidas no FRONT e usuario pode add
        public Conta Comprador { get; set; }
        public Evento Evento { get; set; }
    }
}
