using System;

namespace OryxDomain.Models.ACECORS
{
    public class PostModel
    {
        public DateTime Data { get; set; }
        public int? GuiaId { get; set; }
        public bool SomenteComRestricao { get; set; }
        public bool SomenteComSerasa { get; set; }
        public bool SomenteComObs { get; set; }
    }
}
