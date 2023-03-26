using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OryxDomain.Models.TecDataSoft
{


    [XmlRoot(ElementName = "dado_bancario")]
    public class DadoBancario
    {

        [XmlElement(ElementName = "banco")]
        public string Banco { get; set; }

        [XmlElement(ElementName = "agencia")]
        public string Agencia { get; set; }

        [XmlElement(ElementName = "conta")]
        public string Conta { get; set; }
    }

    [XmlRoot(ElementName = "dados_bancarios")]
    public class DadosBancarios
    {

        [XmlElement(ElementName = "dado_bancario")]
        public List<DadoBancario> DadoBancario { get; set; }
    }

    [XmlRoot(ElementName = "cliente")]
    public class Cliente
    {

        [XmlElement(ElementName = "situacao")]
        public string Situacao { get; set; }

        [XmlElement(ElementName = "ds_situacao")]
        public string DsSituacao { get; set; }

        [XmlElement(ElementName = "cd_cliente")]
        public int CdCliente { get; set; }

        [XmlElement(ElementName = "nm_cliente")]
        public string NmCliente { get; set; }

        [XmlElement(ElementName = "fl_liberado_compra_cheque")]
        public string FlLiberadoCompraCheque { get; set; }

        [XmlElement(ElementName = "fl_bloqueado_para_compra")]
        public string FlBloqueadoParaCompra { get; set; }

        [XmlElement(ElementName = "nr_cpf_cnpj")]
        public string NrCpfCnpj { get; set; }

        [XmlElement(ElementName = "nr_inscricao_estadual")]
        public string NrInscricaoEstadual { get; set; }

        [XmlElement(ElementName = "ds_identidade")]
        public string DsIdentidade { get; set; }

        [XmlElement(ElementName = "ds_orgao_emissor")]
        public string DsOrgaoEmissor { get; set; }

        [XmlElement(ElementName = "fl_sexo")]
        public string FlSexo { get; set; }

        [XmlElement(ElementName = "cd_estado_civil")]
        public int CdEstadoCivil { get; set; }

        [XmlElement(ElementName = "nm_razao_social")]
        public string NmRazaoSocial { get; set; }

        [XmlElement(ElementName = "dt_nascimento")]
        public string DtNascimento { get; set; }

        [XmlElement(ElementName = "nr_cep")]
        public string NrCep { get; set; }

        [XmlElement(ElementName = "nm_cidade")]
        public string NmCidade { get; set; }

        [XmlElement(ElementName = "nm_estado")]
        public string NmEstado { get; set; }

        [XmlElement(ElementName = "cd_ibge")]
        public int CdIbge { get; set; }

        [XmlElement(ElementName = "ds_endereco")]
        public string DsEndereco { get; set; }

        [XmlElement(ElementName = "nr_endereco")]
        public string NrEndereco { get; set; }

        [XmlElement(ElementName = "nm_bairro")]
        public string NmBairro { get; set; }

        [XmlElement(ElementName = "ds_complemento")]
        public string DsComplemento { get; set; }

        [XmlElement(ElementName = "telefone_residencial")]
        public string TelefoneResidencial { get; set; }

        [XmlElement(ElementName = "telefone_celular")]
        public string TelefoneCelular { get; set; }

        [XmlElement(ElementName = "fax")]
        public string Fax { get; set; }

        [XmlElement(ElementName = "email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "outros")]
        public string Outros { get; set; }

        [XmlElement(ElementName = "saldo")]
        public double Saldo { get; set; }

        [XmlElement(ElementName = "cd_guia")]
        public string CdGuia { get; set; }

        [XmlElement(ElementName = "nm_guia")]
        public string NmGuia { get; set; }

        [XmlElement(ElementName = "nr_sindicato")]
        public string NrSindicato { get; set; }

        [XmlElement(ElementName = "dados_bancarios")]
        public DadosBancarios DadosBancarios { get; set; }
    }

    [XmlRoot(ElementName = "clientes")]
    public class Clientes
    {

        [XmlElement(ElementName = "cliente")]
        public Cliente Cliente { get; set; }
    }


}
