using System;
using System.Collections.Generic;
using System.Text;

namespace OryxDomain.Models.IntegraSH
{
    public class IntegraShReturnModel
    {
        public string Loja { get; set; }
        public string Func { get; set; }
        public DateTime Data { get; set; }
        public DateTime Hora { get; set; }
        public string Software { get; set; }
        public string CodigoRetorno { get; set; }
        public string Descricao { get; set; }

    }
}
