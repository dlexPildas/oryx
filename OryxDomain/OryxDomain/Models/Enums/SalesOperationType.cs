using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum SalesOperationType
    {
        [Description("Nenhum")]
        None = 0,
        [Description("Venda")]
        Sales = 1,
        [Description("Devolução")]
        Return = 2,
        [Description("Devolução de consignado")]
        ConsignmentInput = 3,
        [Description("Saída em consignação")]
        ConsignmentOutPut = 4,
        [Description("Entrada")]
        Input = 5,
    }
}
