using OryxDomain.Models;
using System.Collections.Generic;

namespace Products.Models
{
    public class FindReturnModel
    {
        public string Product { get; set; }
        public string Option { get; set; }
        public string Size { get; set; }
        public IList<ReturnItemModel> LstItems { get; set; }
        public string Cf1cliente { get; set; }
        public bool Consigned { get; set; }
        public int Qty { get; set; }
        public bool Input { get; set; }
    }
}
