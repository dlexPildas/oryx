using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum CashierFlowType
    {
        [Description("Nenhum")]
        NONE = 0,
        [Description("Saída")]
        EXIT = 1,
        [Description("Entrada")]
        ENTRY = 2
    }
}
