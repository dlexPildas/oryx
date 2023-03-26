using OryxDomain.Models.Oryx;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class ProductCartModel
    {
        public ProductCartModel()
        {
        }

        public string Pr0produto { get; set; }
        public string Pr0desc { get; set; }
        public string Pr0imagem { get; set; }
        public decimal Pr0pesobru { get; set; }
        public decimal Pr0pesoliq { get; set; }

        public string Pr2opcao { get; set; }
        public string Cr1nome { get; set; }

        public string Pr3tamanho { get; set; }
        public string Gr1desc { get; set; }

        public decimal Vd5preco { get; set; }

        

        public string Eancodigo { get; set; }


        public string Of3peca { get; set; }
        public bool Of3naoconf { get; set; }

        public IList<PR2> LstPr2 { get; set; }
        public IList<PR3> LstPr3 { get; set; }
        public bool ConfIndis { get; set; }
        public string Of3rfid { get; set; }

        public decimal Stock { get; set; }
    }
}
