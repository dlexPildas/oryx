using System;

namespace OryxDomain.Models
{
    public class SearchConsigmentOrder
    {
        public string Vd1pedido { get; set; }
        public string Cf1nome { get; set; }
        public DateTime Vd1abert { get; set; }
        public string OrderDate
        {
            get
            {
                return this.Vd1abert.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public int QtyShips { get; set; }
        public int QtyItems { get; set; }
        public decimal Cv5total { get; set; }
        public decimal Vd1total { get; set; }
        public bool Closed { get; set; }
    }
}
