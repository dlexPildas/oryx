using System;
using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class CO1
    {
        public string Co1fecha { get; set; }
        public DateTime Co1abert { get; set; }
        public string ClosingDate
        {
            get
            {
                return this.Co1abert.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public string Co1repres { get; set; }
        public string Cf6nome { get; set; }
        public DateTime Co1inicio { get; set; }
        public string InitDate
        {
            get
            {
                return this.Co1inicio.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public DateTime Co1fim { get; set; }
        public string EndDate
        {
            get
            {
                return this.Co1fim.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string Co1usuario { get; set; }
        public IList<CO2> LstCo2 { get; set; }
    }
}
