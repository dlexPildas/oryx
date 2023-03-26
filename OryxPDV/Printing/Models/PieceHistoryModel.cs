using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using System;
using System.Collections.Generic;

namespace Printing.Models
{
    public class PieceHistoryModel
    {
        public string Cr1Nome { get; set; }
        public string Pr0Desc { get; set; }
        public string Pr0Refer { get; set; }
        public string OfhDoc { get; set; }
        public string OfgNome { get; set; }
        public string Of7Doc { get; set; }
        public DateTime Of1Geracao { get; set; }
        public string Of1GeracaoFormated
        {
            get
            {
                return this.Of1Geracao.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public DateTime? Of0Leitura { get; set; }

        public IList<OFA> OfaLst { get; set; }
        public IList<ShiftReportsModel> ShiftReportsLst { get; set; }
        public IList<GuideModel> GuideLst { get; set; }
        public IList<DevolutionModel> Devolutions { get; set; }
        public IList<OF0> Of0Lst { get; set; }
        public LastExpeditionModel LastExpeditionModel { get; set; }
        public OF3 OF3 { get; set; }

    }
}
