using System.Collections.Generic;

namespace Order.Models
{
    public class PayloadOrderModel
    {
        public string Message { get; set; }
        public string CodAuthSalesMall { get; set; }
        public bool IsError { get; set; }
        public bool Billed { get; set; }
        public string Vdkdoc { get; set; }
        public string ShipSequence { get; set; }
        public IList<string> RelatsPath { get; set; }
    }
}
