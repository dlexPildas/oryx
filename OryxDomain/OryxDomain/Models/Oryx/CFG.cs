namespace OryxDomain.Models.Oryx
{
    public class CFG
    {
        public CFG()
        {
        }

        public CFG(string cfgcliente, string cfgcnae, string cfgativida)
        {
            Cfgcliente = cfgcliente;
            Cfgcnae = cfgcnae;
            Cfgativida = cfgativida;
        }

        public string Cfgcliente { set; get; }
        public string Cfgcnae { set; get; }
        public string Cfgativida { set; get; }
    }
}
