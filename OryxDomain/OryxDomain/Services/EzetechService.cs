using Microsoft.Extensions.Configuration;
using OryxDomain.Http;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Ezetech.Return;
using OryxDomain.Models.Ezetech.Sending;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class EzetechService
    {
        readonly MallParametersRepository MallParametersRepository;
        readonly RepresentativeRepository RepresentativeRepository;
        readonly AddressService AddressService;

        readonly IConfiguration configuration;

        public EzetechService(IConfiguration configuration)
        {
            this.configuration = configuration;
            AddressService = new AddressService(configuration);
            RepresentativeRepository = new RepresentativeRepository(configuration["OryxPath"] + "oryx.ini");
            MallParametersRepository = new MallParametersRepository(configuration["OryxPath"] + "oryx.ini");
        }


        #region agencia
        public async Task<CF6> FindAgencia(string codigoAgencia)
        {
            PD5 pd5 = await MallParametersRepository.Find(MallIntegrationType.Ezetech);
            
            ValidateParameters(pd5);

            string agencyReturn = HttpUtilities.GetAsync(
                  pd5.Pd5url
                , "agencia"
                , codigoAgencia+"/"
                , customAuthorization: string.Format("Token {0}", pd5.Pd5senha)).Result;

            AgencyPayload agencyPayload = JsonSerializer.Deserialize<AgencyPayload>(agencyReturn, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (agencyPayload.Erro != null)
                throw new Exception(string.Format("<b>Retorno do shopping:</b> {0}<br/>Código do erro: {1}", agencyPayload.Erro.Mensagem, agencyPayload.Erro.Codigo));

            if (agencyPayload.Agencia == null)
                throw new Exception("Dados da agência não retornados pelo shopping");

            CF6 cf6 = new CF6
            {
                Cf6repres = agencyPayload.Agencia.Codigo,
                Cf6nome = string.IsNullOrWhiteSpace(agencyPayload.Agencia.Fantasia) ? agencyPayload.Agencia.Apelido : agencyPayload.Agencia.Fantasia
            };

            FormatterService formatterService = new FormatterService(configuration);

            int cf6represSize = await formatterService.FindFieldLength(nameof(cf6.Cf6repres));
            cf6.Cf6repres = Formatters.FormatId(cf6.Cf6repres, cf6represSize);

            cf6 = (CF6)await formatterService.ValidateFormatBasicByDC1(cf6);

            CF6 cf6Aux = await RepresentativeRepository.Find(cf6.Cf6repres);

            if (cf6Aux == null)
                await RepresentativeRepository.Insert(cf6);

            return cf6;
        }
        #endregion agencia

        #region venda
        public async Task<dynamic> CreateSales(SalesSendingModel salesSending)
        {
            PD5 pd5 = await MallParametersRepository.Find(MallIntegrationType.Ezetech);

            ValidateParameters(pd5);

            salesSending.Venda.Software = pd5.Pd5usuario;

            string agencyReturn = HttpUtilities.CallPostAsync(
                  pd5.Pd5url
                , "venda/"
                , JsonSerializer.Serialize(salesSending, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                , customAuthorization: string.Format("Token {0}", pd5.Pd5senha)).Result;

            SalesPayload salesPayload = JsonSerializer.Deserialize<SalesPayload>(agencyReturn, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (salesPayload.Erro != null)
                throw new Exception(string.Format("<b>Retorno do shopping:</b> {0}<br/>Código do erro: {1}", salesPayload.Erro.Mensagem, salesPayload.Erro.Codigo));

            if ((object)salesPayload.Autorizacoes == null)
                throw new Exception("Nenhuma autorização retornada shopping");

            return salesPayload.Autorizacoes;
        }

        #endregion venda

        #region cliente
        public async Task<FindCustomerShoppingPayloadModel> FindCustomer(string cnpj, bool showErrorCode = false)
        {
            PD5 pd5 = await MallParametersRepository.Find(MallIntegrationType.Ezetech);
            
            ValidateParameters(pd5);

            cnpj = cnpj.OnlyNumbers();

            string customerReturn = HttpUtilities.GetAsync(
                  pd5.Pd5url
                , "cliente"
                , cnpj + "/"
                , customAuthorization: string.Format("Token {0}", pd5.Pd5senha)).Result;

            CustomerPayload customerPayload = JsonSerializer.Deserialize<CustomerPayload>(customerReturn, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (customerPayload.Erro != null)
            {
                //em caso de código de erro:
                //1. 06 (Cliente CNPJ não cadastrado)
                //2. 08 (Cliente CPF não cadastrado.)
                //3. 12 (Cliente inativo)
                if (customerPayload.Erro.Codigo.Equals("06") ||
                   customerPayload.Erro.Codigo.Equals("08") ||
                   customerPayload.Erro.Codigo.Equals("12"))
                {
                    string message = string.Format("<b>Retorno do shopping:</b> {0}", customerPayload.Erro.Mensagem);
                    if (showErrorCode)
                        message += string.Format("<br/>Código do erro: {0}", customerPayload.Erro.Codigo);

                    return new FindCustomerShoppingPayloadModel()
                    {
                        Cf1 = null,
                        IsError = true,
                        Message = message
                    };
                }
            }

            if (customerPayload.Cliente == null)
                throw new Exception("Dados do cliente não retornados pelo shopping");

            CF1 cf1 = new CF1();
            cf1.Cf1cliente = Formatters.OnlyNumbers(customerPayload.Cliente.Cnpj_cpf);
            cf1.Cf1nome = customerPayload.Cliente.Razao_nome;
            cf1.Cf1fant = customerPayload.Cliente.Fantasia;
            cf1.Cf1ender1 = customerPayload.Cliente.Addr_logra;
            cf1.Cf1numero = customerPayload.Cliente.Addr_numer;
            cf1.Cf1compl = customerPayload.Cliente.Addr_compl;
            cf1.Cf1bairro = customerPayload.Cliente.Addr_bairr;
            cf1.Cf3nome = customerPayload.Cliente.Cidade;
            cf1.Cf1cep = Formatters.OnlyNumbers(customerPayload.Cliente.Addr_cep);
            cf1.Cf3estado = customerPayload.Cliente.Uf;
            cf1.Cf1fone = customerPayload.Cliente.Fone;
            cf1.Cf1confone = customerPayload.Cliente.Celular;
            cf1.Cf1email = customerPayload.Cliente.Email;
            cf1.Cf1insest = customerPayload.Cliente.Ie_rg;
            cf1.MensagensTmp = new List<string>();

            if (!string.IsNullOrWhiteSpace(customerPayload.Cliente.Mensagem))
                cf1.MensagensTmp.Add(customerPayload.Cliente.Mensagem);

            try
            {
                cf1.Cf1abert = DateTime.ParseExact(customerPayload.Cliente.Abert_nasc, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cf1.Cf1conanan = cf1.Cf1abert.Year.ToString();
                cf1.Cf1conandi = cf1.Cf1abert.Day.ToString();
                cf1.Cf1conanme = cf1.Cf1abert.Month.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (customerPayload.Situacao != null)
            {
                if (customerPayload.Situacao.Situacao_fin.Equals("INATIVO"))
                    cf1.MensagensTmp.Add("Retorno do shopping: Cliente inativo");
                
                if (customerPayload.Situacao.Situacao_fin.Equals("BLOQUEADO"))
                    cf1.MensagensTmp.Add("Retorno do shopping: Cliente bloqueado");
                
                if (!string.IsNullOrWhiteSpace(customerPayload.Situacao.Limite))
                {
                    cf1.LimitShoppingIntegration = Convert.ToDecimal(customerPayload.Situacao.Limite);
                    cf1.LimitShoppingIntegration = cf1.LimitShoppingIntegration / 100;
                }

            }

            //save address from shopping
            CF2 cf2 = new CF2()
            {
                Cf2cep = Formatters.OnlyNumbers(customerPayload.Cliente.Addr_cep),
                Cf2local = customerPayload.Cliente.Ibge,
                Cf2logra = customerPayload.Cliente.Addr_logra
            };
            CF3 cf3 = new CF3()
            {
                Cf3local = customerPayload.Cliente.Ibge,
                Cf3estado = customerPayload.Cliente.Uf,
                Cf3nome = customerPayload.Cliente.Cidade,
                Cf3regiao = string.Empty
            };

            await AddressService.VerifyAndSaveAddressFromShopping(cf2, cf3);

            return new FindCustomerShoppingPayloadModel(){ Cf1 = cf1 };
        }

         #endregion cliente

        private static void ValidateParameters(PD5 pd5)
        {
            if (pd5 == null)
                throw new Exception("Parâmetros de integração com shopping não cadastrados.");

            if (string.IsNullOrWhiteSpace(pd5.Pd5url))
                throw new Exception("URL para integração com o serviço do shopping não cadastrada");

            if (string.IsNullOrWhiteSpace(pd5.Pd5senha))
                throw new Exception("Token da loja não informado nos parâmetros de shopping");
        }

        public string GetPaymentCode(CodNFCE cv8codnfce)
        {
            switch (cv8codnfce)
            {
                case CodNFCE.Nenhum:
                case CodNFCE.Dinheiro:
                case CodNFCE.ValeAlimentacao:
                case CodNFCE.ValeRefeicao:
                case CodNFCE.ValePresente:
                case CodNFCE.ValeCombustivel:
                case CodNFCE.DepositoBancario:
                case CodNFCE.TransferenciaBancaria:
                case CodNFCE.CashBack:
                case CodNFCE.SemPagamento:
                case CodNFCE.Outros:
                    return "1";
                case CodNFCE.Cheque:
                    return "2";
                case CodNFCE.CartaoDeCredito:
                    return "3";
                case CodNFCE.CartaoDeDebito:
                    return "4";
                case CodNFCE.PIX:
                    return "5";
                case CodNFCE.Crediario:
                case CodNFCE.BlqBancario:
                    return "6";
                default:
                    return "1";
            }
        }
    }

}
