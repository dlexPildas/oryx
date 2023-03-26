using System;
using System.Collections.Generic;

namespace OryxDomain.Models.Ezetech.Sending
{
    public class SalesSendingModel
    {
        public Sales Venda { get; set; }
       
        public IDictionary<string, Object> Pagamento { get; set; }
    }
}
