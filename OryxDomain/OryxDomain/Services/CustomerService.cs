using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class CustomerService
    {
        readonly CustomerRepository CustomerRepository;
        readonly ParametersRepository ParametersRepository;

		readonly IConfiguration Configuration;
        public CustomerService(IConfiguration configuration)
        {
            Configuration = configuration;
            CustomerRepository = new CustomerRepository(configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(configuration["OryxPath"] + "oryx.ini");

		}
        public async Task ValidateCustomerStatus(string cf1cliente, bool insert = false)
        {
            CF1 cf1 = await CustomerRepository.FindByCpfCnpj(cf1cliente);
            B2B b2b = await ParametersRepository.GetB2b();

            if (b2b == null)
                throw new Exception("Parâmetros para E-Commerce não cadastrados.");
            if (cf1 != null)
            {
                cf1.Blocks = await CustomerRepository.FindBlocks(cf1cliente);
                //cliente com cadastro pendente aprovação
                if (cf1.Cf1tipo.Equals(b2b.B2bclipen))
                {
                    throw new Exception("Já existe cadastro para esse CNPJ pendente. Confira seu e-mail e confirme seu cadastro.");
                }
                //cliente com cadastro pendente de confirmação por e-mail
                if (cf1.Cf1tipo.Equals(b2b.B2bclicon))
                {
                    throw new Exception("Já temos um cadastro pendente de aprovação para esse CNPJ. Aguarde a confirmação do seu cadastro por e-mail.");
                }
                if (cf1.Blocks != null && cf1.Blocks.Any())
                {
                    throw new Exception("Já existe uma conta bloqueada para este CNPJ. Motivo(s):" + string.Join(", ", cf1.Blocks.Select(cvs => cvs.Cvsmotivob)));
                }
                if (insert)
                {
                    throw new Exception("Cliente já cadastrado!");
                }
            }
        }
		
		#region CF1 (Customer) Validator
		public void Validate(CF1 cf1Customer)
		{
			//cnpj e cpf
			ValidateCF1Cliente(cf1Customer.Cf1cliente);

			//nome do cliente
			cf1Customer.Cf1nome = Formatters.RemoveSpecialCharacterByNames(cf1Customer.Cf1nome);

			//nome fantasia
			cf1Customer.Cf1fant = Formatters.RemoveSpecialCharacterByNames(cf1Customer.Cf1fant);

			////Telefone_principal
			//if (!Validators.IsPhone(cf1Customer.Cf1fone))
			//{
			//	throw new Exception("Telefone inválido");
			//}

			////Telefone_celular
			//if (!Validators.IsPhone(cf1Customer.Cf1confone))
			//{
			//	throw new Exception("Celular inválido");
			//}

			//Email
			if (!string.IsNullOrWhiteSpace(cf1Customer.Cf1email) &&
				!Validators.IsEMail(cf1Customer.Cf1email))
			{
				throw new Exception("E-mail inválido");
			}
			
			//Data de nascimento
			if((!string.IsNullOrWhiteSpace(cf1Customer.Cf1conanan) && !string.IsNullOrWhiteSpace(cf1Customer.Cf1conandi) && !string.IsNullOrWhiteSpace(cf1Customer.Cf1conanme))
				&& Convert.ToInt32(cf1Customer.Cf1conanan) > DateTime.Now.Year && (Convert.ToInt32(cf1Customer.Cf1conandi) > 0 && Convert.ToInt32(cf1Customer.Cf1conandi) <= 31))
            {
				throw new Exception("Data de nascimento inválida.");
			}

			//CPF do contato
			if (!string.IsNullOrWhiteSpace(cf1Customer.Cf1concpf))
			{
				cf1Customer.Cf1concpf = cf1Customer.Cf1concpf.OnlyNumbers();
				if(!Validators.IsCpf(cf1Customer.Cf1concpf))
					throw new Exception("CPF do contato inválido");
			}

			//Data de abertura
			if (cf1Customer.Cf1abert.CompareTo(DateTime.UtcNow) > 0)
            {
				throw new Exception("Data de abertura deve ser menor ou igual ao dia atual.");
			}

			//endereço
			cf1Customer.Cf1ender1 = Formatters.RemoveSpecialCharacterByNames(cf1Customer.Cf1ender1);

			//Ponto de referência
			cf1Customer.Cf1ender2 = Formatters.RemoveSpecialCharacterByNames(cf1Customer.Cf1ender2);
			
			if (!string.IsNullOrWhiteSpace(cf1Customer.Cf1cep))
            {
				cf1Customer.Cf1cep = cf1Customer.Cf1cep.OnlyNumbers();
				Validators.ValidateCep(cf1Customer.Cf1cep);
			}
			Format(cf1Customer);
			new FormatterService(Configuration).ValidateFormatBasicByDC1(cf1Customer).Wait();
		}

		public void ValidateCF1Cliente(string cf1cliente, bool acceptCPF = true)
		{
			if (string.IsNullOrWhiteSpace(cf1cliente))
			{
				throw new MissingFieldException("CPF OU CNPJ não informado.");
			}
			cf1cliente = cf1cliente.OnlyNumbers();

			if (!acceptCPF && cf1cliente.Length == 11)
			{
				throw new Exception("Não é possível cadastrar um cliente pessoa física.");
			}

			if (!(cf1cliente.Length == 11 ? Validators.IsCpf(cf1cliente) : Validators.IsCnpj(cf1cliente)))
			{
				throw new Exception("CPF ou CNPJ inválido");
			}
		}

		#endregion CF1 (Customer) Validator

		#region CF1 (Customer) Formater
		
        /// <summary>Valida e formata o objeto CF1(Cliente)</summary>
        public async void Format(CF1 cf1)
        {
			//Formata CF1Cliente
			DC1 dc1 = await new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini").FindDC1ByDc1campo(nameof(cf1.Cf1cliente));
			cf1.Cf1cliente = cf1.Cf1cliente.OnlyNumbers();
			/*
			//Formata razão social
			dc1 = await new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini").FindDC1ByDc1campo(nameof(cf1.Cf1nome));
			cf1.Cf1nome = Formatters.FormatField(cf1.Cf1nome, dc1.Dc1tamanho);

			//Nome fantasia
			dc1 = await new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini").FindDC1ByDc1campo(nameof(cf1.Cf1fant));
			cf1.Cf1fant = Formatters.FormatField(cf1.Cf1fant, dc1.Dc1tamanho);

			//IE
			dc1 = await new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini").FindDC1ByDc1campo(nameof(cf1.Cf1insest));
			cf1.Cf1insest = Formatters.FormatField(cf1.Cf1insest, dc1.Dc1tamanho);

			//Telefone_principal
			dc1 = await new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini").FindDC1ByDc1campo(nameof(cf1.Cf1fone));
			cf1.Cf1fone = Formatters.FormatField(cf1.Cf1fone, dc1.Dc1tamanho);

			//Telefone celular
			dc1 = await new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini").FindDC1ByDc1campo(nameof(cf1.Cf1confone));
			cf1.Cf1confone = Formatters.FormatField(cf1.Cf1confone, dc1.Dc1tamanho);

			//E-mail
			dc1 = await new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini").FindDC1ByDc1campo(nameof(cf1.Cf1email));
			cf1.Cf1email = Formatters.FormatField(cf1.Cf1email, dc1.Dc1tamanho);
			*/
		}

		#endregion CF1 (Customer) Formater



	}
}
