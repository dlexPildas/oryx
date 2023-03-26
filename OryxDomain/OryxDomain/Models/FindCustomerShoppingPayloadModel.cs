using OryxDomain.Models.Oryx;

namespace OryxDomain.Models
{
    public class FindCustomerShoppingPayloadModel
    {
        public CF1 Cf1 { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}
