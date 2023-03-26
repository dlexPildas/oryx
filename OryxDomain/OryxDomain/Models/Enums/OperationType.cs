using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum OperationType
    {
        [Description("Estadual")]
        STATE = 0,
        [Description("Interestadual")]
        INTERSTATE = 1,
    }
}
