using System;

namespace OryxDomain.Models.Oryx
{
    public class VE2
    {
        public string Ve2meta { get; set; }
        public DateTime Ve2mes { get; set; }
        public string Ve2mesano
        { 
            get 
            {
                return this.Ve2mes.ToString("MM/yyyy");
            }
        }
        public decimal Ve2valor { get; set; }
        public string Ve2tipo { get; set; }
        public string Ve1nome { get; set; }
    }
}
