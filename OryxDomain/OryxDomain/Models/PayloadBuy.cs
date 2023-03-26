namespace OryxDomain.Models
{
    public class PayloadBuy
    {
        public bool IsError { get; set; }
        public string MessageError { get; set; }
        public StepCreateOrderType Step { get; set; }
        public string Vd1pedido { get; set; }
    }
}
