using System;
using System.Collections.Generic;

namespace OryxDomain.Models.IntegraSH.Sending
{
    public class SalesSendingModel : IntegraShSendModel
    {
        public SalesSendingModel() : base()
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
        public DateTime Abertura { get; set; }
        public string Contato { get; set; }
        public string Cpf_contato { get; set; }

        // [AGENCIA]
        public string CodigoAgencia { get; set; }

        //[PAGAMENTO]
        public decimal Total { get; set; }
        public string Numero { get; set; }
        public List<Parcela> Parcelas { get; set; }

    }
    public class Parcela
    {
        public string Banco { get; set; }
        public string Cheque { get; set; }
        public DateTime Venci { get; set; }
        public decimal Valor { get; set; }

    }

}
