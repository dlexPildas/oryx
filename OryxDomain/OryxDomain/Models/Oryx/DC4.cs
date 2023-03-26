using OryxDomain.Models.Enums;
using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class DC4
    {
        public string Dc4usuario { get; set; }
        public string Dc4nome { get; set; }
        public string Dc4senha { get; set; }
        public string Pd0codigo { get; set; }
        public IList<DC5> LstDc5 { get; set; }
        public IList<DC9> LstDc9 { get; set; }
        public string Lx9acesso { get; set; }
        public string Company { get; set; }
        public OryxModuleType Module { get; set; }
    }
}
