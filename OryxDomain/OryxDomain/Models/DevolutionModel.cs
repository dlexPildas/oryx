using System;

namespace OryxDomain.Models
{
    public class DevolutionModel
    {
        public string Vd1cliente { get; set; }
        public string VdlDoc { get; set; }
        public string VdlPedido { get; set; }
        public string VdlPeca { get; set; }
        public DateTime VdlLeitura { get; set; }
        public string VdlLeituraFormated
        {
            get
            {
                return this.VdlLeitura.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
        public string Cf1Nome { get; set; }
    }
}
