namespace OryxDomain.Models.Ezetech.Return
{
    public class CustomerPayload : EzetechPayload
    {
        public Customer Cliente { get; set; }
        public CustomerSituation Situacao { get; set; }
    }
}
