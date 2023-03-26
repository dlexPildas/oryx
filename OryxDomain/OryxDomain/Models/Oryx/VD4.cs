using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class VD4
    {
        public string Vd4conpgto { get; set; }
        public string Vd4nome { get; set; }
        public string Vd4titulo { get; set; }
        public bool Vd4mensal { get; set; }
        public int Vd4nmeses { get; set; }
        public bool Vd4foracat { get; set; }
        public decimal Vd4limdesc { get; set; }
        public IList<VD9> LstVd9 { get; set; }
    }
}
