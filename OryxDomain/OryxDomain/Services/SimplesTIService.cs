using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Models.TecDataSoft;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using OryxDomain.WebServices;
using SimplesTI;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OryxDomain.Services
{
    public class SimplesTIService
    {
        readonly IConfiguration configuration;
        readonly ParametersRepository ParametersRepository;
        readonly MallParametersRepository MallParametersRepository;
        readonly RepresentativeRepository RepresentativeRepository;
        readonly AddressService AddressService;

        public SimplesTIService(IConfiguration configuration)
        {
            this.configuration = configuration;
            ParametersRepository = new ParametersRepository(configuration["OryxPath"] + "oryx.ini");
            RepresentativeRepository = new RepresentativeRepository(configuration["OryxPath"] + "oryx.ini");
            MallParametersRepository = new MallParametersRepository(configuration["OryxPath"] + "oryx.ini");
            AddressService = new AddressService(configuration);
        }

        public async Task<FindCustomerShoppingPayloadModel> FindCustumer(string cpfCnpj)
        {
            PD5 pd5 = await MallParametersRepository.Find(MallIntegrationType.SimplesTI);
            if (pd5 == null)
            {
                throw new Exception("Parâmetros de integração com Simples TI não cadastrados.");
            }
            LX0 lx0 = await ParametersRepository.GetLx0();
            if (pd5 == null)
                throw new Exception("Parâmetros shopping não encontrados.");
            string url = pd5.Pd5url;
            string loginSoftwareHouse = pd5.Pd5usuario;
            string senhaSoftwareHouse = pd5.Pd5senha;
            string loginLojista = lx0.Cf1login;
            string senhaLojista = lx0.Cf1senha;

            CustomBinding binding = new CustomBinding(new CustomTextMessageBindingElement("ISO-8859-1", "text/xml", MessageVersion.Soap11), new HttpTransportBindingElement());
            //BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress(url);

            serverSimplesTI_SMBPortTypeClient simplesTi = new serverSimplesTI_SMBPortTypeClient(binding, endpointAddress);

            var retornaDadosClientesResponse = await simplesTi.retorna_dados_clientesAsync(loginSoftwareHouse,
                                                                                             senhaSoftwareHouse,
                                                                                             loginLojista,
                                                                                             senhaLojista,
                                                                                             cpfCnpj);
            Clientes clientesSimplesTI = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Clientes));
            try
            {
                retornaDadosClientesResponse.@return = Regex.Replace(retornaDadosClientesResponse.@return, "&(?!(amp|apos|quot|lt|gt);)", "&amp;");
                using (StringReader reader = new StringReader(retornaDadosClientesResponse.@return))
                {
                    clientesSimplesTI = (Clientes)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("retxmlcustomer" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".txt", retornaDadosClientesResponse.@return);
                throw ex;
            }

            if (clientesSimplesTI == null)
            {
                return null;
            }
            Cliente clienteSimplesTI = clientesSimplesTI.Cliente;
            if (!"LIBERADO".Equals(clienteSimplesTI.Situacao))
            {
                return new FindCustomerShoppingPayloadModel()
                {
                    Cf1 = null,
                    IsError = true,
                    Message = "<b>Retorno do shopping:</b> " + clienteSimplesTI.DsSituacao
                };
            }
            CF1 cf1 = new CF1();
            cf1.Cf1cliente = clienteSimplesTI.NrCpfCnpj;
            cf1.Cf1nome = clienteSimplesTI.NmRazaoSocial;
            cf1.Cf1fant = clienteSimplesTI.NmCliente;
            cf1.Cf1cep = clienteSimplesTI.NrCep;
            cf1.Cf1email = clienteSimplesTI.Email;
            cf1.Cf3estado = clienteSimplesTI.NmEstado;
            cf1.Cf1numero = clienteSimplesTI.NrEndereco;
            cf1.Cf1bairro = clienteSimplesTI.NmBairro;
            cf1.Cf1ender1 = clienteSimplesTI.DsEndereco;
            cf1.Cf1compl = clienteSimplesTI.DsComplemento;
            cf1.Cf1fax = clienteSimplesTI.Fax;
            cf1.Cf1fone = clienteSimplesTI.TelefoneResidencial;
            cf1.Cf1confone = clienteSimplesTI.TelefoneCelular;
            cf1.Cf1insest = clienteSimplesTI.NrInscricaoEstadual;
            cf1.Cf3nome = clienteSimplesTI.NmCidade;

            if (!string.IsNullOrWhiteSpace(clienteSimplesTI.DtNascimento))
            {
                DateTime dtnascimento = DateTime.Parse(clienteSimplesTI.DtNascimento);
                cf1.Cf1conandi = dtnascimento.Day.ToString();
                cf1.Cf1conanme = dtnascimento.Month.ToString();
                cf1.Cf1conanan = dtnascimento.Year.ToString();
            }
            cf1.Cf1contato = clienteSimplesTI.NmCliente;

            if (!string.IsNullOrWhiteSpace(clienteSimplesTI.FlLiberadoCompraCheque) || string.IsNullOrWhiteSpace(clienteSimplesTI.FlBloqueadoParaCompra))
            {
                cf1.MensagensTmp = new List<string>();
                if (!string.IsNullOrWhiteSpace(clienteSimplesTI.FlLiberadoCompraCheque))
                    cf1.MensagensTmp.Add(string.Format("Liberado compra cheque: {0}", clienteSimplesTI.FlLiberadoCompraCheque));

                if (!string.IsNullOrWhiteSpace(clienteSimplesTI.FlBloqueadoParaCompra))
                    cf1.MensagensTmp.Add(string.Format("Bloqueado para compra: {0}", clienteSimplesTI.FlBloqueadoParaCompra));
            }

            CF6 cf6 = await InsertCf6(clienteSimplesTI.NrSindicato, clienteSimplesTI.NmGuia);
            cf1.Cf6 = cf6;

            //save address from shopping
            CF2 cf2 = new CF2()
            {
                Cf2cep = clienteSimplesTI.NrCep,
                Cf2local = clienteSimplesTI.CdIbge.ToString(),
                Cf2logra = clienteSimplesTI.DsEndereco
            };
            CF3 cf3 = new CF3()
            {
                Cf3local = clienteSimplesTI.CdIbge.ToString(),
                Cf3estado = clienteSimplesTI.NmEstado,
                Cf3nome = clienteSimplesTI.NmCidade,
                Cf3regiao = string.Empty
            };

            await AddressService.VerifyAndSaveAddressFromShopping(cf2, cf3);

            return new FindCustomerShoppingPayloadModel() { Cf1 = cf1 };
        }

        public async Task<CF6> FindAgencia(string cdGuia)
        {
            PD5 pd5 = await MallParametersRepository.Find(MallIntegrationType.SimplesTI);
            LX0 lx0 = await ParametersRepository.GetLx0();
            if (pd5 == null)
                throw new Exception("Parâmetros shopping não encontrados.");
            string url = pd5.Pd5url;
            string loginSoftwareHouse = pd5.Pd5usuario;
            string senhaSoftwareHouse = pd5.Pd5senha;
            string loginLojista = lx0.Cf1login;
            string senhaLojista = lx0.Cf1senha;

            CustomBinding binding = new CustomBinding(new CustomTextMessageBindingElement("ISO-8859-1", "text/xml", MessageVersion.Soap11), new HttpTransportBindingElement());
            //BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress(url);

            serverSimplesTI_SMBPortTypeClient simplesTi = new serverSimplesTI_SMBPortTypeClient(binding, endpointAddress);

            var agenciaRet = await simplesTi.retorna_dados_guiasAsync(loginSoftwareHouse,
                                                                        senhaSoftwareHouse,
                                                                        loginLojista,
                                                                        senhaLojista,
                                                                        "S" + cdGuia);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Guias));
                Guias guias = new Guias();
                agenciaRet.@return = Regex.Replace(agenciaRet.@return, "&(?!(amp|apos|quot|lt|gt);)", "&amp;");
                using (StringReader reader = new StringReader(agenciaRet.@return))
                {
                    guias = (Guias)serializer.Deserialize(reader);
                }

                CF6 cf6 = await InsertCf6(guias.Guia.NrSindicato, guias.Guia.NmGuia);

                return cf6;
            }
            catch (Exception ex)
            {
                File.WriteAllText("retxmlagency" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".txt", agenciaRet.@return);
                return null;
            }
        }

        public async Task<VendasRet> SendSales(Vendas venda)
        {
            PD5 pd5 = await MallParametersRepository.Find(MallIntegrationType.SimplesTI);
            LX0 lx0 = await ParametersRepository.GetLx0();
            if (pd5 == null)
                throw new Exception("Parâmetros shopping não encontrados.");
            string url = pd5.Pd5url;
            string loginSoftwareHouse = pd5.Pd5usuario;
            string senhaSoftwareHouse = pd5.Pd5senha;
            string loginLojista = lx0.Cf1login;
            string senhaLojista = lx0.Cf1senha;

            CustomBinding binding = new CustomBinding(new CustomTextMessageBindingElement("ISO-8859-1", "text/xml", MessageVersion.Soap11), new HttpTransportBindingElement());
            //BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress(url);

            serverSimplesTI_SMBPortTypeClient tecdatasoft = new serverSimplesTI_SMBPortTypeClient(binding, endpointAddress);
            XmlSerializer serializer = new XmlSerializer(typeof(Vendas));
            string vendaXml = "";
            StringBuilder sb = new StringBuilder();
            var emptyNs = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            using (XmlWriter xmlWriter = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true }))
            {
                serializer.Serialize(xmlWriter, venda, emptyNs);
            }
            vendaXml = sb.ToString();

            bool saveLog = bool.Parse(configuration["SaveLogIntegracao"]);

            if(saveLog)
                File.WriteAllText("envxmlsales"+DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")+".txt", vendaXml);

            var vendaRetXml = await tecdatasoft.registra_venda_geralAsync(loginSoftwareHouse,
                                                                        senhaSoftwareHouse,
                                                                        loginLojista,
                                                                        senhaLojista,
                                                                        venda.CF1Cliente,
                                                                        venda.DataVenda.ToString("dd/MM/yyyy"),
                                                                        venda.CF6Repres,
                                                                        vendaXml);
            try
            {
                VendasRet vendaRet = new VendasRet();
                serializer = new XmlSerializer(typeof(VendasRet));
                vendaRetXml.@return = Regex.Replace(vendaRetXml.@return, "&(?!(amp|apos|quot|lt|gt);)", "&amp;");
                using (StringReader reader = new StringReader(vendaRetXml.@return))
                {
                    vendaRet = (VendasRet)serializer.Deserialize(reader);
                }

                return vendaRet;
            }
            catch (Exception ex)
            {
                File.WriteAllText("retxmlsales" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".txt", vendaRetXml.@return);
                throw ex;
            }
            
        }

        public string GetSimplesTIInstallmentType(CodNFCE cv8codnfce)
        {
            return cv8codnfce switch
            {
                CodNFCE.Nenhum => "DINHEIRO",
                CodNFCE.Dinheiro => "DINHEIRO",
                CodNFCE.Cheque => "CHEQUE",
                CodNFCE.CartaoDeCredito => "CARTAO",
                CodNFCE.CartaoDeDebito => "CARTAO",
                CodNFCE.Crediario => "DINHEIRO",
                CodNFCE.ValeAlimentacao => "CARTAO",
                CodNFCE.ValeRefeicao => "CARTAO",
                CodNFCE.ValePresente => "CARTAO",
                CodNFCE.ValeCombustivel => "CARTAO",
                CodNFCE.BlqBancario => "BOLETO",
                CodNFCE.DepositoBancario => "DINHEIRO",
                CodNFCE.PIX => "PIX",
                CodNFCE.TransferenciaBancaria => "DINHEIRO",
                CodNFCE.CashBack => "DINHEIRO",
                CodNFCE.SemPagamento => "",
                CodNFCE.Outros => "DINHEIRO",
                _ => "DINHEIRO",
            };
        }

        private async Task<CF6> InsertCf6(string nrSindicato, string nmGuia)
        {
            CF6 cf6;
            FormatterService formatterService = new FormatterService(configuration);

            int cf6represSize = await formatterService.FindFieldLength(nameof(cf6.Cf6repres));
            nrSindicato = Formatters.FormatId(nrSindicato, cf6represSize);

            cf6 = await RepresentativeRepository.Find(nrSindicato);
            if (cf6 == null)
            {
                cf6 = new CF6
                {
                    Cf6repres = nrSindicato,
                    Cf6nome = nmGuia
                };

                cf6 = (CF6)await formatterService.ValidateFormatBasicByDC1(cf6);
                await RepresentativeRepository.Insert(cf6);
            }

            return cf6;
        }
    }
}
