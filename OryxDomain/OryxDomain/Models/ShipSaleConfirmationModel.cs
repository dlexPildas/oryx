using System;

namespace OryxDomain.Models
{
    public class ShipSaleConfirmationModel
    {
        public bool Selected { get; set; }
        public string Vd6embarq { get; set; }
        public DateTime Vd6abert { get; set; }
        public string OpeningDate
        {
            get
            {
                return this.Vd6abert.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public int QtyItems { get; set; }
        public decimal Amount { get; set; }
    }
}
