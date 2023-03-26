using IniParser;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.IntegraSH;
using OryxDomain.Models.IntegraSH.Return;
using OryxDomain.Models.IntegraSH.Sending;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class IntegraShService
    {
        private const string RETURN_FILE_NAME = "RETORNO.SHG";
        private const string SEND_FILE_NAME = "REMESSA.SHG";
        private const string SOFTWARE_NAME = "IntegraSH.exe";
        private const int TIMEOUT_RETURNFILE_MILLIS = 90000;
        private const int DELAY_RETURN_FILE_SEARCH = 1000;

        readonly ParametersRepository ParametersRepository;
        readonly MallParametersRepository MallParametersRepository;
        readonly RepresentativeRepository RepresentativeRepository;
        readonly CustomerRepository CustomerRepository;
        readonly TerminalRepository TerminalRepository;
        readonly AddressService AddressService;

        readonly IConfiguration configuration;

        //private string Terminal;
        private string IntegraSHPath;
        private string Host;

        public IntegraShService(IConfiguration configuration)
        {
            this.configuration = configuration;
            AddressService = new AddressService(configuration);
            ParametersRepository = new ParametersRepository(configuration["OryxPath"] + "oryx.ini");
            RepresentativeRepository = new RepresentativeRepository(configuration["OryxPath"] + "oryx.ini");
            MallParametersRepository = new MallParametersRepository(configuration["OryxPath"] + "oryx.ini");
            CustomerRepository = new CustomerRepository(configuration["OryxPath"] + "oryx.ini");
            TerminalRepository = new TerminalRepository(configuration["OryxPath"] + "oryx.ini");
        }

        #region agencia
        public async Task<CF6> FindAgencia(string codigoAgencia, string terminalId)
        {
            LX0 lx0 = await ParametersRepository.GetLx0();
            CF1 cf1 = await CustomerRepository.FindByCpfCnpj(lx0.Lx0cliente);
            PD5 pd5 = await MallParametersRepository.Find(MallIntegrationType.IntegraSH);
            PD0 pd0 = await TerminalRepository.Find(terminalId);
            //Terminal = terminalId;
            IntegraSHPath = pd0.Pd0caminho;
            Host = pd0.Pd0host;

            AgencySendingModel agencySending = new AgencySendingModel();
            agencySending.Codigo = codigoAgencia;

            agencySending.Data = DateTime.Today;
            agencySending.Hora = DateTime.Now;
            agencySending.Func = cf1.Cf1concpf.OnlyNumbers();
            agencySending.Loja = lx0.Cf1login.OnlyNumbers();
            agencySending.Software = pd5.Pd5usuario;

            var agencyReturn = FindAgency(agencySending);

            ValidateSendReturn(agencySending, agencyReturn);
            ValidateReturn(agencyReturn);

            CF6 cf6 = new CF6();
            cf6.Cf6repres = agencyReturn.Codigo;
            cf6.Cf6nome = agencyReturn.Nome;
            FormatterService formatterService = new FormatterService(configuration);
            
            int cf6represSize = await formatterService.FindFieldLength(nameof(cf6.Cf6repres));
            cf6.Cf6repres = Formatters.FormatId(cf6.Cf6repres, cf6represSize);
            
            cf6 = (CF6)await formatterService.ValidateFormatBasicByDC1(cf6);

            CF6 cf6Aux = await RepresentativeRepository.Find(cf6.Cf6repres);

            if (cf6Aux == null)
            {
                await RepresentativeRepository.Insert(cf6);
            }

            return cf6;
        }

        private AgencyReturnModel FindAgency(AgencySendingModel sendingModel)
        {
            var sendContent = CreateAgencySendingFile(sendingModel);
            var integraShReturn = this.RequestSending(sendContent);

            var parser = new IniDataParser();

            parser.Configuration.SkipInvalidLines = true;
            parser.Configuration.ParseComments = false;
            IniData data = parser.Parse(integraShReturn);

            AgencyReturnModel agencyReturnModel = new AgencyReturnModel();
            agencyReturnModel.Loja = data["CONSULTA"]["LOJA"];
            agencyReturnModel.Func = data["CONSULTA"]["FUNC"];
            agencyReturnModel.Data = DateTime.ParseExact(data["CONSULTA"]["DATA"], "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
            agencyReturnModel.Hora = DateTime.ParseExact(data["CONSULTA"]["HORA"], "HHmmss", System.Globalization.CultureInfo.InvariantCulture);
            agencyReturnModel.Software = data["CONSULTA"]["SOFTWARE"];

            agencyReturnModel.CodigoRetorno = data["RESULTADO"]["CODIGO"];
            agencyReturnModel.Descricao = data["RESULTADO"]["DESCRICAO"];

            agencyReturnModel.Nome = data["AGENCIA"]["NOME"];
            agencyReturnModel.Codigo = data["AGENCIA"]["CODIGO"];

            return agencyReturnModel;
        }

        private string CreateAgencySendingFile(AgencySendingModel agencySending)
        {
            StringBuilder builder = new StringBuilder();
            builder = builder.AppendLine("[CONSULTA]").AppendLine()
                .AppendLine(string.Format("LOJA={0}", agencySending.Loja))
                .AppendLine(string.Format("FUNC={0}", agencySending.Func))
                .AppendLine(string.Format("DATA={0}", agencySending.Data.ToString("ddMMyyyy")))
                .AppendLine(string.Format("HORA={0}", agencySending.Hora.ToString("HHmmss")))
                .AppendLine(string.Format("SOFTWARE={0}", agencySending.Software))
                .AppendLine()
                .AppendLine("[AGENCIA]")
                .AppendLine(string.Format("CODIGO={0}", agencySending.Codigo));
            return builder.ToString();
        }
        #endregion agencia

        #region venda
        public async Task<IList<string>> CreateSales(SalesSendingModel salesSending, string terminalId)
        {
            LX0 lx0 = await ParametersRepository.GetLx0();
            CF1 cf1 = await CustomerRepository.FindByCpfCnpj(lx0.Lx0cliente);
            PD5 pd5 = await MallParametersRepository.Find(MallIntegrationType.IntegraSH);
            if (pd5 == null)
                throw new Exception("Parâmetros de integração com shopping não cadastrados.");
            
            PD0 pd0 = await TerminalRepository.Find(terminalId);
            //Terminal = terminalId;
            IntegraSHPath = pd0.Pd0caminho;
            Host = pd0.Pd0host;

            salesSending.Data = DateTime.Today;
            salesSending.Hora = DateTime.Now;
            salesSending.Func = cf1.Cf1concpf.OnlyNumbers();
            salesSending.Loja = lx0.Cf1login.OnlyNumbers();
            salesSending.Software = pd5.Pd5usuario;

            var salesReturn = SendSales(salesSending);

            ValidateSendReturn(salesSending, salesReturn);
            ValidateReturn(salesReturn);
            
            return salesReturn.Autorizacoes;
        }

        private SalesReturnModel SendSales(SalesSendingModel sendingModel)
        {
            var sendContent = CreateSalesSendingFile(sendingModel);
            var integraShReturn = this.RequestSending(sendContent);
            var parser = new IniDataParser();

            parser.Configuration.SkipInvalidLines = true;
            parser.Configuration.ParseComments = false;
            IniData data = parser.Parse(integraShReturn);

            SalesReturnModel salesReturnModel = new SalesReturnModel();

            salesReturnModel.Loja = data["VENDA"]["LOJA"];
            salesReturnModel.Func = data["VENDA"]["FUNC"];
            salesReturnModel.Data = DateTime.ParseExact(data["VENDA"]["DATA"], "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
            salesReturnModel.Hora = DateTime.ParseExact(data["VENDA"]["HORA"], "HHmmss", System.Globalization.CultureInfo.InvariantCulture);
            salesReturnModel.Software = data["VENDA"]["SOFTWARE"];

            salesReturnModel.CodigoRetorno = data["RESULTADO"]["CODIGO"];
            salesReturnModel.Descricao = data["RESULTADO"]["DESCRICAO"];

            salesReturnModel.Numeroautorizacao = data["AUTORIZACAO"]["NUMERO"];

            var properties = data["AUTORIZACAO"].Where(x => x.Key.Contains("AUTORIZA"));
            if(properties!=null && properties.Any())
            {
                salesReturnModel.Autorizacoes = new List<String>();
                foreach (IniParser.Model.Property prop in properties)
                {
                    salesReturnModel.Autorizacoes.Add(prop.Value);
                }
            }
            
            return salesReturnModel;
        }

        private string CreateSalesSendingFile(SalesSendingModel salesSending)
        {
            StringBuilder builder = new StringBuilder();
            builder = builder.AppendLine("[VENDA]")
                .AppendLine(string.Format("LOJA={0}", salesSending.Loja))
                .AppendLine(string.Format("FUNC={0}", salesSending.Func))
                .AppendLine(string.Format("DATA={0}", salesSending.Data.ToString("ddMMyyyy")))
                .AppendLine(string.Format("HORA={0}", salesSending.Hora.ToString("HHmmss")))
                .AppendLine(string.Format("SOFTWARE={0}", salesSending.Software))
                .AppendLine()
                .AppendLine("[CLIENTE]")
                .AppendLine(string.Format("CODIGO={0}", salesSending.Codigo))
                .AppendLine(string.Format("RAZAO={0}", Formatters.RemoveDiacritics(salesSending.Razao)))
                .AppendLine(string.Format("FANTASIA={0}", Formatters.RemoveDiacritics(salesSending.Fantasia)))
                .AppendLine(string.Format("END={0}", Formatters.RemoveDiacritics(salesSending.End)))
                .AppendLine(string.Format("NR_END={0}", salesSending.Nr_end))
                .AppendLine(string.Format("COMPL={0}", salesSending.Compl))
                .AppendLine(string.Format("BAIRRO={0}", Formatters.RemoveDiacritics(salesSending.Bairro)))
                .AppendLine(string.Format("IBGE={0}", salesSending.Ibge))
                .AppendLine(string.Format("MUNICIPIO={0}", Formatters.RemoveDiacritics(salesSending.Municipio)))
                .AppendLine(string.Format("CEP={0}", salesSending.Cep))
                .AppendLine(string.Format("UF={0}", salesSending.Uf))
                .AppendLine(string.Format("FONE={0}", salesSending.Fone))
                .AppendLine(string.Format("CEL={0}", salesSending.Cel))
                .AppendLine(string.Format("EMAIL={0}", salesSending.Email))
                .AppendLine(string.Format("IE={0}", salesSending.Ie))
                .AppendLine(string.Format("ABERTURA={0}", salesSending.Abertura.ToString("ddMMyyyy")))
                .AppendLine(string.Format("CONTATO={0}", salesSending.Contato))
                .AppendLine(string.Format("CPF_CONTATO={0}", salesSending.Cpf_contato))

                 .AppendLine("[AGENCIA]")
                .AppendLine(string.Format("CODIGO={0}", salesSending.CodigoAgencia))

                .AppendLine("[PAGAMENTO]")
                .AppendLine(string.Format("TOTAL={0}", salesSending.Total.ToString("F2").Replace(".", "").Replace(",","").PadLeft(12, '0')))
                .AppendLine(string.Format("NUMERO={0}", salesSending.Parcelas.Count));

            for (int i = 0; i < salesSending.Parcelas.Count; i++)
            {
                Parcela parcela = salesSending.Parcelas[i];
                string installmentNumber = (i+1).ToString().PadLeft(2, '0');

                switch (parcela.Banco)
                {
                    case "000":
                        builder = builder.AppendLine(string.Format("CHEQUE{0}={1}", installmentNumber, "000000"));
                        break;
                    case "999":
                        builder = builder.AppendLine(string.Format("CHEQUE{0}={1}", installmentNumber, "999999"));
                        break;
                    default:
                        builder = builder.AppendLine(string.Format("CHEQUE{0}={1}", installmentNumber, parcela.Cheque));
                        break;
                }

                builder = builder.AppendLine(string.Format("BANCO{0}={1}", installmentNumber, parcela.Banco))
                .AppendLine(string.Format("VENCI{0}={1}", installmentNumber, parcela.Venci.ToString("ddMMyyyy")))
                .AppendLine(string.Format("VALOR{0}={1}", installmentNumber, parcela.Valor.ToString("F2").Replace(".", "").Replace(",", "").PadLeft(12, '0')));
            }

            return builder.ToString();
        }

        #endregion venda

        #region cliente
        public async Task<FindCustomerShoppingPayloadModel> FindCustomer(string cnpj, string terminalId)
        {
            LX0 lx0 = await ParametersRepository.GetLx0();
            CF1 cf1 = await CustomerRepository.FindByCpfCnpj(lx0.Lx0cliente);
            PD5 pd5 = await MallParametersRepository.Find(MallIntegrationType.IntegraSH);
            if (pd5 == null)
                throw new Exception("Parâmetros shopping não encontrados.");

            if (cf1 == null || string.IsNullOrWhiteSpace(cf1.Cf1concpf))
                throw new Exception("Configuração para integração do shopping incompleta: revise o cadastro do cliente");

            PD0 pd0 = await TerminalRepository.Find(terminalId);
            if (pd0 == null)
                throw new Exception("Terminal não encontrado: " + terminalId);
            
            //Terminal = terminalId;
            IntegraSHPath = pd0.Pd0caminho;
            Host = pd0.Pd0host;

            ClientSendingModel clientSending = new ClientSendingModel();
            clientSending.Codigo = cnpj.OnlyNumbers();

            clientSending.Data = DateTime.Today;
            clientSending.Hora = DateTime.Now;
            clientSending.Func = cf1.Cf1concpf.OnlyNumbers();
            clientSending.Loja = lx0.Cf1login.OnlyNumbers();
            clientSending.Software = pd5.Pd5usuario;

            var customerReturn = FindCustumer(clientSending);

            ValidateSendReturn(clientSending, customerReturn);

            switch (customerReturn.CodigoRetorno)
            {
                case "03":
                case "05":
                case "06":
                case "07":
                    return new FindCustomerShoppingPayloadModel()
                    {
                        Cf1 = null,
                        IsError = true,
                        Message = "<b>Retorno do shopping:</b> " + customerReturn.Descricao
                    };
            }

            cf1 = new CF1();
            cf1.Cf1cliente = customerReturn.Codigo;
            cf1.Cf1nome = customerReturn.Razao;
            cf1.Cf1fant = customerReturn.Fantasia;
            cf1.Cf1ender1 = customerReturn.End;
            cf1.Cf1numero = customerReturn.Nr_end;
            cf1.Cf1compl = customerReturn.Compl;
            cf1.Cf1bairro = customerReturn.Bairro;
            cf1.Cf3nome = customerReturn.Municipio;
            cf1.Cf1cep = customerReturn.Cep;
            cf1.Cf3estado = customerReturn.Uf;
            cf1.Cf1fone = customerReturn.Fone;
            cf1.Cf1confone = customerReturn.Cel;
            cf1.Cf1email = customerReturn.Email;
            cf1.Cf1insest = customerReturn.Ie;
            cf1.Cf1abert = customerReturn.Abertura;
            if (customerReturn.Dt_nascimento != null)
            {
                cf1.Cf1conanan = customerReturn.Dt_nascimento.Year.ToString();
                cf1.Cf1conandi = customerReturn.Dt_nascimento.Day.ToString();
                cf1.Cf1conanme = customerReturn.Dt_nascimento.Month.ToString();
            }
            if (customerReturn.CodigoRetorno.Equals("04"))
            {
                cf1.MensagensTmp = new List<string>()
                {
                    customerReturn.Descricao
                };
            }
            if (!string.IsNullOrWhiteSpace(customerReturn.Limite))
            {
                cf1.LimitShoppingIntegration = Convert.ToDecimal(customerReturn.Limite);
                cf1.LimitShoppingIntegration = cf1.LimitShoppingIntegration / 100;
            }

            //save address from shopping
            CF2 cf2 = new CF2()
            {
                Cf2cep = customerReturn.Cep,
                Cf2local = customerReturn.Ibge,
                Cf2logra = customerReturn.End
            };
            CF3 cf3 = new CF3()
            {
                Cf3local = customerReturn.Ibge,
                Cf3estado = customerReturn.Uf,
                Cf3nome = customerReturn.Municipio,
                Cf3regiao = string.Empty
            };

            await AddressService.VerifyAndSaveAddressFromShopping(cf2, cf3);

            return new FindCustomerShoppingPayloadModel() { Cf1 = cf1 };
        }

        private ClientReturnModel FindCustumer(ClientSendingModel sendingModel)
        {
            var sendContent = createCustumerSendingFile(sendingModel);
            var integraShReturn = this.RequestSending(sendContent);
            var fileLines = integraShReturn.Split("\r\n");

            ClientReturnModel clientReturnModel = new ClientReturnModel();
            foreach (string line in fileLines)
            {
                var lineContent = line.Split("=");
                if (lineContent.Length > 1)
                {
                    switch (lineContent[0].ToString())
                    {
                        case "LOJA":
                            clientReturnModel.Loja = lineContent[1];
                            break;
                        case "FUNC":
                            clientReturnModel.Func = lineContent[1];
                            break;
                        case "DATA":
                            clientReturnModel.Data = DateTime.ParseExact(lineContent[1], "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        case "HORA":
                            clientReturnModel.Hora = DateTime.ParseExact(lineContent[1], "HHmmss", System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        case "SOFTWARE":
                            clientReturnModel.Software = lineContent[1];
                            break;
                        case "CODIGO":
                            if (lineContent[1].Length == 2)
                            {
                                clientReturnModel.CodigoRetorno = lineContent[1];
                                break;
                            }
                                
                            if (lineContent[1].Length > 10)
                                clientReturnModel.Codigo = lineContent[1];
                            else
                                clientReturnModel.Agencia = lineContent[1];
                            break;
                        case "DESCRICAO":
                            clientReturnModel.Descricao = lineContent[1];
                            break;
                        case "RAZAO":
                            clientReturnModel.Razao = lineContent[1];
                            break;
                        case "FANTASIA":
                            clientReturnModel.Fantasia = lineContent[1];
                            break;
                        case "END":
                            clientReturnModel.End = lineContent[1];
                            break;
                        case "NR_END":
                            clientReturnModel.Nr_end = lineContent[1];
                            break;
                        case "COMPL":
                            clientReturnModel.Compl = lineContent[1];
                            break;
                        case "BAIRRO":
                            clientReturnModel.Bairro = lineContent[1];
                            break;
                        case "IBGE":
                            clientReturnModel.Ibge = lineContent[1];
                            break;
                        case "MUNICIPIO":
                            clientReturnModel.Municipio = lineContent[1];
                            break;
                        case "CEP":
                            clientReturnModel.Cep = lineContent[1];
                            break;
                        case "UF":
                            clientReturnModel.Uf = lineContent[1];
                            break;
                        case "FONE":
                            clientReturnModel.Fone = lineContent[1];
                            break;
                        case "CEL":
                            clientReturnModel.Cel = lineContent[1];
                            break;
                        case "EMAIL":
                            clientReturnModel.Email = lineContent[1];
                            break;
                        case "IE":
                            clientReturnModel.Ie = lineContent[1];
                            break;
                        case "ABERTURA":
                            if (!string.IsNullOrWhiteSpace(lineContent[1]))
                                clientReturnModel.Abertura = DateTime.ParseExact(lineContent[1], "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        case "CONTATO":
                            clientReturnModel.Contato = lineContent[1];
                            break;
                        case "CPF_CONTATO":
                            clientReturnModel.Cpf_contato = lineContent[1];
                            break;
                        case "DT_NASCIMENTO":
                            if (!string.IsNullOrWhiteSpace(lineContent[1]))
                                clientReturnModel.Dt_nascimento = DateTime.ParseExact(lineContent[1], "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        case "BLOQUEADO":
                            clientReturnModel.Bloqueado = lineContent[1];
                            break;
                        case "INATIVO":
                            clientReturnModel.Inativo = lineContent[1];
                            break;
                        case "LIMITE":
                            clientReturnModel.Limite = lineContent[1];
                            break;
                        case "OBS":
                            clientReturnModel.Obs = lineContent[1];
                            break;
                        case "INF_BANCO":
                            clientReturnModel.Inf_banco = lineContent[1];
                            break;
                    }
                }
            }
            return clientReturnModel;
        }

        private string createCustumerSendingFile(ClientSendingModel custumerSending)
        {
            StringBuilder builder = new StringBuilder();
            builder = builder.AppendLine("[CONSULTA]").AppendLine()
                .AppendLine(string.Format("LOJA={0}", custumerSending.Loja))
                .AppendLine(string.Format("FUNC={0}", custumerSending.Func))
                .AppendLine(string.Format("DATA={0}", custumerSending.Data.ToString("ddMMyyyy")))
                .AppendLine(string.Format("HORA={0}", custumerSending.Hora.ToString("HHmmss")))
                .AppendLine(string.Format("SOFTWARE={0}", custumerSending.Software))
                .AppendLine()
                .AppendLine()
                .AppendLine("[CLIENTE]")
                .AppendLine(string.Format("CODIGO={0}", custumerSending.Codigo));
            return builder.ToString();
        }

        #endregion cliente

        #region file maniipulation
        private string RequestSending(string content)
        {
            //IntegraSHPath = CreateTerminalIntegrationFiles();

            string pathStringReturnFile = Path.Combine(IntegraSHPath, RETURN_FILE_NAME);
            string pathStringSendFile = Path.Combine(IntegraSHPath, SEND_FILE_NAME);
            if (File.Exists(pathStringSendFile))
            {
                File.Delete(pathStringSendFile);
            }
            if (File.Exists(pathStringReturnFile))
            {
                File.Delete(pathStringReturnFile);
            }

            if (CreateSendingFile(content))
            {
                if (ExecuteIntegraSH())
                {
                    return GetReturnFile();
                }
                throw new Exception("Erro ao executar integração com IntegraSH");

            }
            throw new Exception("Não foi possível criar arquivo de envio para integração com IntegraSH");
        }

        private bool ExecuteIntegraSH()
        {
            string softwarePath = Path.Combine(IntegraSHPath, SOFTWARE_NAME);
            if (!File.Exists(softwarePath))
            {
                throw new Exception("Não foi possível localizar o programa para integração com IntegraSH: " + SOFTWARE_NAME);
            }
            string user = configuration["SSHUser"];
            string password = configuration["SSHPassword"];
            using (SshClient ssh = new SshClient(Host, user, password))
            {
                ssh.Connect();
                var x = ssh.RunCommand(softwarePath);
                return true;
            }
        }

        private bool CreateSendingFile(string content)
        {
            string pathFile = Path.Combine(IntegraSHPath, SEND_FILE_NAME);
            try
            {
                using (FileStream fs = File.Create(pathFile))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(content);
                    fs.Write(info, 0, info.Length);
                }
                return File.Exists(pathFile);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao criar arquivo de remessa de integração com IntegraSH: " + ex.Message);
            }
        }

        private string GetReturnFile()
        {
            string pathString = Path.Combine(IntegraSHPath, RETURN_FILE_NAME);
            int waitedTime = 0;
            do
            {
                if (File.Exists(pathString))
                {
                    return File.ReadAllText(pathString, Encoding.GetEncoding(1252));
                }
                else
                {
                    //foreach (ProcessThread thread in Process.Threads)
                    //{
                    //    using (FileStream fs = File.Create("C:/integrash/001/log.txt"))
                    //    {
                    //        // Adding some info into the file
                    //        byte[] info = new UTF8Encoding(true).GetBytes(string.Format("{0} {1}", thread.ThreadState.ToString(), thread.WaitReason.ToString()));
                    //        fs.Write(info, 0, info.Length);
                    //    }
                    //    if (thread.ThreadState == System.Diagnostics.ThreadState.Wait
                    //        && thread.WaitReason == ThreadWaitReason.UserRequest)
                    //    {
                    //        Process.StandardInput.Close();
                    //    }
                    //} 
                    Thread.Sleep(DELAY_RETURN_FILE_SEARCH);
                    waitedTime += DELAY_RETURN_FILE_SEARCH;
                }
            } while (waitedTime < TIMEOUT_RETURNFILE_MILLIS);
            throw new Exception("Arquivo de retorno não foi encontrado");
        }
        
        /*
        private string CreateTerminalIntegrationFiles()
        {
            if (!Directory.Exists(IntegraSHPath))
            {
                throw new Exception("Integração com shopping não está configurada: não foi possível encontrar o caminho " + IntegraSHPath);
            }
            string pathTerminalFile = IntegraSHPath + Terminal;
            if (!Directory.Exists(pathTerminalFile))
            {
                Directory.CreateDirectory(pathTerminalFile);
                string[] files = Directory.GetFiles(IntegraSHPath);
                foreach(String file in files)
                {
                    string fileName = file.Split(Path.DirectorySeparatorChar).Last();
                    File.Copy(file, Path.Combine(pathTerminalFile, fileName), true);
                }

                UpdateIniFile(pathTerminalFile);
            }
            return pathTerminalFile;
        }

        private void UpdateIniFile(string pathTerminalFile)
        {
            string configuration = File.ReadAllText(Path.Combine(pathTerminalFile, "CODIGOSH.INI"));
            var fileLines = configuration.Split("\r\n");
            string confAux = "";
            foreach (string line in fileLines)
            {
                if (line.Contains("CAMINHO"))
                {
                    confAux = confAux +  "CAMINHO=" + pathTerminalFile + "\r\n";
                    continue;
                }
                confAux = confAux + line + "\r\n";
            }
            File.WriteAllText(Path.Combine(pathTerminalFile, "CODIGOSH.INI"), confAux);
        }
        */
        #endregion file maniipulation


        private void ValidateSendReturn(IntegraShSendModel sendModel, IntegraShReturnModel returnModel)
        {
            if (sendModel.Data.ToString("ddMMyyyy") != returnModel.Data.ToString("ddMMyyyy")
               || sendModel.Hora.ToString("HHmmss") != returnModel.Hora.ToString("HHmmss")
               || sendModel.Func != returnModel.Func
               || sendModel.Loja != returnModel.Loja)
            {
                throw new Exception("Retorno da consulta diverge com a busca.");
            }
        }

        private void ValidateReturn(IntegraShReturnModel integraShReturn)
        {
            /*
              00 Ok 
               03 Inativo Agência ou Cliente 
               04 Bloqueado Agência ou Cliente 
               05 Limites Excedidos 
               06 Excedeu Limite de dias do Prazo ou Valor da folha excedeu o limite - cheque 
               07 Cadastro não consta na base Agência ou Cliente 
               */

            if (integraShReturn.CodigoRetorno.Equals("03") ||
                integraShReturn.CodigoRetorno.Equals("04"))
            {
                throw new Exception(integraShReturn.Descricao);
            }
        }

        public string GetIntegraBankName(CodNFCE cv8codnfce)
        {
            switch (cv8codnfce)
            {
                case CodNFCE.Nenhum:
                case CodNFCE.Dinheiro:
                case CodNFCE.Crediario:
                case CodNFCE.CashBack:
                case CodNFCE.DepositoBancario:
                case CodNFCE.TransferenciaBancaria:
                case CodNFCE.Outros:
                    return "000";

                case CodNFCE.CartaoDeCredito:
                case CodNFCE.CartaoDeDebito:
                case CodNFCE.ValeAlimentacao:
                case CodNFCE.ValeRefeicao:
                case CodNFCE.ValePresente:
                case CodNFCE.ValeCombustivel:
                    return "999";
                default:
                    return "NNN";
            }
        }

    }
}
