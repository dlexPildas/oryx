using System;
using System.Collections.Generic;
using System.Text;

namespace OryxDomain.Models.IntegraSH.Sending
{
    public class AgencySendingModel : IntegraShSendModel
    {
        public AgencySendingModel() : base()
        {
        }

        public string Codigo { get; set; }
    }
}
