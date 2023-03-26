using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class ReckoningModel
    {
        public string Vdecliente { get; set; }
        public IList<SalesItemModel> Items { get; set; }
        public decimal Vdedescon { get; set; }
    }
}
