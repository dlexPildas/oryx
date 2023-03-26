namespace OryxDomain.Models.Oryx
{
    public class VE0
    {
        public string Ve0vend { get; set; }
        public string Ve0cliente { get; set; }
        public string Cf1nome { get; set; }
        public bool Ve0ativo { get; set; }
        public string Activated
        {
            get
            {
                return this.Ve0ativo ? "Sim" : "Não";
            }
        }
        public decimal Ve0comis { get; set; }
    }
}
