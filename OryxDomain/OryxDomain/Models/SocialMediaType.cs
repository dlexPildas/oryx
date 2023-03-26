using System.ComponentModel;

namespace OryxDomain.Models
{
    public enum SocialMediaType
    {
        [Description("mdi-facebook")]
        Facebook = 1,
        [Description("mdi-twitter")]
        Twitter = 2,
        [Description("mdi-linkedin")]
        Linkedin = 3,
        [Description("mdi-instagram")]
        Instagram = 4
    }
}
