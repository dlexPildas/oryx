namespace OryxDomain.Models
{
    public class CepModel
    {
        public string cep { get; set; }
        public string logradouro { get; set; }
        public string bairro { get; set; }
        public string localidade { get; set; }
        public string uf { get; set; }
        public string ibge { get; set; }
        public bool erro { get; set; }
    }
}
