using OryxDomain.Models;
using System.Collections.Generic;

namespace Order.Models
{
    public class PayloadShipForSaleConfirmationModel
    {
        public string Cf1cliente { get; set; }
        public IList<ShipSaleConfirmationModel> LstShip { get; set; }
    }
}
