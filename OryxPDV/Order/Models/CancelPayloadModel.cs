using OryxDomain.Models;
using System.Collections.Generic;

namespace Order.Models
{
    public class CancelPayloadModel
    {
        public IList<Vd7CartModel> LstVd7 { get; set; }
        public string Cf1cliente { get; set; }
        public bool Resale { get; set; }
    }
}
