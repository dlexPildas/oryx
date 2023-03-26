using System;

namespace Order.Models
{
    public class PrintReportSalesByPeriod
    {
        public DateTime FromCv5emissao { get; set; }
        public DateTime ToCv5emissao { get; set; }
        public string FromCv5tipo { get; set; }
        public string ToCv5tipo { get; set; }
        public string FromCv5repres { get; set; }
        public string ToCv5repres { get; set; }
        public string Cv5clinome { get; set; }
        public bool OnlyCashier { get; set; }
    }
}
