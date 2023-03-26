using OryxDomain.Models.Enums;
using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class PayloadEmitBankSlipModel
    {
        public IList<string> RelatsPath { get; set; }
        public string InstallmentStep { get; set; }
        public StepEmitBankSlipValidation Step { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
