using System;
using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class VD2
    {
        public string Vd2pedido { get; set; }
        public string Vd2produto { get; set; }
        public string Vd2etiq { get; set; }
        public DateTime Vd2entrega { get; set; }
        public string Vd2detalhe { get; set; }
        public float Vd2comis { get; set; }
        public IList<PR0> PR0 { get; set; }
    }
}
