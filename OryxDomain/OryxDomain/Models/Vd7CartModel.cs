using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class Vd7CartModel
    {
        public string Vd7volume { get; set; }
        public IList<SalesItemModel> Items { get; set; }
        public string Vd7embarq { get; set; }
        public IList<ItemsHeaderModel> Headers { get; set; }
    }
}
