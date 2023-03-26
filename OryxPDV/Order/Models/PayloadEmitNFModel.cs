namespace Order.Models
{
    public class PayloadEmitNFModel
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string RelatPath { get; set; }
    }
}
