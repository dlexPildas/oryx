using OryxDomain.Models.Oryx;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class PayloadCustomerHistoryModel
    {
        public IList<CustomerHistoryModel> LstHistory { get; set; }
        public IList<CV1> LstFiscalDocTypes { get; set; }
        public decimal ConsignmentAmount { get; set; }
        public decimal SalesAmount { get; set; }
        public int QtySalesItems { get; set; }
        public int QtyConsignmentItems { get; set; }
    }
}
