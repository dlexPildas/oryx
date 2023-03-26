using System;
using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class VDE
    {
        #region Oryx Gestão
        public string Vdedoc { get; set; }
        public string Vdecliente { get; set; }
        public string Cf1nome { get; set; }
        public DateTime? Vdefecha { get; set; }
        public string ClosingDate
        {
            get
            {
                if (this.Vdefecha.HasValue)
                {
                    return this.Vdefecha.Value.ToString("dd/MM/yyyy HH:mm");
                }
                return string.Empty;
            }
        }
        public string Vdeobserva { get; set; }
        public string Vdeid { get; set; }
        public string Vdeopercom { get; set; }
        public decimal Vdedescfat { get; set; }
        public bool Invoiced { get; set; }
        public string OrderInvoiced { get; set; }
        #endregion

        #region Oryx Esquadrias
        public string Vdepedido { get; set; }
        public string Vdeembarq { get; set; }
        public string Vdeinsumo { get; set; }
        public string Vdecor { get; set; }
        public string Vdetamins { get; set; }
        public decimal Vdeqtde { get; set; }
        #endregion

        public IList<VDF> LstVdf { get; set; }
        public IList<VDZ> LstVdz { get; set; }
    }
}
