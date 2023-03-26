using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class FiscalService
    {
        #region Resources instances
        private readonly IConfiguration Configuration;
        private readonly SisService SisService;
        private readonly PriceService PriceService;
        private readonly PriceListRepository PriceListRepository;
        private readonly PaymentConditionRepository PaymentConditionRepository;
        private readonly BusinessOperationRepository BusinessOperationRepository;
        private readonly NaturesOfOperationRepository NaturesOfOperationRepository;
        private readonly FiscalDocumentTypeRepository FiscalDocumentTypeRepository;
        private readonly ParametersRepository ParametersRepository;
        private readonly CustomerRepository CustomerRepository;
        private readonly FiscalDocumentRepository FiscalDocumentRepository;
        private readonly FiscalParametersRepository FiscalParametersRepository;
        private readonly LogisticIntegrationService FinancialParametersRepository;
        private readonly BillingAddressRepository BillingAddressRepository;
        private readonly ShippingAddressRepository ShippingAddressRepository;
        private readonly RepresentativeRepository RepresentativeRepository;
        private readonly TransporterRepository TransporterRepository;
        private readonly ProductRepository ProductRepository;
        private readonly StockService StockService;
        private readonly SupplyRepository SupplyRepository;
        private readonly ProductSizesRepository ProductSizesRepository;
        private readonly FiscalClassificationRepository FiscalClassificationRepository;
        private readonly DictionaryRepository DictionaryRepository;
        private readonly SpecificTaxesRepository SpecificTaxesRepository;
        private readonly TitleRepository TitleRepository;
        private readonly FormatterService FormatterService;
        private readonly PDVParametersRepository PDVParametersRepository;
        private readonly AddressService AddressService;
        #endregion

        #region variables
        public string nomeDoCertificado;
        public bool oryxnfe;
        public LXA lxa;
        public LX3 lx3;
        public LX0 lx0;
        public LXD lxd;
        public OryxNFeModel oryxNFeModel;
        private int arredondar;
        private bool naocalcularbaseicms;
        public bool contigencia;
        readonly ILogger Logger;
        #endregion
        public FiscalService(IConfiguration configuration, ILogger _logger)
        {
            Configuration = configuration;
            PriceListRepository = new PriceListRepository(Configuration["OryxPath"] + "oryx.ini");
            PaymentConditionRepository = new PaymentConditionRepository(Configuration["OryxPath"] + "oryx.ini");
            BusinessOperationRepository = new BusinessOperationRepository(Configuration["OryxPath"] + "oryx.ini");
            NaturesOfOperationRepository = new NaturesOfOperationRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalDocumentTypeRepository = new FiscalDocumentTypeRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            CustomerRepository = new CustomerRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalDocumentRepository = new FiscalDocumentRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalParametersRepository = new FiscalParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            FinancialParametersRepository = new FinancialParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            BillingAddressRepository = new BillingAddressRepository(Configuration["OryxPath"] + "oryx.ini");
            ShippingAddressRepository = new ShippingAddressRepository(Configuration["OryxPath"] + "oryx.ini");
            RepresentativeRepository = new RepresentativeRepository(Configuration["OryxPath"] + "oryx.ini");
            TransporterRepository = new TransporterRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductRepository = new ProductRepository(Configuration["OryxPath"] + "oryx.ini");
            SupplyRepository = new SupplyRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductSizesRepository = new ProductSizesRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalClassificationRepository = new FiscalClassificationRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
            SpecificTaxesRepository = new SpecificTaxesRepository(Configuration["OryxPath"] + "oryx.ini");
            TitleRepository = new TitleRepository(Configuration["OryxPath"] + "oryx.ini");
            PDVParametersRepository = new PDVParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            StockService = new StockService(Configuration);
            SisService = new SisService(configuration);
            PriceService = new PriceService(configuration);
            FormatterService = new FormatterService(configuration);
            AddressService = new AddressService(Configuration);
            Init().Wait();
            naocalcularbaseicms = false;
            Logger = _logger;
        }

        private async Task Init()
        {
            //identificando existencia de OryxNFe
            oryxNFeModel = await ParametersRepository.FindOryxNFE();
            lx3 = await FiscalParametersRepository.Find();
            //pegando parametros financeiros
            lxa = await FinancialParametersRepository.Find();
            if (lxa == null)
                throw new Exception("Parâmetros financeiros não cadastrados");
            
            if (lx3 == null)
                throw new Exception("Parâmetros fiscais não cadastrados");
            lx0 = await ParametersRepository.GetLx0();

            lxd = await PDVParametersRepository.Find();

            nomeDoCertificado = oryxNFeModel.Lx0certif.Trim();
            if (!string.IsNullOrWhiteSpace(oryxNFeModel.Lx0certif2))
                nomeDoCertificado += oryxNFeModel.Lx0certif2.Trim();

            /*
            IF !DIRECTORY(this.reposnf)
	            mkdir(this.reposnf)
            ENDIF
            IF DIRECTORY(this.reposnf+"email")=.f.
	            mkdir(this.reposnf+"email")
            ENDIF
            */
            oryxnfe = true;
            arredondar = 3 - (int)lxa.Lxaarredon;
            if (lxa.Lxaarredon == RoundType.NONE || lxa.Lxaarredon == RoundType.UNDEFINED)
            {
                arredondar = 2;
            }
        }

        public async Task<bool> CondicaoValida(string conpgto, string lista)
        {
            CV6 cv6 = await PriceListRepository.Find(lista);
            int prazoMedio = await SisService.PrazoMedio(conpgto);

            if (cv6 != null && cv6.Cv6limpraz >= 0 && prazoMedio > cv6.Cv6limpraz)
            {
                throw new Exception(string.Format("Prazo médio ({0} dias) excedido conforme lista aplicada. ({1} dias)", prazoMedio, cv6.Cv6limpraz));
            }
            return true;
        }

        public async Task<CV5> CondicaoValida(CV5 cv5)
        {
            VD4 vd4 = await PaymentConditionRepository.Find(cv5.Cv5conpgto);
            if (vd4 != null)
            {
                cv5.Cv5conpnom = vd4.Vd4nome;
                cv5.Cv5titulo = vd4.Vd4titulo;
            }
            return cv5;
        }

        public async Task<CV5> ListaValida(CV5 cv5)
        {
            CV6 cv6 = await PriceListRepository.Find(cv5.Cv5lista);
            if (cv6 != null)
            {
                cv5.Cv5nomlis = cv6.Cv6nome;
                cv5.Cv5limdesc = cv6.Cv6limdesc;
                cv5.Cv5limdesp = cv6.Cv6limdesp;
                cv5.Cv5limdias = cv6.Cv6limdias;
                cv5.Cv5limpraz = cv6.Cv6limpraz;
            }
            return cv5;
        }

        public async Task<CV5> OpercomValida(CV5 cv5, IList<CV7> lstCv7, FatherFieldType campopai, OryxModuleType module, bool aceitarCfop = false, bool informarCliente = false)
        {
            CV3 cv3 = await BusinessOperationRepository.Find(cv5.Cv5opercom);
            if (cv3 != null)
            {
                if (cv3.Cv3inativa){
                    throw new Exception(string.Format("Operacao comercial {0} inativa.", cv3.Cv3opercom));
                }

                cv5.Cv5contad = cv3.Cv3contad;
                cv5.Cv5contac = cv3.Cv3contac;
                cv5.Cv5lista = cv3.Cv3lista;
                cv5.Cv5conpgto = cv3.Cv3conpgto;
                cv5.Cv5editar = cv3.Cv3editar;
                cv5.Cv5observ = cv3.Cv3observ;
                if (!string.IsNullOrWhiteSpace(cv3.Cv3cliente) || (cv3.Cv3cupom && informarCliente))
                {
                    cv5.Cv5cliente = cv3.Cv3cliente;
                }

                if(cv5.Cv5frete == FreightType.NONE)
                    cv5.Cv5frete = cv3.Cv3frete;
                cv5.Cv5tamanho = cv3.Cv3tamanho;
                cv5.Cv5ean = cv3.Cv3ean;
                cv5.Cv5cupom = cv3.Cv3cupom;
                cv5.Cv5totaliz = cv3.Cv3totaliz;
                cv5.Cv5aliqicm = cv3.Cv3aliqicm;
                cv5.Cv5aliqsub = cv3.Cv3aliqsub;
                cv5.Cv5basesup = cv3.Cv3basesub;
                cv5.Cv5naocont = cv3.Cv3naocont;
                cv5.Cv5ajuste = cv3.Cv3ajuste;
                cv5.Cv5credpis = cv3.Cv3credpis;
                cv5.Cv5credcof = cv3.Cv3credcof;
                cv5.Cv5credicm = cv3.Cv3credicm;
                cv5.Cv5tottri1 = cv3.Cv3tottri1;
                cv5.Cv5cbenef = cv3.Cv3cbenef;

                foreach (CV7 cv7 in lstCv7.Where(c => c.Cv7emissor == cv5.Cv5emissor && c.Cv7tipo == cv5.Cv5tipo && c.Cv7doc == cv5.Cv5doc))
                {
                    cv7.Cv7aliqicm = cv3.Cv3aliqicm;
                    cv7.Cv7aliqsub = cv3.Cv3aliqsub;
                    cv7.Cv7cst = cv3.Cv3cst;
                    cv7.Cv7origem = cv3.Cv3origem;
                    if (!aceitarCfop)
                    {
                        cv7.Cv7cfop = cv3.Cv3cfop;
                    }
                }

                cv5.Cv5baseicp = cv3.Cv3baseicm;
                cv5.Cv5baseisp = cv3.Cv3baseise;
                cv5.Cv5baseoup = cv3.Cv3baseout;
                cv5.Cv5pdif = cv3.Cv3pdif;
                cv5.Cv5operdes = cv3.Cv3nome;
                cv5.Cv5tipo = cv3.Cv3docfis;
                cv5.Cv5cfop = cv3.Cv3cfop;
                cv5.Cv5entsai = cv3.Cv3entsai;
                cv5.Cv5emispro = cv3.Cv3emissao;
                cv5.Cv5cic = cv3.Cv3cic;
                cv5.Cv5cnpj = cv3.Cv3cnpj;
                cv5.Cv5itemfab = cv3.Cv3itemfab;
                cv5.Cv5consig = cv3.Cv3consig;
                cv5.Cv5credsim = cv3.Cv3credsim;
                cv5.Cv5aliqipi = cv3.Cv3aliqipi;
                cv5.Cv5cst = cv3.Cv3cst;
                cv5.Cv5origem = cv3.Cv3origem;
                cv5.Cv5peca = cv3.Cv3peca;
                FI1 fi1 = await NaturesOfOperationRepository.Find(cv3.Cv3cfop);
                cv5.Cv5cfopnom = fi1.Fi1nome;
                CV1 cv1 = await FiscalDocumentTypeRepository.Find(cv5.Cv5tipo);
                cv5.Cv5modelo = cv1.Cv1modelo;
                cv5.Cv5subser = cv1.Cv1subser;
                cv5.Cv5serie = cv1.Cv1serie;
                LX0 lx0 = await ParametersRepository.GetLx0();
                if (cv3.Cv3emissao)
                {
                    cv5.Cv5emissor = lx0.Lx0cliente;
                    cv5 = await EmissorValido(cv5);
                }
                else
                {
                    //gravando cliente se for entrada ou desembolso
                    if (cv3.Cv3entsai == GoodsFlowType.GOODS_ENTRY || cv3.Cv3entsai == GoodsFlowType.DISBURSEMENT)
                    {
                        cv5.Cv5cliente = lx0.Lx0cliente;
                    }
                }
                cv5.Cv5espevol = cv3.Cv3tipovol.GetEnumDescription();
                cv5.Cv5marcvol = cv3.Cv3marcvol;
                cv5.Cv5cstipi = cv3.Cv3cstipi;
                cv5.Cv5enqipi = cv3.Cv3enqipi;
                cv5.Cv5cstpis = cv3.Cv3cstpis;
                cv5.Cv5basepis = cv3.Cv3basepis;
                cv5.Cv5aliqpis = cv3.Cv3aliqpis;
                cv5.Cv5cstcof = cv3.Cv3cstcof;
                cv5.Cv5basecof = cv3.Cv3basecof;
                cv5.Cv5aliqcof = cv3.Cv3aliqcof;
                cv5.Cv5baseiss = cv3.Cv3baseiss;
                cv5.Cv5aliqiss = cv3.Cv3aliqiss;
                cv5.Cv5codiss = cv3.Cv3codiss;
                if (!string.IsNullOrWhiteSpace(cv5.Cv5lista))
                    await ListaValida(cv5);
                
                if (!string.IsNullOrWhiteSpace(cv5.Cv5cliente))
                    await ClienteValido(cv5, null, module);

                if (!string.IsNullOrWhiteSpace(cv5.Cv5conpgto))
                    await CondicaoValida(cv5.Cv5conpgto, cv5.Cv5lista);

                //validações no validaLocal do cv5opercom
                if (campopai == FatherFieldType.CV5ATENDE)
                {
                    //TODO
                    throw new NotImplementedException();
                }
                if (campopai == FatherFieldType.CV5DOCDEV && cv5.Cv5entsai != GoodsFlowType.GOODS_ENTRY)
                {
                    throw new Exception("Operação deve ser de entrada.");
                }

                if (cv5.Cv5emispro)
                {
                    switch (lx0.Lx0crt)
                    {
                        case TaxRegimeType.SIMPLE_NATIONAL:
                        case TaxRegimeType.SIMPLE_NATIONAL_EXCESS:
                            if (cv5.Cv5cst.Length < 3)
                                throw new Exception("Cst ICMS invalida para o regime tributario (simples).");
                            break;
                        case TaxRegimeType.NORMAL_REGIME:
                            if (cv5.Cv5cst.Length >= 3)
                                throw new Exception("Cst ICMS invalida para o regime tributario (normal).");
                            break;
                        default:
                            throw new Exception("Codigo do regime tributario indefinido em parametros.");
                    }
                }

                if (module == OryxModuleType.ORYX_PLACE)
                {
                    throw new NotImplementedException();
                    //if (campopai == FatherFieldType.CV5ORDEM)
                    //{
                    //    //TODO
                    //}
                    //if (campopai == FatherFieldType.CV5MEBID)
                    //{
                    //    //TODO
                    //}
                }

                //validações de NFC-e
                if(cv1.Cv1modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CUPOM_FISCAL_ELETRÔNICO")) &&
                    (string.IsNullOrWhiteSpace(lx0.Lx0token) ||
                    lx0.Lx0csc == new Guid()))
                {
                    throw new Exception("Para emissão de Cupom Fiscal, é necessário informar um código de segurança do contribuinte (token NFC-e)");
                }

                //fim das validações
            }
            return cv5;
        }

        public async Task<CV5> EmissorValido(CV5 cv5)
        {
            CF1 cf1 = await CustomerRepository.FindByCpfCnpj(cv5.Cv5emissor);
            if (cf1 != null && string.IsNullOrWhiteSpace(cf1.Cf3estado))
            {
                throw new Exception("Emissor sem CEP ou inválido");
            }

            cv5.Cv5emisnom = cf1.Cf1nome;
            cv5.Cv5emisest = cf1.Cf3estado;
            return cv5;
        }

        public async Task<string> ProximoNumero(CV5 cv5, string tipo = "")
        {
            if (string.IsNullOrWhiteSpace(tipo))
                tipo = cv5.Cv5tipo;
            string proximoNumero = await FiscalDocumentRepository.FindLastDocByCv5Tipo(tipo);
            
            if (string.IsNullOrWhiteSpace(proximoNumero))
                proximoNumero = "0";

            int lengthCv5doc = await FormatterService.FindFieldLength(nameof(cv5.Cv5doc));

            int plusValue = Convert.ToInt32(string.Format("1{0}1", "0".PadLeft(lengthCv5doc - 1, '0')));

            proximoNumero = (Convert.ToInt64(proximoNumero) + plusValue).ToString().Substring(1, lengthCv5doc);

            string cv1ultimo = await FiscalDocumentTypeRepository.FindLastNumber(tipo);
            cv1ultimo = string.IsNullOrWhiteSpace(cv1ultimo) ? "0" : cv1ultimo;
            if (Convert.ToInt64(cv1ultimo) >= Convert.ToInt64(proximoNumero))
                return (Convert.ToInt64(cv1ultimo) + plusValue).ToString().Substring(1, lengthCv5doc);
            return proximoNumero;
        }

        public async Task<CV5> ClienteValido(CV5 cv5, IList<CVJ> lstCVJ, OryxModuleType module, bool dadosAlterados = false)
        {
            CF1 cf1 = await CustomerRepository.FindByCpfCnpj(cv5.Cv5cliente);
            if (cf1 == null && module == OryxModuleType.ORYX_PV)
            {
                if (string.IsNullOrWhiteSpace(lxd.Lxdcliente))
                    throw new Exception("Cliente padrão não definido. Entre em contato com o suporte técnico.");
                cf1 = await CustomerRepository.FindByCpfCnpj(lxd.Lxdcliente);
            }

            if (!dadosAlterados)
            {
                cv5.Cv5clinome = cf1.Cf1nome;
                cv5.Cv5endcli = cf1.Cf1ender1;
                cv5.Cv5cep = cf1.Cf1cep;
                cv5.Cv5baicli = cf1.Cf1bairro;
                cv5.Cv5numcli = cf1.Cf1numero;
                cv5.Cv5compl = cf1.Cf1compl;

                //verificando se há endereço de entrega / cobrança para o caso de saida de mercadorias
                if (cv5.Cv5entsai == GoodsFlowType.GOODS_EXIT && lstCVJ != null)
                {
                    lstCVJ = lstCVJ.Where(cvj => !cvj.Cvjlinha.Equals("T") && !cvj.Cvjlinha.Equals("U") && !cvj.Cvjlinha.Equals("V")).ToList();
                    if (lx3.Lx3entrega == PrintAddressType.BILLINGADDRESS || lx3.Lx3entrega == PrintAddressType.BOTH)
                    {
                        //VERIFICANDO SE HÁ ENDEREÇO DE COBRANÇA
                        CF0 cf0 = await BillingAddressRepository.Find(cv5.Cv5cliente);
                        if (cf0 != null)
                        {
                            //INCLUINDO EM LINHAS DE cobrança
                            IncludeObsBillingAddress(lstCVJ, cv5, cf0);
                        }
                        
                        CFB cfb = await ShippingAddressRepository.Find(cv5.Cv5cliente);
                        if (cfb != null)
                        {
                            if (lx3.Lx3entrega == PrintAddressType.NONE || lx3.Lx3entrega == PrintAddressType.BILLINGADDRESS)
                            {
                                //"Cliente possui endereço específico para Entrega."
                            }
                            lstCVJ = lstCVJ.Where(cvj => !cvj.Cvjlinha.Equals("W") && !cvj.Cvjlinha.Equals("X") && !cvj.Cvjlinha.Equals("Y") && !cvj.Cvjlinha.Equals("Z")).ToList();
                            //INCLUINDO EM LINHAS DE entrega
                            IncludeObsShippingAddress(lstCVJ, cv5, cfb);
                            cv5.Cv5endentr = string.Format("{0} {1}", cfb.Cfbendent.Trim(), cfb.Cfbproximo);
                            cv5.Cv5ufentr = cfb.Cf3estado;
                            cv5.Cv5desentr = cv5.Cv5cliente;
                            cv5.Cv5cepentr = cfb.Cfbcepent;
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(cf1.Cf3nome) && !dadosAlterados)
            {
                cv5.Cv5estado = cf1.Cf3estado;
                cv5.Cv5nomeloc = cf1.Cf3nome;
                cv5.Cv5local = cf1.Cf2local;
                cv5.Cv5compl = cf1.Cf1compl;
            }
            if (cv5.Cv5emispro && cv5.Cv5entsai == GoodsFlowType.GOODS_ENTRY)
            {
                cv5.Cv5emisest = cf1.Cf3estado;
            }
            if (!dadosAlterados)
            {
                cv5.Cv5fone = cf1.Cf1fone;
                cv5.Cv5insest = cf1.Cf1insest;
                cv5.Cv5repres = cf1.Cf1repres;
                cv5.Cv5redesp = cf1.Cf1redesp;
                //removido pois precisa selecionar em tela o transportador
                //cv5.Cv5transp = cf1.Cf1transp;
            }
            cv5 = await RepresValido(cv5);
            cv5 = await TranspValido(cv5);
            if (!cv5.Cv5consig)
                await AddressService.VerifyAndSaveAddress(cv5.Cv5cep);

            cv5.Cv5clinome = Formatters.RemoveSpecialCharacterByNames(cv5.Cv5clinome);
            cv5.Cv5endcli = Formatters.RemoveSpecialCharacterByNames(cv5.Cv5endcli);
            cv5.Cv5nomeloc = string.IsNullOrWhiteSpace(cv5.Cv5nomeloc) ? string.Empty : Formatters.RemoveSpecialCharacterByNames(cv5.Cv5nomeloc);
            return cv5;
        }

        public async Task<CV5> RepresValido(CV5 cv5)
        {
            CF6 cf6 = await RepresentativeRepository.Find(cv5.Cv5repres);
            if (cf6 != null)
            {
                cv5.Cv5repnom = cf6.Cf6nome;
                if (cv5.Cv5comis == 0)
                    cv5.Cv5comis = cf6.Cf6comis;
            }
            return cv5;
        }

        public async Task<CV5> TranspValido(CV5 cv5)
        {
            CF7 cf7 = await TransporterRepository.Find(cv5.Cv5transp);
            if (cf7 != null)
            {
                cv5.Cv5trnome = cf7.Cf7nome;
                if (cf7.Cf7proprio)
                    cv5.Cv5trcnpj = cv5.Cv5cliente;
                else
                    cv5.Cv5trcnpj = cf7.Cf7cliente;
            }
            return await TrCnpjValido(cv5);
        }

        public async Task<CV5> TrCnpjValido(CV5 cv5)
        {
            if (string.IsNullOrWhiteSpace(cv5.Cv5trcnpj))
                return cv5;
            CF1 cf1 = await CustomerRepository.FindByCpfCnpj(cv5.Cv5trcnpj);
            if (cf1 != null)
            {
                cv5.Cv5trnome = cf1.Cf1nome;
                cv5.Cv5trender = Formatters.FormatAddress(cf1);
                cv5.Cv5trinscr = cf1.Cf1insest;
                if (!string.IsNullOrWhiteSpace(cf1.Cf3estado))
                {
                    cv5.Cv5trlocal = cf1.Cf3nome;
                    cv5.Cv5trestad = cf1.Cf3estado;
                }
            }
            return cv5;
        }

        public async Task<IList<CV7>> AgruparItens(CV5 cv5, IList<CV7> lstCv7, IList<CV8> lstCv8, OryxModuleType module, bool naocalcularparcelas, bool groupItemsByLineId, bool totalFisico = false)
        {
            IList<CV7> payloadList = new List<CV7>();
            //verificar se o documento pode ser agrupado
            if (DictionaryRepository.FindDC1ByDc1campo("cv7subtot") != null )
            {
                if (lstCv7.Where(cv7 => cv7.Cv7subtot > 0).Any())
                {
                    throw new Exception("Documento subtotalizado nao pode ser agrupado.");
                }
            }
            if (module != OryxModuleType.ORYX_PLACE)
            {
                IList<CV7> itensNaoAgrupados;
                if (groupItemsByLineId)
                {
                    itensNaoAgrupados = lstCv7.OrderBy(cv7 => cv7.Cv7item)
                               .ToList();
                }
                else
                {
                    itensNaoAgrupados = lstCv7.OrderBy(cv7 => cv7.Cv7pedido)
                                   .ThenBy(cv7 => cv7.Cv7codigo)
                                   .ThenBy(cv7 => cv7.Cv7cor)
                                   .ThenBy(cv7 => cv7.Cv7tamanho)
                                   .ThenBy(cv7 => cv7.Cv7ean)
                                   .ThenBy(cv7 => cv7.Cv7cfop)
                                   .ThenBy(cv7 => cv7.Cv7desc)
                                   .ThenBy(cv7 => cv7.Cv7infadic)
                                   .ThenBy(cv7 => cv7.Cv7vlunit)
                                   .ThenBy(cv7 => cv7.Cv7descon)
                                   .ToList();
                }
                foreach (CV7 cv7 in itensNaoAgrupados)
                {
                    PR0 pr0;
                    IN1 in1;
                    string codigoean = "";
                    cv7.Cv7cprod = StockService.Sku(cv7.Cv7codigo, cv5.Cv5itemfab, cv7.Cv7cor, cv7.Cv7tamanho);
                    
                    //desabilitado pois no Oryx não é utilizado mais cor e tamanho nas informações adicionais do produto
                    //if (!lx3.Lx3sku)
                    //{
                    //    cv7.Cv7infadic = string.Empty;
                    //    string color = string.IsNullOrWhiteSpace(cv7.Cv7cor) ? "" : "Cor " + cv7.Cv7cor;
                    //    string size = string.IsNullOrWhiteSpace(cv7.Cv7tamanho) ? "" : "Tam " + cv7.Cv7tamanho;
                    //    string infAdic = string.Format("{0} {1} ", color, size);
                    //    if (!cv7.Cv7infadic.Contains(infAdic))
                    //        cv7.Cv7infadic += infAdic;
                    //}
                    if (string.IsNullOrWhiteSpace(cv7.Cv7ean))
                    {
                        if (cv5.Cv5itemfab != SalesItemType.MATERIALS)
                        {
                            pr0 = await ProductRepository.Find<PR0>(cv7.Cv7codigo);
                            if (pr0 !=null)
                                codigoean = pr0.Pr0ean;
                        }
                        else
                        {
                            in1 = await SupplyRepository.Find(cv7.Cv7codigo);
                            if (in1 != null)
                               codigoean = in1.In1ean;
                        }
                    }
                    CV7 findCv7;
                    if (groupItemsByLineId)
                    {
                        findCv7 = payloadList.FirstOrDefault(c => c.Cv7item == cv7.Cv7item);
                    }
                    else
                    { 
                        findCv7 = payloadList.FirstOrDefault(c => c.Cv7pedido == cv7.Cv7pedido &&
                                                            c.Cv7ean == cv7.Cv7ean &&
                                                            c.Cv7codigo == cv7.Cv7codigo &&
                                                            c.Cv7cfop == cv7.Cv7cfop &&
                                                            c.Cv7desc == cv7.Cv7desc &&
                                                            c.Cv7infadic == cv7.Cv7infadic &&
                                                            c.Cv7vlunit == cv7.Cv7vlunit &&
                                                            c.Cv7descon == cv7.Cv7descon &&
                                                            c.Cv7tamanho == cv7.Cv7tamanho &&
                                                            c.Cv7cor == cv7.Cv7cor &&
                                                            c.Cv7flag1 == cv7.Cv7flag1);
                    }
                    if (findCv7 != null)
                    {
                        findCv7.Cv7qtde += cv7.Cv7qtde;
                        findCv7.Cv7descon += cv7.Cv7descon;
                    }
                    else
                    {
                        if (cv7.Cv7qtde > 0)
                        {
                            if (string.IsNullOrWhiteSpace(cv7.Cv7ean) && !string.IsNullOrWhiteSpace(codigoean))
                            {
                                cv7.Cv7ean = codigoean;
                            }
                            payloadList.Add(cv7);
                        }
                    }
                }
            }
            else
            {
                foreach (CV7 cv7 in lstCv7)
                {
                    cv7.Cv7cprod = StockService.Sku(cv7.Cv7codigo, cv5.Cv5itemfab);
                }
                payloadList = lstCv7;
                //TODO fazer o resto da parte da rastreabilidade
            }
            payloadList = await Renumerar(payloadList);
            await Totalizar(cv5, payloadList, lstCv8, module, totalFisico, true, true, naocalcularparcelas, naoRecalcularDesconto: true);
            return payloadList;
        }

        public async Task<string> GerarStrNfe(string cv5emissor, string cv5tipo, string cv5doc)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = Configuration["OryxPath"] + Configuration["ModuleOryx"];
                p.StartInfo.WorkingDirectory = Configuration["OryxPath"];
                p.StartInfo.Arguments = string.Format("\"integracao\" \"do form integracao with \" \"'{0};{1};{2}'\"", cv5emissor, cv5tipo, cv5doc);
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.WaitForExit(Convert.ToInt32(Configuration["TimeOutCallOryx"]));

                if (!p.HasExited)
                {
                    p.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            CV5 cv5 = await FiscalDocumentRepository.Find(cv5doc, cv5tipo, cv5emissor);
            Logger.LogWarning(cv5.Cv5xml);
            return cv5.Cv5xml;
        }

        public async Task<bool> FecharCupom(CV5 cv5, IList<CV8> lstCv8, IList<CV7> lstCv7, string paymentCondition)
        {
            string mensagemfinal;
            bool avista = false;
            decimal valorliquido, totaldodesconto;
            if (string.IsNullOrWhiteSpace(paymentCondition))
            {
                paymentCondition = "Dinheiro";
            }
            else
            {
                CV2 cv2 = await TitleRepository.Find(paymentCondition);
                paymentCondition = cv2 != null ? cv2.Cv2nome : "Dinheiro";
            }
            mensagemfinal = cv5.Cv5observ.Trim();
            if (cv5.Cv5credsim > 0)
                mensagemfinal = string.Format("TOTAL APROX DE IMPOSTOS (Lei 12741): R$ {0} ({1}%). {2}", (cv5.Cv5totalnf * cv5.Cv5credsim / 100).ToString(), cv5.Cv5credsim, mensagemfinal);

            CV8 cv8 = lstCv8.FirstOrDefault();
            if (cv8 != null)
            {
                if (string.IsNullOrWhiteSpace(cv8.Cv8parcela))
                    avista = true;

                if (cv8.Cv8valor == cv5.Cv5totalnf && cv8.Cv8vencim == cv5.Cv5emissao)
                    avista = true;
            }

            totaldodesconto = lstCv7.Aggregate(0M, (acc, cv7) => acc + cv7.Cv7descon);
            valorliquido = cv5.Cv5totalnf;
            //TODO
            return true;
        }

        public bool Devolucao(string cfop)
        {
            if (cfop.Equals("1201") || cfop.Equals("1202") || cfop.Equals("1203") || cfop.Equals("1204") || cfop.Equals("1208") || cfop.Equals("1209") || cfop.Equals("1410") || cfop.Equals("1411") || cfop.Equals("1503") || cfop.Equals("1504") || cfop.Equals("1505") || cfop.Equals("1506") || cfop.Equals("1553") || cfop.Equals("1660") || cfop.Equals("1661") || cfop.Equals("1662") || cfop.Equals("1918") || cfop.Equals("1919") || cfop.Equals("2201"))
            {
                return true;
            }
            if (cfop.Equals("2202") || cfop.Equals("2203") || cfop.Equals("2204") || cfop.Equals("2208") || cfop.Equals("2209") || cfop.Equals("2410") || cfop.Equals("2411") || cfop.Equals("2503") || cfop.Equals("2504") || cfop.Equals("2505") || cfop.Equals("2506") || cfop.Equals("2553") || cfop.Equals("2660") || cfop.Equals("2661") || cfop.Equals("2662") || cfop.Equals("2918") || cfop.Equals("2919") || cfop.Equals("3201") || cfop.Equals("3202"))
            {
                return true;
            }
            if (cfop.Equals("3211") || cfop.Equals("3503") || cfop.Equals("3553") || cfop.Equals("5201") || cfop.Equals("5202") || cfop.Equals("5208") || cfop.Equals("5209") || cfop.Equals("5210") || cfop.Equals("5410") || cfop.Equals("5411") || cfop.Equals("5412") || cfop.Equals("5413") || cfop.Equals("5503") || cfop.Equals("5553") || cfop.Equals("5555") || cfop.Equals("5556") || cfop.Equals("5660") || cfop.Equals("5661") || cfop.Equals("5662"))
            {
                return true;
            }
            if (cfop.Equals("5918") || cfop.Equals("5919") || cfop.Equals("5921") || cfop.Equals("6201") || cfop.Equals("6202") || cfop.Equals("6208") || cfop.Equals("6209") || cfop.Equals("6210") || cfop.Equals("6410") || cfop.Equals("6411") || cfop.Equals("6412") || cfop.Equals("6413") || cfop.Equals("6503") || cfop.Equals("6553") || cfop.Equals("6555") || cfop.Equals("6556") || cfop.Equals("6660") || cfop.Equals("6661") || cfop.Equals("6662"))
            {
                return true;
            }
            if (cfop.Equals("6918") || cfop.Equals("6919") || cfop.Equals("6921") || cfop.Equals("7201") || cfop.Equals("7202") || cfop.Equals("7210") || cfop.Equals("7211") || cfop.Equals("7553") || cfop.Equals("7556"))
            {
                return true;
            }
            return false;
        }

        public async Task<CV7> ProdutoValido(CV7 cv7, CV5 cv5, OryxModuleType module, string authorization, bool naovalidarpreco = false)
        {
            if (cv5.Cv5itemfab != SalesItemType.MATERIALS)
            {
                PR0 pr0 = await ProductRepository.Find<PR0>(cv7.Cv7codigo);
                cv7.Cv7desc = string.IsNullOrWhiteSpace(pr0.Pr0descfis) ? pr0.Pr0desc : pr0.Pr0descfis;
                if (pr0.Pr0revenda == ProductType.SERVICE)
                    cv7.Cv7servico = true;

                cv7.Cv7refer = pr0.Pr0refer;
                cv7.Cv7unmed = string.IsNullOrWhiteSpace(pr0.Pr0unmed) ? "un" : pr0.Pr0unmed;
                cv7.Cv7codclas = pr0.Pr0classif;
                if (lx3.Lx3peso == ProductWeightType.GROSS)
                {
                    cv7.Cv7peso = pr0.Pr0pesobru;
                }
                else
                {
                    cv7.Cv7peso = pr0.Pr0pesoliq;
                    PR3 pr3 = await ProductSizesRepository.Find(cv7.Cv7codigo, cv7.Cv7tamanho);
                    if (pr3 != null && pr3.Pr3pesoliq > 0)
                    {
                        cv7.Cv7peso = pr3.Pr3pesoliq;
                    }
                }
                cv7 = await ClassificacaoFiscalValida(cv7, cv5);
                if (!naovalidarpreco)
                {
                    cv7 = await PrecoValido(cv7, cv5, module, authorization);
                }
                return cv7;
            }
            //TODO para validação do produto do tipo MATERIAIS
            return cv7;
        }

        public async Task<CV7> ClassificacaoFiscalValida(CV7 cv7, CV5 cv5)
        {
            FI0 fi0 = await FiscalClassificationRepository.Find(cv7.Cv7codclas);
            if (fi0 != null)
            {
                cv7.Cv7classif = fi0.Fi0classif;
                cv7.Cv7cfop = cv5.Cv5cfop;
            }
            return cv7;
        }
        public async Task<CV7> PrecoValido(CV7 cv7, CV5 cv5, OryxModuleType module, string authorization)
        {
            if (cv5.Cv5itemfab != SalesItemType.MATERIALS)
            {
                switch (module)
                {
                    case OryxModuleType.ORYX_PLACE:
                        cv7.Cv7vlunit = await PriceService.FindMaterialPrice(cv7.Cv7codigo, cv5.Cv5lista, cv5.Cv5cliente, authorization);
                        break;
                    case OryxModuleType.ORYX_ESQUADRIAS:
                        cv7.Cv7vlunit = await PriceService.FindEsquadrias(cv7.Cv7codigo, cv5.Cv5lista, cv7.Cv7cor, authorization);
                        break;
                    default:
                        cv7.Cv7vlunit = await PriceService.Find(cv7.Cv7codigo, cv5.Cv5lista, cv7.Cv7tamanho, cv7.Cv7cor, authorization);
                        break;
                }
            }
            cv7.Cv7descon = Math.Round(cv7.Cv7vlunit * (cv5.Cv5descon / 100), 2, MidpointRounding.AwayFromZero);
            return cv7;
        }

        public async Task<IList<CV7>> Renumerar(IList<CV7> lstCv7)
        {
            for (int i = 0; i < lstCv7.Count; i++)
            {
                CV7 cv7 = lstCv7[i];
                int cv7ItemSize = await FormatterService.FindFieldLength(nameof(cv7.Cv7item));
                cv7.Cv7item = (i + 1).ToString().PadLeft(cv7ItemSize, '0');
            }
            return lstCv7;
        }

        public async Task Totalizar(
              CV5 cv5
            , IList<CV7> lstCv7
            , IList<CV8> lstCv8
            , OryxModuleType module
            , bool totalFisico = false
            , bool naoAjustarPeloPercentual = false
            , bool naoEliminarOutrosMeios = false
            , bool naoCalcularParcelas = false
            , bool naoRecalcularItens = false
            , bool naoRecalcularDesconto = false
            )
        {
            decimal descfat = 0;
            bool icmsIpi;
            decimal aliqsub, aliqicm, aliqipi, baseicm, baseise, baseout, basesub, basepis, basecof, aliqpis, aliqcof, outrosvalores, outrosbaseicm, outrosbaseise, outrosbaseout, desconv, totaldodesconto, poutrosbaseise, poutrosbaseout, poutrosbaseicm;
            string cstpis, cstcof, cbenef;
            
            //Nota sem valor contabil apenas para lançamento
            if (cv5.Cv5naocont)
                return;

            if (cv5.Cv5cfop.StartsWith("3"))
                icmsIpi = true;
            else
                icmsIpi = lx3.Lx3icmsipi;

            //eliminando outros meios de pagamento inseridos
            DC1 dc1Cv5titulo = await DictionaryRepository.FindDC1ByDc1campo(nameof(cv5.Cv5titulo1));
            if (dc1Cv5titulo != null)
            {
                if (!naoEliminarOutrosMeios)
                {
                    cv5.Cv5titulo1 = 0;
                    cv5.Cv5titulo2 = 0;
                    cv5.Cv5titulo3 = 0;
                    lstCv8 = lstCv8.Where(cv8 => !string.IsNullOrWhiteSpace(cv8.Cv8tipotit)).ToList();
                    lstCv8 = await ReenumerarParcelas(lstCv8);
                }
            }
            aliqsub = cv5.Cv5aliqsub;
            aliqicm = cv5.Cv5aliqicm;
            aliqipi = cv5.Cv5aliqipi;
            baseicm = cv5.Cv5baseicp;
            baseise = cv5.Cv5baseisp;
            baseout = cv5.Cv5baseoup;
            basesub = cv5.Cv5basesup;
            cstpis = cv5.Cv5cstpis;
            cstcof = cv5.Cv5cstcof;
            basepis = cv5.Cv5basepis;
            basecof = cv5.Cv5basecof;
            aliqpis = cv5.Cv5aliqpis;
            aliqcof = cv5.Cv5aliqcof;
            cbenef = cv5.Cv5cbenef;

            if (cv5.Cv5cupom)
            {
                cv5.Cv5vlfrete = 0;
                cv5.Cv5outras = 0;
                cv5.Cv5seguro = 0;
            }
            outrosvalores = cv5.Cv5vlfrete + cv5.Cv5outras + cv5.Cv5seguro;
            outrosbaseicm = outrosvalores * (baseicm / 100);
            outrosbaseise = outrosvalores * (baseise / 100);
            outrosbaseout = outrosvalores * (baseout / 100);

            //incluindo item de serviço se existir
            lstCv7 = lstCv7.Where(cv7 => string.IsNullOrWhiteSpace(cv7.Cv7codiss)).ToList();
            if (cv5.Cv5vlrserv > 0 && !string.IsNullOrWhiteSpace(cv5.Cv5codiss))
            {
                lstCv7.Add(new CV7()
                {
                    Cv7codigo = cv5.Cv5codiss,
                    Cv7desc = DescricaoServico(cv5.Cv5codiss),
                    Cv7unmed = "un",
                    Cv7qtde = 1,
                    Cv7vlunit = cv5.Cv5vlrserv,
                    Cv7vltotal = cv5.Cv5vlrserv,
                    Cv7classif = "00",
                    Cv7codiss = cv5.Cv5codiss,
                    Cv7cfop = cv5.Cv5cfop,
                    Cv7servico = true
                    //restante das csts baseadas na da operacao
                }) ;
            }

            //calculando valor total do item
            if (!naoRecalcularItens)
            {
                foreach (CV7 cv7 in lstCv7)
                {
                    cv7.Cv7vltotal = cv7.Cv7vlunit * cv7.Cv7qtde;
                }
            }
            if (!naoRecalcularDesconto)
            {
                foreach (CV7 cv7 in lstCv7)
                {
                    cv7.Cv7descon = 0;
                    if (cv7.Cv7desconp == 0)
                        cv7.Cv7descon = Math.Round(cv7.Cv7vltotal * (cv5.Cv5descon / 100), 2, MidpointRounding.AwayFromZero);
                    if (cv7.Cv7desconp != 0)
                        cv7.Cv7descon = Math.Round(cv7.Cv7vltotal * (cv7.Cv7desconp / 100), 2, MidpointRounding.AwayFromZero);
                }
            }
            desconv = DescontoValor(lstCv7, cv5);
            //verificando a compatibilidade com o desconto percentual
            if (!naoAjustarPeloPercentual)
            {
                if (Math.Round(desconv,1) != Math.Round(cv5.Cv5desconv,1))
                    cv5.Cv5desconv = desconv;
            }
            totaldodesconto = cv5.Cv5desconv;
            totaldodesconto = Math.Round(lstCv7.Aggregate(totaldodesconto, (acc, cv7) => acc - cv7.Cv7descon), 2, MidpointRounding.AwayFromZero);
            foreach (CV7 cv7 in lstCv7)
            {
                if (totaldodesconto<0)
                {
                    if ((cv7.Cv7vltotal + cv7.Cv7descon + totaldodesconto) > 0)
                    {
                        cv7.Cv7descon += totaldodesconto;
                    }
                }
                else
                {
                    if ((cv7.Cv7vltotal - (cv7.Cv7descon + totaldodesconto)) > 0)
                        cv7.Cv7descon += totaldodesconto;
                }
                
                // se a operação comercial for alterada, alíquota de IPI dos itens é ajustada, independente de ser zero
                if (aliqipi > 0)
                    cv7.Cv7aliqipi = aliqipi;
                
                if (string.IsNullOrWhiteSpace(cv7.Cv7codiss))
                {
                    cv7.Cv7aliqicm = aliqicm;
                    cv7.Cv7aliqsub = aliqsub;

                    // calculando base icms por item
                    cv7.Cv7baseicm = (cv7.Cv7vltotal - cv7.Cv7descon + (icmsIpi ? cv7.Cv7ipi : 0)) * (baseicm / 100);
                    cv7.Cv7baseise = (cv7.Cv7vltotal - cv7.Cv7descon) * (baseise / 100);
                    cv7.Cv7baseout = (cv7.Cv7vltotal - cv7.Cv7descon) * (baseout / 100);

                    //gravando CST E ORIGEM
                    cv7.Cv7cst = cv5.Cv5cst;
                    cv7.Cv7cstipi = cv5.Cv5cstipi;
                }
                if (cv5.Cv5vlrserv > 0 && !string.IsNullOrWhiteSpace(cv7.Cv7codiss))
                {
                    cv7.Cv7baseicm = 0;
                    cv7.Cv7baseise = 100;
                    cv7.Cv7baseout = 0;

                    if (lx0.Lx0crt == TaxRegimeType.NORMAL_REGIME)
                        cv7.Cv7cst = "41";
                    else
                        cv7.Cv7cst = "400";

                    if (cv5.Cv5entsai == GoodsFlowType.GOODS_EXIT)
                        cv7.Cv7cstipi = "52";
                    else
                        cv7.Cv7cstipi = "02";

                    cv7.Cv7baseipi = 0;
                }

                if (cv5.Cv5emispro)
                {
                    if(aliqipi > 0)
                        cv7.Cv7baseipi = cv7.Cv7vltotal - cv7.Cv7descon;

                    cv7.Cv7basesup = basesub;
                    cv7.Cv7basesub = 0;
                    cv7.Cv7valsub = 0;
                }
                cv7.Cv7origem = cv5.Cv5origem;
                // calculando base para substituição por item
                if( await DictionaryRepository.FindDC1ByDc1campo(nameof(cv7.Cv7baseicp)) != null)
                {
                    //gravando para permitir posterior recalculo
                    cv7.Cv7baseicp = baseicm;
                }

                cv7.Cv7cstpis = cstpis;
                cv7.Cv7cstcof = cstcof;
                cv7.Cv7basepis = basepis;
                cv7.Cv7basecof = basecof;
                cv7.Cv7aliqpis = aliqpis;
                cv7.Cv7aliqcof = aliqcof;
                cv7.Cv7cbenef = cbenef;
                cv7.Cv7pdif = cv5.Cv5pdif;

                // definindo tributações específicas
                FindEspecificTaxes(cv7, cv5, ref descfat, icmsIpi);

                // calculando base para substituição por item
                // 23/05/2014 valor do ipi incluido na base substituição
                if (cv7.Cv7basesup > 0)
                {
                    cv7.Cv7basesub = CalculateCv7basesub(cv7.Cv7cst, cv7.Cv7vltotal, cv7.Cv7ipi, cv7.Cv7descon, cv7.Cv7baseicm, cv7.Cv7basesup);
                }
                if (cv7.Cv7basesub > 0)
                    cv7.Cv7valsub = CalculateCv7valsub(cv7.Cv7basesub, cv7.Cv7aliqsub, cv7.Cv7cst, cv7.Cv7vltotal, cv7.Cv7baseicm, cv7.Cv7aliqicm);

                if (cv7.Cv7valsub < 0)
                    cv7.Cv7valsub = 0;

                if (cv7.Cv7pfcpst > 0 && cv7.Cv7cst.Equals("202"))
                    cv7.Cv7vfcpst = Math.Round(cv7.Cv7pfcpst* cv7.Cv7basesub/100, 2, MidpointRounding.AwayFromZero);
            }

            totaldodesconto = cv5.Cv5desconv;
            //inserindo base de outros valores de forma rateada
            decimal basedorateio = lstCv7.Aggregate(0M, (acc, c) => acc + c.Cv7vltotal);

            if (basedorateio == 0 || outrosbaseise == 0)
                poutrosbaseise = 0;
            else
                poutrosbaseise = outrosbaseise / basedorateio;

            if (basedorateio == 0 || outrosbaseout == 0)
                poutrosbaseout = 0;
            else
                poutrosbaseout = outrosbaseout / basedorateio;

            if (basedorateio == 0 || outrosbaseicm == 0)
                poutrosbaseicm = 0;
            else
                poutrosbaseicm = outrosbaseicm / basedorateio;

            decimal nValorRateado = 0;
            foreach (CV7 cv7 in lstCv7)
            {
                nValorRateado = Math.Round(cv7.Cv7vltotal * poutrosbaseise, 2, MidpointRounding.AwayFromZero);
                if(cv7.Cv7baseise + nValorRateado > 0)
                {
                    cv7.Cv7baseise += nValorRateado;
                    outrosbaseise -= nValorRateado;
                }
                nValorRateado = Math.Round(cv7.Cv7vltotal * poutrosbaseout, 2, MidpointRounding.AwayFromZero);
                if (cv7.Cv7baseout + nValorRateado > 0)
                {
                    cv7.Cv7baseout += nValorRateado;
                    outrosbaseout -= nValorRateado;
                }
                nValorRateado = Math.Round(cv7.Cv7vltotal * poutrosbaseicm, 2, MidpointRounding.AwayFromZero);
                if (cv7.Cv7baseicm + nValorRateado > 0)
                {
                    cv7.Cv7baseicm += nValorRateado;
                    outrosbaseicm -= nValorRateado;
                }
            }
            //*********************

            decimal totalbaseicm = 0;
            decimal totalbaseise = 0;
            decimal totalbaseout = 0;
            decimal totalbaseipi = 0;
            decimal totalicm = 0;
            decimal totalipi = 0;
            decimal totalbasesub = 0;
            decimal totalsub = 0;
            decimal totaldedescontos = 0;
            decimal totalprodutos = 0;
            decimal totalfcpst = 0;
            decimal qtdevol = 0;
            decimal pesobru = 0;
            decimal pesoliq = 0;
            foreach (CV7 cv7 in lstCv7)
            {
                cv7.Cv7baseise += outrosbaseise;
                cv7.Cv7baseise = Math.Round(cv7.Cv7baseise, 2, MidpointRounding.AwayFromZero);
                cv7.Cv7baseout += outrosbaseout;
                cv7.Cv7baseout = Math.Round(cv7.Cv7baseout, 2, MidpointRounding.AwayFromZero);
                cv7.Cv7baseicm += outrosbaseicm;
                cv7.Cv7baseicm = Math.Round(cv7.Cv7baseicm, 2, MidpointRounding.AwayFromZero);
                totalbaseicm += cv7.Cv7baseicm;
                totalbaseise += cv7.Cv7baseise;
                totalbaseout += cv7.Cv7baseout;
                totalbaseipi += cv7.Cv7baseipi;
                totalfcpst += cv7.Cv7vfcpst;

                //valor icm
                cv7.Cv7valicm = Math.Round(cv7.Cv7baseicm * (cv7.Cv7aliqicm / 100), 2, MidpointRounding.AwayFromZero);
                cv7.Cv7valicm = Math.Round(cv7.Cv7valicm * (1 - (cv7.Cv7pdif/ 100)), 2, MidpointRounding.AwayFromZero);

                totalicm += cv7.Cv7valicm;

                //valor ipi
                if (cv5.Cv5aliqipi != 0 && cv5.Cv5emispro)
                    cv7.Cv7ipi = cv7.Cv7baseipi * (cv7.Cv7aliqipi / 100);

                totalipi += cv7.Cv7ipi;

                // base substituição
                totalbasesub += cv7.Cv7basesub;

                // valor por substituição
                totalsub += cv7.Cv7valsub;

                // totalizando desconto
                totaldedescontos += cv7.Cv7descon;

                // valor dos produtos
                totalprodutos += cv7.Cv7vltotal;

                qtdevol += cv7.Cv7qtde;
                pesobru += cv7.Cv7peso * cv7.Cv7qtde;

                // zerando aliquota de icms quando base icms igual a zero
                if(cv7.Cv7baseicm == 0)
                {
                    cv7.Cv7aliqicm = 0;
                }
            }

            if (naocalcularbaseicms)
            {
                throw new NotImplementedException();
                //TODO flag apenas é setada quando se clica em um botão na tela de itens da NF, no qual serve para não calcular a base de icms somando o frete - na exportação não se soma o frete para o cálaculo
            }

            cv5.Cv5baseicm = totalbaseicm;
            cv5.Cv5baseise = totalbaseise;
            cv5.Cv5baseout = totalbaseout;
            cv5.Cv5basesub = totalbasesub;
            cv5.Cv5valsub = totalsub;
            cv5.Cv5desconv = totaldedescontos;
            cv5.Cv5totalpr = totalprodutos;
            cv5.Cv5ipi = totalipi;
            cv5.Cv5baseipi = totalbaseipi;
            cv5.Cv5valicm = totalicm;
            cv5.Cv5vfcpst = totalfcpst;

            if (!icmsIpi)
            {
                cv5.Cv5baseout += totalipi;
            }

            //valor total da nota
            cv5.Cv5totalnf = cv5.Cv5totalpr + cv5.Cv5valsub + cv5.Cv5seguro + cv5.Cv5vlfrete + cv5.Cv5outras + cv5.Cv5ipi - cv5.Cv5desconv;

            //totalização física
            if (!totalFisico)
            {
                pesoliq = pesobru;
                if (module == OryxModuleType.ORYX_PLACE)
                {
                    throw new NotImplementedException();
                    //TODO
                }
                //if (!naoRecalcularItens)
                //{
                //    cv5.Cv5pesobru = pesobru;
                //    cv5.Cv5pesoliq = pesoliq;
                //}
                if (cv5.Cv5espevol.ToLower().Equals("avulso"))
                {
                    int sizeCv5qtdevol = await FormatterService.FindFieldLength(nameof(cv5.Cv5qtdevol));
                    int maxvol = Convert.ToInt32(new string('9', sizeCv5qtdevol));
                    if (qtdevol>maxvol)
                    {
                        qtdevol = maxvol;
                        throw new Exception("Quantidade de Volumes excedeu limite (Limite: " + maxvol + ").");
                    }
                    cv5.Cv5qtdevol = qtdevol;
                }
                else
                    if (!cv5.Cv5espevol.ToLower().Equals("cilindro(s)"))
                        cv5.Cv5qtdevol = 1;
            }
            if (cv5.Cv5tottri1)
            {
                decimal tottrib = 0;
                foreach (CV7 cv7 in lstCv7)
                {
                    cv7.Cv7tottrib = (cv7.Cv7vltotal - cv7.Cv7descon) * (cv7.Cv7tottrib / 100);
                    cv7.Cv7tottrib = Math.Round(cv7.Cv7tottrib, 2, MidpointRounding.AwayFromZero);
                    tottrib += cv7.Cv7tottrib;
                }
                cv5.Cv5tottrib = tottrib;
            }


            //gravando desconto na fatura
            if (descfat > 0)
                cv5.Cv5descfat = descfat;

            if (!naoCalcularParcelas)
            {
                await CriarParcelas(cv5, lstCv8, true);
                lstCv8 = AjustarParcelas(lstCv8, cv5);
            }
        }

        public async Task CriarParcelas(CV5 cv5, IList<CV8> lstCv8, bool criarsenecessario = false)
        {
            decimal nparcelas, nparcelas2, parcela = 1;
            int proximovencimento = 0;
            DateTime vencimentoajustado;
            IList<VD9> lstVd9 = await PaymentConditionRepository.FindVd9List(cv5.Cv5conpgto);
            nparcelas = lstVd9.Count;
            nparcelas2 = lstCv8.Where(cv8 => string.IsNullOrWhiteSpace(cv8.Cv8tipotit)).ToList().Count;
            if (criarsenecessario)
            {
                if (nparcelas == nparcelas2)
                    return;
            }

            lstCv8 = lstCv8.Where(cv8 => string.IsNullOrWhiteSpace(cv8.Cv8tipotit)).ToList();
            for (int i = 0; i < lstVd9.Count && nparcelas>=parcela; i++)
            {
                proximovencimento += lstVd9[i].Vd9caren;
                vencimentoajustado = DateTime.Now.AddDays(proximovencimento);
                if (lxa.Lxadiautil)
                {
                    if (vencimentoajustado.DayOfWeek == DayOfWeek.Saturday)
                        vencimentoajustado.AddDays(2);
                    if (vencimentoajustado.DayOfWeek == DayOfWeek.Sunday)
                        vencimentoajustado.AddDays(1);

                }
                lstCv8.Add(new CV8()
                {
                    Cv8emissor = cv5.Cv5emissor,
                    Cv8tipo = cv5.Cv5tipo,
                    Cv8doc = cv5.Cv5doc,
                    Cv8vencim = vencimentoajustado,
                    Cv8parcela = "99"
                });
            }

            lstCv8 = AjustarParcelas(lstCv8, cv5);
            lstCv8 = await ReenumerarParcelas(lstCv8);
        }

        public IList<CV8> AjustarParcelas(IList<CV8> lstCv8, CV5 cv5)
        {
            return lstCv8;
            //int nParcelas;
            //decimal totalNf = cv5.Cv5totalnf = cv5.Cv5descfat;
            ////verificando se é necessário
            //if (SomadasParcelas(lstCv8) == totalNf)
            //{
            //    return lstCv8;
            //}
            ////chamando o total de parcelas da condição
            //IList<VD9> lstVd9 = await PaymentConditionRepository.FindVd9List(cv5.Cv5conpgto);
            //nParcelas = lstVd9.Count;
            //decimal valorAParcelar = ValorParcelar();
            //decimal valorDaParcela = Math.Round(valorAParcelar/nParcelas, arredondar);
            //foreach (CV8 cv8 in lstCv8)
            //{
            //    if (!string.IsNullOrWhiteSpace(cv8.Cv8tipotit))
            //        continue;
            //    cv8.Cv8valor = valorDaParcela;
            //    valorAParcelar = -valorDaParcela;
            //}

        }

        public decimal DescontoValor(IList<CV7> lstCv7, CV5 cv5)
        {
            decimal sumDiscount = lstCv7.Where(cv7 => cv7.Cv7desconp == 0).Sum(cv7 => cv7.Cv7vltotal * (cv5.Cv5descon / 100));
            sumDiscount += lstCv7.Where(cv7 => cv7.Cv7desconp != 0).Sum(cv7 => cv7.Cv7descon);
            return sumDiscount;
        }

        public string DescricaoServico(string cv5codiss)
        {
            return Constants.ServicesType.GetValueOrDefault(cv5codiss, "Serviço");
        }

        public async Task<IList<CV8>> ReenumerarParcelas(IList<CV8> lstCv8)
        {
            lstCv8 = lstCv8.Where(c => c.Cv8valor != 0).OrderBy(c => Convert.ToInt32(c.Cv8parcela)).ThenBy(c => c.Cv8vencim).ToList();
            for (int i = 0; i < lstCv8.Count; i++)
            {
                CV8 cv8 = lstCv8[i];
                int cv8parcelasize = await FormatterService.FindFieldLength(nameof(cv8.Cv8parcela));
                cv8.Cv8parcela = (i + 1).ToString().PadLeft(cv8parcelasize, '0');
            }
            return lstCv8;
        }
        #region private
        private void IncludeObsBillingAddress(IList<CVJ> lstCVJ, CV5 cv5, CF0 cf0)
        {
            lstCVJ.Add(new CVJ()
            {
                Cvjdoc = cv5.Cv5doc,
                Cvjemissor = cv5.Cv5emissor,
                Cvjtipo = cv5.Cv5tipo,
                Cvjlinha = "T",
                Cvjobserva = "----- Endereço de Cobrança -----"
            });
            lstCVJ.Add(new CVJ()
            {
                Cvjdoc = cv5.Cv5doc,
                Cvjemissor = cv5.Cv5emissor,
                Cvjtipo = cv5.Cv5tipo,
                Cvjlinha = "U",
                Cvjobserva = cf0.Cf0endcob
            });
            lstCVJ.Add(new CVJ()
            {
                Cvjdoc = cv5.Cv5doc,
                Cvjemissor = cv5.Cv5emissor,
                Cvjtipo = cv5.Cv5tipo,
                Cvjlinha = "V",
                Cvjobserva = string.Format("{0} {1}-{2}", Formatters.FormatCep(cf0.Cf0cepcob), cf0.Cf3nome.Trim(), cf0.Cf3estado)
            });
        }

        private void IncludeObsShippingAddress(IList<CVJ> lstCVJ, CV5 cv5, CFB cfb)
        {
            lstCVJ.Add(new CVJ()
            {
                Cvjdoc = cv5.Cv5doc,
                Cvjemissor = cv5.Cv5emissor,
                Cvjtipo = cv5.Cv5tipo,
                Cvjlinha = "W",
                Cvjobserva = "----- Endereço de Entrega -----"
            });
            lstCVJ.Add(new CVJ()
            {
                Cvjdoc = cv5.Cv5doc,
                Cvjemissor = cv5.Cv5emissor,
                Cvjtipo = cv5.Cv5tipo,
                Cvjlinha = "X",
                Cvjobserva = cfb.Cfbendent
            });
            if (!string.IsNullOrWhiteSpace(cfb.Cfbproximo))
            {
                lstCVJ.Add(new CVJ()
                {
                    Cvjdoc = cv5.Cv5doc,
                    Cvjemissor = cv5.Cv5emissor,
                    Cvjtipo = cv5.Cv5tipo,
                    Cvjlinha = "Y",
                    Cvjobserva = cfb.Cfbproximo
                });
            }
            lstCVJ.Add(new CVJ()
            {
                Cvjdoc = cv5.Cv5doc,
                Cvjemissor = cv5.Cv5emissor,
                Cvjtipo = cv5.Cv5tipo,
                Cvjlinha = "Z",
                Cvjobserva = string.Format("{0} {1}-{2}", Formatters.FormatCep(cfb.Cfbcepent), cfb.Cf3nome.Trim(), cfb.Cf3estado)
            });
        }

        private void FindEspecificTaxes(CV7 cv7, CV5 cv5, ref decimal descfat, bool icmsIpi)
        {
            if (string.IsNullOrWhiteSpace(cv7.Cv7codigo))
                return;

            OriginModel origin;
            CVN cvn;
            if (cv5.Cv5itemfab != SalesItemType.MATERIALS)
            {
                origin = ProductRepository.FindSpecificTaxes(cv7.Cv7codigo).Result;
                cvn = SpecificTaxesRepository.Find(cv5.Cv5opercom, cv7.Cv7codigo).Result;
            }
            else
            {
                origin = SupplyRepository.FindSpecificTaxes(cv7.Cv7codigo).Result;
                cvn = SpecificTaxesRepository.Find(cv5.Cv5opercom, cvninsumo: cv7.Cv7codigo).Result;
            }
            if (origin != null && !string.IsNullOrWhiteSpace(origin.Origin))
            {
                cv7.Cv7origem = origin.Origin;
            }
            if (cvn != null)
            {
                if (cvn.Cvndescfat)
                {
                    descfat += cv7.Cv7vltotal - cv7.Cv7descon;
                }
                cv7.Cv7cfop = cvn.Cvncfop;
                cv7.Cv7cst = cvn.Cvncst;
                cv7.Cv7origem = string.IsNullOrWhiteSpace(origin.Origin) ? cvn.Cvnorigem : origin.Origin;
                cv7.Cv7baseicm = cvn.Cvnbaseicm * (cv7.Cv7vltotal - cv7.Cv7descon + (icmsIpi ? cv7.Cv7ipi : 0)) / 100;
                cv7.Cv7aliqicm = cvn.Cvnaliqicm;
                cv7.Cv7baseise = cvn.Cvnbaseise * (cv7.Cv7vltotal - cv7.Cv7descon) / 100;
                cv7.Cv7baseout = cvn.Cvnbaseout * (cv7.Cv7vltotal - cv7.Cv7descon) / 100;
                cv7.Cv7basesup = cvn.Cvnbasesub;
                cv7.Cv7aliqsub = cvn.Cvnaliqsub;
                cv7.Cv7aliqipi = cvn.Cvnaliqipi;
                cv7.Cv7cstipi = cvn.Cvncstipi;
                cv7.Cv7cbenef = cvn.Cvncbenef;
                cv7.Cv7infadic = cvn.Cvninfadic;
                cv7.Cv7pdif = cvn.Cvnpdif;
                cv7.Cv7pfcpst = cvn.Cvnpfcp;

                if (DictionaryRepository.FindDC1ByDc1campo(nameof(cv7.Cv7baseicp)).Result != null)
                {
                    cv7.Cv7baseicp = cvn.Cvnbaseicm;
                }
                if (cvn.Cvnaliqipi > 0)
                {
                    cv7.Cv7baseipi = cv7.Cv7vltotal - cv7.Cv7descon;
                    cv7.Cv7ipi = cv7.Cv7baseipi * (cv7.Cv7aliqipi / 100);
                }
                else
                {
                    cv7.Cv7baseipi = 0;
                    cv7.Cv7ipi = 0;
                }
                cv7.Cv7contad = cvn.Cvncontad;
                cv7.Cv7contac = cvn.Cvncontac;
                //pis / cofins
                cv7.Cv7cstpis = cvn.Cvncstpis;
                cv7.Cv7cstcof = cvn.Cvncstcof;
                cv7.Cv7basepis = cvn.Cvnbasepis;
                cv7.Cv7basecof = cvn.Cvnbasecof;
                cv7.Cv7aliqpis = cvn.Cvnaliqpis;
                cv7.Cv7aliqcof = cvn.Cvnaliqcof;
            }
            if (cv5.Cv5tottri1)
            {
                //independentemente de haver trib especifica
                cv7.Cv7tottrib = origin.Tottrib;
            }
        }

        #region cv7
        private decimal CalculateCv7basesub(string cv7cst, decimal cv7vltotal, decimal cv7ipi, decimal cv7descon, decimal cv7baseicm, decimal cv7basesup)
        {
            decimal value;
            if (cv7cst.Trim().Length == 3)
                value = cv7vltotal + cv7ipi - cv7descon;
            else
                value = cv7baseicm + cv7ipi;
            return Math.Round(value * (1 + (cv7basesup / 100)), 2, MidpointRounding.AwayFromZero);
        }

        private decimal CalculateCv7valsub(decimal cv7basesub, decimal cv7aliqsub, string cv7cst, decimal cv7vltotal, decimal cv7baseicm, decimal cv7aliqicm)
        {
            decimal value;
            if (cv7cst.Trim().Length == 3)
                value = cv7vltotal;
            else
                value = cv7baseicm;
            return Math.Round((cv7basesub * (cv7aliqsub / 100)) - value * (cv7aliqicm / 100), 2, MidpointRounding.AwayFromZero);
        }
        #endregion

        #endregion
    }
}
