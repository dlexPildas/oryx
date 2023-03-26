using System;
using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class CX0
    {
        public string Cx0caixa { get; set; }
        public DateTime Cx0abert { get; set; }
        public string OpenDate
        {
            get
            {
                return this.Cx0abert.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public string Cx0usuabe { get; set; }
        public DateTime Cx0fecha { get; set; }
        public string CloseDate
        {
            get
            {
                return this.Cx0fecha.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public string Cx0usufec { get; set; }
        public decimal Cx0valini { get; set; }
        public string Cx0codigo { get; set; }
        public IList<CX1> LstCx1 { get; set; }
    }
}
