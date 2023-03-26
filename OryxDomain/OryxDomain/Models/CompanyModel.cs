using OryxDomain.Models.Oryx;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class CompanyModel
    {
        public string Lx0cliente { get; set; }
        public string Cf1fant { get; set; }
        public string Cf1nome { get; set; }
        public string B2bwhats { get; set; }
        public IList<B2M> SocialMedias { get; set; }
        public string B2dsobre { get; set; }
        public string B2ddescri { get; set; }
        public IList<ContactModel> Contacts { get; set; }
        public GeoLocationModel GeoLocation { get; set; }
    }
}
