using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum ImplantedPiecesType
    {
        [Description("Nenhum")]
        NONE = 0,
        [Description("Indiferente")]
        INDIFFERENT = 1,
        [Description("Somente Não Implantadas")]
        ONLY_NOT_IMPLANTED = 2,
        [Description("Somente Implantadas")]
        ONLY_IMPLANTED = 3
    }
}
