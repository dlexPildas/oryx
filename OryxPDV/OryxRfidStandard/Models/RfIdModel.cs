using System;
using System.Collections.Generic;
using System.Text;

namespace OryxRfidStandard.Models
{
    public class RfIdModel
    {
        //public DateTime dt { get; set; }
        public int cod_antena { get; set; }
        public int cod_portal { get; set; }
        public string tag { get; set; }
        public int rssi { get; set; }
        public string alias_portal { get; set; }
    }
}
