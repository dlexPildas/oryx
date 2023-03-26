using Microsoft.AspNetCore.Http;

namespace OryxDomain.Models.Oryx
{
    public class B2I
    {
        public int B2icodigo { get; set; }
        public string B2icaminho { get; set; }
        public string B2iproduto { get; set; }
        public bool B2iprincip { get; set; }
        public IFormFile B2iimage { get; set; }
    }
}
