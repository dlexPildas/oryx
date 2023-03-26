using OryxDomain.Utilities;
using OryxDomain.Models.Enums;

namespace OryxDomain.Models.Oryx
{
    public class PD2
    {
        public int Pd2codigo { get; set; }
        public SalesOperationType Pd2tipoope { get; set; }
        public string Pd2tipoopeDesc { get { return this.Pd2tipoope.GetEnumDescription(); } }
        public string Pd2tipo { get; set; }
        public string Cv1nome { get; set; }
        public OperationType Pd2estadua { get; set; }
        public string Pd2estaduaDesc { get { return this.Pd2estadua.GetEnumDescription(); } }
        public bool Pd2contrib { get; set; }
        public string Pd2contribDesc { get { return this.Pd2contrib ? "Sim" : "Não"; } }
        public bool Pd2emispro { get; set; }
        public string Pd2emisproDesc { get { return this.Pd2emispro ? "Sim" : "Não"; } }
        public string Pd2opercom { get; set; }
        public string Cv3nome { get; set; }
    }
}
