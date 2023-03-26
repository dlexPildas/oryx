using System;
using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class VD6
    {
        public string Vd6pedido { get; set; }
        public string Vd6embarq { get; set; }
        public string Vd6usuario { get; set; }
        public DateTime Vd6abert { get; set; }
        public string OpeningDate
        {
            get
            {
                return this.Vd6abert.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public DateTime Vd6fecha { get; set; }
        public string ClosingDate
        {
            get
            {
                return this.Vd6fecha.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public string Vd6docconf { get; set; }
        public IList<VD7> LstVd7 { get; set; }
    }
}
