using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum VolumeType
    {
        [Description("Nenhum")]
        NONE = 0,
        [Description("Avulso")]
        IN_BULK = 1,
        [Description("Caixa")]
        BOX = 2,
        [Description("Volume")]
        VOLUME = 3
    }
}
