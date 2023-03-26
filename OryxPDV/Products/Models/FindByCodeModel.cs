using OryxDomain.Models;
using System.Collections.Generic;

namespace Products.Models
{
    public class FindByCodeModel
    {
        public string Product { get; set;}
        public string List { get; set; }
        public string Volume { get; set; }
        public string Pedido { get; set; }
        public IList<SalesItemModel> LstVd8 { get; set; }
    }
}
