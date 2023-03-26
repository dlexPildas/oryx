using System.Collections.Generic;

namespace OryxDomain.Models.ACECORS
{
    public class PayloadListPassengers
    {
        public Header Cabecalho { get; set; }
        public Request Solicitacao { get; set; }
        public int TotalPassageiros { get; set; }
        public IList<Passenger> Passageiros { get; set; }
    }
}
