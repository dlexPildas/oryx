using OryxDomain.Utilities;
using System;

namespace OryxDomain.Models.Oryx
{
    public class B2M
    {
        public string Icon 
        { 
            get
            {
                return Formatters.GetEnumDescription(this.Tipo);
            }
        }
        public SocialMediaType Tipo 
        {
            get
            {
                return (SocialMediaType)Convert.ToInt64(this.B2mtipo);
            }
        }
        public string B2mtipo { get; set; }
        public string B2mlink { get; set; }
    }
}
