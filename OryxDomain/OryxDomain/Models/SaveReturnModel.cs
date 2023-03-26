using OryxDomain.Models.Oryx;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class SaveReturnModel
    {
        public VDK Vdk { get; set; }
        public IList<ReturnItemModel> LstItems { get; set; }
        public bool PrintReceipt { get; set; }
        public string Docfis { get; set; }
        public FiscalDocReferencedModel DocReturn { get; set; }
        public bool Emispro { get; set; }
    }
}
