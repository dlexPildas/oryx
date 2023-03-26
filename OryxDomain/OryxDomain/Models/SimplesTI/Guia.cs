using System.Xml.Serialization;

namespace OryxDomain.Models.TecDataSoft
{

    [XmlRoot(ElementName = "guia")]
    public class Guia
    {
        [XmlElement(ElementName = "cd_guia")]
        public string CdGuia { get; set; }

        [XmlElement(ElementName = "nm_guia")]
        public string NmGuia { get; set; }

        [XmlElement(ElementName = "nr_cpf_cnpj")]
        public string NrCpfCnpj { get; set; }
        [XmlElement(ElementName = "nr_inscricao_estadual")]
        public string NrInscricaoEstadual { get; set; }
        [XmlElement(ElementName = "nr_cep")]
        public string NrCep { get; set; }
        [XmlElement(ElementName = "nm_cidade")]
        public string NmCidade { get; set; }
        [XmlElement(ElementName = "nm_estado")]
        public string NmEstado { get; set; }
        [XmlElement(ElementName = "cd_ibge")]
        public string CdIbge { get; set; }
        [XmlElement(ElementName = "ds_endereco")]
        public string DsEndereco { get; set; }
        [XmlElement(ElementName = "nr_endereco")]
        public string NrEndereco { get; set; }
        [XmlElement(ElementName = "nm_bairro")]
        public string NmBairro { get; set; }
        [XmlElement(ElementName = "ds_complemento")]
        public string DsComplemento { get; set; }
        [XmlElement(ElementName = "telefone_celular")]
        public string TelefoneCelular { get; set; }
        [XmlElement(ElementName = "email")]
        public string Email { get; set; }
        [XmlElement(ElementName = "nr_sindicato")]
        public string NrSindicato { get; set; }
    }

    [XmlRoot(ElementName = "guias")]
    public class Guias
    {

        [XmlElement(ElementName = "guia")]
        public Guia Guia { get; set; }
    }
}
