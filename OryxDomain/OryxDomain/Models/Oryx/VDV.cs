using System;

namespace OryxDomain.Models.Oryx
{
    public class VDV
    {
        public string Vdvvolume { get; set; }
        public string Vdvpeca { get; set; }
        public decimal Vdvpreco { get; set; }
        public DateTime Vdvleitura { get; set; }
        public string ReadingDate
        {
            get
            {
                return this.Vdvleitura.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public decimal Vdvqtde { get; set; }
        public string Vdvproduto { get; set; }
        public string Vdvopcao { get; set; }
        public string Vdvtamanho { get; set; }
        public string Pr0desc { get; set; }
        public string Cr1nome { get; set; }
    }
}
