using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum MallIntegrationType
    {
        [Description("Nenhum")]
        None = 0,
        [Description("Simples TI")]
        SimplesTI = 1,
        [Description("Integra SH")]
        IntegraSH = 2,
        [Description("Ezetech")]
        Ezetech = 3
    }
}
