using System;

namespace OryxDomain.Models.IntegraSH.Return
{
    public class AgencyReturnModel : IntegraShReturnModel
    {
        public AgencyReturnModel(): base()
        {
        }

        //[AGENCIA]
        public string Codigo { get; set; }
        public string Nome { get; set; }
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

    }
}
