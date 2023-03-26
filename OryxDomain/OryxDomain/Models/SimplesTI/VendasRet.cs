using System.Collections.Generic;
using System.Xml.Serialization;

namespace OryxDomain.Models.TecDataSoft
{
    [XmlRoot(ElementName = "parcela")]
    public class ParcelaRet
    {

        [XmlElement(ElementName = "cd_venda")]
        public int CdVenda { get; set; }

        [XmlElement(ElementName = "cd_movimentacao")]
        public int CdMovimentacao { get; set; }

        [XmlElement(ElementName = "vl_movimentacao")]
        public double VlMovimentacao { get; set; }

        [XmlElement(ElementName = "tipo_pagamento")]
        public string TipoPagamento { get; set; }

        [XmlElement(ElementName = "banco")]
        public string Banco { get; set; }

        [XmlElement(ElementName = "agencia")]
        public string Agencia { get; set; }

        [XmlElement(ElementName = "conta")]
        public string Conta { get; set; }

        [XmlElement(ElementName = "nr_cheque")]
        public string NrCheque { get; set; }
    }

    [XmlRoot(ElementName = "venda")]
    public class VendaRet
    {

        [XmlElement(ElementName = "situacao")]
        public string Situacao { get; set; }

        [XmlElement(ElementName = "ds_situacao")]
        public string DsSituacao { get; set; }

        [XmlElement(ElementName = "codigo_venda")]
        public int CodigoVenda { get; set; }

        [XmlArray("parcelas")]
        [XmlArrayItem("parcela")]
        public List<ParcelaRet> Parcelas { get; set; }
    }

    [XmlRoot(ElementName = "vendas")]
    public class VendasRet
    {
        [XmlElement(ElementName = "venda")]
        public VendaRet Venda { get; set; }
    }


}
