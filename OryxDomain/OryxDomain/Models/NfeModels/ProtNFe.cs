using System.Xml.Serialization;

namespace OryxDomain.Models.NfeModels
{
    [XmlRoot("protNFe")]
    public class ProtNFe
    {
        [XmlElement("infProt")]
        public InfProt InfProt { get; set; }
    }
}
