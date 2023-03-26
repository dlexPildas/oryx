using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class VD7
    {
        public string Vd7pedido { get; set; }
        public string Vd7embarq { get; set; }
        public string Vd7volume { get; set; }
        public IList<VD8> LstVd8 { get; set; }
        public IList<VDV> LstVdv { get; set; }
    }
}
