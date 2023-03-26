using System;
using System.Collections.Generic;
using System.Text;

namespace OryxDomain.Models.IntegraSH.Return
{
    public class ClientReturnModel : IntegraShReturnModel
    {
        public ClientReturnModel() : base()
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
        public DateTime Dt_nascimento { get; set; }
        public string Bloqueado { get; set; }
        public string Inativo { get; set; }
        public string Limite { get; set; }
        public string Obs { get; set; }
        public string Inf_banco { get; set; }
        public string Agencia { get; set; }
    }
}
