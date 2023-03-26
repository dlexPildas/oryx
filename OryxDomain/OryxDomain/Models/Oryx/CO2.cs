using System;

namespace OryxDomain.Models.Oryx
{
    public class CO2
    {
        public string Co2fecha { get; set; }
        public string Co2doc { get; set; }
        public string Co2tipo { get; set; }
        public string Co2emissor { get; set; }
        public DateTime Cv5emissao { get; set; }
        public string Cv5clinome { get; set; }
        public string IssueDate
        {
            get
            {
                return this.Cv5emissao.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public decimal Cv5totalnf { get; set; }
        public decimal Cv5comis { get; set; }
        public decimal CommissionValue { get; set; }
    }
}
