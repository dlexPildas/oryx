using OryxDomain.Models.Oryx;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class ManagmentProductsModel
    {
        public IList<PR0> Products { get; set; }
        public bool HasNext { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
    }
}
