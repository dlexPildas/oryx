using OryxDomain.Models.Oryx;
using System;

namespace OryxDomain.Models
{
    public class OryxAuthenticatedModel
    {
        public DC4 Dc4 { get; set; }
        public PD0 Pd0 { get; set; }
        public DateTime Exp { get; set; }
        public string Token { get; set; }
        public DateTime Iat { get; set; }
    }
}
