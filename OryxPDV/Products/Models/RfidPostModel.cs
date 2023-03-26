using OryxDomain.Models;
using System.Collections.Generic;

namespace Products.Models
{
    public class RfidPostModel
    {
        public IList<SalesItemModel> LstSalesItems { get; set; }
        public IList<ReturnItemModel> LstReturnItems { get; set; }
    }
}
