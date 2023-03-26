using OryxDomain.Models.Oryx;
using System;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class CustomerHistoryModel
    {
        public bool Selected { get; set; }
        public DateTime Dtabert { get; set; }
        public string HistoryDate
        {
            get
            {
                return this.Dtabert.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string Number { get; set; }
        public int Entsai { get; set; }
        public bool HasRomaneio { get; set; }
        public bool HasNF { get; set; }
        public bool Consignment { get; set; }
        public bool ConsignmentClosed { get; set; }
        public bool HasPendingRomaneio { get; set; }
        public bool HasPendingNF { get; set; }
        public bool IsSalesConfirmation { get; set; }
        public decimal Total { get; set; }
        public decimal InstallmentsAmount { get; set; }
        public IList<CustomerHistoryDocsModel> LstFiscalDocuments { get; set; }
        public IList<CV7> Items { get; set; }
    }
}
