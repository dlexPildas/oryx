using OryxDomain.Models.Enums;
using System;

namespace OryxDomain.Models.Oryx
{
    public class LX2
    {
        public string Lx2padrao { get; set; }
        public DateTime Lx2entrega { get; set; }
        public ImplantedPiecesType Lx2implant { get; set; }
        public DateTime Lx2entreg2 { get; set; }
        public bool Lx2implan2 { get; set; }
        public bool Lx2naoconf { get; set; }
        public bool Lx2cativa { get; set; }
        public bool Lx2opcoes { get; set; }
        public bool Lx2destina { get; set; }
        public bool Lx2preco { get; set; }
        public UnavaiablePieceType Lx2pronta { get; set; }
        public bool Lx2outros { get; set; }
        public string Lx2script { get; set; }
        public bool Lx2etiq { get; set; }
        public decimal Lx2diasdev { get; set; }
        public bool Lx2vencons { get; set; }
        public bool Lx2estdev { get; set; }
        public bool Lx2debest { get; set; }
        public string Lx2cliest { get; set; }
        public string StockName { get; set; }
    }
}
