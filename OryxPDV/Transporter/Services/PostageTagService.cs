using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MelhorEnvio;
using MelhorEnvio.Views;
using OryxDomain.Utilities;
using System.Net;
using OryxDomain.Models.MelhorEnvio;
using System.Linq;

namespace Transporter.Services
{
    public class PostageTagService
    {
        private IConfiguration Configuration;
        private readonly PostageTagRepository PostageTagRepository;
        private readonly OrderRepository OrderRepository;
        private readonly CustomerRepository CustomerRepository;
        private readonly ParametersRepository ParametersRepository;
        private readonly TransporterRepository TransporterRepository;
        private readonly ProductRepository ProductRepository;
        private readonly PostageAgencyRepository PostageAgencyRepository;
        private readonly FiscalDocumentRepository FiscalDocumentRepository;
        private readonly DictionaryService DictionaryService;
        private readonly PostageTagView PostageTagView;


        public PostageTagService(IConfiguration configuration)
        {
            Configuration = configuration;
            PostageTagRepository = new PostageTagRepository(Configuration["OryxPath"] + "oryx.ini");
            OrderRepository = new OrderRepository(Configuration["OryxPath"] + "oryx.ini");
            CustomerRepository = new CustomerRepository(Configuration["OryxPath"] + "oryx.ini");
            TransporterRepository = new TransporterRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductRepository = new ProductRepository(Configuration["OryxPath"] + "oryx.ini");
            PostageAgencyRepository = new PostageAgencyRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalDocumentRepository = new FiscalDocumentRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            PostageTagView PostageTagView = new PostageTagView(Parameters.SqlConnection);

        }

        public async Task<IList<ET3>> FindList()
        {
            IList<ET3> lstet3 = await PostageTagRepository.FindList();

            return lstet3;
        }

        public async Task<ET3> Find(string et3etiquet)
        {
            ET3 et3 = await PostageTagRepository.Find(et3etiquet);

            if (et3 == null)
            {
                throw new Exception(message: "Etiqueta de postagem não cadastrada.");
            }
            return et3;
        }

        public async Task<bool> Save(ET3 et3, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                et3.Et3etiquet = await DictionaryService.GetNextNumber(nameof(et3.Et3etiquet), authorization);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(et3);
                ET3 existslxg = await PostageTagRepository.Find(et3.Et3etiquet);
                if (existslxg != null)
                {
                    throw new Exception(message: "Etiqueta de postagem já cadastrada.");
                }

                affectedRows = await PostageTagRepository.Insert(et3);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(et3);
                affectedRows = await PostageTagRepository.Update(et3);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string et3etiquet)
        {
            int affectedRows = await PostageTagRepository.Delete(et3etiquet);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<ET3>> Search(string search, int limit, int page, IList<PostageTagStatusType> statuslst)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<ET3> postageTags = await PostageTagRepository.Search(search, limit, (page - 1) * limit, statuslst);
            IList<ET3> nextPostageTags = await PostageTagRepository.Search(search, limit, page * limit, statuslst);

            return new SearchPayloadModel<ET3>()
            {
                Items = postageTags,
                Limit = limit,
                HasNext = nextPostageTags != null && nextPostageTags.Count > 0
            };
        }

