using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum OryxModuleType
    {
        [Description("Oryx Gestão")]
        ORYX_GESTAO= 0,
        [Description("Oryx Ponto de Venda")]
        ORYX_PV = 1,
        [Description("Oryx Logística")]
        ORYX_LOGISTICA = 2,
        [Description("Oryx Place")]
        ORYX_PLACE = 3,
        [Description("Oryx Memories")]
        ORYX_MEMORIES = 4,
        [Description("Oryx Esquadrias")]
        ORYX_ESQUADRIAS = 5,
    }
}
