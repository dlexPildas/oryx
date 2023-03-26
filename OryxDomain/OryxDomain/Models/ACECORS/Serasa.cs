namespace OryxDomain.Models.ACECORS
{
    public class Serasa
    {
        public int TotalOcorrencias { get; set; }
        public CCF CCF { get; set; }
        public ChequeRestriction Contumancia { get; set; }
        public ChequeRestriction ChequeLojista { get; set; }
        public ChequeRestriction ContraOrdem { get; set; }
    }
}
