using OryxDomain.Models.Oryx;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class SearchModel
    {
        public IList<CO0> Collections { get; set; }
        public IList<PRS> Groups { get; set; }
        public IList<PR2> Colors { get; set; }
        public IList<PR3> Sizes { get; set; }
        public IList<HomeProductModel> Products { get; set; }
    }
}
