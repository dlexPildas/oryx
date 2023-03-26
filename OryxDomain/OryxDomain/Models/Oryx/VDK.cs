using System;
using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class VDK
    {
        public string Vdkdoc { get; set; }
        public DateTime Vdkabert { get; set; }
        public string ReturnDate
        {
            get
            {
                return this.Vdkabert.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public string Vdkcliente { get; set; }
        public string Vdkmotivo { get; set; }
        public string Vdkusuario { get; set; }
        public bool Vdkmanter { get; set; }
        public string Vdkobserva { get; set; }
        public IList<VDL> LstVdl { get; set; }
        public IList<VDX> LstVdx { get; set; }
        public string Cf1nome { get; set; }
        public string Dc4nome { get; set; }
        public decimal Vdktotal { get; set; }

        public bool HasRomaneio { get; set; }
        public bool HasNF { get; set; }
        public bool HasPendingRomaneio { get; set; }
        public bool HasPendingNF { get; set; }
    }
}
