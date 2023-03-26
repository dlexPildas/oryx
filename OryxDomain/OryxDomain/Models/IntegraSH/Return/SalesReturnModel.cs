using System;
using System.Collections.Generic;
using System.Text;

namespace OryxDomain.Models.IntegraSH.Return
{
    public class SalesReturnModel : IntegraShReturnModel
    {
        public SalesReturnModel(): base()
        {
        }

        //[CLIENTE]
        public string Codigo { get; set; }
        public string Razao { get; set; }
        public string Fantasia { get; set; }
        public string End { get; set; }
        public string Nr_end { get; set; }
        public string Compl { get; set; }
        public string Bairro { get; set; }
        public string Ibge { get; set; }
        public string Municipio { get; set; }
        public string Cep { get; set; }
        public string Uf { get; set; }
        public string Fone { get; set; }
        public string Cel { get; set; }
        public string Email { get; set; }
        public string Ie { get; set; }
        public string Abertura { get; set; }
        public string Contato { get; set; }
        public string Cpf_contato { get; set; }

        //[AGENCIA]
        public string Codigoagencia { get; set; }

        //[PAGAMENTO]
        public string Total { get; set; }
        public string Numero { get; set; }
        public string Banco01 { get; set; }
        public string Cheque01 { get; set; }
        public string Venci01 { get; set; }
        public string Valor01 { get; set; }

        //[AUTORIZACAO]
        public string Numeroautorizacao { get; set; }
        public List<string> Autorizacoes { get; set; }

    }
}
