using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum PostageTagStatusType
    {
        [Description("Nenhum")]
        NONE = 0,
        [Description("Criada")]
        CREATED = 1,
        [Description("Pendente")]
        PENDING = 2,
        [Description("Paga")]
        PAYED = 3,
        [Description("Gerada")]
        GENERATED = 4,
        [Description("Impressa")]
        PRINTED = 5,
        [Description("Cancelada")]
        CANCELED = 6,
        [Description("Entregue")]
        DELIVERED = 7
    }
}
