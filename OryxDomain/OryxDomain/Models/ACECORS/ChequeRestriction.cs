using System;

namespace OryxDomain.Models.ACECORS
{
    public class ChequeRestriction
    {
        public int Quantidade { get; set; }
        public DateTime UltimaOcorrencia { get; set; }
        public string Valor { get; set; }
    }
}
