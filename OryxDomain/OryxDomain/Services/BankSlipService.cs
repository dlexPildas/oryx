using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class BankSlipService
    {
        private readonly IConfiguration Configuration;
        private readonly FiscalDocumentRepository FiscalDocumentRepository;
        private readonly BankAccountsRepository BankAccountsRepository;
        private readonly SisService SisService;
        private readonly BondRemittanceRepository BondRemittanceRepository;
        private readonly PrintingPreferencesRepository PrintingPreferencesRepository;
        private readonly PrintService PrintService;
        public BankSlipService(IConfiguration configuration)
        {
            Configuration = configuration;
            FiscalDocumentRepository = new FiscalDocumentRepository(Configuration["OryxPath"] + "oryx.ini");
            BankAccountsRepository = new BankAccountsRepository(Configuration["OryxPath"] + "oryx.ini");
            BondRemittanceRepository = new BondRemittanceRepository(Configuration["OryxPath"] + "oryx.ini");
            PrintingPreferencesRepository = new PrintingPreferencesRepository(Configuration["OryxPath"] + "oryx.ini");
            SisService = new SisService(Configuration);
            PrintService = new PrintService(Configuration);
        }

        public async Task<PayloadEmitBankSlipModel> Emit(CV5 cv5, string lastValidatedInstallment, StepEmitBankSlipValidation lastStep, string terminal, string authorization, bool print)
        {
            PayloadEmitBankSlipModel payloadEmitBankSlip = new PayloadEmitBankSlipModel()
            {
                InstallmentStep = lastValidatedInstallment,
                Step = lastStep,
                RelatsPath = new List<string>()
            };
            string cv8agente;
            string cv8conta;

            IList<CV8> lstCv8 = await FiscalDocumentRepository.FindAllCv8(cv5.Cv5doc, cv5.Cv5emissor, cv5.Cv5tipo);

            if (lstCv8 == null || lstCv8.Count == 0)
                return payloadEmitBankSlip;

            //pegar apenas parcelas de boleto ou crediario
            lstCv8 = lstCv8.Where(cv8 => cv8.Cv8codnfce == CodNFCE.BlqBancario || cv8.Cv8codnfce == CodNFCE.Crediario).ToList();
            if (lstCv8 == null || lstCv8.Count == 0)
                return payloadEmitBankSlip;

            //validar se existe os parâmetros de boleto (banco e conta)
            IList<string> bankAccounts = await BankAccountsRepository.FindAgentForBankSlip();
            if (bankAccounts == null ||  bankAccounts.Count == 0)
                throw new Exception("Nenhum agente definido para geração de boleto");

            cv8agente = bankAccounts.First();

            IList<string> accounts = await BankAccountsRepository.FindAccountForBankSlip(bankAccounts.First());
            if (accounts == null || accounts.Count == 0)
                throw new Exception("Nenhuma conta definida para geração de boleto");

            cv8conta = accounts.First();

            try
            {
                foreach (CV8 cv8 in lstCv8)
                {
                    if (Convert.ToInt32(cv8.Cv8parcela) > Convert.ToInt32(lastValidatedInstallment))
                        continue;

                    payloadEmitBankSlip.InstallmentStep = cv8.Cv8parcela;

                    //validações da parcela para boleto
                    decimal valor = await SisService.SaldoTitulo(cv8.Cv8emissor, cv8.Cv8tipo, cv8.Cv8doc, cv8.Cv8parcela, cv8.Cv8valor);

                    if (valor == 0)
                        throw new Exception(string.Format("Título {0}-{1} sem valor ou liquidado.", cv8.Cv8doc, cv8.Cv8parcela));

                    //step titulo vencido
                    if (cv8.Cv8vencim < DateTime.Now && 
                        (int)StepEmitBankSlipValidation.EXPIRED_DATE > (int)lastStep)
                    {
                        payloadEmitBankSlip.Step = StepEmitBankSlipValidation.EXPIRED_DATE;
                        throw new Exception(string.Format("Título {0}-{1} vencido. Continuar ?", cv8.Cv8doc, cv8.Cv8parcela));
                    }
                
                    //step agente
                    string agent = await BondRemittanceRepository.FindAgentOfBond(cv8.Cv8emissor, cv8.Cv8tipo, cv8.Cv8doc, cv8.Cv8parcela);
                    if (!string.IsNullOrWhiteSpace(agent) &&
                        (int)StepEmitBankSlipValidation.AGENT > (int)lastStep)
                    {
                        payloadEmitBankSlip.Step = StepEmitBankSlipValidation.AGENT;
                        throw new Exception(string.Format("Título {0}-{1} já possui portador. Continuar?", cv8.Cv8doc, cv8.Cv8parcela));
                    }

                    if (!string.IsNullOrWhiteSpace(cv8.Cv8barras))
                    {
                        //analisando banco bloqueto anterior
                        if (cv8.Cv8agente != cv8agente)
                        {
                            payloadEmitBankSlip.Step = StepEmitBankSlipValidation.VALIDATIONS;
                            throw new Exception(string.Format("Bloqueto {0}-{1} emitido por outro banco. ({2})", cv8.Cv8doc, cv8.Cv8parcela, cv8.Cv8agente));
                        }
                        if (cv8.Cv8conta != cv8conta)
                        {
                            payloadEmitBankSlip.Step = StepEmitBankSlipValidation.VALIDATIONS;
                            throw new Exception(string.Format("Bloqueto {0}-{1} emitido por outra conta. ({2})", cv8.Cv8doc, cv8.Cv8parcela, cv8.Cv8conta));
                        }
                        // conferindo valor
                        string strValue = ((valor * 100) + 10000000000).ToString().Substring(1, 10);
                        if (cv8.Cv8barras.Substring(9, 10) != strValue)
                        {
                            payloadEmitBankSlip.Step = StepEmitBankSlipValidation.VALIDATIONS;
                            string oldValue = (Convert.ToDecimal(cv8.Cv8barras.Substring(9, 10)) /100).ToString("C");
                            throw new Exception(string.Format("Bloqueto {0}-{1} emitido com outro valor. ({2})", cv8.Cv8doc, cv8.Cv8parcela, oldValue));
                        }

                        // conferindo vencimento
                        DateTime baseDate = new DateTime(1997, 10, 7);
                        int vFactor = (int)(cv8.Cv8vencim - baseDate).TotalDays;

                        if (cv8.Cv8barras.Substring(5,4) != vFactor.ToString())
                        {
                            payloadEmitBankSlip.Step = StepEmitBankSlipValidation.VALIDATIONS;
                            throw new Exception(string.Format("Bloqueto {0}-{1} emitido com outro vencimento. ({2})", cv8.Cv8doc, cv8.Cv8parcela, baseDate.AddDays(Convert.ToInt32(cv8.Cv8barras.Substring(5, 4))).ToString("dd/MM/yyyy")));
                        }
                    }
                }

                payloadEmitBankSlip.Step = StepEmitBankSlipValidation.VALIDATIONS;

                //chamar o relatório do boleto para todas parcelas
                bool reimprimir = lstCv8.Where(cv8 => !string.IsNullOrWhiteSpace(cv8.Cv8barras) && cv8.Cv8impres).Any();

                string relatPath = await PrintBankSlip(cv5.Cv5tipo, cv5.Cv5doc, lstCv8.First().Cv8parcela, lstCv8.Last().Cv8parcela, reimprimir, terminal, authorization, print);
                if (!string.IsNullOrWhiteSpace(relatPath))
                    payloadEmitBankSlip.RelatsPath.Add(relatPath);
            }
            catch (Exception ex)
            {
                payloadEmitBankSlip.IsError = true;
                payloadEmitBankSlip.ErrorMessage = ex.Message;
                return payloadEmitBankSlip;
            }

            return payloadEmitBankSlip;
        }

        public async Task<string> PrintBankSlip(string cv8tipo, string cv8doc, string firstCv8parcela, string lastCv8parcela, bool reprint, string terminal, string authorization, bool print)
        {
            string fileName;
            string pd1impres = "Bullzip PDF Printer";
            PD1 pd1 = null;
            decimal vias = 1;
            string cv1relat = "282";

            //pegar o relatório a ser impresso do modelo de documento fiscal

            if (print)
            {
                pd1 = await PrintingPreferencesRepository.Find(terminal, cv1relat);
                if (pd1 == null)
                {
                    print = false;
                    //throw new Exception(string.Format("impressora não definida para o terminal {0} e relatório {1}", terminal, cv1relat));
                }
                else
                {
                    if (pd1.Pd1vias == 0)
                        throw new Exception(string.Format("Número de vias não definido para o terminal {0} e relatório {1}", terminal, cv1relat));
                    pd1impres = pd1.Pd1impres;
                    vias = pd1.Pd1vias;
                }
            }

            PrintModel printModel = new PrintModel()
            {
                Relat = cv1relat
            };

            fileName = string.Format("RELAT_{0}_{1}{2}{3}.pdf", cv1relat, cv8tipo, cv8doc, DateTime.Now.Ticks.ToString());
            printModel.Params = string.Format("{0};{1};{2};{3};{4};{5};{6};{7}", pd1impres, print ? "N" : "S", cv8tipo, cv8doc, firstCv8parcela, lastCv8parcela, reprint ? "1" : "0", fileName);

            for (int i = 0; i < vias; i++)
            {
                await PrintService.Print(printModel, authorization);
            }

            return print ? string.Empty : "/Printing/Print/Document/" + fileName;
        }  
    }
}
