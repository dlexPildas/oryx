using OryxDomain.Models.Enums;
using OryxDomain.Utilities;
using System;

namespace OryxDomain.Models.Oryx
{
    public class CX1
    {
        public string Cx1registr { get; set; }
        public string Cx1caixa { get; set; }
        public MovimentCashierType Cx1tipo { get; set; }
        public string Cx1tipoDesc { get { return this.Cx1tipo.GetEnumDescription(); } }
        public decimal Cx1valor { get; set; }
        public DateTime Cx1data { get; set; }
        public string FlowDate
        {
            get
            {
                return this.Cx1data.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public CashierFlowType Cx1entsai { get; set; }
        public string Cx1entsaiDesc { get { return this.Cx1entsai.GetEnumDescription(); } }
    }
}
