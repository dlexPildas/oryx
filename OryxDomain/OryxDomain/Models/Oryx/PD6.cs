using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class PD6
    {
        public string Pd6emissor { get; set; }
        public string Pd6agentep { get; set; }
        public bool Pd6cruzar { get; set; }
        public string Pd6impres { get; set; }
        public string Pd6nominal { get; set; }
        public IList<PD7> LstPd7 { get; set; }
        public int Pd6rotacao { get; set; }
    }
}
