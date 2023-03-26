using System;

namespace Order.Models
{
    public class SalesRankingModel
    {
        public DateTime FromCv5emissao { get; set; }
        public DateTime ToCv5emissao { get; set; }
        public string FromCv5tipo { get; set; }
        public string ToCv5tipo { get; set; }
        public string FromCv5repres { get; set; }
        public string ToCv5repres { get; set; }
        public string Cv5nomeloc { get; set; }
        public int Limit { get; set; }
        public int Group { get; set; }
    }
}
