using System;

namespace OryxDomain.Models.Oryx
{
    public class VDZ
    {
        public string Vdzdoc { get; set; }
        public string Vdzpeca { get; set; }
        public string Vdzproduto { get; set; }
        public string Vdzopcao { get; set; }
        public string Vdztamanho { get; set; }
        public DateTime Vdzleitura { get; set; }
        public string ReadingDate
        {
            get
            {
                return this.Vdzleitura.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public decimal Vdzqtde { get; set; }
        public string Vdzvolume { get; set; }
        public string Cr1nome { get; set; }
    }
}
