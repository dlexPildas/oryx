using System;

namespace OryxDomain.Models
{
    public class LastExpeditionModel
    {
        public string Cf1Nome { get; set; }
        public string Cf3Nome { get; set; }
        public string Cf3Estado { get; set; }
        public string Vd1Pedido { get; set; }
        public int Vd1Consig { get; set; }
        public string Vd8Volume { get; set; }
        public DateTime Vd8Leitura { get; set; }
        public string Vd8LeituraFormated
        {
            get
            {
                return this.Vd8Leitura.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
        
       
    }
}
