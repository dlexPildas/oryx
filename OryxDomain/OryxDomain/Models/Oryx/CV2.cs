using OryxDomain.Models.Enums;
using System;

namespace OryxDomain.Models.Oryx
{
    public class CV2
    {
        public string Cv2titulo { get; set; }
        public string Cv2nome { get; set; }
        public string Cv2layout { get; set; }
        public string Cv2codnfce { get; set; }
        public CodNFCE NfceCode 
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(Cv2codnfce))
                {
                    return CodNFCE.Nenhum;
                }
                CodNFCE codNFCE = CodNFCE.Outros;
                return (CodNFCE)Enum.Parse(codNFCE.GetType(), Cv2codnfce);
            }
            set { }
        }
        public string Cv2cnpjope { get; set; }
    }
}
