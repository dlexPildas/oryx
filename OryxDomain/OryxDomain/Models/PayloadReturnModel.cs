using OryxDomain.Models.Oryx;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class PayloadReturnModel
    {
        public VDK Vdk { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }
        public bool SavedVDK { get; set; }
        public IList<string> RelatsPath { get; set; }
    }
}
