using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class PD0
    {
        public string Pd0codigo { get; set; }
        public string Pd0nome { get; set; }
        public string Pd0antena { get; set; }
        public string Pd0caminho { get; set; }
        public string Pd0host { get; set; }
        public IList<PD1> PrinterPreferences { get; set; }
    }
}
