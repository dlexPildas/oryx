using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class GR0
    {
        public string Gr0desc { get; set; }
        public string Gr0grade { get; set; }
        public IList<GR1> LstGr1 { get; set; }
    }
}
