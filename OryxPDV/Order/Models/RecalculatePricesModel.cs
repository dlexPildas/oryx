using OryxDomain.Models;
using System.Collections.Generic;

namespace Order.Models
{
    public class RecalculatePricesModel
    {
        public string List { get; set; }
        public IList<Vd7CartModel> LstVd7 { get; set; }
    }
}
