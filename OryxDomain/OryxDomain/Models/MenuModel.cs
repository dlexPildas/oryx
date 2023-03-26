using OryxDomain.Models.Oryx;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class MenuModel
    {
        public IList<CO0> Collections { get; set; }
        public IList<PRS> Groups { get; set; }
    }
}
