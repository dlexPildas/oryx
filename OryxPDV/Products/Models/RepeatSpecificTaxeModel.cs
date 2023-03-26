using System.Collections.Generic;

namespace Products.Models
{
    public class RepeatSpecificTaxeModel
    {
        public IList<string> CodeLst { get; set; }

        public string Cvnopercom { get; set; }
        public string Cvnproduto { get; set; }
        public string Cvninsumo { get; set; }
    }
}
