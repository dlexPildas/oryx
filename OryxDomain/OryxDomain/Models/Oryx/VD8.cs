using System;

namespace OryxDomain.Models.Oryx
{
    public class VD8
    {
        public string Vd8volume { get; set; }
        public string Vd8peca { get; set; }
        public decimal Vd8preco { get; set; }
        public DateTime Vd8leitura { get; set; }
        public string ReadingDate
        {
            get
            {
                return this.Vd8leitura.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public string Of3produto { get; set; }
        public string Of3opcao { get; set; }
        public string Of3tamanho { get; set; }
        public string Of3ordem { get; set; }
        public string Of3lote { get; set; }
        public string Pr0desc { get; set; }
        public string Cr1nome { get; set; }
    }
}
