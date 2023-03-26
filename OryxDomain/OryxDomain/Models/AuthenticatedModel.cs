using OryxDomain.Models.Oryx;
using System;

namespace OryxDomain.Models
{
    public class AuthenticatedModel
    {
        public bool Authenticated { get; set; }
        public CF1 Cf1 { get; set; }
        public DateTime Iat { get; set; }
        public DateTime Exp { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
