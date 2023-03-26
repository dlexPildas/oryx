using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Order.Models;
using OryxDomain.Http;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Exceptions;
using OryxDomain.Models.Oryx;
using OryxDomain.Models.TecDataSoft;
using OryxDomain.Repository;
using OryxDomain.Services;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Services
{
    public class OrderService
    {
        private readonly IConfiguration Configuration;
        private readonly OrderRepository OrderRepository;
        private readonly FormatterService FormatterService;
        private readonly SimplesTIService SimplesTIService;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        private readonly ParametersRepository ParametersRepository;
        private readonly PDVParametersRepository PDVParametersRepository;
        private readonly APIParametersRepository APIParametersRepository;
        private readonly CustomerRepository CustomerRepository;
        private readonly ProductRepository ProductRepository;
        private readonly PrintingPreferencesRepository PrintingPreferencesRepository;
        private FiscalDocumentService FiscalDocumentService;
        private readonly FiscalDocumentTypeRepository FiscalDocumentTypeRepository;
        private readonly FiscalDocumentRepository FiscalDocumentRepository;
        private readonly IntegraShService IntegraShService;
        private readonly AddressRepository AddressRepository;
        private readonly LinkedStatesRepository LinkedStatesRepository;
        private readonly TitleRepository TitleRepository;
        private readonly PurchaseCreditRepository PurchaseCreditRepository;
        private readonly CashierRepository CashierRepository;
        private readonly RepresentativeRepository RepresentativeRepository;
        private readonly LogRegisterRepository LogRegisterRepository;
        private readonly VendorRepository VendorRepository;
        private readonly OrderParametersRepository OrderParametersRepository;
        private readonly SecurityRepository SecurityRepository;
        private readonly PaymentConditionRepository PaymentConditionRepository;
        private readonly PrintService PrintService;
        private readonly StockService StockService;
        private readonly OryxDomain.Services.OrderService OrderServiceDomain;
        private readonly ProductService ProductService;
        private readonly LogServices LogServices;
        private readonly EzetechService EzetechService;

        readonly ILogger Logger;

        private LXD lxd;
        private LX0 lx0;
        private LXE lxe;
        private LX2 lx2;

        #region public methods
        public OrderService(IConfiguration configuration, ILogger<OrderService> logger)
        {
            Configuration = configuration;
            OrderRepository = new OrderRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            CustomerRepository = new CustomerRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductRepository = new ProductRepository(Configuration["OryxPath"] + "oryx.ini");
            PrintingPreferencesRepository = new PrintingPreferencesRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalDocumentTypeRepository = new FiscalDocumentTypeRepository(Configuration["OryxPath"] + "oryx.ini");
            PDVParametersRepository = new PDVParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            APIParametersRepository = new APIParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalDocumentRepository = new FiscalDocumentRepository(Configuration["OryxPath"] + "oryx.ini");
            AddressRepository = new AddressRepository(Configuration["OryxPath"] + "oryx.ini");
            LinkedStatesRepository = new LinkedStatesRepository(Configuration["OryxPath"] + "oryx.ini");
            TitleRepository = new TitleRepository(Configuration["OryxPath"] + "oryx.ini");
            PurchaseCreditRepository = new PurchaseCreditRepository(Configuration["OryxPath"] + "oryx.ini");
            CashierRepository = new CashierRepository(Configuration["OryxPath"] + "oryx.ini");
            RepresentativeRepository = new RepresentativeRepository(Configuration["OryxPath"] + "oryx.ini");
            LogRegisterRepository = new LogRegisterRepository(Configuration["OryxPath"] + "oryx.ini");
            VendorRepository = new VendorRepository(Configuration["OryxPath"] + "oryx.ini");
            OrderParametersRepository = new OrderParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            SecurityRepository = new SecurityRepository(Configuration["OryxPath"] + "oryx.ini");
            PaymentConditionRepository = new PaymentConditionRepository(Configuration["OryxPath"] + "oryx.ini");
            FormatterService = new FormatterService(Configuration);
            SimplesTIService = new SimplesTIService(Configuration);
            IntegraShService = new IntegraShService(Configuration);
            DictionaryService = new DictionaryService(Configuration);
            PrintService = new PrintService(Configuration);
            StockService = new StockService(Configuration);
            OrderServiceDomain = new OryxDomain.Services.OrderService(Configuration);
            ProductService = new ProductService(Configuration);
            LogServices = new LogServices(Configuration);
            EzetechService = new EzetechService(configuration);
            Logger = logger;
        }

        public async Task<VD1> Find(string vd1pedido)
        {
            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            //validando id do pedido
            if (string.IsNullOrWhiteSpace(vd1pedido))
            {
                throw new MissingFieldException("Número do pedido não informado.");
            }
            int length = await FormatterService.FindFieldLength(nameof(vd1pedido));
            vd1pedido = Formatters.FormatField(vd1pedido, length);

            VD1 order = await OrderRepository.Find(vd1pedido);
            if (order == null)
            {
                throw new Exception(message: "Pedido não encontrado.");
            }

            order.LstVd2 = await OrderRepository.FindVd2(vd1pedido);
            order.LstVd3 = await OrderRepository.FindVd3(vd1pedido);
            order.LstVd5 = await OrderRepository.FindVd5(vd1pedido);
            order.LstVd6 = await OrderRepository.FindVd6(vd1pedido);
            foreach (VD6 vd6 in order.LstVd6)
            {
                vd6.LstVd7 = await OrderRepository.FindVd7(vd1pedido, vd6.Vd6embarq);
                
                foreach (VD7 vd7 in vd6.LstVd7)
                {
                    if (lxd.Lxdean)
                    {
                        vd7.LstVdv = await OrderRepository.FindVdV(vd7.Vd7volume);
                    }
                    else
                    {
                        vd7.LstVd8 = await OrderRepository.FindVd8(vd7.Vd7volume);
                    }
                }
            }
            return order;
        }

        public async Task<PayloadOrderModel> Save(SaveVd1Model saveModel, string authorization, OryxModuleType module, string terminal, string lx9acesso, bool forUpdate = false)
        {
            PayloadOrderModel payloadOrderModel = new PayloadOrderModel()
            {
                CodAuthSalesMall = saveModel.Vd1.CodAuthSalesMall,
                Vdkdoc = saveModel.Vdkmodel != null && saveModel.Vdkmodel.Vdk != null ? saveModel.Vdkmodel.Vdk.Vdkdoc : string.Empty,
                ShipSequence = saveModel.ShipSequence,
                RelatsPath = new List<string>()
            };
            decimal orderAmount = 0;
            CV3 cv3 = null;
            lx0 = await ParametersRepository.GetLx0();
            lx2 = await OrderParametersRepository.Find();
            lxd = await PDVParametersRepository.Find();
            lxe = await APIParametersRepository.Find();

            await LogServices.Register(new LX8()
            {
                Lx8acao = forUpdate ? "A" : "I",
                Lx8acesso = lx9acesso,
                Lx8arquivo = "VD1",
                Lx8chave = saveModel.Vd1.Vd1pedido,
                Lx8datah = DateTime.Now,
                Lx8usuario = saveModel.Vd1.Vd1usuario,
                Lx8info = JsonConvert.SerializeObject(saveModel)
            });

            await InitialValidationsSaveOrder(terminal);

            bool isInvoiced = false;
            bool edicao = false;
            bool existsConsignedOrder = false;
            string shipSequence = string.Empty;
            IList<string> lstCV5 = null;
            CV5 cv5 = new CV5();
            IList<CV7> lstCv7 = new List<CV7>();
            IList<CV8> lstCv8 = new List<CV8>();
            IList<CVJ> lstCvj = new List<CVJ>();
            IList<CVQ> lstCvq = null;
            CVT cvt = null;
            IList<SalesItemModel> lstOnlyNF = null;

            if (lxd.Lxddocven2.Equals(saveModel.Docfis))
            {
                saveModel.Vd1.Vd1consig = true;
                saveModel.LstCv8 = new List<CV8>();
                saveModel.Vd1.Vd1vltroca = 0;
            }

            if (saveModel.Vd1.Vd1consig)
            {
                VD1 consignedOrder = await OrderRepository.FindOpenConsignedByCf1cliente(saveModel.Vd1.Vd1cliente);
                if (consignedOrder != null)
                {
                    consignedOrder.Vd1observa = saveModel.Vd1.Vd1observa;
                    consignedOrder.Vd1conpgto = saveModel.Vd1.Vd1conpgto;
                    consignedOrder.Vd1descon = saveModel.Vd1.Vd1descon;
                    consignedOrder.Vd1despon = saveModel.Vd1.Vd1despon;
                    consignedOrder.Vd1lista = saveModel.Vd1.Vd1lista;
                    consignedOrder.Vd1repres = saveModel.Vd1.Vd1repres;
                    consignedOrder.Vd1vend = saveModel.Vd1.Vd1vend;
                    saveModel.Vd1 = consignedOrder;
                    
                    existsConsignedOrder = true;

                    if (!string.IsNullOrWhiteSpace(saveModel.ShipSequence))
                    {
                        shipSequence = saveModel.ShipSequence;
                    }
                    else
                    {
                        VD6 openVd6Consig = await OrderRepository.FindLastVd6Embarq(saveModel.Vd1.Vd1pedido);
                        if (openVd6Consig == null || (openVd6Consig != null && openVd6Consig.Vd6fecha > Constants.MinDateOryx))
                            shipSequence = await OrderServiceDomain.FindNextShipSequence(saveModel.Vd1.Vd1pedido);
                        else
                            shipSequence = openVd6Consig.Vd6embarq;
                    }
                }
                else
                {
                    shipSequence = await OrderServiceDomain.FindNextShipSequence(saveModel.Vd1.Vd1pedido);
                }
                payloadOrderModel.ShipSequence = shipSequence;
            }

            orderAmount = await ValidateOrder(saveModel);

            //preparando VD1
            saveModel.Vd1.Vd1observa = Formatters.FormatField(saveModel.Vd1.Vd1observa, await FormatterService.FindFieldLength(nameof(saveModel.Vd1.Vd1observa)));
            saveModel.Vd1.Vd1pronta = true;
            saveModel.Vd1.Vd1emissor = lx0.Lx0cliente;

            if (!string.IsNullOrWhiteSpace(saveModel.Vd1.Vd1repres))
            {
                CF6 cf6 = await RepresentativeRepository.Find(saveModel.Vd1.Vd1repres);
                if (cf6 == null)
                    throw new Exception(string.Format("Representante {0} não encontrado", saveModel.Vd1.Vd1repres));
                saveModel.Vd1.Vd1comis = cf6.Cf6comis;
            }

            if (!string.IsNullOrWhiteSpace(saveModel.Vd1.Vd1vend))
            {
                VE0 ve0 = await VendorRepository.Find(saveModel.Vd1.Vd1vend);
                if (ve0 == null)
                    throw new Exception(string.Format("Vendedor {0} não encontrado", saveModel.Vd1.Vd1vend));
                saveModel.Vd1.Vd1comven = ve0.Ve0comis;
            }

            lstCv8 = saveModel.LstCv8;
            if (!saveModel.Vd1.Vd1consig)
                lstOnlyNF = PopulateLstOnlyNF(saveModel.LstVd7);

            //encontrando vd1opercom
            saveModel.Vd1.Vd1opercom = "0";
            if (!string.IsNullOrWhiteSpace(saveModel.Docfis))
            {
                cv3 = await FindBusinessOperation(saveModel.Vd1.Vd1cliente, saveModel.Vd1.Vd1consig, saveModel.Docfis, authorization);
                saveModel.Vd1.Vd1opercom = cv3.Cv3opercom;
            }

            if (!saveModel.Vd1.Vd1consig)
            {
                //validar se existem notas emitidas vinculada aos embarques do pedido
                lstCV5 = await OrderRepository.FindInvoincedOrder(saveModel.Vd1.Vd1pedido, saveModel.Docfis);

                if (lstCV5 == null || lstCV5.Count == 0)
                {
                    //aplicar a ideia de excluir o pedido quando atualizar ou quando der erro ao inserir, emitir nota e tudo mais
                    if (!forUpdate)
                        await DeleteOrderForInsert(saveModel.Vd1);
                }
                else
                {
                    isInvoiced = true;
                    payloadOrderModel.Billed = true;
                }
            }

            if (saveModel.Vd1.Vd1consig)
            {
                //validar se existem notas emitidas vinculada aos embarques do pedido
                CV5 cv5DocShip = await FiscalDocumentRepository.FindByShip(saveModel.Vd1.Vd1pedido, shipSequence, saveModel.Docfis);
                if (cv5DocShip != null)
                    lstCV5 = new List<string>() { cv5DocShip.Cv5doc };
                if (lstCV5 == null || lstCV5.Count == 0)
                {
                    //aplicar a ideia de excluir o pedido quando atualizar ou quando der erro ao inserir, emitir nota e tudo mais
                    if (saveModel.Vd1.Vd1consig && existsConsignedOrder)
                        await DeleteOrderForSaveConsigned(saveModel.Vd1.Vd1pedido, shipSequence);
                    else
                        await DeleteOrderForInsert(saveModel.Vd1);
                }
                else
                {
                    isInvoiced = true;
                    payloadOrderModel.Billed = true;
                }
            }

            if (!isInvoiced)
                await ValidateCustomerCredit(saveModel, authorization);

            if (!isInvoiced)
            {
                //revalidando peças disponíveis
                if (!lxd.Lxdean && !lx2.Lx2debest)
                {
                    await ValidatePiecesInStock(saveModel);
                }

                if ((forUpdate && (!saveModel.Vd1.Vd1consig || (saveModel.Vd1.Vd1consig && existsConsignedOrder))) || (!forUpdate && saveModel.Vd1.Vd1consig && existsConsignedOrder))
                {
                    //em caso de consignado: atualizar vd1 e inserir apenas vd6 vd7, vd8 vdv e adicionar saldo ou novos items do vd2/vd3/vd5
                    await PrepareAndUpdateOrder(saveModel, shipSequence, authorization, forUpdate);
                }
                else
                {
                    //caso consignado, existindo ou não o embarque ele vai ser excluído acima (pois não estará faturado) e pode ser inserido sempre
                    saveModel.Vd1.Vd1abert = DateTime.Now;
                    await PrepareAndSaveOrder(saveModel, authorization);
                }

                // remover as peças do pedido de estoque de transferência da filial
                if (lx2.Lx2debest && !lxd.Lxdean)
                {
                    await StockService.RemovePiecesFromTransferOrder(saveModel.LstVd7);
                }
            }
            else
            {
                //recuperar a nota gerada - todas as tabelas vinculadas conforme o tipo de documento e operação
                new FiscalDocumentService(Configuration, Logger).Recover(FatherFieldType.CV5PEDIDO, saveModel.Vd1.Vd1pedido, saveModel.Docfis, ref cv5, ref lstCv7, ref lstCv8, ref lstCvj, ref lstCvq, ref cvt);

                if (!cv5.Cv5opercom.Equals(cv3.Cv3opercom))
                    saveModel.Vd1.Vd1abert = DateTime.Now;
                
                await OrderRepository.Update(saveModel.Vd1);
                //reatualizado as parcelas com a informação da tela, pois é o único que não é recalculado
                lstCv8 = saveModel.LstCv8;
                edicao = true;
            }

            //validação de estoque
            if (lxd.Lxdestbloq)
            {
                await ValidateStock(saveModel);
            }

            if (string.IsNullOrWhiteSpace(saveModel.Docfis))
            {
                if (lxd.Lxdetiqvol && (lxd.Lxdtipetiq == VolumeLabelType.PRESALES || lxd.Lxdtipetiq == VolumeLabelType.BOTH) && saveModel.QtyVolumeLabel > 0)
                {
                    string relatPathVolumeLabel =  await PrintVolumeLabel(saveModel.Vd1.Vd1pedido, saveModel.QtyVolumeLabel, terminal, authorization, saveModel.Print);
                    if (!string.IsNullOrWhiteSpace(relatPathVolumeLabel))
                        payloadOrderModel.RelatsPath.Add(relatPathVolumeLabel);
                }

                PrintReportItensByOrder printReportItensByOrder = new PrintReportItensByOrder()
                {
                    FromVd1abert = saveModel.Vd1.Vd1abert.AddHours(-1),
                    ToVd1abert = saveModel.Vd1.Vd1abert.AddHours(1),
                    FromVd1emissor = lx0.Lx0cliente,
                    ToVd1emissor = lx0.Lx0cliente,
                    FromVd1pedido = saveModel.Vd1.Vd1pedido,
                    ToVd1pedido = saveModel.Vd1.Vd1pedido
                };
                payloadOrderModel.RelatsPath.Add(await PrintReportItensByOrder(printReportItensByOrder, terminal, saveModel.Print, authorization));

                payloadOrderModel.Message = "Pedido salvo com sucesso";
                return payloadOrderModel;
            }

            //save return
            if (lxd.Lxddevven &&
                !forUpdate &&
                saveModel.Vdkmodel != null &&
                string.IsNullOrWhiteSpace(saveModel.Vdkmodel.Vdk.Vdkdoc) &&
                saveModel.Vdkmodel.LstItems.Count > 0)
            {
                payloadOrderModel = await SaveReturnInSale(saveModel, payloadOrderModel, module, authorization);
                if (payloadOrderModel.IsError)
                    return payloadOrderModel;
            }

            if (lxd.Lxdetiqvol && (lxd.Lxdtipetiq == VolumeLabelType.SALES || lxd.Lxdtipetiq == VolumeLabelType.BOTH) && saveModel.QtyVolumeLabel > 0)
            {
                string relatPathVolumeLabel = await PrintVolumeLabel(saveModel.Vd1.Vd1pedido, saveModel.QtyVolumeLabel, terminal, authorization, saveModel.Print);
                if (!string.IsNullOrWhiteSpace(relatPathVolumeLabel))
                    payloadOrderModel.RelatsPath.Add(relatPathVolumeLabel);
            }

            try
            {
                if (lxd.Lxdintesho &&
                    saveModel.Transmit &&
                    !saveModel.Vd1.Vd1consig &&
                    !saveModel.Docfis.Equals(lxd.Lxddocven2) &&
                    !saveModel.Docfis.Equals(lxd.Lxddocven5) &&
                    string.IsNullOrWhiteSpace(saveModel.Vd1.CodAuthSalesMall) &&
                    Math.Round(orderAmount, 2) > Math.Round(saveModel.Vd1.Vd1vltroca, 2) &&
                    lstCv8.Where(cv8 => !cv8.Cv8tipotit.Equals(lxd.Lxdtitulod)).Any())
                {
                    saveModel.Vd1.CodAuthSalesMall = await SendSalesToMall(saveModel, terminal);
                    payloadOrderModel.CodAuthSalesMall = saveModel.Vd1.CodAuthSalesMall;
                }

                bool romanef = lxd.Lxdromanf && !saveModel.Docfis.Equals(lxd.Lxddocven1) && !saveModel.Vd1.Vd1consig;
                bool consignf = lxd.Lxdconsnf && saveModel.Vd1.Vd1consig && !saveModel.Docfis.Equals(lxd.Lxddocven5);
                //emitir romaneio
                if (!string.IsNullOrWhiteSpace(saveModel.Docfis) && (romanef || consignf))
                {
                    string bkpdocfis = saveModel.Docfis;
                    string bkpopercom = saveModel.Vd1.Vd1opercom;
                    try
                    {
                        string newDocFis = string.Empty;
                        if (romanef)
                            newDocFis = lxd.Lxddocven1;
                        if (consignf)
                            newDocFis = lxd.Lxddocven5;
                        await EmitMandatoryFiscalDocument(saveModel, authorization, terminal, module, newDocFis, forUpdate, lstOnlyNF, payloadOrderModel);
                        saveModel.Docfis = bkpdocfis;
                        saveModel.Vd1.Vd1opercom = bkpopercom;
                        payloadOrderModel.Billed = true;
                    }
                    catch (Exception ex)
                    {
                        string docname = string.Empty;
                        if (romanef)
                            docname = "Romaneio";
                        if (consignf)
                            docname = "Nota Fiscal";

                        string details = "";
                        string message = "";
                        if (ex.InnerException != null)
                        {
                            details = string.Format("<br/>Erro técnico: {0}", ex.InnerException.Message);
                        }
                        message = string.Format("<b>Erro ao gerar {0}:</b><br/>{1}{2}", docname, ex.Message, details);
                        Logger.LogError(message + "\nStack trace: " + ex.StackTrace);
                        throw new Exception(message);
                    }
                }

                bool hasReturn = lxd.Lxddevven &&
                                 !forUpdate &&
                                 saveModel.Vdkmodel != null;

                string vdkdoc = string.Empty;
                if (hasReturn)
                    vdkdoc = saveModel.Vdkmodel.Vdk.Vdkdoc;
                if (string.IsNullOrWhiteSpace(vdkdoc) && lstCv8.Where(cv8 => cv8.Cv8tipotit == lxd.Lxdtitulod).Any())
                {
                    FNL lastFnl = await PurchaseCreditRepository.FindLastByCustomer(saveModel.Vd1.Vd1cliente);
                    if (lastFnl != null)
                    {
                        CV5 returnCv5 = await FiscalDocumentRepository.Find(lastFnl.Fnldoc, lastFnl.Fnltipo, lastFnl.Fnlemissor);
                        if(returnCv5 != null &&
                          (lxd.Lxddocdev1.Equals(returnCv5.Cv5tipo) ||
                           lxd.Lxddocdev2.Equals(returnCv5.Cv5tipo) ||
                           lxd.Lxddocdev3.Equals(returnCv5.Cv5tipo) ||
                           lxd.Lxddocdev4.Equals(returnCv5.Cv5tipo)))
                            vdkdoc = returnCv5.Cv5docdev;
                    }
                }

                if (!cv5.Cv5emitido && (string.IsNullOrWhiteSpace(cv5.Cv5situa) || (!cv5.Cv5situa.Equals("C") && !cv5.Cv5situa.Equals("I") && !cv5.Cv5situa.Equals("D"))))
                {
                    try
                    {
                        await SaveAndEmitFiscalDocument(
                              saveModel.Vd1.Vd1pedido
                            , saveModel.Vd1.Vdedoc
                            , saveModel.Vd1.Vd1consig
                            , authorization
                            , module
                            , cv5
                            , lstCv7
                            , lstCv8
                            , lstCvj
                            , saveModel.Docfis
                            , vdkdoc
                            , lstOnlyNF
                            , terminal
                            , edicao: edicao || isInvoiced
                            , vltroca: saveModel.Vd1.Vd1vltroca
                            , codAuthSalesMall: saveModel.Vd1.CodAuthSalesMall
                            , shippingInfoOfOrderModel: lxd.Lxdabaoutr ? saveModel.Vd1.ShippingInfoOfOrderModel : null
                            , otherExpenses: lxd.Lxdabaoutr ? saveModel.Vd1.OtherExpenses : null);
                        payloadOrderModel.Billed = true;

                        //salva autorização do shopping - tentar salvar mesmo que ocorra erros na emissão da nota fiscal
                        if (lxd.Lxdintesho &&
                            saveModel.Transmit &&
                            !saveModel.Docfis.Equals(lxd.Lxddocven2) &&
                            !saveModel.Docfis.Equals(lxd.Lxddocven5) &&
                            Math.Round(orderAmount, 2) > Math.Round(saveModel.Vd1.Vd1vltroca, 2)
                        )
                        {
                            await FiscalDocumentService.SaveSalesMallIntegration(saveModel.Vd1.CodAuthSalesMall);
                        }

                        string relatPath = await PrintFiscalDocument(saveModel.Print, authorization, terminal, cv5, vdkdoc);
                        if (!string.IsNullOrWhiteSpace(relatPath))
                            payloadOrderModel.RelatsPath.Add(relatPath);

                        //imprimindo relatório extra
                        if ((saveModel.Docfis.Equals(lxd.Lxddocven3) ||
                            saveModel.Docfis.Equals(lxd.Lxddocven4)) &&
                            lxd.Lxdprinrel &&
                            !lxd.Lxdromanf)
                        {
                            string relatExtraPath = "";
                            if (lxd.Lxdrelat.Equals("324"))
                            {
                                PrintReportItensByOrder printReportItensByOrder = new PrintReportItensByOrder()
                                {
                                    FromVd1abert = saveModel.Vd1.Vd1abert.AddHours(-1),
                                    ToVd1abert = saveModel.Vd1.Vd1abert.AddHours(1),
                                    FromVd1emissor = lx0.Lx0cliente,
                                    ToVd1emissor = lx0.Lx0cliente,
                                    FromVd1pedido = saveModel.Vd1.Vd1pedido,
                                    ToVd1pedido = saveModel.Vd1.Vd1pedido
                                };
                                relatExtraPath = await PrintReportItensByOrder(printReportItensByOrder, terminal, saveModel.Print, authorization);
                                if (!string.IsNullOrWhiteSpace(relatExtraPath))
                                    payloadOrderModel.RelatsPath.Add(relatExtraPath);
                            }
                            if (lxd.Lxdrelat.Equals("031"))
                            {
                                relatExtraPath = await PrintFiscalDocument(saveModel.Print, authorization, terminal, cv5, vdkdoc, lxd.Lxddocven1);
                                if (!string.IsNullOrWhiteSpace(relatExtraPath))
                                    payloadOrderModel.RelatsPath.Add(relatExtraPath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string details = "";
                        string message = "";
                        if (ex.InnerException != null)
                        {
                            details = string.Format("<br/>Erro técnico: {0}", ex.InnerException.Message);
                        }
                        message = string.Format("<b>Erro durante o faturamento:</b><br/>{0}{1}", ex.Message, details);
                        //salva autorização do shopping - tentar salvar mesmo que ocorra erros na emissão da nota fiscal
                        if (lxd.Lxdintesho &&
                            saveModel.Transmit &&
                            !saveModel.Docfis.Equals(lxd.Lxddocven2) &&
                            !saveModel.Docfis.Equals(lxd.Lxddocven5) &&
                            Math.Round(orderAmount, 2) > Math.Round(saveModel.Vd1.Vd1vltroca, 2)
                        )
                        {
                            await FiscalDocumentService.SaveSalesMallIntegration(saveModel.Vd1.CodAuthSalesMall);
                        }
                        Logger.LogError(message + "\nStack trace: " + ex.StackTrace);
                        
                        await LogServices.Register(new LX8()
                        {
                            Lx8acao = "I",
                            Lx8acesso = lx9acesso,
                            Lx8arquivo = "VD1",
                            Lx8chave = saveModel.Vd1.Vd1pedido,
                            Lx8datah = DateTime.Now,
                            Lx8usuario = saveModel.Vd1.Vd1usuario,
                            Lx8info = string.Format("ERRO|MESSAGE: {0} \n ERRO|STACKTRACE: {1}", ex.Message, ex.StackTrace),
                        });

                        throw new Exception(message);
                    }
                }
                else
                {
                    if (cv5.Cv5situa != "I" && cv5.Cv5situa != "D")
                    {
                        FiscalDocumentService = new FiscalDocumentService(
                              Configuration
                            , cv5
                            , lstCv7
                            , lstCv8
                            , lstCvj
                            , null
                            , null
                            , module
                            , saveModel.Docfis
                            , Logger
                            , true);
                        payloadOrderModel.Billed = true;
                        string relatPath = await PrintFiscalDocument(saveModel.Print, authorization, terminal, cv5, vdkdoc);
                        if (!string.IsNullOrWhiteSpace(relatPath))
                            payloadOrderModel.RelatsPath.Add(relatPath);
                    }
                    else
                    {
                        throw new Exception(string.Format("Nota fiscal {0}. Realize uma nova venda com outro CNPJ do cliente.", cv5.Cv5situa == "D" ? "denegada" : "inutilizada"));
                    }
                }
            }
            catch (Exception ex)
            {
                payloadOrderModel.Billed = true;
                if (!saveModel.Vd1.Vd1consig)
                    {
                    //se existem notas emitidas vinculada aos embarques do pedido, retornar faturado
                    lstCV5 = await OrderRepository.FindInvoincedOrder(saveModel.Vd1.Vd1pedido, saveModel.Docfis);
                    if(lstCV5 == null || lstCV5.Count == 0)
                        payloadOrderModel.Billed = false;
                }
                else
                {
                    //se existem notas emitidas vinculada aos embarques do pedido, retornar faturado
                    CV5 cv5DocShip = await FiscalDocumentRepository.FindByShip(saveModel.Vd1.Vd1pedido, shipSequence, saveModel.Docfis);
                    if (cv5DocShip == null)
                        payloadOrderModel.Billed = false;
                }
                payloadOrderModel.IsError = true;
                payloadOrderModel.Message = ex.Message;

                await LogServices.Register(new LX8()
                {
                    Lx8acao = "I",
                    Lx8acesso = lx9acesso,
                    Lx8arquivo = "VD1",
                    Lx8chave = saveModel.Vd1.Vd1pedido,
                    Lx8datah = DateTime.Now,
                    Lx8usuario = saveModel.Vd1.Vd1usuario,
                    Lx8info = string.Format("ERRO|MESSAGE: {0} \n ERRO|STACKTRACE: {1}", ex.Message, ex.StackTrace),
                });
            }

            if (!payloadOrderModel.IsError)
            {
                bool issued = true;
                //verificar se o documento fiscal não ficou pendente antes de retornar
                if (!saveModel.Vd1.Vd1consig)
                {
                    lstCV5 = await OrderRepository.FindInvoincedOrder(saveModel.Vd1.Vd1pedido, saveModel.Docfis);
                    foreach (string cv5doc in lstCV5)
                    {
                        CV5 issuedCv5 = await FiscalDocumentRepository.Find(cv5doc, saveModel.Docfis, lx0.Lx0cliente);
                        if (!issuedCv5.Cv5emitido)
                            issued = false;
                    }
                }
                else
                {
                    CV5 cv5DocShip = await FiscalDocumentRepository.FindByShip(saveModel.Vd1.Vd1pedido, shipSequence, saveModel.Docfis);
                    issued = cv5.Cv5emitido;
                }
                if (!issued)
                {
                    payloadOrderModel.IsError = true;
                    payloadOrderModel.Message = "Não foi possível finalizar o pedido devido o documento fiscal estar pendente. Tente novamente ou contate o suporte técnico.";
                }
                else
                {
                    payloadOrderModel.Message = "Pedido finalizado com sucesso!";
                }
            }

            return payloadOrderModel;
        }

        public async Task<VD1> FindConsignedByCf1cliente(string cf1cliente)
        {
            if (string.IsNullOrWhiteSpace(cf1cliente))
            {
                throw new Exception("Cliente não informado");
            }
            
            VD1 order = await OrderRepository.FindOpenConsignedByCf1cliente(cf1cliente);

            if (order != null && order.Vd1total > 0)
            {
                return order;
            }
            return null;
        }

        public async Task<bool> Delete(string vd1pedido, string lx9acesso, string authorization)
        {
            lx0 = await ParametersRepository.GetLx0();

            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            LX2 lx2 = await OrderParametersRepository.Find();
            if (lx2 == null)
                throw new Exception("Parâmetros comerciais de Pedidos não cadastrados");

            string lx9usuario = await SecurityRepository.FindLx9Valid(lx9acesso);

            IList<CV5> lstCv5 = await FiscalDocumentRepository.FindByOrder(vd1pedido);
            
            if (lstCv5 != null && lstCv5.Count > 0)
            {
                throw new Exception("Pedido faturado não pode ser excluído.");
            }

            //todo verificar se tem dependencias

            VD1 vd1 = await Find(vd1pedido);

            if (lx2.Lx2debest && !lxd.Lxdean)
            {
                foreach (VD6 vd6 in vd1.LstVd6)
                {
                    foreach (VD7 vd7 in vd6.LstVd7)
                    {
                        await StockService.AddPiecesToTransferOrder(authorization, lx9usuario, lstVd8: vd7.LstVd8);
                    }
                }
            }

            int affectedRows = await OrderRepository.DeleteCompleteOrder(vd1pedido);

            if (lx0.Lx0logs)
            {
                await LogRegisterRepository.Insert(new LX8()
                {
                    Lx8acao = "E",
                    Lx8acesso = lx9acesso,
                    Lx8arquivo = "VD1",
                    Lx8chave = vd1pedido,
                    Lx8datah = DateTime.Now,
                    Lx8usuario = lx9usuario,
                    Lx8info = JsonConvert.SerializeObject(vd1)
                });
            }

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<VD1>> Search(string search, int limit, int page, bool pending, DateTime? since, DateTime? until, string orderBy)
        {
            bool hasNext;
            search = string.IsNullOrEmpty(search) ? string.Empty : search;
            since = since.HasValue ? new DateTime(since.Value.Year, since.Value.Month, since.Value.Day, 0, 0, 0) : Constants.MinDateOryx;
            until = until.HasValue ? new DateTime(until.Value.Year, until.Value.Month, until.Value.Day, 23, 59, 59) : DateTime.UtcNow;
            orderBy = string.IsNullOrWhiteSpace(orderBy) ? "DESC" : orderBy;

            IList<VD1> orders;

            if (pending)
            {
                //ignorar pedidos que possuam embarque fechado - pois enmbarque fechado signififca que possuem algum documento fiscal emitido
                orders = await OrderRepository.SearchPending(search, limit, (page - 1) * limit, since.Value, until.Value, orderBy);
                IList<VD1> nextOrders = await OrderRepository.SearchPending(search, limit, page * limit, since.Value, until.Value, orderBy);
                hasNext = nextOrders != null && nextOrders.Count > 0;
            }
            else
            {
                orders = await OrderRepository.Search(search, limit, (page - 1) * limit, since.Value, until.Value, orderBy);
                IList<VD1> nextOrders = await OrderRepository.Search(search, limit, page * limit, since.Value, until.Value, orderBy);
                hasNext = nextOrders != null && nextOrders.Count > 0;
            }

            return new SearchPayloadModel<VD1>()
            {
                Items = orders,
                Limit = limit,
                HasNext = hasNext
            };
        }

        public async Task<SaveVd1Model> Recover(string vd1pedido)
        {
            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            //validando id do pedido
            if (string.IsNullOrWhiteSpace(vd1pedido))
            {
                throw new MissingFieldException("Número do pedido não informado.");
            }
            int length = await FormatterService.FindFieldLength(nameof(vd1pedido));
            vd1pedido = Formatters.FormatField(vd1pedido, length);

            VD1 order = await OrderRepository.Find(vd1pedido);
            if (order == null)
            {
                throw new Exception(message: "Pedido não encontrado.");
            }
            
            VD6 lastVd6 = await OrderRepository.FindLastVd6Embarq(order.Vd1pedido);
            await ValidateRecoverOrder(order, lastVd6);
            
            if (lastVd6 == null || (lastVd6 != null && order.Vd1consig && lastVd6.Vd6fecha > Constants.MinDateOryx))
            {
                return new SaveVd1Model()
                {
                    Vd1 = order,
                    LstVd7 = new List<Vd7CartModel>()
                    {
                        new Vd7CartModel()
                        {
                            Vd7volume = "1",
                            Items = new List<SalesItemModel>(),
                            Vd7embarq = string.Empty
                        }
                    }
                };
            }

            lastVd6.LstVd7 = await OrderRepository.FindVd7(lastVd6.Vd6pedido, lastVd6.Vd6embarq);
            foreach (VD7 vd7 in lastVd6.LstVd7)
            {
                if (lxd.Lxdean)
                {
                    vd7.LstVdv = await OrderRepository.FindVdV(vd7.Vd7volume);
                }
                else
                {
                    vd7.LstVd8 = await OrderRepository.FindVd8(vd7.Vd7volume);
                }
            }

            return new SaveVd1Model()
            {
                Vd1 = order,
                LstVd7 = await ConvertVd7ToCart(lastVd6.LstVd7, lxd.Lxdean)
            };
        }

        public async Task<IList<string>> Print(string cv5pedido, string cv5embarq, OryxModuleType module, string authorization, string terminal)
        {
            IList<string> relatsPath = new List<string>();
            IList<CV7> lstCv7 = null;
            IList<CV8> lstCv8 = null;
            IList<CVJ> lstCvj = null;
            IList<CVQ> lstCvq = null;
            CVT cvt = null;
            lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");
                        
            IList<CV5> lstCv5 = await FiscalDocumentRepository.FindLstByShip(cv5pedido, cv5embarq);
            if (lstCv5 == null || lstCv5.Count == 0)
            {
                throw new Exception(string.Format("Embarque {0} do pedido {1} não faturado.", cv5embarq, cv5pedido));
            }

            foreach (CV5 existCv5 in lstCv5)
            {
                CV5 cv5 = existCv5;

                if (!cv5.Cv5tipo.Equals(lxd.Lxddocven1) &&
                    !cv5.Cv5tipo.Equals(lxd.Lxddocven2) &&
                    !cv5.Cv5emitido)
                {
                    continue;
                }
                if ((cv5.Cv5tipo.Equals(lxd.Lxddocven1) ||
                    cv5.Cv5tipo.Equals(lxd.Lxddocven1)) &&
                    (cv5.Cv5situa.Equals("C") || cv5.Cv5situa.Equals("I") || cv5.Cv5situa.Equals("D")))
                {
                    continue;
                }
                
                new FiscalDocumentService(Configuration, Logger).Recover(FatherFieldType.CV5PEDIDO, cv5pedido, cv5.Cv5tipo, ref cv5, ref lstCv7, ref lstCv8, ref lstCvj, ref lstCvq, ref cvt, cv5embarq: cv5embarq);

                FiscalDocumentService = new FiscalDocumentService(
                              Configuration
                            , cv5
                            , lstCv7
                            , lstCv8
                            , lstCvj
                            , null
                            , null
                            , module
                            , cv5.Cv5tipo
                            , Logger
                            , true);
                string relatPath = await PrintFiscalDocument(false, authorization, terminal, cv5, cv5.Cv5docdev);
                if (!string.IsNullOrWhiteSpace(relatPath))
                    relatsPath.Add(relatPath);
            }

            return relatsPath;
        }

        public async Task<PayloadEmitNFModel> EmitFiscalDocument(EmitNFVd1Model emitNFModel, string authorization, OryxModuleType module, string terminal)
        {
            PayloadEmitNFModel payloadOrderModel = new PayloadEmitNFModel();

            bool isInvoiced = false;
            bool edicao = false;

            lx0 = await ParametersRepository.GetLx0();
            lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            lxe = await APIParametersRepository.Find();
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");
            
            string shipSequence = string.Empty;
            IList<string> lstCV5 = null;
            CV5 cv5 = new CV5();
            IList<CV7> lstCv7 = new List<CV7>();
            IList<CV8> lstCv8 = new List<CV8>();
            IList<CVJ> lstCvj = new List<CVJ>();
            IList<CVQ> lstCvq = null;
            CVT cvt = null;
            CV3 cv3 = null;
            IList<SalesItemModel> lstOnlyNF = null;
            ShippingInfoOfOrderModel shippingInfoOfOrderModel = null;
            OtherExpensesModel otherExpenses = null;
            //definir o docfis, conforme o pedido - venda ou consignado
            string docfis = emitNFModel.Vd1consig ? lxd.Lxddocven2 : lxd.Lxddocven1;

            //recuperar o pedido completo com seu embarque
            if (emitNFModel.Vd1consig)
            {
                VD1 consignedOrder = await OrderRepository.Find(emitNFModel.Vd1pedido);
                if (consignedOrder == null)
                    throw new Exception(string.Format("Não foi possível encontrar o pedido {0} consignado para o cliente {1}", emitNFModel.Vd1pedido, emitNFModel.Vd1cliente));
                
                emitNFModel.Vd1pedido = consignedOrder.Vd1pedido;
                //recuperar o último embarque emitido - selecionado em tela ou pegar o último
                if (string.IsNullOrWhiteSpace(emitNFModel.ShipSequence))
                {
                    VD6 openVd6Consig = await OrderRepository.FindLastVd6Embarq(emitNFModel.Vd1pedido);
                    if (openVd6Consig == null)
                        throw new Exception("Pedido consignado sem embarque");

                    shipSequence = openVd6Consig.Vd6embarq;
                }
                else
                {
                    shipSequence = emitNFModel.ShipSequence;
                }

                ValidateEmitFiscalDocForOrder(emitNFModel.Vd1pedido, ref shipSequence);
                await ValidateOrderItems(emitNFModel.Vd1pedido, shipSequence, lstOnlyNF);
                if (string.IsNullOrWhiteSpace(lxd.Lxddocven5))
                {
                    DC1 cLxddocven5 = await DictionaryRepository.FindDC1ByDc1campo(nameof(lxd.Lxddocven5));
                    throw new Exception(string.Format("Não foi possível emitir o documento: Parâmetro {0} não definido", cLxddocven5.Dc1nome));
                }
                emitNFModel.Docfis = lxd.Lxddocven5;
            }
            else
            {
                ValidateEmitFiscalDocForOrder(emitNFModel.Vd1pedido, ref shipSequence);

                IList<string> romaneios = await OrderRepository.FindInvoincedOrder(emitNFModel.Vd1pedido, docfis);
                
                //em caso de nota a vulsa, não pegar dados do romaneio
                if (romaneios != null && romaneios.Count > 0)
                {
                    //pegando apenas o primeiro romaneio - em caso de múltiplos romaneios vai precisar tratar diferente para a venda
                    CV5 cv5Romaneio = await FiscalDocumentRepository.Find(romaneios[0], docfis, lx0.Lx0cliente);

                    if (cv5Romaneio == null)
                        throw new Exception("Não foi possível encontrar o romaneio para o pedido de venda " + emitNFModel.Vd1pedido);

                    emitNFModel.Vdedoc = cv5Romaneio.Cv5docconf;

                    //recuperando parcelas do romaneio para recriar no documento fiscal
                    lstCv8 = await RecoverInstallments(cv5Romaneio);
                    
                    if (lxd.Lxdabaoutr)
                    {
                        shippingInfoOfOrderModel = new ShippingInfoOfOrderModel()
                        {
                            Cv5espevol = cv5Romaneio.Cv5espevol,
                            Cv5marcvol = cv5Romaneio.Cv5marcvol,
                            Cv5numevol = cv5Romaneio.Cv5numevol,
                            Cv5pesobru = cv5Romaneio.Cv5pesobru,
                            Cv5pesoliq = cv5Romaneio.Cv5pesoliq,
                            Cv5qtdevol = cv5Romaneio.Cv5qtdevol
                        };

                        otherExpenses = new OtherExpensesModel()
                        {
                            Cv5outras = cv5Romaneio.Cv5outras
                        };
                    }

                    //recuperando itens lançados a vulso na nota fiscal durante a venda
                    lstOnlyNF = await RecoverOnlyNFItems(cv5Romaneio);

                    if (string.IsNullOrWhiteSpace(emitNFModel.Docfis))
                        throw new Exception("Não foi possível emitir o documento: Parâmetro de tipo de documento fiscal não definido");
                    //emitNFModel.Docfis = lxd.Lxddocven4;
                }
                else
                {
                    IList<string> lstCv5PendingNF = await OrderRepository.FindInvoincedOrder(emitNFModel.Vd1pedido, lxd.Lxddocven4);
                    IList<string> lstCv5PendingCF = await OrderRepository.FindInvoincedOrder(emitNFModel.Vd1pedido, lxd.Lxddocven3);
                    if ((lstCv5PendingNF == null || lstCv5PendingNF.Count == 0) && (lstCv5PendingCF == null || lstCv5PendingCF.Count == 0))
                        throw new Exception(string.Format("Nenhuma nota/cupom fiscal pendente para o pedido {0}", emitNFModel.Vd1pedido));

                    if (lstCv5PendingNF.Count > 0)
                    {
                        if (string.IsNullOrWhiteSpace(lxd.Lxddocven4) || string.IsNullOrWhiteSpace(emitNFModel.Docfis))
                        {
                            DC1 cLxddocven4 = await DictionaryRepository.FindDC1ByDc1campo(nameof(lxd.Lxddocven4));
                            throw new Exception(string.Format("Não foi possível emitir o documento: Parâmetro {0} não definido", cLxddocven4.Dc1nome));
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(lxd.Lxddocven3) || string.IsNullOrWhiteSpace(emitNFModel.Docfis))
                        {
                            DC1 cLxddocven3 = await DictionaryRepository.FindDC1ByDc1campo(nameof(lxd.Lxddocven3));
                            throw new Exception(string.Format("Não foi possível emitir o documento: Parâmetro {0} não definido", cLxddocven3.Dc1nome));
                        }
                    }
                }
            }

            //encontrando vd1opercom
            emitNFModel.Vd1opercom = "0";
            if (!string.IsNullOrWhiteSpace(emitNFModel.Docfis))
            {
                cv3 = await FindBusinessOperation(emitNFModel.Vd1cliente, emitNFModel.Vd1consig, emitNFModel.Docfis, authorization);
                emitNFModel.Vd1opercom = cv3.Cv3opercom;
            }

            if (!emitNFModel.Vd1consig)
            {
                //validar se existem notas emitidas vinculada aos embarques do pedido
                lstCV5 = await OrderRepository.FindInvoincedOrder(emitNFModel.Vd1pedido, emitNFModel.Docfis);
            }
            else
            {
                //validar se existem notas emitidas vinculada aos embarques do pedido
                CV5 cv5DocShip = await FiscalDocumentRepository.FindByShip(emitNFModel.Vd1pedido, shipSequence, emitNFModel.Docfis);
                if (cv5DocShip != null)
                    lstCV5 = new List<string>() { cv5DocShip.Cv5doc };
            }
            if (lstCV5 != null && lstCV5.Count > 0)
                isInvoiced = true;

            if (isInvoiced)
            {
                //recuperar a nota gerada - todas as tabelas vinculadas conforme o tipo de documento e operação
                new FiscalDocumentService(Configuration, Logger)
                    .Recover(
                          FatherFieldType.CV5PEDIDO
                        , emitNFModel.Vd1pedido
                        , emitNFModel.Docfis
                        , ref cv5
                        , ref lstCv7
                        , ref lstCv8
                        , ref lstCvj
                        , ref lstCvq
                        , ref cvt);

                edicao = true;
                
                //recuperando itens lançados a vulso na nota fiscal durante a venda
                lstOnlyNF = await RecoverOnlyNFItems(cv5);

                //recuperar as parcelas conforme o romaneio, em caso de romaneio + nota fiscal, devido o valor de troca que na nota vira devolução
                if (!emitNFModel.Vd1consig)
                {
                    IList<string> romaneios = await OrderRepository.FindInvoincedOrder(emitNFModel.Vd1pedido, lxd.Lxddocven1);
                    if (romaneios != null && romaneios.Count > 0)
                    {
                        // pegando apenas o primeiro romaneio -em caso de múltiplos romaneios vai precisar tratar diferente para a venda
                        CV5 cv5Romaneio = await FiscalDocumentRepository.Find(romaneios[0], lxd.Lxddocven1, lx0.Lx0cliente);
                        if (cv5Romaneio != null)
                        {
                            lstCv8 = await RecoverInstallments(cv5Romaneio);
                        }
                    }
                }

                if (lxd.Lxdabaoutr)
                {
                    shippingInfoOfOrderModel = new ShippingInfoOfOrderModel()
                    {
                        Cv5espevol = cv5.Cv5espevol,
                        Cv5marcvol = cv5.Cv5marcvol,
                        Cv5numevol = cv5.Cv5numevol,
                        Cv5pesobru = cv5.Cv5pesobru,
                        Cv5pesoliq = cv5.Cv5pesoliq,
                        Cv5qtdevol = cv5.Cv5qtdevol
                    };

                    otherExpenses = new OtherExpensesModel()
                    {
                        Cv5outras = cv5.Cv5outras
                    };
                }
            }
            if (!emitNFModel.Vd1consig)
                await ValidateOrderItems(emitNFModel.Vd1pedido, shipSequence, lstOnlyNF);

            try
            {
                if (!cv5.Cv5emitido && (string.IsNullOrWhiteSpace(cv5.Cv5situa) || (!cv5.Cv5situa.Equals("C") && !cv5.Cv5situa.Equals("I") && !cv5.Cv5situa.Equals("D"))))
                {
                    try
                    {
                        await SaveAndEmitFiscalDocument(
                              emitNFModel.Vd1pedido
                            , emitNFModel.Vdedoc
                            , emitNFModel.Vd1consig
                            , authorization
                            , module
                            , cv5
                            , lstCv7
                            , lstCv8
                            , lstCvj
                            , emitNFModel.Docfis
                            , string.Empty
                            , lstOnlyNF
                            , terminal
                            , edicao: edicao || isInvoiced
                            , opercom: emitNFModel.Vd1opercom
                            , notValidateShip: true
                            , shippingInfoOfOrderModel: shippingInfoOfOrderModel
                            , otherExpenses: otherExpenses);

                        payloadOrderModel.RelatPath = await PrintFiscalDocument(emitNFModel.Print, authorization, terminal, cv5, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        string details = "";
                        if (ex.InnerException != null)
                        {
                            details = string.Format("<br/>Erro técnico: {0}", ex.InnerException.Message);
                        }
                        //throw new Exception(string.Format("<b>Erro durante o faturamento:</b><br/>{0}{1}<br/>{2}", ex.Message, details, ex.StackTrace));
                        throw new Exception(string.Format("<b>Erro durante o faturamento:</b><br/>{0}{1}", ex.Message, details));
                    }
                }
                else
                {
                    FiscalDocumentService = new FiscalDocumentService(
                          Configuration
                        , cv5
                        , lstCv7
                        , lstCv8
                        , lstCvj
                        , null
                        , null
                        , module
                        , emitNFModel.Docfis
                        , Logger
                        , true);
                    payloadOrderModel.RelatPath = await PrintFiscalDocument(emitNFModel.Print, authorization, terminal, cv5, string.Empty);
                }
            }
            catch (Exception ex)
            {
                payloadOrderModel.IsError = true;
                payloadOrderModel.Message = ex.Message;
            }

            if (!payloadOrderModel.IsError)
                payloadOrderModel.Message = "Nota Fiscal emitida com sucesso!";

            return payloadOrderModel;
        }

        public async Task<bool> HasFiscalDocument(string vd1pedido)
        {
            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            VD1 vd1 = await OrderRepository.Find(vd1pedido);

            if(vd1 == null)
                throw new Exception("Pedido " + vd1pedido + " não encontrado.");
            
            if (vd1.Vd1consig)
            {
                VD6 lastVd6 = await OrderRepository.FindLastVd6Embarq(vd1pedido);
                if (lastVd6 == null)
                    return true;
                
                CV5 cv5DocShip = await FiscalDocumentRepository.FindByShip(vd1pedido, lastVd6.Vd6embarq, lxd.Lxddocven5);
                return cv5DocShip != null;
            }

            IList<CV5> lstCv5 = await FiscalDocumentRepository.FindByOrder(vd1pedido);
            return lstCv5 != null && lstCv5.Where(cv5 => 
                (cv5.Cv5tipo.Equals(lxd.Lxddocven3) || cv5.Cv5tipo.Equals(lxd.Lxddocven4) || cv5.Cv5tipo.Equals(lxd.Lxddocven5)) && //se for o tipo de documento fiscal de NF
                ((cv5.Cv5emitido && string.IsNullOrWhiteSpace(cv5.Cv5situa)) || //e a nota esteja emitida
                 (!cv5.Cv5emitido && (cv5.Cv5situa.Equals("C") || cv5.Cv5situa.Equals("I") || cv5.Cv5situa.Equals("D"))))).Any(); //ou a nota esteja denegada, cancelada ou inutilizada
        }

        public async Task<IList<CV5>> FindAllCv5(string vd1pedido)
        {
            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");
            
            VD1 vd1 = await OrderRepository.Find(vd1pedido);
            
            if (vd1 == null)
                throw new Exception("Pedido " + vd1pedido + "não encontrado.");

            if(lxd.Lxdintesho)
                return await FiscalDocumentRepository.FindByOrderWithAuthentication(vd1pedido);
            return await FiscalDocumentRepository.FindByOrder(vd1pedido);
        }

        public async Task<CancelPayloadModel> Cancel(string vd1pedido, string reason, bool resale, string authorization, string lx9acesso)
        {
            string messageError = string.Empty;

            FiscalDocumentService = new FiscalDocumentService(Configuration, Logger);

            lx0 = await ParametersRepository.GetLx0();

            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            LX2 lx2 = await OrderParametersRepository.Find();
            if (lx2 == null)
                throw new Exception("Parâmetros comerciais de Pedidos não cadastrados");

            string lx9usuario = await SecurityRepository.FindLx9Valid(lx9acesso);

            //procurando pedido
            VD1 vd1 = await Find(vd1pedido);

            CancelPayloadModel cancelPayload = new CancelPayloadModel()
            {
                LstVd7 = new List<Vd7CartModel>(),
                Cf1cliente = vd1.Vd1cliente,
                Resale = resale
            };

            //populando items para refazer a venda
            foreach (VD6 vd6 in vd1.LstVd6)
                cancelPayload.LstVd7 = cancelPayload.LstVd7.Concat(await ConvertVd7ToCart(vd6.LstVd7, lxd.Lxdean)).ToList();

            //se for consignado agrupa todos os volumes dos embarques em um volume apenas
            if (vd1.Vd1consig)
            {
                Vd7CartModel newVolume = new Vd7CartModel();
                newVolume.Items = new List<SalesItemModel>();
                foreach (var vd7 in cancelPayload.LstVd7)
                {
                    newVolume.Items = newVolume.Items.Concat(vd7.Items).ToList();
                }
                cancelPayload.LstVd7 = new List<Vd7CartModel>() { newVolume };
            }

            //buscar todas as notas vinculadas ao pedido
            IList<CV5> lstCv5 = await FindAllCv5(vd1pedido);

            //caso seja faturado -verificar documentos fiscais do pedido
            foreach (CV5 cv5 in lstCv5)
            {
                //validar se já está cancelado, denegado ou inutilizado para não lançar exceção
                if (cv5.Cv5situa == "C" || cv5.Cv5situa == "D" || cv5.Cv5situa == "I")
                    continue;
                
                //caso documento seja interno emitido ou não, precisa cancelar
                //Caso seja um documento fiscal e estiver emitido, cancelar o documento
                if (cv5.Cv5modelo.Equals(Constants.FiscalModels.GetValueOrDefault("DOCUMENTO_INTERNO")) ||
                    cv5.Cv5emitido)
                {
                    bool payload = await FiscalDocumentService.Cancel(cv5.Cv5doc, cv5.Cv5tipo, cv5.Cv5emissor, reason, authorization);
                    if (!payload)
                        messageError += string.Format("Não foi possível cancelar o documento {0}-{1}", cv5.Cv5doc, cv5.Cv5tipo);
                }
                else
                {
                    //Caso seja documento fiscal e estiver pendente, inutilizar a nota
                    bool payload = await FiscalDocumentService.Disable(cv5.Cv5doc, cv5.Cv5tipo, cv5.Cv5emissor, reason, authorization);
                    if (!payload)
                        messageError += string.Format("Não foi possível inutilizar o documento {0}-{1}", cv5.Cv5doc, cv5.Cv5tipo);
                }
            }

            if (!string.IsNullOrWhiteSpace(messageError))
                throw new Exception(messageError);

            //Excluir itens dos volumes (VDV e VD8) - excluir também o VD7 - manter os dados em memória
            foreach (VD6 vd6 in vd1.LstVd6)
            {
                foreach (VD7 vd7 in vd6.LstVd7)
                {
                    int affectedRows = await OrderRepository.DeleteVd8ByVolume(vd7.Vd7volume);
                    affectedRows = await OrderRepository.DeleteVd8ByVolume(vd7.Vd7volume);
                    affectedRows = await OrderRepository.DeleteVd7(vd7.Vd7volume);

                    //voltar para o estoque da loja quando cancela um pedido
                    if (lx2.Lx2debest && !lxd.Lxdean)
                        await StockService.AddPiecesToTransferOrder(authorization, lx9usuario, lstVd8: vd7.LstVd8);
                }
            }

            //Voltar o saldo do cliente caso haja cv5descfat(romaneio ou NF a vulso)
            CV5 mandatoryDoc = lstCv5.FirstOrDefault(cv5 => cv5.Cv5tipo.Equals(lxd.Lxddocven1) || cv5.Cv5tipo.Equals(lxd.Lxddocven2));

            if(mandatoryDoc == null)
                mandatoryDoc = lstCv5.FirstOrDefault(cv5 => cv5.Cv5tipo.Equals(lxd.Lxddocven3) || cv5.Cv5tipo.Equals(lxd.Lxddocven4) || cv5.Cv5tipo.Equals(lxd.Lxddocven5));

            if (mandatoryDoc != null && mandatoryDoc.Cv5descfat > 0)
            {
                //não é possível descobrir quais documentos de devolução geraram o crédito para retornar para o mesmo, vai ser colocado a NF de saída mesmo
                //CV5 returnDoc = await FiscalDocumentRepository.FindByCv5docdev(mandatoryDoc.Cv5docdev);

                await PurchaseCreditRepository.Insert(new FNL()
                {
                    Fnlcliente = mandatoryDoc.Cv5cliente,
                    Fnlcredito = mandatoryDoc.Cv5descfat,
                    Fnldatacre = mandatoryDoc.Cv5emissao,
                    Fnldoc = mandatoryDoc.Cv5doc,
                    Fnlemissor = mandatoryDoc.Cv5emissor,
                    Fnltipo = mandatoryDoc.Cv5tipo
                });
            }

            //gravando log de cancelamento
            if (lx0.Lx0logs)
            {
                await LogRegisterRepository.Insert(new LX8()
                {
                    Lx8acao = "E",
                    Lx8acesso = lx9acesso,
                    Lx8arquivo = "VD1",
                    Lx8chave = vd1pedido,
                    Lx8datah = DateTime.Now,
                    Lx8usuario = lx9usuario,
                    Lx8info = JsonConvert.SerializeObject(vd1)
                });
            }

            return cancelPayload;
        }

        public async Task<string> PrintReportItensByOrder(PrintReportItensByOrder model, string terminal, bool print, string authorization)
        {
            string dc9relat = "324";
            string pd1impres = "Bullzip PDF Printer";

            if (print)
            {
                PD1 pd1 = await PrintingPreferencesRepository.Find(terminal, dc9relat);
                if (pd1 == null)
                {
                    print = false;
                    //throw new Exception(string.Format("Impressora não definida para o terminal {0} e relatório {1}", terminal, dc9relat));
                }
                else
                {
                    if (pd1.Pd1vias == 0)
                        throw new Exception(string.Format("Número de vias não definido para o terminal {0} e relatório {1}", terminal, dc9relat));
                    pd1impres = pd1.Pd1impres;
                }
            }

            string pdfName = string.Format("RELAT_{0}_{1}.pdf", dc9relat, DateTime.Now.Ticks.ToString());

            PrintModel printModel = new PrintModel()
            {
                Relat = dc9relat,
                Params = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", pd1impres, "S", model.FromVd1abert.ToString("dd/MM/yyyy HH:mm:ss"), model.ToVd1abert.ToString("dd/MM/yyyy HH:mm:ss"), model.FromVd1emissor, model.ToVd1emissor, model.FromVd1pedido, model.ToVd1pedido, pdfName)
            };

            await PrintService.Print(printModel, authorization);

            return "/Printing/Print/Document/" + pdfName;
        }

        public async Task<string> GetNextNumber()
        {
            string nextNumber = await OrderRepository.FindLastVd1pedido();

            if (string.IsNullOrWhiteSpace(nextNumber))
                nextNumber = "0";

            int lengthVd1pedido = await FormatterService.FindFieldLength("vd1pedido");
            
            int plusValue = Convert.ToInt32(string.Format("1{0}1", "0".PadLeft(lengthVd1pedido - 1, '0')));

            nextNumber = (Convert.ToInt64(nextNumber) + plusValue).ToString().Substring(1, lengthVd1pedido);

            DC1 dc1 = await DictionaryRepository.FindDC1ByDc1campo("vd1pedido");
            if (dc1 == null)
                throw new Exception("Dicionário não encontrado para o campo VD1PEDIDO");

            string dc1ultimo = dc1.Dc1ultimo;

            dc1ultimo = string.IsNullOrWhiteSpace(dc1ultimo) ? "0" : dc1ultimo;

            if (Convert.ToInt64(dc1ultimo) >= Convert.ToInt64(nextNumber))
                nextNumber =(Convert.ToInt64(dc1ultimo) + plusValue).ToString().Substring(1, lengthVd1pedido);

            await DictionaryRepository.SaveNextNumber("vd1pedido", nextNumber);

            return nextNumber;
        }

        #endregion

        #region private methods
        private async Task<string> FindPd2Opercom(SalesOperationType salesOperation, string docfis, OperationType operation, bool taxPayer, string cv4estado, string authorization)
        {
            string response = await HttpUtilities.GetAsync(
                lxe.Lxebaseurl,
                string.Format("OryxPdv/SalesOperationParameters/FindOperCom?pd2tipoope={0}&pd2tipo={1}&pd2estadua={2}&pd2contrib={3}&pd2emispro={4}&cv4estado={5}", (int)salesOperation, docfis, (int)operation, taxPayer, true, cv4estado),
                string.Empty,
                authorization);

            ReturnModel<string> returnModel = JsonConvert.DeserializeObject<ReturnModel<string>>(response);

            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }

            return returnModel.ObjectModel;
        }
        private async Task DeleteOrderForInsert(VD1 vd1)
        {
            VD1 existVd1 = await OrderRepository.Find(vd1.Vd1pedido);
            if (existVd1 == null)
                return;

            await OrderRepository.DeleteCompleteOrder(vd1.Vd1pedido);
        }

        private async Task DeleteOrderForSaveConsigned(string vd1pedido, string shipSequence)
        {
            VD6 openVd6Consig = await OrderRepository.FindLastVd6Embarq(vd1pedido);
            if (openVd6Consig == null)
                return;


            //excluir o vd6 vd7, vd8 vdv e remover saldo do vd3 - igual a devolução
            IList<ShipItem> lstShipItems;
            if (lxd.Lxdean)
                lstShipItems = await OrderRepository.FindAllShipItemsByVdv(vd1pedido, shipSequence);
            else
                lstShipItems = await OrderRepository.FindAllShipItemsByVd8(vd1pedido, shipSequence);

            await OrderRepository.DeleteCompleteOrderConsigned(vd1pedido, shipSequence, lstShipItems);
        }
        
        private async Task<decimal> ValidateOrder(SaveVd1Model saveModel)
        {
            decimal orderAmount = 0;
            decimal itemsAmount = 0;
            decimal installmentAmount = 0;
            int qtyProducts = 0;
            int qtyOnlyNF = 0;

            if (!Validators.IsCnpj(saveModel.Vd1.Vd1cliente) && !Validators.IsCpf(saveModel.Vd1.Vd1cliente))
            {
                throw new Exception("CPF/CNPJ do cliente inválido");
            }

            if (string.IsNullOrWhiteSpace(saveModel.Vd1.Vd1pedido))
            {
                throw new Exception("Número do pedido não informado");
            }

            if (saveModel.LstVd7 == null || saveModel.LstVd7.Count == 0)
            {
                throw new Exception("Nenhum volume para o pedido " + saveModel.Vd1.Vd1pedido);
            }

            foreach (Vd7CartModel vd7 in saveModel.LstVd7)
            {
                foreach (SalesItemModel product in vd7.Items)
                {
                    if (product.Total == 0)
                    {
                        throw new Exception("Item " + product.Vd2produto + " com quantidade ou valor em branco.");
                    }
                    itemsAmount += product.Total;
                    if(!product.OnlyNF || (product.OnlyNF && !saveModel.Vd1.Vd1consig))
                        qtyProducts++;

                    if (product.OnlyNF)
                        qtyOnlyNF++;
                }
            }

            if(qtyProducts == 0 && !string.IsNullOrWhiteSpace(saveModel.Docfis))
            {
                if(qtyOnlyNF > 0)
                    throw new Exception("Não é possível realizar consignação de peças avulsas.");
                throw new Exception("Nenhum item informado no carrinho.");
            }


            if (saveModel.Docfis.Equals(lxd.Lxddocven3) && saveModel.LstCv8.Count == 0)
                throw new Exception("Nenhuma parcela informada para o cupom fiscal.");

            foreach (CV8 cv8 in saveModel.LstCv8)
            {
                //bool isCard = cv8.Cv8codnfce == CodNFCE.CartaoDeCredito ||
                //              cv8.Cv8codnfce == CodNFCE.CartaoDeDebito ||
                //              cv8.Cv8codnfce == CodNFCE.ValeAlimentacao |
                //              cv8.Cv8codnfce == CodNFCE.ValeRefeicao ||
                //              cv8.Cv8codnfce == CodNFCE.ValeCombustivel;
                //if (isCard && string.IsNullOrWhiteSpace(cv8.Cv8band))
                //{
                //    throw new Exception(string.Format("Informação de bandeira para operação cartão é obrigatório na parcela {0}", cv8.Cv8parcela));
                //}
                //if (isCard && string.IsNullOrWhiteSpace(cv8.Cv8nsu))
                //{
                //    throw new Exception(string.Format("Informação de NSU para operação cartão é obrigatório na parcela {0}", cv8.Cv8parcela));
                //}
                if (cv8.Cv8valor < 0)
                    throw new Exception(string.Format("Valor da parcela {0} não pode ser negativo.", cv8.Cv8parcela));

                if (cv8.Cv8valor == 0)
                    throw new Exception(string.Format("Valor da parcela {0} não pode ser zero.", cv8.Cv8parcela));

                if (cv8.Cv8codnfce == CodNFCE.Cheque)
                {
                    if (lxd.Lxdcheque && string.IsNullOrWhiteSpace(cv8.Cv8agente))
                        throw new Exception(string.Format("Banco não informado na parcela {0}", cv8.Cv8parcela));

                    if (lxd.Lxdcheque && string.IsNullOrWhiteSpace(cv8.Cv8cmc7))
                        throw new Exception(string.Format("Número do cheque não informado na parcela {0}", cv8.Cv8parcela));

                    if (cv8.Cv8cmc7.Length > 0 && cv8.Cv8cmc7.Length < 34)
                    {
                        cv8.Cv8cmc7 = string.Format(" {0}         {1}               ", cv8.Cv8agente.PadLeft(4, '0').Substring(1,3), cv8.Cv8cmc7.Trim().PadLeft(6, '0').Substring(0, 6));
                    }
                }
                installmentAmount += cv8.Cv8valor;
            }
            orderAmount = itemsAmount + saveModel.Vd1.Vd1vlfrete - saveModel.Vd1.Vd1descon;
            if (saveModel.Vd1.OtherExpenses != null)
                orderAmount += saveModel.Vd1.OtherExpenses.Cv5outras;

            if (!saveModel.Vd1.Vd1consig)
            {
                if ((orderAmount - saveModel.Vd1.Vd1vltroca) < 0)
                {
                    throw new Exception("Pedido ainda possui saldo de troca.");
                }

                //regra temporária: Quando tiver crédito (conforme fluxo Bizagi), fazer apenas a parte de crédito = total da venda
                //if (saveModel.Vd1.Vd1vltroca > 0 && 
                //    (orderAmount - saveModel.Vd1.Vd1vltroca) > 0 &&
                //    !saveModel.Docfis.Equals(lxd.Lxddocven1) &&
                //    !saveModel.Docfis.Equals(lxd.Lxddocven2))
                //{
                //    throw new Exception("Valor da nota não pode ser maior que o saldo de troca do cliente.");
                //}
                CodNFCE codNFCE = CodNFCE.Nenhum;
                VD4 vd4 = await PaymentConditionRepository.Find(saveModel.Vd1.Vd1conpgto);
                if (vd4 == null)
                    throw new Exception(string.Format("Condição de pagamento {0} não encontrada", saveModel.Vd1.Vd1conpgto));
                
                CV2 cv2 = await TitleRepository.Find(vd4.Vd4titulo);
                if (cv2 == null)
                    throw new Exception(string.Format("Tipo de título {0} não encontrado", vd4.Vd4titulo));

                codNFCE = cv2.NfceCode;
                if (saveModel.LstCv8 != null && Math.Round(orderAmount,2) != Math.Round(installmentAmount,2) && codNFCE != CodNFCE.SemPagamento)
                {
                    throw new Exception("Valor de parcelas não fechou. Valor: R$" + installmentAmount.ToString());
                }
            }

            if (string.IsNullOrWhiteSpace(saveModel.Vd1.Vd1lista))
            {
                throw new Exception("Nenhuma lista de preço definida");
            }
            if (string.IsNullOrWhiteSpace(saveModel.Vd1.Vd1conpgto))
            {
                throw new Exception("Nenhuma condição de pagamento definida");
            }

            if (lxd.Lxdintesho && saveModel.Transmit && string.IsNullOrWhiteSpace(saveModel.Vd1.Vd1repres))
            {
                throw new Exception("Nenhum representante informado");
            }

            return orderAmount;
        }

        private async Task<IList<FNL>> FindPurchaseCredit(string vd1cliente, string authorization)
        {
            string response = await HttpUtilities.GetAsync(
                lxe.Lxebaseurl,
                string.Format("PurchaseCredit/PurchaseCredit/Find/{0}", vd1cliente),
                string.Empty,
                authorization);

            ReturnListModel<FNL> returnModel = JsonConvert.DeserializeObject<ReturnListModel<FNL>>(response);

            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }

            return returnModel.ObjectModel;
        }

        private async Task<string> SendSalesToMall(SaveVd1Model saveModel, string terminal)
        {
            string codAuthSalesMall = "";
            switch (lxd.Lxdtipinsh)
            {
                case MallIntegrationType.SimplesTI:
                    codAuthSalesMall = await SendSalesMallSimplesTI(saveModel);
                    break;
                case MallIntegrationType.IntegraSH:
                    codAuthSalesMall = await SendSalesMallIntegraSH(saveModel, terminal);
                    break;
                case MallIntegrationType.Ezetech:
                    codAuthSalesMall = await SendSalesMallEzetech(saveModel);
                    break;
            }
            return codAuthSalesMall;            
        }

        private async Task<string> SendSalesMallIntegraSH(SaveVd1Model saveModel, string terminal)
        {
            CF1 cf1 = await CustomerRepository.FindByCpfCnpj(saveModel.Vd1.Vd1cliente);
            CF2 cf2 = await AddressRepository.FindCf2ByCep(cf1.Cf1cep);
            if (cf2 == null)
                throw new Exception(string.Format("CEP {0} não localizado", cf1.Cf1cep));

            CF3 cf3 = await AddressRepository.FindCf3ByIbge(cf2.Cf2local);

            if (cf3 == null)
                throw new Exception(string.Format("Localidade {0} (IBGE) não encontrada", cf2.Cf2local));

            OryxDomain.Models.IntegraSH.Sending.SalesSendingModel salesSendingModel = new OryxDomain.Models.IntegraSH.Sending.SalesSendingModel()
            {
                Codigo = saveModel.Vd1.Vd1cliente,
                Razao = cf1.Cf1nome,
                Fantasia = cf1.Cf1fant,
                Cpf_contato = cf1.Cf1concpf,
                Ie = cf1.Cf1insest,
                Email = cf1.Cf1email,
                Uf = cf1.Cf3estado,
                Cep = cf1.Cf1cep,
                Contato = cf1.Cf1contato,
                Bairro = cf1.Cf1bairro,
                End = cf1.Cf1ender1,
                Nr_end = cf1.Cf1numero,
                Fone = cf1.Cf1fone,
                Cel = cf1.Cf1confone,
                Abertura = cf1.Cf1abert,
                Compl = cf1.Cf1compl,
                Ibge = cf2.Cf2local,
                Municipio = cf3.Cf3nome,
                CodigoAgencia = saveModel.Vd1.Vd1repres,
                Numero = saveModel.LstCv8.Count.ToString(),
                Total = saveModel.Vd1.Vd1total,
            };

            salesSendingModel.Parcelas = new List<OryxDomain.Models.IntegraSH.Sending.Parcela>();

            foreach (CV8 cv8 in saveModel.LstCv8)
            {
                if (!cv8.Cv8tipotit.Equals(lxd.Lxdtitulod))
                {
                    OryxDomain.Models.IntegraSH.Sending.Parcela parcela = new OryxDomain.Models.IntegraSH.Sending.Parcela();
                    if (cv8.Cv8codnfce == CodNFCE.Cheque)
                    {
                        if (string.IsNullOrWhiteSpace(cv8.Cv8cmc7))
                            throw new Exception(string.Format("Número do cheque não informado para a parcela {0}", cv8.Cv8parcela));
                        if (string.IsNullOrWhiteSpace(cv8.Cv8agente))
                            throw new Exception(string.Format("Número do banco não informado para a parcela {0}", cv8.Cv8parcela));
                        parcela.Banco = cv8.Cv8agente.PadLeft(4, '0').Substring(1, 3);
                        parcela.Cheque = cv8.Cv8cmc7.Substring(13, 6);
                    }
                    else
                    {
                        parcela.Banco = IntegraShService.GetIntegraBankName(cv8.Cv8codnfce);
                        if ("000".Equals(parcela.Banco))
                        {
                            parcela.Cheque = "000000";
                        }
                        else if("999".Equals(parcela.Banco))
                        {
                            parcela.Cheque = "999999";
                        }
                    }
                    parcela.Venci = cv8.Cv8vencim;
                    parcela.Valor = cv8.Cv8valor;
                    salesSendingModel.Parcelas.Add(parcela);
                }
            }

            IList<string> lstAuth = await IntegraShService.CreateSales(salesSendingModel, terminal);
            return string.Join("; ", lstAuth);
        }

        private async Task<string> SendSalesMallSimplesTI(SaveVd1Model saveModel)
        {
            Vendas sales = new Vendas()
            {
                CF1Cliente = saveModel.Vd1.Vd1cliente,
                CF6Repres = "S"+Convert.ToInt32(saveModel.Vd1.Vd1repres).ToString(),
                DataVenda = DateTime.Now,
                Parcela = new List<Parcela>()
            };

            foreach (CV8 cv8 in saveModel.LstCv8)
            {
                if (!cv8.Cv8tipotit.Equals(lxd.Lxdtitulod))
                {
                    string Cv8cmc7 = cv8.Cv8cmc7;
                    if (cv8.Cv8codnfce == CodNFCE.Cheque)
                    {
                        if (string.IsNullOrWhiteSpace(cv8.Cv8cmc7))
                            throw new Exception(string.Format("Número do cheque não informado para a parcela {0}", cv8.Cv8parcela));
                        if (string.IsNullOrWhiteSpace(cv8.Cv8agente))
                            throw new Exception(string.Format("Número do banco não informado para a parcela {0}", cv8.Cv8parcela));
                        Cv8cmc7 = string.Format("#{0}#0#0#{1}", cv8.Cv8agente.PadLeft(4, '0').Substring(1, 3), cv8.Cv8cmc7.Substring(13, 6));
                    }

                    sales.Parcela.Add(new OryxDomain.Models.TecDataSoft.Parcela()
                    {
                        Cmc7 = Cv8cmc7,
                        DataVencimento = cv8.Cv8vencim.ToString("dd/MM/yyyy"),
                        Tipo = SimplesTIService.GetSimplesTIInstallmentType(cv8.Cv8codnfce),
                        Valor = cv8.Cv8valor
                    });
                }
            }

            VendasRet ret = await SimplesTIService.SendSales(sales);
            if (ret.Venda.Situacao.Equals("RESTRICAO"))
            {
                throw new Exception(string.Format("Erro integração shopping: {0}", ret.Venda.DsSituacao));
            }
            
                
            if (ret.Venda.CodigoVenda == 0)
            {
                throw new Exception("Erro integração shopping: Não foi possível obter um código de venda");
            }

            string autenticacao = "IDs";

            foreach (ParcelaRet parcela in ret.Venda.Parcelas)
            {
                autenticacao += "{" + string.Format(".{0}.", parcela.CdMovimentacao) + "}";
            }

            return autenticacao;
        }
        private async Task<string> SendSalesMallEzetech(SaveVd1Model saveModel)
        {
            OryxDomain.Models.Ezetech.Sending.SalesSendingModel salesSendingModel = new OryxDomain.Models.Ezetech.Sending.SalesSendingModel()
            {
                Venda = new OryxDomain.Models.Ezetech.Sending.Sales()
                {
                    Agencia_cod = saveModel.Vd1.Vd1repres,
                    Cli_cnpj_cpf = Validators.IsCnpj(saveModel.Vd1.Vd1cliente) ? Formatters.FormatCNPJ(saveModel.Vd1.Vd1cliente) : Formatters.FormatCPF(saveModel.Vd1.Vd1cliente) ,
                    Qnt_parcelas = saveModel.LstCv8.Where(cv8 => !cv8.Cv8tipotit.Equals(lxd.Lxdtitulod)).ToList().Count.ToString(),
                    Valor_total = string.Format(CultureInfo.InvariantCulture, "{0}", saveModel.LstCv8.Where(cv8 => !cv8.Cv8tipotit.Equals(lxd.Lxdtitulod)).Sum(cv8 => cv8.Cv8valor)),
                },
                Pagamento = new ExpandoObject() as IDictionary<string, Object>
            };

            int installment = 1;
            foreach (CV8 cv8 in saveModel.LstCv8.Where(cv8 => !cv8.Cv8tipotit.Equals(lxd.Lxdtitulod)))
            {
                OryxDomain.Models.Ezetech.Sending.Parcela parcela = new OryxDomain.Models.Ezetech.Sending.Parcela();
                if (cv8.Cv8codnfce == CodNFCE.Cheque)
                {
                    if (string.IsNullOrWhiteSpace(cv8.Cv8cmc7))
                        throw new Exception(string.Format("Número do cheque não informado para a parcela {0}", cv8.Cv8parcela));
                    if (string.IsNullOrWhiteSpace(cv8.Cv8agente))
                        throw new Exception(string.Format("Número do banco não informado para a parcela {0}", cv8.Cv8parcela));
                    parcela.Banco = cv8.Cv8agente.PadLeft(4, '0').Substring(1, 3);
                    parcela.Cheque = cv8.Cv8cmc7.Substring(13, 6);
                }
                
                parcela.Pagamento_cod = EzetechService.GetPaymentCode(cv8.Cv8codnfce);
                
                parcela.Vencimento = cv8.Cv8vencim.ToString("dd/MM/yyyy");
                parcela.Valor = string.Format(CultureInfo.InvariantCulture, "{0}", cv8.Cv8valor);
                salesSendingModel.Pagamento.Add(string.Format("parcela{0}", installment), parcela);
                installment++;
            }

            object authorizations = await EzetechService.CreateSales(salesSendingModel);
            System.Text.Json.JsonElement element = (System.Text.Json.JsonElement)authorizations;

            IList<string> lstAuth = new List<string>();
            for (int i = 0; i < saveModel.LstCv8.Where(cv8 => !cv8.Cv8tipotit.Equals(lxd.Lxdtitulod)).Count(); i++)
            {
                lstAuth.Add(element.GetProperty(string.Format("parcela{0}", i + 1)).GetString());
            }

            return string.Join("; ", lstAuth);
        }

        private async Task<string> FindVd1opercom(string vd1cliente, bool vd1consig, string docfis, string authorization)
        {
            SalesOperationType salesOperation = vd1consig ? SalesOperationType.ConsignmentOutPut : SalesOperationType.Sales;

            CF1 cf1Issuer = await CustomerRepository.FindByCpfCnpj(lx0.Lx0cliente);
            CF1 customer = await CustomerRepository.FindByCpfCnpj(vd1cliente);

            OperationType operation = OperationType.STATE;
            if (customer !=null &&
                !cf1Issuer.Cf3estado.Equals(customer.Cf3estado))
            {
                operation = OperationType.INTERSTATE;
            }

            bool taxPayer = false;
            if (customer != null &&
                Validators.IsCnpj(customer.Cf1cliente) &&
                !string.IsNullOrWhiteSpace(customer.Cf1insest) &&
                !customer.Cf1insest.ToUpper().Equals("ISENTO")
            ){
                taxPayer = true;
            }

            return await FindPd2Opercom(salesOperation, docfis, operation, taxPayer, customer != null ? customer.Cf3estado : cf1Issuer.Cf3estado, authorization);
        }

        private async Task<CV3> FindBusinessOperation(string vd1opercom, string authorization)
        {
            string response = await HttpUtilities.GetAsync(
                lxe.Lxebaseurl,
                string.Format("Oryx/BusinessOperation/Find/{0}", vd1opercom),
                string.Empty,
                authorization);

            ReturnModel<CV3> returnModel = JsonConvert.DeserializeObject<ReturnModel<CV3>>(response);

            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }

            return returnModel.ObjectModel;
        }

        private async Task ValidateOpercom(CV3 cv3, string vd1cliente, bool isConsigned)
        {
            if (isConsigned && !cv3.Cv3consig)
                throw new Exception("Operação " + cv3.Cv3opercom + " - " + cv3.Cv3nome + " não é para consignado.");
            if (!isConsigned && cv3.Cv3consig)
                throw new Exception("Operação " + cv3.Cv3opercom + " - " + cv3.Cv3nome + " marcada como consignada.");

            if (!cv3.Cv3cic && Validators.IsCpf(vd1cliente))
            {
                throw new Exception(string.Format("Operação {0} - {1} não é para pessoa física", cv3.Cv3opercom, cv3.Cv3nome));
            }
            if (!cv3.Cv3cnpj && Validators.IsCnpj(vd1cliente))
            {
                throw new Exception(string.Format("Operação {0} - {1} não é para pessoa jurídica", cv3.Cv3opercom, cv3.Cv3nome));
            }

            if (cv3.Cv3entsai != GoodsFlowType.GOODS_EXIT)
            {
                throw new Exception(string.Format("Operação {0} - {1} não é de saída.", cv3.Cv3opercom, cv3.Cv3nome));
            }

            if (!cv3.Cv3emissao)
            {
                throw new Exception("Emissão de documento fiscal não é própria.");
            }

            if (cv3.Cv3itemfab != SalesItemType.PRODUCTS)
            {
                throw new Exception(string.Format("Operação {0} - {1} não é para produtos.", cv3.Cv3opercom, cv3.Cv3nome));
            }
            CV1 model = await FiscalDocumentTypeRepository.Find(cv3.Cv3docfis);
            if (!model.Cv1modelo.Equals(Constants.FiscalModels.GetValueOrDefault("DOCUMENTO_INTERNO")))
            {
                CF1 cf1 = await CustomerRepository.FindByCpfCnpj(vd1cliente);
                if(cf1 == null)
                    cf1 = await CustomerRepository.FindByCpfCnpj(lx0.Lx0cliente);
                CV4 cv4 = await LinkedStatesRepository.Find(cv3.Cv3opercom, cf1.Cf3estado);
                if (cv4 == null)
                {
                    throw new Exception(string.Format("UF do Cliente não é Compatível com Operação.<br/>Operação comercial: {0} - UF: {1}", cv3.Cv3opercom, cf1.Cf3estado));
                }
            }
        }

        private async Task<bool> PrepareAndSaveOrder(SaveVd1Model saveModel, string authorization)
        {
            IList<VD2> lstVd2 = new List<VD2>();
            IList<VD3> lstVd3 = new List<VD3>();
            IList<VD5> lstVd5 = new List<VD5>();
            IList<VD8> lstVd8 = new List<VD8>();
            IList<VDV> lstVdv = new List<VDV>();

            await FormatterService.ValidateFormatBasicByDC1(saveModel.Vd1);

            await OrderRepository.Insert(saveModel.Vd1);

            string shipSequence = await OrderServiceDomain.FindNextShipSequence(saveModel.Vd1.Vd1pedido);

            VD6 vd6 = new VD6()
            {
                Vd6abert = DateTime.Now,
                Vd6embarq = shipSequence,
                Vd6pedido = saveModel.Vd1.Vd1pedido,
                Vd6usuario = saveModel.Vd1.Vd1usuario,
                Vd6fecha = Constants.MinDateOryx
            };
            await FormatterService.ValidateFormatBasicByDC1(vd6);
            await OrderRepository.SaveVd6(vd6);

            saveModel.LstVd7.First().Vd7embarq = shipSequence;

            foreach (Vd7CartModel vd7CartModel in saveModel.LstVd7)
            {
                string vd7volume = await DictionaryService.GetNextNumber("VD7VOLUME", authorization, false);
                VD7 vd7 = new VD7()
                {
                    Vd7embarq = shipSequence,
                    Vd7pedido = saveModel.Vd1.Vd1pedido,
                    Vd7volume = vd7volume,
                };
                
                await OrderRepository.SaveVd7(vd7);
                await DictionaryRepository.SaveNextNumber("VD7VOLUME", vd7volume);

                foreach (SalesItemModel product in vd7CartModel.Items.Where(sli => !sli.OnlyNF))
                {
                    if (!lstVd2.Where(v => v.Vd2produto == product.Vd2produto).Any())
                    {
                        lstVd2.Add(new VD2()
                        {
                            Vd2pedido = saveModel.Vd1.Vd1pedido,
                            Vd2produto = product.Vd2produto,
                            Vd2entrega = saveModel.Vd1.Vd1entrega
                        });
                    }

                    VD3 vd3 = lstVd3.FirstOrDefault(v => v.Vd3produto == product.Vd2produto && v.Vd3opcao == product.Vd3opcao && v.Vd3tamanho == product.Vd3tamanho);
                    if (vd3 == null)
                    {
                        lstVd3.Add(new VD3()
                        {
                            Vd3pedido = saveModel.Vd1.Vd1pedido,
                            Vd3produto = product.Vd2produto,
                            Vd3opcao = product.Vd3opcao,
                            Vd3tamanho = product.Vd3tamanho,
                            Vd3qtde = product.Vd3qtde
                        });
                    }
                    else
                    {
                        vd3.Vd3qtde += product.Vd3qtde;
                    }

                    if (!lstVd5.Where(v => v.Vd5produto == product.Vd2produto && v.Vd5opcao == product.Vd3opcao && v.Vd5tamanho == product.Vd3tamanho).Any())
                    {
                        lstVd5.Add(new VD5()
                        {
                            Vd5pedido = saveModel.Vd1.Vd1pedido,
                            Vd5produto = product.Vd2produto,
                            Vd5tamanho = product.Vd3tamanho,
                            Vd5preco = product.Vd5preco,
                            Vd5opcao = product.Vd3opcao
                        });
                    }

                    if (lxd.Lxdean)
                    {
                        VDV existVdv = lstVdv.FirstOrDefault(
                                            vdv => vdv.Vdvpeca == product.Vdvpeca &&
                                                   vdv.Vdvproduto == product.Vd2produto &&
                                                   vdv.Vdvopcao == product.Vd3opcao &&
                                                   vdv.Vdvtamanho == product.Vd3tamanho &&
                                                   vdv.Vdvpreco == product.Vd5preco);

                        if (existVdv != null)
                        {
                            existVdv.Vdvqtde += product.Vd3qtde;
                            continue;
                        }
                        lstVdv.Add(new VDV()
                        {
                            Vdvleitura = product.Vd3leitura,
                            Vdvpeca = product.Vdvpeca,
                            Vdvpreco = product.Vd5preco,
                            Vdvqtde = product.Vd3qtde,
                            Vdvvolume = vd7volume,
                            Vdvproduto = product.Vd2produto,
                            Vdvopcao = product.Vd3opcao,
                            Vdvtamanho = product.Vd3tamanho
                        });
                    }
                    else
                    {
                        lstVd8.Add(new VD8()
                        {
                            Vd8leitura = product.Vd3leitura,
                            Vd8peca = product.Vd8peca,
                            Vd8preco = product.Vd5preco,
                            Vd8volume = vd7volume
                        });
                    }
                }
            }

            await SaveVdItems(lstVd2, lstVd3, lstVd5);
            
            foreach (VD8 vd8 in lstVd8)
            {
                await FormatterService.ValidateFormatBasicByDC1(vd8);
                await OrderRepository.SaveVd8(vd8);
            }
            foreach (VDV vdv in lstVdv)
            {
                await FormatterService.ValidateFormatBasicByDC1(vdv);
                await OrderRepository.SaveVdv(vdv);
            }

            return true;
        }

        private async Task<bool> PrepareAndUpdateOrder(SaveVd1Model saveModel, string _shipSequence, string authorization, bool forUpdate)
        {
            IList<VD8> lstVd8 = new List<VD8>();
            IList<VDV> lstVdv = new List<VDV>();

            await FormatterService.ValidateFormatBasicByDC1(saveModel.Vd1);

            await OrderRepository.Update(saveModel.Vd1);

            string shipSequence = saveModel.Vd1.Vd1consig ? string.Empty : saveModel.LstVd7.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v.Vd7embarq))?.Vd7embarq;
            if (string.IsNullOrWhiteSpace(shipSequence))
            {
                if (saveModel.Vd1.Vd1consig)
                    shipSequence = _shipSequence;
                else
                    shipSequence = await OrderServiceDomain.FindNextShipSequence(saveModel.Vd1.Vd1pedido);

                VD6 vd6 = new VD6()
                {
                    Vd6abert = DateTime.Now,
                    Vd6embarq = shipSequence,
                    Vd6pedido = saveModel.Vd1.Vd1pedido,
                    Vd6usuario = saveModel.Vd1.Vd1usuario,
                    Vd6fecha = Constants.MinDateOryx
                };

                await FormatterService.ValidateFormatBasicByDC1(vd6);

                await OrderRepository.SaveVd6(vd6);
            }
            
            foreach (Vd7CartModel vd7CartModel in saveModel.LstVd7)
            {
                string vd7volume = vd7CartModel.Vd7volume;
                if (saveModel.Vd1.Vd1consig && forUpdate)
                    vd7CartModel.Vd7embarq = shipSequence;
                if (string.IsNullOrWhiteSpace(vd7CartModel.Vd7embarq))
                {
                    vd7volume = await DictionaryService.GetNextNumber("VD7VOLUME", authorization, false);

                    VD7 vd7 = new VD7()
                    {
                        Vd7embarq = shipSequence,
                        Vd7pedido = saveModel.Vd1.Vd1pedido,
                        Vd7volume = vd7volume,
                    };

                    await FormatterService.ValidateFormatBasicByDC1(vd7);
                    
                    await OrderRepository.SaveVd7(vd7);
                    await DictionaryRepository.SaveNextNumber("VD7VOLUME", vd7volume);
                }
                else
                {
                    //caso o pedido salvo for emitido como consignado, atualizar o volume com o novo pedido e embarque
                    if (saveModel.Vd1.Vd1consig)
                    {
                        VD7 existsVolume = await OrderRepository.FindVd7(vd7volume);
                        if (existsVolume != null)
                            await OrderRepository.DeleteCompleteOrder(existsVolume.Vd7pedido);
                        
                        await OrderRepository.SaveVd7(new VD7()
                        {
                            Vd7embarq = shipSequence,
                            Vd7pedido = saveModel.Vd1.Vd1pedido,
                            Vd7volume = vd7volume
                        });
                    }

                    await OrderRepository.DeleteVd8ByVolume(vd7volume);
                    await OrderRepository.DeleteVdvByVolume(vd7volume);
                }

                vd7CartModel.Vd7embarq = shipSequence;

                foreach (SalesItemModel product in vd7CartModel.Items.Where(sli => !sli.OnlyNF))
                {
                    if (lxd.Lxdean)
                    {
                        VDV existVdv = lstVdv.FirstOrDefault(
                                            vdv => vdv.Vdvpeca == product.Vdvpeca &&
                                                   vdv.Vdvproduto == product.Vd2produto &&
                                                   vdv.Vdvopcao ==  product.Vd3opcao &&
                                                   vdv.Vdvtamanho == product.Vd3tamanho &&
                                                   vdv.Vdvpreco == product.Vd5preco);

                        if (existVdv != null)
                        {
                            existVdv.Vdvqtde += product.Vd3qtde;
                            continue;
                        }
                        lstVdv.Add(new VDV()
                        {
                            Vdvleitura = product.Vd3leitura,
                            Vdvpeca = product.Vdvpeca,
                            Vdvpreco = product.Vd5preco,
                            Vdvqtde = product.Vd3qtde,
                            Vdvvolume = vd7volume,
                            Vdvproduto = product.Vd2produto,
                            Vdvopcao = product.Vd3opcao,
                            Vdvtamanho = product.Vd3tamanho
                        });
                    }
                    else
                    {
                        lstVd8.Add(new VD8()
                        {
                            Vd8leitura = product.Vd3leitura,
                            Vd8peca = product.Vd8peca,
                            Vd8preco = product.Vd5preco,
                            Vd8volume = vd7volume
                        });
                    }
                }
            }

            foreach (VD8 vd8 in lstVd8)
            {
                await FormatterService.ValidateFormatBasicByDC1(vd8);
                await OrderRepository.SaveVd8(vd8);
            }
            foreach (VDV vdv in lstVdv)
            {
                await FormatterService.ValidateFormatBasicByDC1(vdv);
                await OrderRepository.SaveVdv(vdv);
            }

            await UpdateVdItemTables(saveModel.Vd1);

            return true;
        }

        private async Task UpdateVdItemTables(VD1 vd1)
        {
            IList<VD2> lstVd2 = new List<VD2>();
            IList<VD3> lstVd3 = new List<VD3>();
            IList<VD5> lstVd5 = new List<VD5>();
            IList<VD8> lstVd8 = await OrderRepository.FindVd8ByOrder(vd1.Vd1pedido);
            IList<VDV> lstVdv = await OrderRepository.FindVdvByOrder(vd1.Vd1pedido);

            foreach (VD8 vd8 in lstVd8)
            {
                ProductCartModel product = await ProductRepository.FindByOF3Table<ProductCartModel>(vd8.Vd8peca);
                PopulateVdLists(lstVd2, lstVd3, lstVd5, product, 1, vd8.Vd8preco, vd1);
            }
            
            foreach (VDV vdv in lstVdv)
            {
                ProductCartModel product = null;
                if (string.IsNullOrWhiteSpace(vdv.Vdvpeca))
                {
                    product = new ProductCartModel()
                    {
                        Pr0produto = vdv.Vdvproduto,
                        Pr2opcao = vdv.Vdvopcao,
                        Pr3tamanho = vdv.Vdvtamanho
                    };
                }
                else
                {
                    product = await ProductRepository.FindByEanTable<ProductCartModel>(vdv.Vdvpeca);
                    if (product == null)
                    {
                        product = await ProductRepository.FindByEan<ProductCartModel>(vdv.Vdvpeca);
                        product.Pr2opcao = string.Empty;
                        product.Pr3tamanho = string.Empty;
                    }
                }
                PopulateVdLists(lstVd2, lstVd3, lstVd5, product, vdv.Vdvqtde, vdv.Vdvpreco, vd1);
            }

            await OrderRepository.DeleteVd2(vd1.Vd1pedido);
            await OrderRepository.DeleteVd3(vd1.Vd1pedido);
            await OrderRepository.DeleteVd5(vd1.Vd1pedido);

            await SaveVdItems(lstVd2, lstVd3, lstVd5);
        }

        private void PopulateVdLists(
              IList<VD2> lstVd2
            , IList<VD3> lstVd3
            , IList<VD5> lstVd5
            , ProductCartModel product
            , decimal qty
            , decimal price
            , VD1 vd1)
        {
            if (!lstVd2.Where(v => v.Vd2produto == product.Pr0produto).Any())
            {
                lstVd2.Add(new VD2()
                {
                    Vd2pedido = vd1.Vd1pedido,
                    Vd2produto = product.Pr0produto,
                    Vd2entrega = vd1.Vd1entrega
                });
            }

            VD3 vd3 = lstVd3.FirstOrDefault(v => v.Vd3produto == product.Pr0produto && v.Vd3opcao == product.Pr2opcao && v.Vd3tamanho == product.Pr3tamanho);
            if (vd3 == null)
            {
                lstVd3.Add(new VD3()
                {
                    Vd3pedido = vd1.Vd1pedido,
                    Vd3produto = product.Pr0produto,
                    Vd3opcao = product.Pr2opcao,
                    Vd3tamanho = product.Pr3tamanho,
                    Vd3qtde = qty
                });
            }
            else
            {
                vd3.Vd3qtde+=qty;
            }

            if (!lstVd5.Where(v => v.Vd5produto == product.Pr0produto && v.Vd5opcao == product.Pr2opcao && v.Vd5tamanho == product.Pr3tamanho).Any())
            {
                lstVd5.Add(new VD5()
                {
                    Vd5pedido = vd1.Vd1pedido,
                    Vd5produto = product.Pr0produto,
                    Vd5tamanho = product.Pr3tamanho,
                    Vd5preco = price,
                    Vd5opcao = product.Pr2opcao
                });
            }
        }

        private async Task SaveVdItems(IList<VD2> lstVd2, IList<VD3> lstVd3, IList<VD5> lstVd5)
        {
            foreach (VD2 vd2 in lstVd2)
            {
                await FormatterService.ValidateFormatBasicByDC1(vd2);
                await OrderRepository.SaveVd2(vd2);
            }
            foreach (VD3 vd3 in lstVd3)
            {
                await FormatterService.ValidateFormatBasicByDC1(vd3);
                await OrderRepository.SaveVd3(vd3);
            }
            foreach (VD5 vd5 in lstVd5)
            {
                await FormatterService.ValidateFormatBasicByDC1(vd5);
                await OrderRepository.SaveVd5(vd5);
            }
        }

        private async Task<string> PrintFiscalDocument(bool print, string authorization, string terminal, CV5 cv5, string vdkdoc = "", string cv5tipo = "")
        {
            string fileName;
            string pd1impres = "Bullzip PDF Printer";
            PD1 pd1 = null;

            if (string.IsNullOrWhiteSpace(cv5tipo))
                cv5tipo = cv5.Cv5tipo;
            
            try
            {
                //pegar o relatório a ser impresso do modelo de documento fiscal
                CV1 cv1 = await FiscalDocumentTypeRepository.Find(cv5tipo);
                if (cv1 == null || string.IsNullOrWhiteSpace(cv1.Cv1relat))
                    throw new Exception(string.Format("Relatório para impressão não definido para o tipo de documento fiscal {0}", cv5tipo));

                if (print)
                {
                    pd1 = await PrintingPreferencesRepository.Find(terminal, cv1.Cv1relat);
                    if (pd1 == null)
                    {
                        print = false;
                        //throw new Exception(string.Format("impressora não definida para o terminal {0} e relatório {1}", terminal, cv1.Cv1relat));
                    }
                    else
                    {
                        if (pd1.Pd1vias == 0)
                            throw new Exception(string.Format("Número de vias não definido para o terminal {0} e relatório {1}", terminal, cv1.Cv1relat));
                        pd1impres = pd1.Pd1impres;
                    }

                    if (cv5.Cv5impres)
                        return "";
                }

                PrintModel printModel = new PrintModel()
                {
                    Relat = cv1.Cv1relat
                };
                
                if (cv5tipo.Equals(lxd.Lxddocven1) || cv5.Cv5tipo.Equals(lxd.Lxddocven2))
                {
                    fileName = string.Format("RELAT_{0}_{1}{2}{3}{4}.pdf", cv1.Cv1relat, cv5.Cv5emissor, cv5.Cv5tipo, cv5.Cv5doc, DateTime.Now.Ticks);
                    printModel.Params = string.Format("{0};{1};{2};{3};{4};{5};{6}", pd1impres, print ? "N" : "S", cv5.Cv5emissor, cv5.Cv5tipo, cv5.Cv5doc, string.IsNullOrWhiteSpace(vdkdoc) ? "99999" : vdkdoc, fileName);
                }
                else
                {
                    fileName = string.Format("RELAT_{0}_{1}{2}{3}.pdf", cv1.Cv1relat, cv5.Cv5tipo, cv5.Cv5doc, DateTime.Now.Ticks);
                    printModel.Params = string.Format("{0};{1};{2};{3};{4}", pd1impres, print ? "N" : "S", cv5.Cv5tipo, cv5.Cv5doc, fileName);
                }

                await FiscalDocumentService.Print(printModel, print ? pd1.Pd1vias : 1,  authorization);

                return print ? string.Empty : "/Printing/Print/Document/" + fileName;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("<b>Erro durante a impressão do documento:</b><br/>{0}", ex.Message));
            }
        }

        private async Task EmitMandatoryFiscalDocument(SaveVd1Model saveModel, string authorization, string terminal, OryxModuleType module, string newDocFis, bool forUpdate, IList<SalesItemModel> lstOnlyNF, PayloadOrderModel payloadOrderModel)
        {
            CV5 cv5 = new CV5();
            IList<CV7> lstCv7 = new List<CV7>();
            IList<CV8> lstCv8 = saveModel.LstCv8;
            IList<CVJ> lstCvj = new List<CVJ>();
            IList<CVQ> lstCvq = null;
            CVT cvt = null;
            bool isInvoiced = false;
            bool edicao = false;

            bool hasReturn = lxd.Lxddevven &&
                                 !forUpdate &&
                                 saveModel.Vdkmodel != null;

            string vdkdoc = string.Empty;
            if (hasReturn)
                vdkdoc = saveModel.Vdkmodel.Vdk.Vdkdoc;

            if (string.IsNullOrWhiteSpace(vdkdoc) && lstCv8.Where(cv8 => cv8.Cv8tipotit == lxd.Lxdtitulod).Any())
            {
                FNL lastFnl = await PurchaseCreditRepository.FindLastByCustomer(saveModel.Vd1.Vd1cliente);
                if (lastFnl != null)
                {
                    CV5 returnCv5 = await FiscalDocumentRepository.Find(lastFnl.Fnldoc, lastFnl.Fnltipo, lastFnl.Fnlemissor);
                    if(returnCv5 != null &&
                        (lxd.Lxddocdev1.Equals(returnCv5.Cv5tipo) ||
                         lxd.Lxddocdev2.Equals(returnCv5.Cv5tipo) ||
                         lxd.Lxddocdev3.Equals(returnCv5.Cv5tipo) ||
                         lxd.Lxddocdev4.Equals(returnCv5.Cv5tipo)))
                        vdkdoc = returnCv5.Cv5docdev;
                }
            }

            //usado como documento fiscal mandatório da operação de venda na emissão e geração de NF - define a baixa do embarque
            string sourceDocfis = saveModel.Docfis;
            saveModel.Docfis = newDocFis;

            saveModel.Vd1.Vd1opercom = await FindVd1opercom(saveModel.Vd1.Vd1cliente, saveModel.Vd1.Vd1consig, saveModel.Docfis, authorization);
            CV3 cv3 = await FindBusinessOperation(saveModel.Vd1.Vd1opercom, authorization);

            bool isConsigned = saveModel.Docfis.Equals(lxd.Lxddocven2) || saveModel.Docfis.Equals(lxd.Lxddocven5);

            await ValidateOpercom(cv3, saveModel.Vd1.Vd1cliente, isConsigned);

            //recuperar a nota gerada - todas as tabelas vinculadas conforme o tipo de documento e operação
            new FiscalDocumentService(Configuration, Logger).Recover(FatherFieldType.CV5PEDIDO, saveModel.Vd1.Vd1pedido, saveModel.Docfis, ref cv5, ref lstCv7, ref lstCv8, ref lstCvj, ref lstCvq, ref cvt);
            if (cv5 != null)
            {
                if (!cv5.Cv5opercom.Equals(cv3.Cv3opercom))
                    edicao = true;
                else
                    isInvoiced = true;
                //reatualizado as parcelas com a informação da tela, pois é o único que não é recalculado
                lstCv8 = saveModel.LstCv8;
            }
            if (cv5 == null)
                cv5 = new CV5();

            if (!cv5.Cv5emitido && (string.IsNullOrWhiteSpace(cv5.Cv5situa) || (!cv5.Cv5situa.Equals("C") && !cv5.Cv5situa.Equals("I") && !cv5.Cv5situa.Equals("D"))))
            {
                await SaveAndEmitFiscalDocument(
                      saveModel.Vd1.Vd1pedido
                    , saveModel.Vd1.Vdedoc
                    , saveModel.Vd1.Vd1consig
                    , authorization
                    , module
                    , cv5
                    , lstCv7
                    , lstCv8
                    , lstCvj
                    , sourceDocfis
                    , vdkdoc
                    , lstOnlyNF
                    , terminal
                    , saveModel.Vd1.Vd1opercom
                    , edicao: edicao || isInvoiced
                    , codAuthSalesMall: saveModel.Vd1.CodAuthSalesMall
                    , shippingInfoOfOrderModel: lxd.Lxdabaoutr ? saveModel.Vd1.ShippingInfoOfOrderModel : null
                    , otherExpenses: lxd.Lxdabaoutr ? saveModel.Vd1.OtherExpenses : null);

                if (lxd.Lxdintesho &&
                    saveModel.Transmit &&
                    saveModel.Docfis.Equals(lxd.Lxddocven1)
                ){
                    await FiscalDocumentService.SaveSalesMallIntegration(saveModel.Vd1.CodAuthSalesMall);
                }
            }
            else
            {
                FiscalDocumentService = new FiscalDocumentService(Configuration, Logger);
                FiscalDocumentService.cv5 = cv5;
            }

            string relatPath = await PrintFiscalDocument(
                    saveModel.Print
                , authorization
                , terminal
                , cv5
                , vdkdoc);

            if (!string.IsNullOrWhiteSpace(relatPath))
                payloadOrderModel.RelatsPath.Add(relatPath);
        }

        private async Task SaveAndEmitFiscalDocument(
              string vd1pedido
            , string vdedoc
            , bool vd1consig
            , string authorization
            , OryxModuleType module
            , CV5 cv5
            , IList<CV7> lstCv7
            , IList<CV8> lstCv8
            , IList<CVJ> lstCvj
            , string docfis
            , string vdkdoc
            , IList<SalesItemModel> lstOnlyNF
            , string terminal
            , string opercom = ""
            , bool edicao = false
            , bool notValidateShip = false
            , decimal vltroca = 0
            , string codAuthSalesMall = ""
            , ShippingInfoOfOrderModel shippingInfoOfOrderModel = null
            , OtherExpensesModel otherExpenses = null)
        {
            FiscalDocumentService = new FiscalDocumentService(
                  Configuration
                , cv5
                , lstCv7
                , lstCv8
                , lstCvj
                , null
                , null
                , module
                , docfis
                , Logger
                , true);
            await FiscalDocumentService.IncludeByCv5pedido(
                  FatherFieldType.CV5PEDIDO
                , vd1pedido
                , authorization
                , terminal
                , codAuthSalesMall
                , true
                , opercom
                , edicao
                , vdedoc
                , vdkdoc
                , notValidateShip: notValidateShip
                , lstOnlyNF
                , shippingInfoOfOrderModel
                , otherExpenses);
            await FiscalDocumentService.Emit(edicao, vd1consig, vltroca);
        }

        private async Task ValidateRecoverOrder(VD1 order, VD6 lastVd6)
        {
            if (!order.Vd1consig && lastVd6 != null)
            {
                lastVd6 = await OrderRepository.FindLastVd6Embarq(order.Vd1pedido);
                //Verificando se está aberto
                if (lastVd6.Vd6fecha > Constants.MinDateOryx)
                {
                    throw new Exception(string.Format("Embarque fechado em {0}.", lastVd6.Vd6fecha.ToString("dd/MM/yyyy")));
                }
                //verificando se há documento para o pedido e embarque
                CV5 openedCv5 = await FiscalDocumentRepository.FindFirstByShip(lastVd6.Vd6pedido, lastVd6.Vd6embarq);
                if (openedCv5 != null)
                {
                    throw new Exception(string.Format("Documento: {0} aberto para o pedido {1} e embarque {2}.", openedCv5.Cv5doc, lastVd6.Vd6pedido, lastVd6.Vd6embarq));
                }
            }
        }

        private async Task<IList<Vd7CartModel>> ConvertVd7ToCart(IList<VD7> lstVd7, bool lxdean)
        {
            IList<Vd7CartModel> listShip = new List<Vd7CartModel>();
            foreach (VD7 vd7 in lstVd7)
            {
                IList<SalesItemModel> lstItems = new List<SalesItemModel>();
                if (lxdean)
                {
                    foreach (VDV vdv in vd7.LstVdv)
                    {
                        PR0 pr0 = await ProductRepository.Find<PR0>(vdv.Vdvproduto);
                        lstItems.Add(new SalesItemModel()
                        {
                            Pr0desc = pr0.Pr0desc,
                            Pr0imagem = pr0.Pr0imagem,
                            Vd2produto = vdv.Vdvproduto,
                            Vd3leitura = vdv.Vdvleitura,
                            Vd3opcao = vdv.Vdvopcao,
                            Vd3qtde = vdv.Vdvqtde,
                            Vd3tamanho = vdv.Vdvtamanho,
                            Vd5preco = vdv.Vdvpreco,
                            Vdvpeca = vdv.Vdvpeca,
                            Cr1nome = vdv.Cr1nome,
                            Pr0pesobru = pr0.Pr0pesobru,
                            Pr0pesoliq = pr0.Pr0pesoliq,
                            PriceList = vdv.Vdvpreco,
                            Total = vdv.Vdvqtde* vdv.Vdvpreco
                        });
                    }
                }
                else
                {
                    foreach (VD8 vd8 in vd7.LstVd8)
                    {
                        PR0 pr0 = await ProductRepository.Find<PR0>(vd8.Of3produto);
                        lstItems.Add(new SalesItemModel()
                        {
                            Pr0desc = pr0.Pr0desc,
                            Pr0imagem = pr0.Pr0imagem,
                            Vd2produto = vd8.Of3produto,
                            Vd3leitura = vd8.Vd8leitura,
                            Vd3opcao = vd8.Of3opcao,
                            Vd3qtde = 1,
                            Vd3tamanho = vd8.Of3tamanho,
                            Vd5preco = vd8.Vd8preco,
                            Vd8peca = vd8.Vd8peca,
                            Cr1nome = vd8.Cr1nome,
                            Pr0pesobru = pr0.Pr0pesobru,
                            Pr0pesoliq = pr0.Pr0pesoliq,
                            PriceList = vd8.Vd8preco,
                            Total = vd8.Vd8preco
                        });
                    }
                }
                listShip.Add(new Vd7CartModel()
                {
                    Vd7volume = vd7.Vd7volume,
                    Vd7embarq = vd7.Vd7embarq,
                    Items = lstItems
                });
            }
            return listShip;
        }

        private async Task<PayloadOrderModel> SaveReturnInSale(SaveVd1Model saveModel, PayloadOrderModel payloadOrderModel, OryxModuleType module, string authorization)
        {
            saveModel.Vdkmodel.Docfis = lxd.Lxddocdev1;
            if (lxd.Lxdean && saveModel.Vd1.Vd1consig)
                saveModel.Vdkmodel.Docfis = lxd.Lxddocdev2;

            saveModel.Vdkmodel.Vdk.Vdkmotivo = lxd.Lxdmotidev;
            string jsonResponse = await HttpUtilities.CallPostAsync(
                  lxe.Lxebaseurl
                , "/Return/Return/Insert"
                , JsonConvert.SerializeObject(saveModel.Vdkmodel),
                  authorization);

            ReturnModel<PayloadReturnModel> returnModel = JsonConvert.DeserializeObject<ReturnModel<PayloadReturnModel>>(jsonResponse);

            if (returnModel.IsError)
            {
                payloadOrderModel.IsError = true;
                payloadOrderModel.Message = returnModel.MessageError + "<br/>";
            }
            if (returnModel.ObjectModel != null && returnModel.ObjectModel.IsError)
            {
                payloadOrderModel.IsError = true;
                payloadOrderModel.Message += returnModel.ObjectModel.Message;
            }
            
            payloadOrderModel.Vdkdoc = returnModel.ObjectModel != null && returnModel.ObjectModel.Vdk != null 
                                        ? returnModel.ObjectModel.Vdk.Vdkdoc
                                        : string.Empty;
            saveModel.Vdkmodel.Vdk.Vdkdoc = payloadOrderModel.Vdkdoc;
            return payloadOrderModel;
        }

        private async Task<CV3> FindBusinessOperation(string vd1cliente, bool vd1consig, string docfis, string authorization)
        {
            string vd1opercom = await FindVd1opercom(vd1cliente, vd1consig, docfis, authorization);
            CV3 cv3 = await FindBusinessOperation(vd1opercom, authorization);

            await ValidateOpercom(cv3, vd1cliente, vd1consig);
            return cv3;
        }

        private void ValidateEmitFiscalDocForOrder(string vd1pedido, ref string vd6embarq)
        {
            VD6 vd6;

            if (string.IsNullOrWhiteSpace(vd6embarq))
            {
                vd6 = OrderRepository.FindLastVd6Embarq(vd1pedido).Result;
            }
            else
            {
                vd6 = OrderRepository.FindVd6(vd1pedido, vd6embarq).Result;
            }

            if(vd6 == null)
                throw new Exception("Embarque não encontrado para o pedido " + vd1pedido);

            vd6embarq = vd6.Vd6embarq;

            if (vd6.Vd6fecha == Constants.MinDateOryx)
                throw new Exception(string.Format("Não é possível emitir o documento devido o embarque {0} estar aberto.", vd6.Vd6embarq));

        }

        private async Task ValidateOrderItems(string vd1pedido, string vd6embarq, IList<SalesItemModel> lstOnlyNF = null)
        {
            IList<ShipItem> lstShipItems;
            if (lxd.Lxdean)
                lstShipItems = await OrderRepository.FindAllShipItemsByVdv(vd1pedido, vd6embarq);
            else
                lstShipItems = await OrderRepository.FindAllShipItemsByVd8(vd1pedido, vd6embarq);

            if (lstShipItems.Count == 0 && (lstOnlyNF == null || lstOnlyNF.Count == 0))
                throw new Exception("Nenhum item informado no carrinho.");
        }

        private IList<SalesItemModel> PopulateLstOnlyNF(IList<Vd7CartModel> lstVd7)
        {
            IList<SalesItemModel> lstOnlyNF = new List<SalesItemModel>();

            foreach (Vd7CartModel vd7CartModel in lstVd7)
            {
                lstOnlyNF = lstOnlyNF.Concat(vd7CartModel.Items.Where(sli => sli.OnlyNF)).ToList();
            }

            return lstOnlyNF;
        }

        private async Task<IList<SalesItemModel>> RecoverOnlyNFItems(CV5 cv5Romaneio)
        {
            IList<SalesItemModel> lstOnlyNF = new List<SalesItemModel>();

            IList<CV7> lstCv7Romaneio = await FiscalDocumentRepository.FindAllCv7(cv5Romaneio.Cv5doc, cv5Romaneio.Cv5emissor, cv5Romaneio.Cv5tipo);

            if (lstCv7Romaneio != null && lstCv7Romaneio.Count > 0)
            {
                lstCv7Romaneio = lstCv7Romaneio.Where(c => c.Cv7flag1).ToList();
                foreach (CV7 cv7 in lstCv7Romaneio)
                {
                    lstOnlyNF.Add(new SalesItemModel()
                    {
                        Vd2produto = cv7.Cv7codigo,
                        Vd3opcao = cv7.Cv7cor,
                        Vd3tamanho = cv7.Cv7tamanho,
                        Vd3qtde = (int)cv7.Cv7qtde,
                        Vd5preco = cv7.Cv7vlunit,
                        OnlyNF = true,
                    });
                }
            }

            return lstOnlyNF;
        }

        private async Task<IList<CV8>> RecoverInstallments(CV5 cv5Romaneio)
        {
            IList<CV8> lstCv8 = await FiscalDocumentRepository.FindAllCv8(cv5Romaneio.Cv5doc, cv5Romaneio.Cv5emissor, cv5Romaneio.Cv5tipo);
            if (cv5Romaneio.Cv5descfat > 0)
            {
                if (lstCv8 == null)
                {
                    lstCv8 = new List<CV8>();
                }
                CV2 cv2Dev = await TitleRepository.Find(lxd.Lxdtitulod);
                if (cv2Dev == null)
                {
                    throw new Exception("Tipo de título para devolução/troca não encontrado");
                }
                lstCv8.Add(new CV8()
                {
                    Cv8codnfce = cv2Dev.NfceCode,
                    Cv8doc = cv5Romaneio.Cv5doc,
                    Cv8tipo = cv5Romaneio.Cv5tipo,
                    Cv8emissor = cv5Romaneio.Cv5emissor,
                    Cv8valor = cv5Romaneio.Cv5descfat,
                    Cv8parcela = (lstCv8.Count + 1).ToString(),
                    Cv8tipotit = cv2Dev.Cv2titulo,
                    Cv8vencim = cv5Romaneio.Cv5emissao
                });
            }

            if(lstCv8 != null && lstCv8.Count > 0)
            {
                int days = (int)(DateTime.Now - lstCv8.First().Cv8vencim).TotalDays;
                if (days > 0)
                {
                    foreach (CV8 cv8 in lstCv8)
                    {
                        cv8.Cv8vencim = cv8.Cv8vencim.AddDays(days);
                    }
                }
            }
            return lstCv8;
        }

        private async Task ValidateCustomerCredit(SaveVd1Model saveModel, string authorization)
        {
            IList<FNL> lstFnl = await FindPurchaseCredit(saveModel.Vd1.Vd1cliente, authorization);

            decimal vdkamount = 0;

            if (lxd.Lxddevven && saveModel.Vdkmodel != null && saveModel.Vdkmodel.Vdk != null && saveModel.Vdkmodel.LstItems.Count > 0)
            {
                vdkamount = saveModel.Vdkmodel.LstItems.Sum(rim => rim.Total);
            }

            decimal fncreditoAmount = 0;
            if (lstFnl != null)
                fncreditoAmount = lstFnl.Sum(fnl => fnl.Fnlcredito);

            decimal vd1vltroca = saveModel.LstCv8 != null && saveModel.LstCv8.Count > 0 ? saveModel.LstCv8.Where(cv8 => cv8.Cv8tipotit.Equals(lxd.Lxdtitulod)).Sum(cv8 => cv8.Cv8valor) : 0;

            if (vd1vltroca > 0 &&
                (fncreditoAmount + vdkamount) < vd1vltroca)
            {
                throw new Exception("Cliente não possui Saldo de troca suficiente.");
            }
        }

        private async Task InitialValidationsSaveOrder(string terminal)
        {
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");

            if (lx2 == null)
                throw new Exception("Parâmetros comerciais de Pedidos não cadastrados");

            if (lxd.Lxdcaixa)
            {
                if (string.IsNullOrWhiteSpace(terminal))
                    throw new Exception("Terminal não informado");

                CX0 cx0 = await CashierRepository.FindLastByTerminal(terminal);

                if (cx0 == null || (cx0 != null && cx0.Cx0fecha > Constants.MinDateOryx))
                    throw new Exception(string.Format("O caixa não está aberto para o terminal {0}. Realize a abertura do caixa.", terminal));
            }
        }
        private async Task<string> PrintVolumeLabel(string vd1pedido, decimal qtyVolumeLabel, string terminal, string authorization, bool print)
        {
            string fileName;
            string pd1impres = "Bullzip PDF Printer";
            string dc9relat = "321";
            
            try
            {
                PD1 pd1 = await PrintingPreferencesRepository.Find(terminal, dc9relat);
                if (pd1 == null)
                {
                    print = false;
                    pd1 = new PD1()
                    {
                        Pd1vias = 1
                    };
                    //throw new Exception(string.Format("impressora não definida para o terminal {0} e relatório {1}", terminal, dc9relat));
                    }
                else
                {
                    if (pd1.Pd1vias == 0)
                        throw new Exception(string.Format("Número de vias não definido para o terminal {0} e relatório {1}", terminal, dc9relat));
                    pd1impres = pd1.Pd1impres;
                }                
                PrintModel printModel = new PrintModel()
                {
                    Relat = dc9relat
                };

                fileName = string.Format("RELAT_{0}_{1}{2}.pdf", dc9relat, vd1pedido, qtyVolumeLabel);
                printModel.Params = string.Format("{0};{1};{2};{3};{4}", pd1impres, print ? "N" : "S", vd1pedido, qtyVolumeLabel, fileName);

                for (int i = 0; i < pd1.Pd1vias; i++)
                {
                    new PrintService(Configuration).Print(printModel, authorization).Wait();
                }


                return print ? string.Empty : "/Printing/Print/Document/" + fileName;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("<b>Erro durante a impressão do documento:</b><br/>{0}", ex.Message));
            }
        }

        private async Task ValidatePiecesInStock(SaveVd1Model saveModel)
        {
            foreach (Vd7CartModel vd7 in saveModel.LstVd7)
            {
                foreach (SalesItemModel product in vd7.Items)
                {
                    if (product.OnlyNF)
                        continue;
                    bool naoConforme = false;
                    bool confIndis = false;
                    string volume = vd7.Vd7volume;

                    VD7 savedVD7 = await OrderRepository.FindVd7ByPiece(product.Vd8peca, saveModel.Vd1.Vd1pedido);
                    if (savedVD7 != null)
                        volume = savedVD7.Vd7volume;

                    try
                    {
                        ProductService.ValidateOf3peca(product.Vd8peca, volume, saveModel.Vd1.Vd1pedido, vd7.Items, ref naoConforme, ref confIndis, true);
                    }
                    catch (StockValidationException ex)
                    {
                        if (savedVD7 == null)
                            throw ex;
                    }
                }
            }
        }

        //validando estoque ao finalizar o pedido
        private async Task ValidateStock(SaveVd1Model saveModel)
        {
            IList<SalesItemModel> lstItems = new List<SalesItemModel>();
            foreach (Vd7CartModel vd7 in saveModel.LstVd7)
                lstItems = lstItems.Concat(vd7.Items).ToList();

            var lstItemsGroup = from item in lstItems
                                where !item.OnlyNF
                                orderby item.Vd2produto, item.Vd3opcao, item.Cr1nome, item.Vd3tamanho
                                group item by new { item.Vd2produto, item.Vd3opcao, item.Cr1nome, item.Vd3tamanho } into salesItem
                                select new { Vd2produto = salesItem.Key.Vd2produto, Vd3opcao = salesItem.Key.Vd3opcao, Cr1nome = salesItem.Key.Cr1nome, Vd3tamanho = salesItem.Key.Vd3tamanho, Vd3qtde = salesItem.Sum(si => si.Vd3qtde) };
                        
            foreach (var item in lstItemsGroup)
            {
                //buscando estoque disponível
                decimal stock = await StockService.FindStock(item.Vd2produto, item.Vd3opcao, item.Vd3tamanho);
                //buscando estoque consumido pelo pedido em caso de o pedido já estar salvo
                decimal stockOrder = await StockService.FindStockByOrder(item.Vd2produto, item.Vd3opcao, item.Vd3tamanho, saveModel.Vd1.Vd1pedido);
                
                if ((stock - stockOrder - item.Vd3qtde) < 0)
                {
                    string message = "Produto {0} sem estoque disponível";
                    string variant = "";
                    if (!string.IsNullOrWhiteSpace(item.Vd3tamanho))
                        variant += string.Format(" TAM {0}", item.Vd3tamanho);

                    if (!string.IsNullOrWhiteSpace(item.Vd3opcao))
                        variant += string.Format(" COR {0} {1}", item.Vd3opcao, item.Cr1nome);

                    if (!string.IsNullOrWhiteSpace(variant))
                    {
                        variant = " para a variação" + variant;
                    }
                    throw new Exception(string.Format(message + variant, item.Vd2produto));
                }
            }
        }
        #endregion
    }
}
