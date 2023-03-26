using OryxDomain.Models.Oryx;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class ProductModel
    {
        public string Pr0produto { get; set; }
        public string Pr2opcao { get; set; }
        public string Pr3tamanho { get; set; }
        public string Pr0desc { get; set; }
        public IList<B2I> LstB2i { get; set; }
        public string Cr1nome { get; set; }
        public string Gr1desc { get; set; }
        public string Pr0colecao { get; set; }
        public string Pr0etiq { get; set; }
        public string Pr0grupo { get; set; }
        public decimal Pr0preco { get; set; }
        public string Pr0ficha { get; set; }
        public IList<PR2> Pr2List { get; set; }
        public IList<PR3> Pr3List { get; set; }
        public IList<HomeProductModel> RelatedProducts { get; set; }
    }
}
