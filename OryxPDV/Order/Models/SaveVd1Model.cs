using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using System.Collections.Generic;

namespace Order.Models
{
    public class SaveVd1Model
    {
        public VD1 Vd1 { get; set; }
        public IList<CV8> LstCv8 { get; set; }
        public IList<Vd7CartModel> LstVd7 { get; set; }
        public bool Transmit { get; set; }
        public bool Print { get; set; }
        public string Docfis { get; set; }
        public SaveReturnModel Vdkmodel { get; set; }
        public string ShipSequence { get; set; }
        public decimal QtyVolumeLabel { get; set; }
    }
}
