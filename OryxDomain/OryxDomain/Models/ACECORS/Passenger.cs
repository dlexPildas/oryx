using System.Collections.Generic;

namespace OryxDomain.Models.ACECORS
{
    public class Passenger
    {
        public Travel Viagem { get; set; }
        public int CodigoGuia { get; set; }
        public string Nomeguia { get; set; }
        public string CidadeGuia { get; set; }
        public int PassageiroId { get; set; }
        public string Nome { get; set; }
        public string Cnpj { get; set; }
        public string IE { get; set; }
        public string Cpf { get; set; }
        public string AberturaEmpresa { get; set; }
        public string Responsavel { get; set; }
        public Address Endereco { get; set; }
        public string Regiao { get; set; }
        public string TelefoneComercial { get; set; }
        public string TelefoneResidencial { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string NomeAutorizado1 { get; set; }
        public string CpfAutorizado1 { get; set; }
        public string NomeAutorizado2 { get; set; }
        public string CpfAutorizado2 { get; set; }
        public string NomeAutorizado3 { get; set; }
        public string CpfAutorizado3 { get; set; }
        public IList<BankAccount> InformacaoBancaria { get; set; }
        public IList<AssociationRestriction> RestricaoAcecors { get; set; }
        public Serasa Serasa { get; set; }
    }
}
