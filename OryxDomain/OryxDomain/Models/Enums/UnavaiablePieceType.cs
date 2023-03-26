using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum UnavaiablePieceType
    {
        [Description("Nenhum")]
        NONE = 0,
        [Description("Ignorar")]
        IGNORE = 1,
        [Description("Confirmar Embarque")]
        CONFIRM_SHIP = 2,
        [Description("Rejeitar Embarque")]
        REJECT_SHIP = 3
    }
}
