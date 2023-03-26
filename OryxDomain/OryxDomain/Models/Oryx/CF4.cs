using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class CF4
    {
        public string Cf4estado { get; set; }
        public string Cf4nome { get; set; }
        public IList<CF6> RelatedStores { get; set; }
    }
}
