using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OryxDomain.Http;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.NfeModels;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace OryxDomain.Services
{
    public class FiscalDocumentService
    {
        ILogger Logger;
        //iniciar sessão de dados
        public CV5 cv5;
        public IList<CV7> lstCv7;
        public IList<CV8> lstCv8;
        public IList<CVJ> lstCvj;
        public IList<CVQ> lstCvq;
        public CVT cvt;
        private bool naocalcularparcelas;
        private VD1 vd1;
        private IList<ShipItem> lstShipItems;
        private IList<InvoiceShipItems> lstInvoiceShipItems;
        private LXD lxd;
        private LXE lxe;
        private readonly OryxModuleType module;
        private readonly string docfis;
        private CV5 ThirdPartyDoc;
        private bool groupItemsByLineId;

        private IConfiguration Configuration;
        private OrderRepository OrderRepository;
        private FiscalDocumentRepository FiscalDocumentRepository;
        private SalesConfirmationRepository SalesConfirmationRepository;
        private PurchaseOrderRepository PurchaseOrderRepository;
        private PDVParametersRepository PDVParametersRepository;
        private ParametersRepository ParametersRepository;
        private APIParametersRepository APIParametersRepository;
        private ProductRepository ProductRepository;
        private FiscalService FiscalService;
        private SisService SisService;
        private TecdatasoftRepository TecdatasoftRepository;
        private PurchaseCreditRepository PurchaseCreditRepository;
        private ReturnRepository ReturnRepository;
        private EanCodificationRepository EanCodificationRepository;
        private SalesConfirmationItemsRepository SalesConfirmationItemsRepository;
        private FormatterService FormatterService;
        private CashierRepository CashierRepository;
        private string[] Alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S" };
        public FiscalDocumentService(IConfiguration configuration, ILogger _logger)
        {
            Configuration = configuration;
            Logger = _logger;
            InitDependences();
        }

        public FiscalDocumentService(
            IConfiguration configuration,
            CV5 _cv5,
            IList<CV7> _lstCv7,
            IList<CV8> _lstCv8,
            IList<CVJ> _lstCvj,
            IList<CVQ> _lstCvq,
            CVT _cvt,
            OryxModuleType _module,
            string _docfis,
            ILogger _logger,
            bool _naocalcularparcelas = false,
            CV5 _thirdPartyDoc = null,
            bool _groupItemsByLineId = false
        )
        {
            Configuration = configuration;
            Logger = _logger;
            InitDependences();
            cv5 = _cv5;
            lstCv7 = _lstCv7;
            lstCv8 = _lstCv8;
            lstCvj = _lstCvj;
            lstCvq = _lstCvq;
            cvt = _cvt;
            module = _module;
            docfis = _docfis;
            naocalcularparcelas = _naocalcularparcelas;
            ThirdPartyDoc = _thirdPartyDoc;
            groupItemsByLineId = _groupItemsByLineId;
        }

        private void InitDependences()
        {
            OrderRepository = new OrderRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalDocumentRepository = new FiscalDocumentRepository(Configuration["OryxPath"] + "oryx.ini");
            SalesConfirmationRepository = new SalesConfirmationRepository(Configuration["OryxPath"] + "oryx.ini");
            PurchaseOrderRepository = new PurchaseOrderRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            PDVParametersRepository = new PDVParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            APIParametersRepository = new APIParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            TecdatasoftRepository = new TecdatasoftRepository(Configuration["OryxPath"] + "oryx.ini");
            PurchaseCreditRepository = new PurchaseCreditRepository(Configuration["OryxPath"] + "oryx.ini");
            EanCodificationRepository = new EanCodificationRepository(Configuration["OryxPath"] + "oryx.ini");
            ReturnRepository = new ReturnRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductRepository = new ProductRepository(Configuration["OryxPath"] + "oryx.ini");
            CashierRepository = new CashierRepository(Configuration["OryxPath"] + "oryx.ini");
            SalesConfirmationItemsRepository = new SalesConfirmationItemsRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalService = new FiscalService(Configuration, Logger);
            SisService = new SisService(Configuration);
            FormatterService = new FormatterService(Configuration);

            lxd = PDVParametersRepository.Find().Result;
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            lxe = APIParametersRepository.Find().Result;
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");
        }

        #region include methods
        public async Task IncludeByCv5pedido(
              FatherFieldType campoPai
            , string valorPai
            , string authorization
            , string terminal
            , string codAuthSalesMall
            , bool noValidateItemPrice = false
            , string opercom = ""
            , bool edit = false
            , string vdedoc = ""
            , string vdkdoc = ""
            , bool notValidateShip = false
            , IList<SalesItemModel> lstOnlyNF = null
            , ShippingInfoOfOrderModel shippingInfoOfOrderModel = null
            , OtherExpensesModel otherExpenses = null)
        {
            string ultimoEmbarque = "";
            bool lVenda = true;
            bool lDevolucao = false;
            bool lDevMateriais = false;
            lstCvj = new List<CVJ>();
            if (module == OryxModuleType.ORYX_ESQUADRIAS || module == OryxModuleType.ORYX_GESTAO || module == OryxModuleType.ORYX_PV)
            {
                VD6 lastVd6 = await OrderRepository.FindLastVd6Embarq(valorPai);
                if (lastVd6 == null)
                {
                    throw new Exception(string.Format("Nenhum embarque aberto para o pedido {0}.", valorPai));
                }
                ultimoEmbarque = lastVd6.Vd6embarq;

                if (!edit && !notValidateShip)
                {
                    //Verificando se está aberto
                    if (lastVd6.Vd6fecha > Constants.MinDateOryx)
                    {
                        throw new Exception(string.Format("Último embarque {0} fechado em {1}.", lastVd6.Vd6embarq, lastVd6.Vd6fecha.ToString("dd/MM/yyyy")));
                    }
                    //verificando se há outro documento para o mesmo pedido e embarque
                    CV5 openedCv5 = await FiscalDocumentRepository.FindFirstByShip(lastVd6.Vd6pedido, lastVd6.Vd6embarq);
                    if (openedCv5 != null)
                    {
                        if (module != OryxModuleType.ORYX_PV ||
                            (module == OryxModuleType.ORYX_PV && openedCv5.Cv5tipo.Equals(docfis)))
                        {
                            throw new Exception(string.Format("Documento: {0} aberto para o pedido {1} e embarque {2}.", openedCv5.Cv5doc, lastVd6.Vd6pedido, lastVd6.Vd6embarq));
                        }
                    }
                }

                if (module == OryxModuleType.ORYX_ESQUADRIAS)
                {
                    //verificando se é venda
                    if ((await OrderRepository.FindVd7(lastVd6.Vd6pedido, lastVd6.Vd6embarq)).Count == 0)
                    {
                        lVenda = false;
                        if ((await OrderRepository.FindVd5(lastVd6.Vd6pedido, lastVd6.Vd6embarq)).Count == 0)
                        {
                            if ((await SalesConfirmationRepository.FindVde(lastVd6.Vd6pedido, lastVd6.Vd6embarq)).Count == 0)
                            {
                                lDevolucao = false;
                            }
                            else
                            {
                                lDevolucao = true;
                                lDevMateriais = true;
                            }
                        }
                        else
                        {
                            lDevolucao = true;
                        }
                    }
                }
            }
            if (module == OryxModuleType.ORYX_MEMORIES)
            {
                CV5 jaFaturado = await FiscalDocumentRepository.FindForMemoriesByOrder(valorPai);
                if (jaFaturado != null)
                {
                    throw new Exception(string.Format("Pedido ja faturado no documento {0} {1}", jaFaturado.Cv5tipo, jaFaturado.Cv5doc));
                }
            }

            cv5.Cv5pedido = valorPai;
            vd1 = await OrderRepository.Find(valorPai);
            switch (module)
            {
                case OryxModuleType.ORYX_GESTAO:
                    cv5.Cv5embarq = ultimoEmbarque;
                    lstShipItems = await OrderRepository.FindAllShipItemsByVd8(cv5.Cv5pedido, cv5.Cv5embarq);
                    break;
                case OryxModuleType.ORYX_PV:
                    cv5.Cv5embarq = ultimoEmbarque;

                    if (!string.IsNullOrWhiteSpace(opercom))
                    {
                        vd1.Vd1opercom = opercom;
                    }

                    if (lxd.Lxdean)
                        lstShipItems = await OrderRepository.FindAllShipItemsByVdv(cv5.Cv5pedido, cv5.Cv5embarq);
                    else
                        lstShipItems = await OrderRepository.FindAllShipItemsByVd8(cv5.Cv5pedido, cv5.Cv5embarq);
                    break;
                case OryxModuleType.ORYX_MEMORIES:
                    lstShipItems = await OrderRepository.FindAllShipItemsForMemories(cv5.Cv5pedido);
                    break;
                case OryxModuleType.ORYX_ESQUADRIAS:
                    cv5.Cv5embarq = ultimoEmbarque;
                    if (lVenda)
                    {
                        lstShipItems = await OrderRepository.FindAllShipItemsForEsquadrias(cv5.Cv5pedido, cv5.Cv5embarq);
                        break;
                    }

                    if (!lDevMateriais)
                    {
                        lstInvoiceShipItems = await FiscalDocumentRepository.FindAllShipItemsForEsquadrias(cv5.Cv5pedido, cv5.Cv5embarq);
                        //TODO
                        //populateCv5();
                        //		replace cv5.cv5opercom WITH itensembarcados.vd6operdev
                        throw new NotImplementedException();
                    }
                    //TODO
                    //lstDevShipItems = await SalesConfirmationRepository.FindAllShipItemsForEsquadrias(cv5.Cv5pedido, cv5.Cv5embarq);
                    //populateCv5()
                    //		replace cv5.cv5opercom WITH itensembarcados.vd6operdev
                    throw new NotImplementedException();
                default:
                    break;
            }
            if (lVenda)
            {
                cv5.Cv5opercom = vd1.Vd1opercom;
            }
            else
            {
                //TODO
                //cv5.Cv5opercom = vd6operdev;
                throw new NotImplementedException();
            }

            if (lVenda)
            {
                cv5.Cv5frete = vd1.Vd1frete;
                cv5.Cv5vlfrete = vd1.Vd1vlfrete;
            }

            cv5 = await FiscalService.OpercomValida(cv5, lstCv7, campoPai, module);
            if (cv5.Cv5emispro && !cv5.Cv5cupom && !cv5.Cv5editar && !edit)
            {
                cv5.Cv5doc = await FiscalService.ProximoNumero(cv5);
            }

            cv5.Cv5cliente = vd1.Vd1cliente;
            cv5 = await FiscalService.ClienteValido(cv5, lstCvj, module);
            if (lVenda)
            {
                if (!vd1.Vd1consig)
                    cv5.Cv5conpgto = vd1.Vd1conpgto;
                cv5 = await FiscalService.CondicaoValida(cv5);
                cv5.Cv5lista = vd1.Vd1lista;
                cv5 = await FiscalService.ListaValida(cv5);
            }
            cv5.Cv5repres = vd1.Vd1repres;
            cv5 = await FiscalService.RepresValido(cv5);
            cv5.Cv5transp = vd1.Vd1transp;
            cv5 = await FiscalService.TranspValido(cv5);
            if (lVenda)
            {
                cv5.Cv5comis = vd1.Vd1comis;
                cv5.Cv5desconv = vd1.Vd1descon;
                cv5.Cv5descon = vd1.Vd1despon;
                cv5.Cv5desdias = vd1.Vd1desdias;
            }

            cv5.Cv5pedrep = vd1.Vd1pedrep;
            cv5.Cv5redesp = vd1.Vd1redesp;
            cv5.Cv5usuario = vd1.Vd1usuario;
            cv5.Cv5emissao = DateTime.Now;
            cv5.Cv5dhsaida = DateTime.Now;
            if (otherExpenses != null)
                cv5.Cv5outras = otherExpenses.Cv5outras;

            //referenciando documento de confirmação
            cv5.Cv5docconf = vdedoc;

            //incluíndo observações
            if (!string.IsNullOrWhiteSpace(vd1.Vd1observa))
                await IncludeObservations(vd1.Vd1observa);

            //incluíndo autorização na nota
            //if (lxd.Lxdintesho && !string.IsNullOrEmpty(codAuthSalesMall))
            //    await IncludeObservations(codAuthSalesMall);

            if (module == OryxModuleType.ORYX_PV && lxd.Lxdcaixa)
            {
                CX0 cx0 = await CashierRepository.FindLastByTerminal(terminal);
                if (cx0 == null || cx0.Cx0fecha > Constants.MinDateOryx)
                    throw new Exception(string.Format("Nenhum caixa aberto para o terminal {0}", terminal));
                cv5.Cv5caixa = cx0.Cx0caixa;
            }

            //referenciando documento de troca na venda
            if(lxd.Lxddocven1.Equals(cv5.Cv5tipo) || lxd.Lxddocven2.Equals(cv5.Cv5tipo))
                cv5.Cv5docdev = vdkdoc;

            if (!string.IsNullOrWhiteSpace(cv5.Cv5conpgto) && !naocalcularparcelas && (lstCv8 == null || lstCv8.Count == 0))
            {
                await FiscalService.CriarParcelas(cv5, lstCv8);
                throw new NotImplementedException(); //TODO
            }

            //gravando itens lstCv7 
            if (module == OryxModuleType.ORYX_GESTAO || module == OryxModuleType.ORYX_PV)
            {
                await CreateFiscalDocumentItemsByOrder(lstOnlyNF);
                //rateio da diferença de desconto total da nota com o desconto dos itens
                decimal totaldodesconto = cv5.Cv5desconv;
                totaldodesconto = lstCv7.Aggregate(totaldodesconto, (acc, cv7) => acc - cv7.Cv7descon);
                if (totaldodesconto != 0)
                    lstCv7[lstCv7.Count - 1].Cv7descon += Math.Round(totaldodesconto, 2, MidpointRounding.AwayFromZero);
            }
            if (module == OryxModuleType.ORYX_ESQUADRIAS && lVenda)
            {
                //TODO
                throw new NotImplementedException();
            }
            if (module == OryxModuleType.ORYX_ESQUADRIAS && !lVenda && lDevolucao && !lDevMateriais)
            {
                //TODO
                throw new NotImplementedException();
            }
            if (module == OryxModuleType.ORYX_ESQUADRIAS && !lVenda && lDevMateriais)
            {
                //TODO
                throw new NotImplementedException();
            }

            if (module == OryxModuleType.ORYX_MEMORIES)
            {
                //TODO
                throw new NotImplementedException();
            }

            //incluindo notas referenciadas
            if (module == OryxModuleType.ORYX_ESQUADRIAS && !lVenda && lDevolucao && !lDevMateriais)
            {
                //TODO
                throw new NotImplementedException();
            }

            lstCv7 = await FiscalService.AgruparItens(cv5, lstCv7, lstCv8, module, naocalcularparcelas, groupItemsByLineId);
            foreach (CV7 cv7 in lstCv7)
            {
                await FiscalService.ProdutoValido(cv7, cv5, module, authorization, noValidateItemPrice);
            }

            //ajuste do valor do desconto na nota conforme valores de troca de produtos/crédito de cliente
            if (module == OryxModuleType.ORYX_GESTAO || module == OryxModuleType.ORYX_PV)
            {
                decimal descontoDevolucao = 0;
                foreach (CV8 parcela in lstCv8)
                {
                    if (parcela.Cv8tipotit == lxd.Lxdtitulod)
                    {
                        descontoDevolucao += parcela.Cv8valor;
                    }
                }
                lstCv8 = lstCv8.Where(parcela => !parcela.Cv8tipotit.Equals(lxd.Lxdtitulod)).ToList();

                if(descontoDevolucao > 0)
                {
                    cv5.Cv5descfat = 0;
                    //tratamento para nota que é de um pedido com romaneio ou somente romaneio
                    IList<string> romaneios = await OrderRepository.FindInvoincedOrder(cv5.Cv5pedido, lxd.Lxddocven1);
                    if(romaneios.Count > 0 || cv5.Cv5tipo.Equals(lxd.Lxddocven1))
                    {
                        if (cv5.Cv5tipo.Equals(lxd.Lxddocven1))
                            cv5.Cv5descfat += descontoDevolucao;
                        cv5.Cv5desconv = vd1.Vd1descon + descontoDevolucao;
                        cv5.Cv5descon = (cv5.Cv5desconv * 100) / lstCv7.Sum(cv7 => cv7.Cv7vltotal);
                        foreach (CV7 cv7 in lstCv7)
                        {
                            cv7.Cv7desconp = cv5.Cv5descon;
                            cv7.Cv7descon = Math.Round(cv7.Cv7desconp / 100 * cv7.Cv7vltotal, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        //se for um CF ou uma NF avulso para o pedido - não colocar o valor de desconto na nota e preencher o cv5descfat
                        cv5.Cv5descfat += descontoDevolucao;
                        cv5.Cv5desconv = vd1.Vd1descon;
                        cv5.Cv5descon = (cv5.Cv5desconv * 100) / lstCv7.Sum(cv7 => cv7.Cv7vltotal);
                        foreach (CV7 cv7 in lstCv7)
                        {
                            cv7.Cv7desconp = cv5.Cv5descon;
                            cv7.Cv7descon = Math.Round(cv7.Cv7desconp / 100 * cv7.Cv7vltotal, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    FitDiscountValueItem();
                }
            }

            await FiscalService.Totalizar(cv5, lstCv7, lstCv8, module, naoCalcularParcelas: naocalcularparcelas, naoRecalcularDesconto: true);
            cv5.Cv5desconv = FiscalService.DescontoValor(lstCv7, cv5);

            await FiscalService.Totalizar(cv5, lstCv7, lstCv8, module, true, naoCalcularParcelas: naocalcularparcelas, naoRecalcularDesconto: true);
            if (!naocalcularparcelas)
            {
                await FiscalService.CriarParcelas(cv5, lstCv8);
            }
            else
            {
                if (module == OryxModuleType.ORYX_PV)
                    SetInstallments();
            }

            //if (module == OryxModuleType.ORYX_GESTAO || module == OryxModuleType.ORYX_PV)
            //{
            //    IList<VD7> lstVd7 = await OrderRepository.FindVd7(cv5.Cv5pedido, cv5.Cv5embarq);
            //    cv5.Cv5qtdevol = lstVd7.Count;
            //}
            if (module == OryxModuleType.ORYX_MEMORIES)
            {
                cv5.Cv5qtdevol = 1;
            }
            if (module == OryxModuleType.ORYX_ESQUADRIAS)
            {
                //TODO
                throw new NotImplementedException();
            }

            //incluíndo valor ST no desconto para notas, para cliente Bijoux
            if (cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("NOTA_FISCAL_ELETRÔNICA")) &&
                cv5.Cv5valsub > 0)
            {
                cv5.Cv5totalnf -= cv5.Cv5valsub;
                cv5.Cv5desconv += cv5.Cv5valsub;
                cv5.Cv5descon = (cv5.Cv5desconv * 100) / lstCv7.Sum(cv7 => cv7.Cv7vltotal);
                foreach (CV7 cv7 in lstCv7)
                {
                    cv7.Cv7desconp = cv5.Cv5descon;
                    cv7.Cv7descon = Math.Round(cv7.Cv7desconp / 100 * cv7.Cv7vltotal, 2, MidpointRounding.AwayFromZero);
                }
                FitDiscountValueItem();
            }

            //incluíndo dados de transporte/entrega
            if (shippingInfoOfOrderModel != null)
            {
                cv5.Cv5qtdevol = shippingInfoOfOrderModel.Cv5qtdevol;
                cv5.Cv5espevol = shippingInfoOfOrderModel.Cv5espevol;
                cv5.Cv5pesobru = shippingInfoOfOrderModel.Cv5pesobru;
                cv5.Cv5pesoliq = shippingInfoOfOrderModel.Cv5pesoliq;
                cv5.Cv5marcvol = shippingInfoOfOrderModel.Cv5marcvol;
                cv5.Cv5numevol = shippingInfoOfOrderModel.Cv5numevol;
            }

            //validação de valor máximo para não identificação do cliente em cupom fiscal
            if (cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CUPOM_FISCAL_ELETRÔNICO")) &&
                !Validators.IsCpf(cv5.Cv5cliente, true) &&
                !Validators.IsCnpj(cv5.Cv5cliente) &&
                FiscalService.lx3.Lx3cfvlmax > 0 &&
                cv5.Cv5totalnf > FiscalService.lx3.Lx3cfvlmax)
            {
                throw new Exception(string.Format("Necessário informar CPF em vendas acima de {0}", string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", FiscalService.lx3.Lx3cfvlmax)));
            }

            await Include(campoPai);
        }

        public async Task IncludeByCv5atende(FatherFieldType campoPai, string valorPai, string authorization, bool noValidateItemPrice = false, string opercom = "", bool edit = false)
        {
            //verificando se há outro documento para o mesmo ATENDA-SE
            CV5 cv5 = await FiscalDocumentRepository.FindByCv5atende(valorPai);
            if (cv5 != null)
            {
                throw new Exception(string.Format("Documento: {0} aberto para o atenda-se {1}.", cv5.Cv5doc, valorPai));
            }

            await Include(campoPai);
        }

        public async Task IncludeByCv5docdev(FatherFieldType campoPai, string valorPai, string authorization, string vdedoc, string opercom = "", bool edit = false, bool createCv7 = false)
        {
            //verificando se há outro documento para a mesma devolução
            CV5 existsCv5 = await FiscalDocumentRepository.FindByReturn(valorPai, opercom);
            if (existsCv5 != null && !edit)
                throw new Exception(string.Format("Documento: {0} aberto para a devolucao {1}.", existsCv5.Cv5doc, valorPai));

            VDK vdk = await ReturnRepository.Find(valorPai);

            if (!string.IsNullOrWhiteSpace(opercom))
                cv5.Cv5opercom = opercom;

            cv5 = await FiscalService.OpercomValida(cv5, lstCv7, campoPai, module);

            if (ThirdPartyDoc != null)
            {
                cv5.Cv5numorig = ThirdPartyDoc.Cv5numorig;
                cv5.Cv5doc = ThirdPartyDoc.Cv5doc;
                cv5.Cv5emissao = ThirdPartyDoc.Cv5emissao;
                cv5.Cv5dhsaida = ThirdPartyDoc.Cv5emissao;
                cv5.Cv5serie = ThirdPartyDoc.Cv5serie;
                cv5.Cv5nfechav = ThirdPartyDoc.Cv5nfechav;
                cv5.Cv5emissor = ThirdPartyDoc.Cv5emissor;
                cv5.Cv5cliente = ThirdPartyDoc.Cv5cliente;
                cv5.Cv5emispro = false;
            }
            else
            {
                cv5.Cv5cliente = vdk.Vdkcliente;
                cv5.Cv5emissao = DateTime.Now;
                cv5.Cv5dhsaida = DateTime.Now;

                //pegar proximo numero de nota
                if (cv5.Cv5emispro && !cv5.Cv5cupom && !cv5.Cv5editar && !edit)
                {
                    cv5.Cv5doc = await FiscalService.ProximoNumero(cv5);
                }
            }
            cv5 = await FiscalService.ClienteValido(cv5, lstCvj, module);
            cv5 = await FiscalService.EmissorValido(cv5);
            cv5 = await FiscalService.CondicaoValida(cv5);
            cv5.Cv5docdev = valorPai;
            cv5.Cv5docconf = vdedoc;

            cv5.Cv5lista = lxd.Lxdlista;
            cv5 = await FiscalService.ListaValida(cv5);

            cv5.Cv5usuario = vdk.Vdkusuario;

            if (createCv7)
            {
                await CreateFiscalDocumentItemsByReturn(vdk, cv5.Cv5tipo);
            }
            else
            {
                foreach (CV7 cv7 in lstCv7)
                {
                    cv7.Cv7doc = cv5.Cv5doc;
                    cv7.Cv7tipo = cv5.Cv5tipo;
                    cv7.Cv7emissor = cv5.Cv5emissor;
                }
            }

            lstCv7 = await FiscalService.AgruparItens(cv5, lstCv7, lstCv8, module, naocalcularparcelas, groupItemsByLineId);

            foreach (CV7 cv7 in lstCv7)
            {
                await FiscalService.ProdutoValido(cv7, cv5, module, authorization, true);
            }

            await FiscalService.Totalizar(cv5, lstCv7, lstCv8, module, naoAjustarPeloPercentual: true, naoRecalcularDesconto: true);

            foreach (CVQ cvq in lstCvq)
            {
                cvq.Cvqdoc = cv5.Cv5doc;
                cvq.Cvqemissor = cv5.Cv5emissor;
                cvq.Cvqtipo = cv5.Cv5tipo;
            }

            await Include(campoPai);
        }

        public async Task IncludeByCv5docrec(FatherFieldType campoPai, string valorPai, string authorization, bool noValidateItemPrice = false, string opercom = "", bool edit = false)
        {
            //Chamando ultimo recebimento
            //TODO essa parte não foi transcrita pois não foi encontrado tabela MEL
            throw new NotImplementedException();

            await Include(campoPai);
        }

        public async Task IncludeByCv5pedcom(FatherFieldType campoPai, string valorPai, string authorization, bool noValidateItemPrice = false, string opercom = "", bool edit = false)
        {
            string pedidoDeCompra, operSaida;
            //Chamando ultimo recebimento
            PC1 saidaPedidoDeCompra = await PurchaseOrderRepository.Find(valorPai);
            pedidoDeCompra = "";
            operSaida = "";
            if (saidaPedidoDeCompra != null)
            {
                pedidoDeCompra = saidaPedidoDeCompra.Pc1pedcom;
                operSaida = saidaPedidoDeCompra.Pc1opersai;
            }

            if (string.IsNullOrWhiteSpace(operSaida))
            {
                throw new Exception(string.Format("Nenhuma operacao de saida para terceirização definida no pedido de compra {0}.", valorPai));
            }
            //verificando se há outro documento para o mesmo pedido de compra e para o mesmo documento de recebimento
            CV5 purchaseOrderDoc = await FiscalDocumentRepository.FindByPurchaseOrder(pedidoDeCompra);
            if (purchaseOrderDoc != null)
            {
                throw new Exception(string.Format("Documento: {0} aberto para o pedido {1}.", purchaseOrderDoc.Cv5doc, pedidoDeCompra));
            }

            await Include(campoPai);
        }

        public async Task IncludeByCv5viagem(FatherFieldType campoPai, string valorPai, string authorization, bool noValidateItemPrice = false, string opercom = "", bool edit = false)
        {
            //verificando se há outro documento fechado para a viagem
            CV5 cv5 = await FiscalDocumentRepository.FindByCv5viagem(valorPai);
            if (cv5 != null)
            {
                throw new Exception(string.Format("Fechamento da viagem {0} efetuado no documento {1} - {2}.", valorPai, cv5.Cv5tipo, cv5.Cv5doc));
            }

            await Include(campoPai);
        }

        public async Task IncludeByCv5visita(FatherFieldType campoPai, string valorPai, string authorization, bool noValidateItemPrice = false, string opercom = "", bool edit = false)
        {
            //verificando se há outro documento fechado para a viagem
            CV5 cv5 = await FiscalDocumentRepository.FindByCv5visita(valorPai);
            if (cv5 != null)
            {
                throw new Exception(string.Format("Fechamento da viagem {0} efetuado no documento {1} - {2}.", valorPai, cv5.Cv5tipo, cv5.Cv5doc));
            }

            await Include(campoPai);
        }

        /*TODO IMPLEMENTAR CONFORME A CONIRMAÇÃO DE VENDA*/
        public async Task IncludeByCv5docconf(FatherFieldType campoPai, string valorPai, string authorization, string opercom, string lx9usuario, bool edit = false)
        {
            if (module == OryxModuleType.ORYX_GESTAO)
            {
                //verificando se há outro documento para o mesma confirmação e para o mesmo documento de recebimento
                CV5 cv5 = await FiscalDocumentRepository.FindByCv5docconf(valorPai, GoodsFlowType.GOODS_ENTRY);
                if (cv5 != null)
                    throw new Exception(string.Format("Documento Fiscal: {0} já utilizado para esta confirmação ({1}).", cv5.Cv5doc, valorPai));

                VDE vde = await SalesConfirmationRepository.Find(valorPai);
                if (vde == null)
                    throw new Exception(string.Format("Confirmação de venda {0} não encontrada",valorPai));

                cv5.Cv5opercom = opercom;

                cv5 = await FiscalService.OpercomValida(cv5, lstCv7, campoPai, module);

                cv5.Cv5cliente = vde.Vdecliente;
                cv5.Cv5emissao = DateTime.Now;
                cv5.Cv5dhsaida = DateTime.Now;

                //pegar proximo numero de nota
                if (cv5.Cv5emispro && !cv5.Cv5cupom && !cv5.Cv5editar && !edit)
                {
                    cv5.Cv5doc = await FiscalService.ProximoNumero(cv5);
                }
                cv5 = await FiscalService.ClienteValido(cv5, lstCvj, module);
                cv5 = await FiscalService.EmissorValido(cv5);
                cv5 = await FiscalService.CondicaoValida(cv5);
                cv5.Cv5docconf = valorPai;

                cv5.Cv5lista = lxd.Lxdlista;
                cv5 = await FiscalService.ListaValida(cv5);

                cv5.Cv5usuario = lx9usuario;

                //await CreateFiscalDocumentItemsBySaleConfirmation(vde, cv5.Cv5tipo);

                lstCv7 = await FiscalService.AgruparItens(cv5, lstCv7, lstCv8, module, naocalcularparcelas, groupItemsByLineId);

                foreach (CV7 cv7 in lstCv7)
                {
                    await FiscalService.ProdutoValido(cv7, cv5, module, authorization, true);
                }

                await FiscalService.Totalizar(cv5, lstCv7, lstCv8, module, naoAjustarPeloPercentual: true, naoRecalcularDesconto: true);

            }

            await Include(campoPai);
        }

        public async Task Include(FatherFieldType campoPai)
        {
            if (string.IsNullOrWhiteSpace(cv5.Cv5opercom))
            {
                cv5.Cv5opercom = FiscalService.lx3.Lx3opercom;
                if (!string.IsNullOrWhiteSpace(cv5.Cv5opercom))
                {
                    cv5 = await FiscalService.OpercomValida(cv5, lstCv7, campoPai, module);
                }
            }
        }
        #endregion 

        public void Recover(FatherFieldType campopai, string valorpai, string docfis, ref CV5 cv5, ref IList<CV7> lstCv7, ref IList<CV8> lstCv8, ref IList<CVJ> lstCvj, ref IList<CVQ> lstCvq, ref CVT cvt, string cv5opercom = "", string cv5embarq = "")
        {
            if (campopai == FatherFieldType.CV5PEDIDO)
            {
                VD6 lastVd6;
                if (string.IsNullOrWhiteSpace(cv5embarq))
                    lastVd6 = OrderRepository.FindLastVd6Embarq(valorpai).Result;
                else
                    lastVd6 = OrderRepository.FindVd6(valorpai, cv5embarq).Result;
                cv5 = FiscalDocumentRepository.FindByShip(valorpai, lastVd6.Vd6embarq, docfis).Result;
            }
            if (campopai == FatherFieldType.CV5DOCDEV)
            {
                cv5 = FiscalDocumentRepository.FindByReturn(valorpai, cv5opercom).Result;
            }
            if (cv5 != null)
            {
                lstCv7 = FiscalDocumentRepository.FindAllCv7(cv5.Cv5doc, cv5.Cv5emissor, cv5.Cv5tipo).Result;
                lstCv8 = FiscalDocumentRepository.FindAllCv8(cv5.Cv5doc, cv5.Cv5emissor, cv5.Cv5tipo).Result;
                lstCvj = FiscalDocumentRepository.FindAllCvj(cv5.Cv5doc, cv5.Cv5emissor, cv5.Cv5tipo).Result;
                lstCvq = FiscalDocumentRepository.FindAllCvq(cv5.Cv5doc, cv5.Cv5emissor, cv5.Cv5tipo).Result;
                cvt = FiscalDocumentRepository.FindCVT(cv5.Cv5doc, cv5.Cv5emissor, cv5.Cv5tipo).Result;
            }
        }

        public async Task UpdatePurchaseCreditFromSales(decimal usedCredit)
        {
            if (usedCredit <= 0)
                return;
            IList<FNL> lstFnl = await PurchaseCreditRepository.FindAll(cv5.Cv5cliente);

            foreach (FNL fnl in lstFnl)
            {
                if (usedCredit >= fnl.Fnlcredito)
                {
                    await PurchaseCreditRepository.Delete(fnl.Fnlemissor, fnl.Fnltipo, fnl.Fnldoc, fnl.Fnlcliente);
                    usedCredit -= fnl.Fnlcredito;
                }
                else
                {
                    fnl.Fnlcredito -= usedCredit;
                    usedCredit = 0;
                    await PurchaseCreditRepository.Update(fnl);
                    break;
                }
            }
        }

        public async Task<bool> SaveSalesMallIntegration(string codAuthSalesMall)
        {
            if (string.IsNullOrWhiteSpace(cv5.Cv5doc))
                return false;

            TecdatasoftModel existAuth = await TecdatasoftRepository.Find(cv5.Cv5emissor, cv5.Cv5tipo, cv5.Cv5doc);
            if(existAuth == null)
            {
                int affectedRows = await TecdatasoftRepository.Insert(new TecdatasoftModel()
                {
                    Cv7emissor = cv5.Cv5emissor,
                    Cv7tipo = cv5.Cv5tipo,
                    Cv7doc = cv5.Cv5doc,
                    Cv7subtot = cv5.Cv5totalnf,
                    Autenticacao = codAuthSalesMall
                });
                return affectedRows == 1;
            }
            return true;
        }

        public async Task<bool> Emit(bool edit = false, bool consigned = false, decimal vltroca = 0)
        {
            if (string.IsNullOrWhiteSpace(cv5.Cv5nfechav) && 
               (cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("NOTA_FISCAL_ELETRÔNICA")) ||
                cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CONHECIMENTO_DE_TRANSPORTE_ELETRÔNICO")) ||
                cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CUPOM_FISCAL_ELETRÔNICO"))) &&
                !cv5.Cv5emispro)
            {
                throw new Exception("Chave de Acesso não informada em Outros/NFE.");
            }
            //TODO validações para fazer em tela

            if ((cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("NOTA_FISCAL_ELETRÔNICA")) ||
                cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CONHECIMENTO_DE_TRANSPORTE_ELETRÔNICO")) ||
                cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CUPOM_FISCAL_ELETRÔNICO"))) && cv5.Cv5emispro)
            {
                ValidateNFe();

                await CalcularChavedeAcesso();
            }

            bool sucess = await Save(edit, vltroca, true, consigned);
            if (!sucess)
            {
                throw new Exception("Não foi possível salvar o documento fiscal");
            }
            
            if(cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CONHECIMENTO_DE_TRANSPORTE_ELETRÔNICO")) && cv5.Cv5emispro){
                //TODO
                throw new NotImplementedException();
            }
            if (cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CONHECIMENTO_DE_TRANSPORTE_ELETRÔNICO")) && cv5.Cv5emispro)
            {
                //TODO
                throw new NotImplementedException();
            }

            if (cv5.Cv5online && cv5.Cv5cupom)
            {
                sucess = await FiscalService.FecharCupom(cv5, lstCv8, lstCv7, cv5.Cv5titulo);
                if (!sucess)
                {
                    throw new Exception("Não foi possível fechar o cupom fiscal.");
                }
            }

            if (!FiscalService.oryxnfe)
            {
                throw new NotImplementedException();
                //TODO
            }
            else
            {
                if((!cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("NOTA_FISCAL_ELETRÔNICA")) &&
                    !cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CONHECIMENTO_DE_TRANSPORTE_ELETRÔNICO")) &&
                    !cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CUPOM_FISCAL_ELETRÔNICO"))) ||
                    !cv5.Cv5emispro
                ){
                    await FiscalDocumentRepository.SetEmit(cv5.Cv5emissor, cv5.Cv5tipo, cv5.Cv5doc, true);
                    cv5.Cv5emitido = true;
                }
            }

            if (cv5.Cv5emispro && !cv5.Cv5online && !cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("DOCUMENTO_INTERNO")))
            {                
                if (!cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("NOTA_FISCAL_ELETRÔNICA")) &&
                    !cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CONHECIMENTO_DE_TRANSPORTE_ELETRÔNICO")) &&
                    !cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CUPOM_FISCAL_ELETRÔNICO")))
                {
                    //TODO
                    throw new NotImplementedException();
                }
                string strXml;
                //gerando arquivo texto para nota eletronica
                if (cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CONHECIMENTO_DE_TRANSPORTE_ELETRÔNICO")))
                {
                    strXml = "";
                }
                else
                {
                    strXml = await FiscalService.GerarStrNfe(cv5.Cv5emissor, cv5.Cv5tipo, cv5.Cv5doc);
                }

                if (!string.IsNullOrWhiteSpace(strXml) && FiscalService.oryxnfe)
                {
                    // enviando NFE
                    if (!cv5.Cv5nfechav.Substring(35,8).Equals(await SisService.CodigoDFE(cv5.Cv5doc)))
                    {
                        throw new Exception("Chave de acesso invalida.");
                    }
                    if (cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CONHECIMENTO_DE_TRANSPORTE_ELETRÔNICO")))
                    {
                        //TODO enviarCTe
                        throw new NotImplementedException();
                    }
                    else
                    {
                        await OryxNetSendNF(strXml, FiscalService.oryxNFeModel.Cf4codibge, FiscalService.oryxNFeModel.Lx0homolnf, FiscalService.nomeDoCertificado, FiscalService.oryxNFeModel.Lx0certpas);
                    }
                }
            }


            return true;
        }

        public async Task<bool> Save(bool edit, decimal vltroca, bool isClosing = false, bool consigned = false)
        {
            if (cv5.Cv5naocont)
            {
                // assegurando-se de que a nota nao gere valor contabil
                cv5.Cv5totalnf = cv5.Cv5valsub;
                cv5.Cv5totalpr = 0;
                cv5.Cv5desconv = 0;
                cv5.Cv5pesobru = 0;
                cv5.Cv5pesoliq = 0;
                cv5.Cv5vlfrete = 0;
                cv5.Cv5outras = 0;
                cv5.Cv5seguro = 0;
                cv5.Cv5qtdevol = 0;

                foreach (CV7 cv7 in lstCv7)
                {
                    cv7.Cv7qtde = 0;
                    cv7.Cv7vlunit = 0;
                    cv7.Cv7vltotal = 0;
                    cv7.Cv7peso = 0;
                    cv7.Cv7descon = 0;
                }

                lstCv8 = new List<CV8>();
            }

            if (cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CONHECIMENTO_DE_TRANSPORTE_ELETRÔNICO")) &&
                string.IsNullOrWhiteSpace(cv5.Cv5observ) &&
                cv5.Cv5emispro)
            {
                throw new Exception("Para CTe preenchimento obrigatório do campo observações.");
            }

            await FiscalService.Renumerar(lstCv7);

            if (lstCv7.Count > 990)
            {
                throw new Exception("Documento com mais de 990 itens diferentes. Tente Agrupar ou excluir.");
            }

            if (FiscalService.Devolucao(cv5.Cv5cfop) &&
                cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("NOTA_FISCAL_ELETRÔNICA")) && 
                cv5.Cv5emispro &&
                isClosing)
            {
                if (lstCvq == null || lstCvq.Count == 0)
                {
                    throw new Exception("Devolução requer nota ou cupom referenciado.");
                }
            }

            foreach (CV7 cv7 in lstCv7)
            {
                if (cv7.Cv7vltotal == 0 && !cv5.Cv5naocont)
                    throw new Exception("Item " + cv7.Cv7codigo + " com quantidade ou valor em branco.");

                if (string.IsNullOrWhiteSpace(cv7.Cv7codclas) &&
                    string.IsNullOrWhiteSpace(cv7.Cv7codiss) &&
                    !cv5.Cv5naocont &&
                    string.IsNullOrWhiteSpace(cv7.Cv7classif))
                    throw new Exception("Item " + cv7.Cv7codigo + " sem classificação fiscal.");

                if (cv7.Cv7peso == 0 &&
                    string.IsNullOrWhiteSpace(cv7.Cv7codiss) &&
                    !cv5.Cv5naocont &&
                    !cv7.Cv7classif.Equals("00") &&
                    !cv7.Cv7servico &&
                    module != OryxModuleType.ORYX_PV)
                    throw new Exception("Item " + cv7.Cv7codigo + " sem peso informado.");
            }

            // verificando condicoes fiscais
            if (string.IsNullOrWhiteSpace(cv5.Cv5observ) && cv5.Cv5ajuste)
            {
                throw new Exception("Notas fiscais de ajuste, requerem texto e motivo legais em observacoes.");
            }

            if (cv5.Cv5emispro)
            {
                int cv5numorigSize = await FormatterService.FindFieldLength(nameof(cv5.Cv5numorig));
                int cv5docSize = await FormatterService.FindFieldLength(nameof(cv5.Cv5doc));
                cv5.Cv5numorig = new string('0', cv5numorigSize - cv5docSize) + cv5.Cv5doc;
            }

            if (cv5.Cv5vlrserv > 0 && string.IsNullOrWhiteSpace(cv5.Cv5codiss))
            {
                throw new Exception("Tipo do Serviço não pode ficar em branco.");
            }

            // verificando se o número já foi utilizado
            CV5 numeroutilizado = await FiscalDocumentRepository.Find(cv5.Cv5doc, cv5.Cv5tipo, cv5.Cv5emissor);
            if (numeroutilizado != null && !cv5.Cv5editar && !edit)
            {
                await SetNewDocNumber();
            }

            // verificando se os valores de parcelas fecham
            decimal valortotal = lstCv8 != null ? lstCv8.Aggregate(0M, (acc, cv8) => acc + cv8.Cv8valor) : 0;
            if (lstCv8 != null && lstCv8.Count > 0 && valortotal == 0)
            {
                if (cv5.Cv5descfat > 0)
                {
                    throw new Exception("Informado desconto na fatura para documento sem valor financeiro.");
                }
                if (valortotal != (cv5.Cv5totalnf - cv5.Cv5descfat))
                {
                    throw new Exception(string.Format("Valor de parcelas não fechou. {0}", valortotal));
                }
                decimal somacontabil = lstCv7
                    .Where(cv7 => !string.IsNullOrWhiteSpace(cv7.Cv7contac.Trim()) || !string.IsNullOrWhiteSpace(cv7.Cv7contad.Trim()))
                    .Aggregate(0M, (acc, cv7) => acc + cv7.Cv7vltotal);
                if (valortotal != somacontabil && somacontabil != 0)
                {
                    throw new Exception(string.Format("Valor de parcelas não fechou com os itens contabilizados. <br/>Parcelas: R$ {0} Contabilizado: {1} <br/>Tente Integrar nos itens, descontos E/ou outras despesas. <br/>Desconto, se informado deve cobrir os itens nao contabilizados.", valortotal, somacontabil));
                }
            }

            // gravando observacoes fiscal se o campo cv5observ estiver em branco e existir documento referenciado
            if (lstCvq != null && lstCvq.Any() && string.IsNullOrWhiteSpace(cv5.Cv5observ))
            {
                foreach (CVQ cvq in lstCvq)
                {
                    if (!string.IsNullOrWhiteSpace(cvq.Cvqdocrefe))
                    {
                        cv5.Cv5observ = string.Format("#Ref {0} {1}-{2}", cv5.Cv5operdes, cvq.Cvqtiprefe, cvq.Cvqdocrefe);
                    }
                }
            }

            await FiscalDocumentRepository.DeleteForInsert(cv5.Cv5doc, cv5.Cv5emissor, cv5.Cv5tipo);

            ValidateFormatBasicFiscalDocument(cv5, lstCv7, lstCv8, cvt, lstCvq, lstCvj).Wait();

            int affectedRows = await FiscalDocumentRepository.SaveFiscalDocument(cv5, lstCv7, lstCv8, cvt, lstCvq, lstCvj);

            // fechando embarque se estiver aberto
            if (!string.IsNullOrWhiteSpace(cv5.Cv5pedido) &&
                !string.IsNullOrWhiteSpace(cv5.Cv5embarq))
            {
                if (module != OryxModuleType.ORYX_PV ||
                    (
                        module == OryxModuleType.ORYX_PV &&
                        (!lxd.Lxdromanf && !consigned) ||
                        (!consigned && lxd.Lxdromanf && cv5.Cv5tipo.Equals(docfis)) ||
                        (consigned && cv5.Cv5tipo.Equals(docfis))
                    )
                )
                {
                    await OrderRepository.CloseShipment(cv5.Cv5pedido, cv5.Cv5embarq, DateTime.Now);
                }
            }

            //descontando o crédito do cliente
            if (module == OryxModuleType.ORYX_PV &&
               !string.IsNullOrWhiteSpace(lxd.Lxdtitulod) &&
               cv5.Cv5entsai == GoodsFlowType.GOODS_EXIT &&
               !cv5.Cv5tipo.Equals(lxd.Lxddocven2) &&
               !cv5.Cv5tipo.Equals(lxd.Lxddocven5) &&
               !edit &&
               (!lxd.Lxdromanf || lxd.Lxdromanf && cv5.Cv5tipo.Equals(docfis)))
            {
                await UpdatePurchaseCreditFromSales(vltroca);
            }

            return affectedRows == 1;
        }

        public async Task Print(PrintModel printModel, decimal vias, string authorization)
        {
            for (int i = 0; i < vias; i++)
            {
                new PrintService(Configuration).Print(printModel, authorization).Wait();
            }
            await FiscalDocumentRepository.SetPrint(cv5.Cv5emissor, cv5.Cv5tipo, cv5.Cv5doc, true);
        }

        public async Task<bool> Cancel(string cv5doc, string cv5tipo, string cv5emissor, string reason, string authorization)
        {
            LXE lxe = await APIParametersRepository.Find();
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");
            
            Dictionary<string, string> querie = new Dictionary<string, string>()
            {
                {"cv5doc", cv5doc},
                {"cv5tipo", cv5tipo},
                {"cv5emissor", cv5emissor},
                {"motivo", reason},
            };

            string jsonResponse = await HttpUtilities.CallPutAsync(
                  lxe.Lxebaseurl
                , "/FiscalDocument/FiscalDocument/Cancel"
                , string.Empty
                , authorization
                , querie);

            ReturnModel<bool> returnModel = JsonConvert.DeserializeObject<ReturnModel<bool>>(jsonResponse);

            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }

            return returnModel.ObjectModel;
        }

        public async Task<bool> Disable(string cv5doc, string cv5tipo, string cv5emissor, string reason, string authorization)
        {
            LXE lxe = await APIParametersRepository.Find();
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");

            Dictionary<string, string> querie = new Dictionary<string, string>()
            {
                {"cv5doc", cv5doc},
                {"cv5tipo", cv5tipo},
                {"cv5emissor", cv5emissor},
                {"motivo", reason},
            };

            string jsonResponse = await HttpUtilities.CallPutAsync(
                  lxe.Lxebaseurl
                , "/FiscalDocument/FiscalDocument/Disable"
                , string.Empty
                , authorization
                , querie);

            ReturnModel<bool> returnModel = JsonConvert.DeserializeObject<ReturnModel<bool>>(jsonResponse);

            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }

            return returnModel.ObjectModel;
        }

        #region private methods

        private async Task<bool> CalcularChavedeAcesso()
        {
            string cUf = await ParametersRepository.FindCodIBGE();
            string aamm = cv5.Cv5emissao.ToString("yy") + cv5.Cv5emissao.ToString("MM");
            string cNF = await SisService.CodigoDFE(cv5.Cv5doc);
            string digit = Convert.ToInt64(cv5.Cv5serie) >= 900 ? "3" : !string.IsNullOrWhiteSpace(cv5.Cv5regdpec) || FiscalService.contigencia ? "4" : "1";
            int cv5serieSize = await FormatterService.FindFieldLength(nameof(cv5.Cv5serie));
            string cNfechav = string.Format("{0}{1}{2}{3}{4}000{5}{6}{7}",cUf,aamm, cv5.Cv5emissor, cv5.Cv5modelo, cv5.Cv5serie.PadLeft(cv5serieSize, '0'), cv5.Cv5doc, digit, cNF);
            cNfechav += SisService.Modulo11(cNfechav);
            cv5.Cv5nfechav = cNfechav;
            return true;
        }

        private async Task SetNewDocNumber()
        {
            string outronumero = await FiscalService.ProximoNumero(cv5);
            cv5.Cv5doc = outronumero;
            foreach (CV7 cv7 in lstCv7)
            {
                cv7.Cv7doc = outronumero;
            }
            foreach (CV8 cv8 in lstCv8)
            {
                cv8.Cv8doc = outronumero;
            }
            if (cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("NOTA_FISCAL_ELETRÔNICA")) &&
                cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CUPOM_FISCAL_ELETRÔNICO")) &&
                FiscalService.oryxnfe)
            {
                await CalcularChavedeAcesso();
            }
        }

        private void ValidateNFe()
        {
            /*
                 * * pre validacao de nota fiscal eletronica *
	             *******************************************
	             * 1. Verificar situacao do contribuinte (Inscricao Estadual) WS - OryxNFe instalado
	             * 2. Verificar codigo do municipio (IBGE)
	             * 3. Verificar unidades de medida dos itens	
	             ******************************
                 */

            if (string.IsNullOrWhiteSpace(cv5.Cv5endcli))
            {
                throw new Exception("Endereço do Cliente não Informado.");
            }

            IList<CV7> invalidItems = lstCv7.Where(c => string.IsNullOrWhiteSpace(c.Cv7unmed)).ToList();
            if (invalidItems.Count > 0)
            {
                throw new Exception(string.Format("Falta unidade de medida nos itens {0}", string.Join(",", invalidItems)));
            }

            if (!cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CUPOM_FISCAL_ELETRÔNICO")) &&
                (cv5.Cv5cliente.Equals("00000000000") || cv5.Cv5cliente.Equals("00000000000000")))
            {
                throw new Exception("CNPJ/CPF invalido.");
            }

            if (cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("CUPOM_FISCAL_ELETRÔNICO")))
            {
                DateTime now = DateTime.Now;
                cv5.Cv5emissao = now;
                cv5.Cv5dhsaida = now;
            }

            if (string.IsNullOrWhiteSpace(cv5.Cv5trcnpj) && !string.IsNullOrWhiteSpace(cv5.Cv5transp))
                throw new Exception(string.Format("CNPJ/CPF  do transportador {0} não informado.<br/>Corrija o cadastro do transportador e redigite seu codigo na nota.", cv5.Cv5transp));

            if (!string.IsNullOrWhiteSpace(cv5.Cv5digval))
                throw new Exception("Este documento ja foi autorizado. Consulte NFe");
        }

        private async Task OryxNetSendNF(string strXml, string codibge, bool lx0homolnf, string nomeDoCertificado, string lx0certpas)
        {
            EnviarModel enviarModel = new EnviarModel()
            {
                Codibge = codibge,
                Homolnf = lx0homolnf ? "2" : "1",
                NomeDoCertificado = nomeDoCertificado,
                StrXml = strXml,
                Lx0certpas = lx0certpas
            };

            string xmlResponse = await HttpUtilities.CallPostAsync(
                  lxe.Lxebaseurl
                , "/OryxNet/Principal/EnviarNFe"
                , JsonConvert.SerializeObject(enviarModel));
            
            XmlDocument xml = new XmlDocument();
            RetEnviNFe payload = new RetEnviNFe();
            XmlNodeList xmlList;
            string message = "";
            
            xml.PreserveWhitespace = true;
            Logger.LogWarning(xmlResponse);
            try
            {
                xml.LoadXml(xmlResponse);
                xmlList = xml.GetElementsByTagName("cStat");
            
                if (xmlList != null && xmlList.Count > 0)
                {
                    payload.CStat = xmlList[xmlList.Count-1].InnerText;
                }
            }
            catch (Exception)
            {
                // em caso de erro tratado pelo OryxNet, vem o retorno fora de padrão XML
                payload.CStat = XmlUtils.FindValueByIndexOf(xmlResponse, "<cStat>", "</cStat>");
                if (payload.CStat.Equals("ERR"))
                {
                    message = XmlUtils.FindValueByIndexOf(xmlResponse, "<xMotivo>", "</xMotivo>");
                    string stack = XmlUtils.FindValueByIndexOf(xmlResponse, "<xErro>", "</xErro>");
                    throw new Exception(string.Format("{0}\n{1}", message, stack));
                }
            }

            switch (payload.CStat)
            {
                case "100":
                    ProtNFe protNFe = null;
                    try
                    {
                        RetEnviNFe ret = XmlUtils.Deserialize<RetEnviNFe>(xmlResponse);
                        protNFe = ret.ProtNFe;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    if (protNFe == null)
                    {
                        try
                        {
                            NfeProc ret = XmlUtils.Deserialize<NfeProc>(xmlResponse);
                            protNFe = ret.ProtNFe;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    if (protNFe != null && protNFe.InfProt != null)
                    {
                        cv5.Cv5nprot = protNFe.InfProt.NProt;
                        cv5.Cv5digval = protNFe.InfProt.DigVal;
                        cv5.Cv5dhrec = protNFe.InfProt.DhRecbto;
                    }
                    cv5.Cv5emitido = true;
                    cv5.Cv5xml = xmlResponse;

                    await FiscalDocumentRepository.Update(cv5);
                    //IF thisform.fiscaL.geraremail(cv5.cv5emissor,cv5.cv5tipo,cv5.cv5doc,pcPara)=.f.
                    //envia e-mails caso lx0email estiver ativo
                    //TODO
                    break;
                case "205":
                case "301":
                case "302":
                case "303":
                case "110":
                    cv5.Cv5situa = "D";
                    cv5.Cv5emitido = false;
                    xmlList = xml.GetElementsByTagName("xMotivo");
                    if (xmlList != null && xmlList.Count > 0)
                    {
                        foreach (XmlNode node in xmlList)
                        {
                            message = message + node.InnerText + ".<br/>";
                        }
                    }
                    cv5.Cv5observ = message;
                    await FiscalDocumentRepository.Update(cv5);
                    throw new Exception(message);
                default:
                    xmlList = xml.GetElementsByTagName("xMotivo");
                    if (xmlList != null && xmlList.Count > 0)
                    {
                        foreach (XmlNode node in xmlList)
                        {
                            message = message + node.InnerText + ".<br/>";
                        }

                    }
                    throw new Exception(message);
            }
        }

        private void SetInstallments()
        {
            foreach (CV8 cv8 in lstCv8)
            {
                cv8.Cv8emissor = cv5.Cv5emissor;
                cv8.Cv8tipo = cv5.Cv5tipo;
                cv8.Cv8doc = cv5.Cv5doc;
            }

            lstCv8 = FiscalService.ReenumerarParcelas(lstCv8).Result;
        }

        private async Task ValidateFormatBasicFiscalDocument(CV5 cv5, IList<CV7> lstCv7, IList<CV8> lstCv8, CVT cvt, IList<CVQ> lstCvq, IList<CVJ> lstCvj)
        {
            await FormatterService.ValidateFormatBasicByDC1(cv5);
            
            foreach (CV7 cv7 in lstCv7)
            {
                await FormatterService.ValidateFormatBasicByDC1(cv7);
            }

            foreach (CV8 cv8 in lstCv8)
            {
                await FormatterService.ValidateFormatBasicByDC1(cv8);
            }

            foreach (CVJ cvj in lstCvj)
            {
                await FormatterService.ValidateFormatBasicByDC1(cvj);
            }

            if (lstCvq != null)
            {
                foreach (CVQ cvq in lstCvq)
                {
                    await FormatterService.ValidateFormatBasicByDC1(cvq);
                }
            }

            if (cvt != null)
            {
                await FormatterService.ValidateFormatBasicByDC1(cvt);
            }
        }

        private async Task IncludeObservations(string cvjobserva)
        {
            int chunks = await FormatterService.FindFieldLength("cvjobserva");

            int cvjlinha = lstCvj.Count;
            for (int i = 0; i < cvjobserva.Length; i+= chunks)
            {
                if (i + chunks > cvjobserva.Length)
                    chunks = cvjobserva.Length - i;
                lstCvj.Add(new CVJ()
                {
                    Cvjdoc = cv5.Cv5doc,
                    Cvjemissor = cv5.Cv5emissor,
                    Cvjtipo = cv5.Cv5tipo,
                    Cvjlinha = Alphabet[cvjlinha],
                    Cvjobserva = cvjobserva.Substring(i, chunks),
                });
                cvjlinha++;
            }
        }
        #region create fiscal document Items
        private async Task CreateFiscalDocumentItemsByOrder(IList<SalesItemModel> lstOnlyNF)
        {
            lstCv7 = new List<CV7>();
            foreach (ShipItem shipItem in lstShipItems)
            {
                CV7 cv7 = new CV7()
                {
                    Cv7emissor = cv5.Cv5emissor,
                    Cv7tipo = cv5.Cv5tipo,
                    Cv7doc = cv5.Cv5doc,
                    Cv7codigo = shipItem.Produto,
                    Cv7cor = shipItem.Opcao,
                    Cv7tamanho = shipItem.Tamanho,
                    Cv7vlunit = shipItem.Preco,
                    Cv7qtde = shipItem.Qtde,
                    Cv7desconp = cv5.Cv5descon,
                    Cv7descon = Math.Round(cv5.Cv5descon / 100 * shipItem.Preco * shipItem.Qtde, 2, MidpointRounding.AwayFromZero)
                };
                //if (module == OryxModuleType.ORYX_GESTAO || !lxd.Lxdean)
                //{
                //    //get weight by OFL - peso das peças agrupadas
                //    //TODO não foi reescrito pois não é utilizado
                //    throw new NotImplementedException();
                //}
                if (cv5.Cv5ean)
                {
                    cv7.Cv7ean = await SisService.Ean(shipItem.Produto, shipItem.Opcao, shipItem.Tamanho);
                }
                lstCv7.Add(cv7);
            }

            if(lstOnlyNF != null && lstOnlyNF.Count > 0)
            {
                IList<CV7> tempLstCv7 = new List<CV7>();
                foreach (SalesItemModel item in lstOnlyNF)
                {
                    CV7 tempCv7 = tempLstCv7.FirstOrDefault(c => c.Cv7codigo == item.Vd2produto &&
                                                                 c.Cv7cor == item.Vd3opcao &&
                                                                 c.Cv7tamanho == item.Vd3tamanho &&
                                                                 c.Cv7vlunit == item.Vd5preco);

                    if(tempCv7 != null)
                    {
                        tempCv7.Cv7qtde += item.Vd3qtde;
                    }
                    else
                    {
                        tempCv7 = new CV7()
                        {
                            Cv7emissor = cv5.Cv5emissor,
                            Cv7tipo = cv5.Cv5tipo,
                            Cv7doc = cv5.Cv5doc,
                            Cv7codigo = item.Vd2produto,
                            Cv7cor = item.Vd3opcao,
                            Cv7tamanho = item.Vd3tamanho,
                            Cv7vlunit = item.Vd5preco,
                            Cv7qtde = item.Vd3qtde,
                            Cv7desconp = cv5.Cv5descon,
                            Cv7descon = Math.Round(cv5.Cv5descon / 100 * item.Vd5preco * item.Vd3qtde, 2, MidpointRounding.AwayFromZero),
                            Cv7flag1 = true
                        };
                        if (cv5.Cv5ean)
                        {
                            tempCv7.Cv7ean = await SisService.Ean(item.Vd2produto, item.Vd3opcao, item.Vd3tamanho);
                        }

                        tempLstCv7.Add(tempCv7);
                    }
                }

                lstCv7 = lstCv7.Concat(tempLstCv7).ToList();
            }
        }

        private async Task CreateFiscalDocumentItemsByReturn(VDK vdk, string cv5tipo)
        {
            lstCv7 = new List<CV7>();
            if (lxd.Lxdean)
            {
                IList<VDX> lstVdx = await ReturnRepository.FindAllVdx(vdk.Vdkdoc);
                lstVdx = lstVdx.Where(vdx => vdx.Vdxqtdeent == 0).ToList();
                foreach (VDX vdx in lstVdx)
                {
                    lstCv7.Add(new CV7()
                    {
                        Cv7item = vdx.Vdxitem,
                        Cv7emissor = cv5.Cv5emissor,
                        Cv7tipo = cv5.Cv5tipo,
                        Cv7doc = cv5.Cv5doc,
                        Cv7codigo = vdx.Vdxproduto,
                        Cv7desc = vdx.Pr0desc,
                        Cv7cor = vdx.Vdxopcao,
                        Cv7tamanho = vdx.Vdxtamanho,
                        Cv7vlunit = vdx.Vdxpreco,
                        Cv7qtde = vdx.Vdxqtde,
                        Cv7unmed = "un",
                        Cv7ean = vdx.Vdxpeca
                    });
                }
            }
            else
            {
                IList<VDL> lstVdl = await ReturnRepository.FindAllVdl(vdk.Vdkdoc);
                foreach (VDL vdl in lstVdl)
                {
                    string eancodigo = "";
                    EAN ean = await EanCodificationRepository.FindEan(vdl.Of3produto, vdl.Of3opcao, vdl.Of3tamanho);
                    if (ean != null)
                    {
                        eancodigo = ean.Eancodigo;
                    }
                    else
                    {
                        PR0 pr0 = await ProductRepository.Find<PR0>(vdl.Of3produto);
                        eancodigo = pr0 == null ? string.Empty : pr0.Pr0ean;
                    }

                    VD1 order = await OrderRepository.Find(vdl.Vdlpedido);
                    
                    if ((order.Vd1consig && (cv5tipo.Equals(lxd.Lxddocdev2) || cv5tipo.Equals(lxd.Lxddocdev4))) ||
                        (!order.Vd1consig && (cv5tipo.Equals(lxd.Lxddocdev1) || cv5tipo.Equals(lxd.Lxddocdev3))))
                    {
                        CV7 existcv7 = lstCv7.FirstOrDefault(cv7 => cv7.Cv7codigo.Equals(vdl.Of3produto) &&
                                                                    cv7.Cv7cor.Equals(vdl.Of3opcao) &&
                                                                    cv7.Cv7tamanho.Equals(vdl.Of3tamanho) &&
                                                                    cv7.Cv7vlunit == vdl.Vdlpreco);
                        if (existcv7 != null)
                        {
                            existcv7.Cv7qtde += 1;
                        }
                        else
                        {
                            lstCv7.Add(new CV7()
                            {
                                Cv7emissor = cv5.Cv5emissor,
                                Cv7tipo = cv5.Cv5tipo,
                                Cv7doc = cv5.Cv5doc,
                                Cv7codigo = vdl.Of3produto,
                                Cv7desc = vdl.Pr0desc,
                                Cv7cor = vdl.Of3opcao,
                                Cv7tamanho = vdl.Of3tamanho,
                                Cv7vlunit = vdl.Vdlpreco,
                                Cv7qtde = 1,
                                Cv7unmed = "un",
                                Cv7ean = eancodigo
                            });
                        }
                    }
                }
            }
        }
        
        /* TODO criar itens da nota conforme a confirmação de venda
        private async Task CreateFiscalDocumentItemsBySaleConfirmation(VDE vde, string cv5tipo)
        {
            int cv7item = 1;
            lstCv7 = new List<CV7>();
            if (lxd.Lxdean)
            {
                IList<VDZ> lstVdz = await SalesConfirmationItemsRepository.FindAllVDZ(vde.Vdedoc);
                
                foreach (VDZ vdz in lstVdz)
                {
                    lstCv7.Add(new CV7()
                    {
                        Cv7item = cv7item.ToString(),
                        Cv7emissor = cv5.Cv5emissor,
                        Cv7tipo = cv5.Cv5tipo,
                        Cv7doc = cv5.Cv5doc,
                        Cv7codigo = vdz.Vdzproduto,
                        Cv7desc = vdz.Pr0desc,
                        Cv7cor = vdz.Vdzopcao,
                        Cv7tamanho = vdz.Vdztamanho,
                        Cv7vlunit = vdz.Vdzpreco,
                        Cv7qtde = vdz.Vdzqtde,
                        Cv7unmed = "un",
                        Cv7ean = vdz.Vdzpeca
                    });
                    cv7item++;
                }
            }
            else
            {
                IList<VDF> lstVdf = await SalesConfirmationItemsRepository.FindAllVDF(vde.Vdedoc);
                foreach (VDF vdf in lstVdf)
                {
                    string eancodigo = "";
                    EAN ean = await EanCodificationRepository.FindEan(vdf.Of3produto, vdf.Of3opcao, vdf.Of3tamanho);
                    if (ean != null)
                    {
                        eancodigo = ean.Eancodigo;
                    }
                    else
                    {
                        PR0 pr0 = await ProductRepository.Find<PR0>(vdf.Of3produto);
                        eancodigo = pr0 == null ? string.Empty : pr0.Pr0ean;
                    }

                    VD1 order = await OrderRepository.Find(vdf.Vdlpedido);

                    if ((order.Vd1consig && (cv5tipo.Equals(lxd.Lxddocdev2) || cv5tipo.Equals(lxd.Lxddocdev4))) ||
                        (!order.Vd1consig && (cv5tipo.Equals(lxd.Lxddocdev1) || cv5tipo.Equals(lxd.Lxddocdev3))))
                    {
                        CV7 existcv7 = lstCv7.FirstOrDefault(cv7 => cv7.Cv7codigo.Equals(vdf.Of3produto) &&
                                                                    cv7.Cv7cor.Equals(vdf.Of3opcao) &&
                                                                    cv7.Cv7tamanho.Equals(vdf.Of3tamanho) &&
                                                                    cv7.Cv7vlunit == vdf.Vdlpreco);
                        if (existcv7 != null)
                        {
                            existcv7.Cv7qtde += 1;
                        }
                        else
                        {
                            lstCv7.Add(new CV7()
                            {
                                Cv7emissor = cv5.Cv5emissor,
                                Cv7tipo = cv5.Cv5tipo,
                                Cv7doc = cv5.Cv5doc,
                                Cv7codigo = vdf.Of3produto,
                                Cv7desc = vdf.Pr0desc,
                                Cv7cor = vdf.Of3opcao,
                                Cv7tamanho = vdf.Of3tamanho,
                                Cv7vlunit = vdf.Vdlpreco,
                                Cv7qtde = 1,
                                Cv7unmed = "un",
                                Cv7ean = eancodigo
                            });
                        }
                    }
                }
            }
        }
        */
        private void FitDiscountValueItem()
        {
            decimal totaldodesconto = cv5.Cv5desconv;
            totaldodesconto = lstCv7.Aggregate(totaldodesconto, (acc, cv7) => acc - cv7.Cv7descon);
            if (totaldodesconto != 0)
                lstCv7[lstCv7.Count - 1].Cv7descon += Math.Round(totaldodesconto, 2, MidpointRounding.AwayFromZero);
        }
        #endregion
        #endregion
    }
}
