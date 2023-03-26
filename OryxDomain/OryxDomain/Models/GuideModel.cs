using System;

namespace OryxDomain.Models
{
    public class GuideModel
    {
        public string Pr4Nome { get; set; }
        public string Ce0Nome { get; set; }
        public DateTime Mv3Entrada { get; set; }
        public string Mv3EntradaFormated
        {
            get
            {
                return this.Mv3Entrada.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public string Mv3Doc { get; set; }
        public DateTime Mv3Saida { get; set; }
        public string Mv3SaidaFormated
        {
            get
            {
                return this.Mv3Saida.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}
