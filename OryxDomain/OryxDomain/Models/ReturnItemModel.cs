using OryxDomain.Models.Oryx;
using System;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class ReturnItemModel
    {
        public string Pr0imagem { get; set; }
        public string Pr0produto { get; set; }
        public string Pr0desc { get; set; }
        public string Pr2opcao { get; set; }
        public string Pr3tamanho { get; set; }
        public decimal Qtde { get; set; }
        public decimal Preco { get; set; }
        public decimal Total { get; set; }
        public string Eancodigo { get; set; }
        public string Of3peca { get; set; }
        public DateTime Leitura { get; set; }
        public string Doc { get; set; }
        public decimal Descon { get; set; }
        public decimal Precodesc { get; set; }
        public bool Gravou { get; set; }

        public string Vdedoc { get; set; }

        public string Cr1nome { get; set; }
        public string Gr1desc { get; set; }
        public IList<PR2> LstPr2 { get; set; }
        public IList<PR3> LstPr3 { get; set; }

        public string Volume { get; set; }

        //for unique pieces
        public string Cf1cliente { get; set; }
        public bool Consigned { get; set; }
        public ReturnTaxesModel Taxes { get; set; }
        public string Of3rfid { get; set; }

        public decimal Vdxqtdeent { get; set; }
        public string Vdxitem { get; set; }
    }
}