        public async Task<bool> UpdateStatus(IList<string> idlst, string authorization)
        {
            foreach (var item in idlst)
            {
                try
                {
                    var et3 = await PostageTagRepository.Find(item);

                    ET3 postageTagMelhorEnvio = await PostageTagView.UpdateStatus(et3.Et3etiquet);

                    et3.Et3status = postageTagMelhorEnvio.Et3status;
                    await Save(et3, authorization, true);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> SendToCart(IList<string> etiquetlst, string authorization)
        {
            foreach (var item in etiquetlst)
            {
                try
                {
                    ET3 et3 = await PostageTagRepository.Find(item);
                    VD1 vd1 = await OrderRepository.Find(et3.Et3pedido);
                    LX0 lx0 = await ParametersRepository.GetLx0();
                    VD2 vd2 = await ProductRepository.FindByvD1(vd1.Vd1pedido);
                    var cv5lst = await FiscalDocumentRepository.FindByOrder(vd1.Vd1pedido);
                    CV5 cv5 = cv5lst.Where(_ => _.Cv5nfechav != null).First();

                    CF1 cf1 = await CustomerRepository.FindByCpfCnpj(vd1.Vd1cliente);
                    CF1 cf1Remetente = await CustomerRepository.FindByCpfCnpj(lx0.Lx0cliente);

                    CF7 cf7 = await TransporterRepository.Find(vd1.Vd1transp);
                    LXG lxg = await PostageAgencyRepository.Find(cf7.Cf7transp);

                    AddToCartModel addToCartModel = new AddToCartModel()
                    {
                        service = int.Parse(cf7.Cf7codext),
                        agency = int.Parse(lxg.Lxgagencia),
                        to = new To()
                        {
                            address = cf1.Cf1ender1,
                            complement = cf1.Cf1compl,
                            district = cf1.Cf1bairro,
                            document = cf1.Cf1cliente.Length > 10 ? null : cf1.Cf1cliente,
                            email = cf1.Cf1email,
                            name = cf1.Cf1nome,
                            number = cf1.Cf1numero,
                            postal_code = cf1.Cf1cep,
                            phone = cf1.Cf1confone,
                            city = cf1.Cf3estado,
                            country_id = "BR",
                            company_document = cf1.Cf1cliente.Length > 10 ? cf1.Cf1cliente : null,
                            //note,
                            state_abbr = cf1.Cf3estado,
                            state_register = cf1.Cf1insest
                        },
                        from = new From()
                        {
                            address = cf1Remetente.Cf1ender1,
                            complement = cf1Remetente.Cf1compl,
                            postal_code = cf1Remetente.Cf1cep,
                            district = cf1Remetente.Cf1bairro,
                            document = cf1Remetente.Cf1cliente,
                            phone = cf1Remetente.Cf1confone,
                            number = cf1Remetente.Cf1numero,
                            name = cf1Remetente.Cf1nome,
                            email = cf1Remetente.Cf1email,
                            city = cf1Remetente.Cf3estado,
                            state_register = cf1Remetente.Cf1insest,
                            company_document = cf1.Cf1cliente.Length > 10 ? cf1.Cf1cliente : null,
                            country_id = "BR",
                            //note
                        },
                        products = new List<Product>(),
                        volumes = new List<Volume>(),
                        options = new Options()
                        {
                            invoice = new Invoice() { key = cv5.Cv5nfechav },
                            non_commercial = false,
                            own_hand = false,
                            receipt = false,
                            platform = "Oryx MelhorEnvio"
                        }
                    };

                    foreach (var pr0 in vd2.PR0)
                    {
                        addToCartModel.products.Add(new Product()
                        {
                            name = pr0.Pr0desc,
                            quantity = vd2.PR0.Where(_ => _.Pr0produto == pr0.Pr0produto).Count(),
                            unitary_value = Convert.ToDouble(pr0.Pr0precoco)
                        });
                    }

                    addToCartModel.options.insurance_value = addToCartModel.products.Sum(_ => _.unitary_value);


                    if (et3 != null && et3.Et3status != PostageTagStatusType.GENERATED)
                    {
                        ET3 postageTagMelhorEnvio = await PostageTagView.SendTocart(addToCartModel, et3);

                        et3.Et3status = postageTagMelhorEnvio.Et3status;
                        await Save(et3, authorization, true);
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;

        }

        public async Task<IList<string>> Generate(IList<string> etiquelst, string authorization)
        {
            List<string> listurls = new List<string>();
            await UpdateStatus(etiquelst, authorization);

            foreach (var item in etiquelst)
            {
                ET3 et3 = await PostageTagRepository.Find(item);
                if (et3.Et3status == PostageTagStatusType.PAYED)
                {
                    IList<string> etiqlst = new List<string>() { et3.Et3etiquet };
                    if (await PostageTagView.GenerateShipment(etiqlst))
                    {
                        et3.Et3status = PostageTagStatusType.GENERATED;
                        string urlprint = await PostageTagView.GeneratePrintUrl(etiqlst);

                        listurls.Add(urlprint);
                    }
                    await Save(et3, authorization);
                }
            }
            return listurls;
        }

        public async Task<IList<string>> Print(IList<string> etiquelst, string authorization)
        {
            List<string> listurls = new List<string>();

            foreach (var item in etiquelst)
            {
                ET3 et3 = await PostageTagRepository.Find(item);
                IList<string> etiqlst = new List<string>() { item };

                string urlprint = await PostageTagView.GeneratePrintUrl(etiqlst);
                listurls.Add(urlprint);

                et3.Et3status = PostageTagStatusType.PRINTED;
                await Save(et3, authorization);
            }

            return listurls;
        }

        public async Task<bool> Cancel(string et3etiq, string authorization)
        {
            ET3 et3 = await PostageTagRepository.Find(et3etiq);

            if(await PostageTagView.RemoveFromCart(et3etiq))
            {
                et3.Et3status = PostageTagStatusType.CANCELED;
                await Save(et3, authorization);
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
