using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum FreightType
    {
        [Description("Nenhum")]
        NONE = 0,
        [Description("Emitente")]
        ISSUER = 1,
        [Description("Destinatário")]
        RECEIVER = 2,
        [Description("Terceiro")]
        THIRD_PARTIES = 3,
        [Description("Sem frete")]
        NO_SHIPPING = 4
    }
}
