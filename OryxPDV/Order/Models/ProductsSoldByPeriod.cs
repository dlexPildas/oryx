using System;

namespace Order.Models
{
    public class ProductsSoldByPeriod
    {
        public DateTime FromCv5emissao { get; set; }
        public DateTime ToCv5emissao { get; set; }
        public string FromCv5tipo { get; set; }
        public string ToCv5tipo { get; set; }
        public string FromPr0colecao { get; set; }
        public string ToPr0colecao { get; set; }
        public string FromPr0etiq { get; set; }
        public string ToPr0etiq { get; set; }
        public string FromPr0familia { get; set; }
        public string ToPr0familia { get; set; }
        public string FromCv7codigo { get; set; }
        public string ToCv7codigo { get; set; }
        public string FromCv5cliente { get; set; }
        public string ToCv5cliente { get; set; }
        public bool DetailGrid { get; set; }
        public bool Cost { get; set; }
        public int People { get; set; }
        public int Products { get; set; }
        public int Order { get; set; }
        public string Cf3regiao { get; set; }
    }
}
