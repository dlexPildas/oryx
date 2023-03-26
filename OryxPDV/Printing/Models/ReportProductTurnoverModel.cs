using System;

namespace Printing.Models
{
    public class ReportProductTurnoverModel
    {
        public DateTime FromCv5emissao { get; set; }
        public DateTime ToCv5emissao { get; set; }
        public string FromCv7codigo { get; set; }
        public string ToCv7codigo { get; set; }
        public string FromPr0colecao { get; set; }
        public string ToPr0colecao { get; set; }
        public string FromPr0etiq { get; set; }
        public string ToPr0etiq { get; set; }
        public string Cv5clinome { get; set; }
        public bool OnlySalesItems { get; set; }
        public int Entsai { get; set; }
    }
}
