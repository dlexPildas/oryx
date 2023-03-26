using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OryxDomain.Models.TecDataSoft
{

    [XmlRoot(ElementName = "parcela")]
    public class Parcela
    {

        [XmlElement(ElementName = "tipo")]
        public string Tipo { get; set; }

        [XmlElement(ElementName = "cmc7")]
        public string Cmc7 { get; set; }

        [XmlElement(ElementName = "data_vencimento")]
        public string DataVencimento { get; set; }

        [XmlElement(ElementName = "valor")]
        public decimal Valor { get; set; }
    }

    [XmlRoot(ElementName = "vendas")]
    public class Vendas
    {
        [XmlIgnore]
        public string CF1Cliente { get; set; }
        [XmlIgnore]
        public string CF6Repres { get; set; }
        [XmlIgnore]
        public DateTime DataVenda { get; set; }

        [XmlElement(ElementName = "parcela")]
        public List<Parcela> Parcela { get; set; }
    }


}
