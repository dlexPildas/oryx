using System;
using System.Xml.Serialization;

namespace OryxDomain.Models.NfeModels
{
    public class InfProt
    {
        [XmlElement("chNFe")]
        public string ChNFe { get; set; }
        [XmlElement("dhRecbto")]
        public DateTime DhRecbto { get; set; }
        [XmlElement("nProt")]
        public string NProt { get; set; }
        [XmlElement("digVal")]
        public string DigVal { get; set; }
        [XmlElement("cStat")]
        public string CStat { get; set; }
        [XmlElement("xMotivo")]
        public string XMotivo { get; set; }
    }
}
