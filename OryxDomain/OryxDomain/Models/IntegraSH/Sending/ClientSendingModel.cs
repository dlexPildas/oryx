using System;
using System.Collections.Generic;
using System.Text;

namespace OryxDomain.Models.IntegraSH.Sending
{
    public class ClientSendingModel : IntegraShSendModel
    {
        public ClientSendingModel() : base()
        {
        }

        public string Codigo { get; set; }
    }
}
