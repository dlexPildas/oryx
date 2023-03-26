using System;

namespace OryxDomain.Models
{
    public class ShiftReportsModel
    {
        public string Ce3Nome { get; set; }
        public string Pr4Nome { get; set; }
        public string Mv4Relat { get; set; }
        public string Mv4Celula { get; set; }
        public DateTime Mv4Inicio { get; set; }
        public string Mv4InicioFormated
        {
            get
            {
                return this.Mv4Inicio.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
    }
}
