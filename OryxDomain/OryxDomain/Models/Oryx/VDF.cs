using System;

namespace OryxDomain.Models.Oryx
{
    public class VDF
    {
        public string Vdfdoc { get; set; }
        public string Vdfpeca { get; set; }
        public DateTime Vdfleitura { get; set; }
        public string ReadingDate
        {
            get
            {
                return this.Vdfleitura.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public string Vdfvolume { get; set; }
        public string Of3produto { get; set; }
        public string Of3opcao { get; set; }
        public string Of3tamanho { get; set; }
        public string Of3rfid { get; set; }
        public string Cr1nome { get; set; }
    }
}
