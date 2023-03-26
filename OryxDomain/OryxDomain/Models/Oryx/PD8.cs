using OryxDomain.Models.Enums;
using OryxDomain.Utilities;

namespace OryxDomain.Models.Oryx
{
    public class PD8
    {
        public string Pd8codigo { get; set; }
        public string Pd8nome { get; set; }
        public string Pd8baseurl { get; set; }
        public string Pd8token { get; set; }
        public IntegrationRepresentativeType Pd8tipo { get; set; }
        public string Pd8tipoDesc { get { return this.Pd8tipo.GetEnumDescription(); } }
        public bool Pd8flag1 { get; set; }
        public bool Pd8flag2 { get; set; }
        public bool Pd8flag3 { get; set; }
    }
}
