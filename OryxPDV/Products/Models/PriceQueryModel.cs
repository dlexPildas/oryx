using OryxDomain.Models;
using System.Collections.Generic;

namespace Products.Models
{
    public class PriceQueryModel
    {
        public string Pr0produto { get; set; }
        public string Pr2opcao { get; set; }
        public string Pr3tamanho { get; set; }
        public decimal Pr0pesoliq { get; set; }
        public decimal Pr0pesobru { get; set; }
        public decimal Vd5preco { get; set; }
        public IList<PriceModel> Prices { get; set; }
        public decimal Stock { get; set; }
        public IList<VariantModel> Variants { get; set; }
    }
}
