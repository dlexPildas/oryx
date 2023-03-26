using OryxDomain.Models.Enums;
using System;

namespace OryxDomain.Models
{
    public class CustomerHistoryPayment
    {
        public string Cv8parcela { get; set; }
        public string Cv8tipotit { get; set; }
        public string Cv2nome { get; set; }
        public CodNFCE Cv8codnfce { get; set; }
        public string Cv8cmc7 { get; set; }
        public decimal Cv8valor { get; set; }
        public DateTime Cv8vencim { get; set; }
        public string DueDate
        {
            get
            {
                return this.Cv8vencim.ToString("dd/MM/yyyy");
            }
        }
        public string Cv8barras { get; set; }
        public string Cv8agente { get; set; }
        public string Cv8conta { get; set; }
        public string Cv8nsu { get; set; }
        public string Cv8band { get; set; }
    }
}
