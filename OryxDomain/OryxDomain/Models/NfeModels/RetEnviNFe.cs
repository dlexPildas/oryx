using System;
using System.Xml.Serialization;

namespace OryxDomain.Models.NfeModels
{
    [XmlRoot("retEnviNFe", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class RetEnviNFe
    {
        [XmlElement("protNFe")]
        public ProtNFe ProtNFe { get; set; }
        [XmlElement("xmotivo")]
        public string Xmotivo { get; set; }
        [XmlElement("tpAmb")]
        public int TpAmb { get; set; }
        [XmlElement("verAplic")]
        public string VerAplic { get; set; }
        [XmlElement("cStat")]
        public string CStat { get; set; }
        [XmlElement("dhRecbto")]
        public DateTime DhRecbto { get; set; }
    }
}
