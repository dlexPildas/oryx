using System.Xml.Serialization;

namespace OryxDomain.Models.NfeModels
{
    [XmlRoot("nfeProc", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class NfeProc
    {
        [XmlElement("protNFe")]
        public ProtNFe ProtNFe { get; set; }
    }
}
