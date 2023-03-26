using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum VolumeLabelType
    {
        [Description("Nunca")]
        NONE = 0,
        [Description("Pré-venda")]
        PRESALES = 1,
        [Description("venda")]
        SALES = 2,
        [Description("Ambos")]
        BOTH = 3,
    }
}
