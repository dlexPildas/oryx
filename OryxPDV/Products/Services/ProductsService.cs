using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Products.Services
{
    public class ProductsService
    {
        private readonly IConfiguration Configuration;
        private readonly ProductRepository ProductRepository;
        private readonly PDVParametersRepository PDVParametersRepository;
        private readonly EanCodificationRepository EanCodificationRepository;
        private readonly ProductionOrderParametersRepository ProductionOrderParametersRepository;
        private readonly PiecesRecordRepository PiecesRecordRepository;
        private readonly ProductSizesRepository ProductSizesRepository;
        private readonly PriceService PriceService;
        private readonly ProductService ProductService;
        private readonly FormatterService FormatterService;
        private readonly SisService SisService;
        private readonly DictionaryService DictionaryService;
        private readonly StockService StockService;
        private readonly DictionaryRepository DictionaryRepository;

        public ProductsService(IConfiguration configuration)
        {
            Configuration = configuration;
            ProductRepository = new ProductRepository(Configuration["OryxPath"] + "oryx.ini");
            PDVParametersRepository = new PDVParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            EanCodificationRepository = new EanCodificationRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductionOrderParametersRepository = new ProductionOrderParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            PiecesRecordRepository = new PiecesRecordRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductSizesRepository = new ProductSizesRepository(Configuration["OryxPath"] + "oryx.ini");
            StockService = new StockService(Configuration);
            FormatterService = new FormatterService(configuration);
            PriceService = new PriceService(Configuration);
            ProductService = new ProductService(Configuration);
            SisService = new SisService(configuration);
            DictionaryService = new DictionaryService(Configuration);
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");

        }

        public async Task<ProductCartModel> FindByCode(string code, string list, string volume, string pedido, IList<SalesItemModel> lstVd8)
        {
            code = code.Trim();

            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            LX1 lx1 = await ProductionOrderParametersRepository.Find();
            if (lx1 == null)
                throw new Exception("Parâmetros ordem de produção não cadastrados");

            bool fillZerosToLeftInProductCode = bool.Parse(Configuration["FillZerosToLeftInProductCode"]);

            ProductCartModel productCartModel;
            if (lxd.Lxdean)
            {
                string eancode = code;
                int eancodigoSize = await FormatterService.FindFieldLength("EANCODIGO");
                if ((new Regex(@"[^\d]").Replace(code, "")).Length == 13)
                {
                    string eandigit = SisService.EanDigito(code.Substring(0, eancodigoSize));
                    if (!code.Substring(12, 1).Equals(eandigit))
                    {
                        if (!fillZerosToLeftInProductCode)
                            throw new Exception("Dígito EAN 13 inválido");
                    }
                    else
                    {
                        eancode = code.Substring(0, eancodigoSize);
                    }
                }

                productCartModel = await ProductRepository.FindByEanTable<ProductCartModel>(eancode);
                if (productCartModel == null)
                {
                    productCartModel = await ProductRepository.FindByEan<ProductCartModel>(eancode);
                    if (productCartModel == null)
                    {
                        if (fillZerosToLeftInProductCode)
                        {
                            code = code.TrimStart('0');
                            int lengthPr0produto = await FormatterService.FindFieldLength("PR0PRODUTO");
                            code = code.PadLeft(lengthPr0produto, '0');
                        }

                        productCartModel = await ProductRepository.Find<ProductCartModel>(code);
                    }
                    if (productCartModel != null)
                    {
                        if (productCartModel.Eancodigo == null)
                            productCartModel.Eancodigo = string.Empty;
                        if (productCartModel.Pr2opcao == null)
                            productCartModel.Pr2opcao = string.Empty;
                        if (productCartModel.Pr3tamanho == null)
                            productCartModel.Pr3tamanho = string.Empty;
                        var colorList = await ProductRepository.FindPr2List(productCartModel.Pr0produto);
                        productCartModel.LstPr2 = new List<PR2>(colorList);

                        var sizeList = await ProductRepository.FindPr3List(productCartModel.Pr0produto);
                        productCartModel.LstPr3 = new List<PR3>(sizeList);
                    }
                }

                if (productCartModel != null && !string.IsNullOrWhiteSpace(productCartModel.Pr3tamanho))
                {
                    PR3 pr3 = await ProductSizesRepository.Find(productCartModel.Pr0produto, productCartModel.Pr3tamanho);
                    if (pr3 != null && pr3.Pr3pesoliq > 0)
                    {
                        productCartModel.Pr0pesoliq = pr3.Pr3pesoliq;
                    }
                }
            }
            else
            {
                bool naoConforme = false;
                bool confIndis = false;

                if (lx1.Lx1rfid)
                {
                    OF3 of3 = await PiecesRecordRepository.FindByRfidOrOf3peca(code);
                    if (of3 == null)
                        throw new Exception(string.Format("Tag RFID {0} não cadastrada para nenhuma peça", code));

                    code = of3.Of3peca;
                }

                bool existCodExt = await DictionaryService.ExistField("OF3CODEXT");
                if (existCodExt)
                {
                    OF3 of3 = await PiecesRecordRepository.FindByOf3codext(code);
                    if (of3 != null)
                        code = of3.Of3peca;
                }

                int lengthOf3peca = await FormatterService.FindFieldLength("of3peca");

                code = code.PadLeft(lengthOf3peca, '0');

                if (code.Length > lengthOf3peca)
                {
                    int difference = code.Length - lengthOf3peca;
                    code = code.Substring(difference, code.Length - difference);
                }

                ProductService.ValidateOf3peca(code, volume, pedido, lstVd8, ref naoConforme, ref confIndis, true);
                productCartModel = await ProductRepository.FindByOF3Table<ProductCartModel>(code);
                productCartModel.Of3naoconf = naoConforme;
                productCartModel.ConfIndis = confIndis;

                if (!string.IsNullOrWhiteSpace(productCartModel.Pr3tamanho))
                {
                    PR3 pr3 = await ProductSizesRepository.Find(productCartModel.Pr0produto, productCartModel.Pr3tamanho);
                    if (pr3 != null && pr3.Pr3pesoliq > 0)
                    {
                        productCartModel.Pr0pesoliq = pr3.Pr3pesoliq;
                    }
                }
            }

            if (productCartModel == null)
            {
                throw new Exception("Produto não encontrado");
            }

            productCartModel.Vd5preco = await PriceService.GetPrice(productCartModel.Pr0produto, list, productCartModel.Pr3tamanho, productCartModel.Pr2opcao, false);
            if (lxd.Lxdestven || lxd.Lxdestneg)
                productCartModel.Stock = await StockService.FindStock(productCartModel.Pr0produto, productCartModel.Pr2opcao, productCartModel.Pr3tamanho);

            return productCartModel;
        }

        public async Task<bool> Save(PR0 pr0, string authorization, bool forUpdate = false)
        {
            int affectedRows;

            await Validate(pr0);

            if (!forUpdate)
            {
                if (string.IsNullOrEmpty(pr0.Pr0produto))
                {
                    pr0.Pr0produto = await DictionaryService.GetNextNumber(nameof(pr0.Pr0produto), authorization);
                }

                await new FormatterService(Configuration).ValidateFormatBasicByDC1(pr0);
                PR0 existpr0 = await ProductRepository.Find<PR0>(pr0.Pr0produto);
                if (existpr0 != null)
                {
                    throw new Exception(message: "Produto já cadastrado.");
                }

                affectedRows = await ProductRepository.Insert(pr0);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(pr0.Pr0produto), pr0.Pr0produto);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(pr0);
                affectedRows = await ProductRepository.Update(pr0);
            }

            return affectedRows == 1;
        }

        private async Task Validate(PR0 pr0)
        {
            pr0.Pr0categ = pr0.Pr0categ.ToUpper();
            pr0.Pr0segment = pr0.Pr0segment.ToUpper();

            if (pr0.Pr0fci.ToString().Trim().IndexOf(' ') >= 0)
            {
                throw new Exception("Número de Fci não pode conter espaços em branco.");
            }

            if (!string.IsNullOrEmpty(pr0.Pr0refcli))
            {
                var pr0byrefcli = await ProductRepository.FindByAlterCode(pr0.Pr0produto, pr0.Pr0refcli.ToUpper());
                var dc1 = await DictionaryRepository.FindDC1ByDc1campo(nameof(pr0.Pr0refcli));

                if (pr0byrefcli != null && dc1 != null)
                {
                    throw new Exception($"{dc1.Dc1nome} pertencente ao produto {pr0byrefcli.Pr0produto}.");
                }

            }

            if (!string.IsNullOrEmpty(pr0.Pr0refer))
            {
                var pr0byref = await ProductRepository.FindByAlterCode(pr0.Pr0produto, pr0.Pr0refer.ToUpper());
                var dc1 = await DictionaryRepository.FindDC1ByDc1campo(nameof(pr0.Pr0refer));

                if (pr0 != null && dc1 != null)
                {
                    throw new Exception($"{dc1.Dc1nome} pertencente ao produto {pr0byref.Pr0produto}.");
                }
            }

            if (!string.IsNullOrEmpty(pr0.Pr0ean))
            {
                var existedean = await EanCodificationRepository.FindByPr0(pr0.Pr0produto);
                if (existedean != null)
                {
                    throw new Exception($"Este produto já possui código EAN detalhado. " +
                        $"{existedean.FirstOrDefault().Eanproduto}, " +
                        $"opção {existedean.FirstOrDefault().Eanopcao}, " +
                        $"tamanho {existedean.FirstOrDefault().Eantamanho}.");
                }

                var existedeanproduto = await EanCodificationRepository.Find(pr0.Pr0ean);
                if (existedeanproduto != null)
                {
                    throw new Exception($"O código {pr0.Pr0ean} já foi utilizado pelo produto " +
                        $"{existedeanproduto.Eanproduto}" +
                        $", cor {existedeanproduto.Eanopcao}" +
                        $", tamanho {existedeanproduto.Eantamanho}");
                }

                var usedean = await ProductRepository.FindUtilizatedEan<PR0>(pr0.Pr0produto, pr0.Pr0ean);
                if (usedean != null)
                {
                    throw new Exception($"O código {pr0.Pr0ean} já foi utilizado pelo produto {usedean.Pr0produto}");
                }
            }
        }

        public async Task<ProductCartModel> Find(string pr0Produto, string list, string authorization)
        {
            ProductCartModel productCartModel = await ProductRepository.FindProductCart(pr0Produto);
            if (productCartModel != null)
            {
                var colorList = await ProductRepository.FindPr2List(pr0Produto);
                productCartModel.LstPr2 = new List<PR2>(colorList);

                var sizeList = await ProductRepository.FindPr3List(pr0Produto);
                productCartModel.LstPr3 = new List<PR3>(sizeList);
                if (!string.IsNullOrWhiteSpace(list))
                {
                    productCartModel.Vd5preco = await PriceService.Find(productCartModel.Pr0produto, list, authorization, productCartModel.Pr3tamanho, productCartModel.Pr2opcao);
                }
            }

            return productCartModel;
        }

        public async Task<string> FindEan(string pr0produto, string pr2opcao, string pr3tamanho)
        {
            EAN ean = await EanCodificationRepository.FindEan(pr0produto, pr2opcao, pr3tamanho);

            if (ean != null)
                return ean.Eancodigo;

            PR0 pr0 = await ProductRepository.Find<PR0>(pr0produto);
            return pr0.Pr0ean;
        }

        public async Task<ReturnItemModel> FindReturnByCode(string product, IList<ReturnItemModel> lstItems, string cf1cliente, bool consigned, int qty, bool input, string option, string size)
        {
            cf1cliente = string.IsNullOrWhiteSpace(cf1cliente) ? string.Empty : cf1cliente;
            cf1cliente = Formatters.OnlyNumbers(cf1cliente);

            product = product.Trim();
            InfosForReturnModel infosForReturn = null;
            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            LX1 lx1 = await ProductionOrderParametersRepository.Find();
            if (lx1 == null)
                throw new Exception("Parâmetros ordem de produção não cadastrados");

            bool fillZerosToLeftInProductCode = bool.Parse(Configuration["FillZerosToLeftInProductCode"]);

            ReturnItemModel returnItemModel;
            if (lxd.Lxdean)
            {
                string eancode = product;

                int eancodigoSize = await FormatterService.FindFieldLength("EANCODIGO");
                if ((new Regex(@"[^\d]").Replace(product, "")).Length == 13)
                {
                    string eandigit = SisService.EanDigito(product.Substring(0, eancodigoSize));
                    if (!product.Substring(12, 1).Equals(eandigit))
                    {
                        if (!fillZerosToLeftInProductCode)
                            throw new Exception("Dígito EAN 13 inválido");
                    }
                    else
                    {
                        eancode = product.Substring(0, eancodigoSize);
                    }
                }

                returnItemModel = await ProductRepository.FindByEanTable<ReturnItemModel>(eancode);
                if (returnItemModel == null)
                {
                    returnItemModel = await ProductRepository.FindByEan<ReturnItemModel>(eancode);
                    if (returnItemModel == null)
                    {
                        if (fillZerosToLeftInProductCode)
                        {
                            product = product.TrimStart('0');
                            int lengthPr0produto = await FormatterService.FindFieldLength("PR0PRODUTO");
                            product = product.PadLeft(lengthPr0produto, '0');
                        }
                        returnItemModel = await ProductRepository.Find<ReturnItemModel>(product);
                    }
                    if (returnItemModel != null)
                    {
                        if (returnItemModel.Eancodigo == null)
                            returnItemModel.Eancodigo = string.Empty;
                        if (returnItemModel.Pr2opcao == null)
                            returnItemModel.Pr2opcao = option;
                        if (returnItemModel.Pr3tamanho == null)
                            returnItemModel.Pr3tamanho = size;

                        var colorList = await ProductRepository.FindPr2List(returnItemModel.Pr0produto);
                        returnItemModel.LstPr2 = new List<PR2>(colorList);

                        var sizeList = await ProductRepository.FindPr3List(returnItemModel.Pr0produto);
                        returnItemModel.LstPr3 = new List<PR3>(sizeList);
                    }
                }
            }
            else
            {
                if (lx1.Lx1rfid)
                {
                    OF3 of3 = await PiecesRecordRepository.FindByRfidOrOf3peca(product);
                    if (of3 == null)
                        throw new Exception(string.Format("Tag RFID {0} não cadastrada para nenhuma peça", product));

                    product = of3.Of3peca;
                }

                bool existCodExt = await DictionaryService.ExistField("OF3CODEXT");
                if (existCodExt)
                {
                    OF3 of3 = await PiecesRecordRepository.FindByOf3codext(product);
                    if (of3 != null)
                        product = of3.Of3peca;
                }

                int lengthOf3peca = await FormatterService.FindFieldLength("of3peca");

                product = product.PadLeft(lengthOf3peca, '0');

                if (product.Length > lengthOf3peca)
                {
                    int difference = product.Length - lengthOf3peca;
                    product = product.Substring(difference, product.Length - difference);
                }
                returnItemModel = await ProductRepository.FindByOF3Table<ReturnItemModel>(product);
            }

            if (returnItemModel == null)
            {
                throw new Exception("Produto não encontrado");
            }

            //validações e retorno de informações da venda: doc, descon, precodesc
            if (!input)
            {
                if (lxd.Lxdean)
                {
                    infosForReturn = await ProductService.ValidateVdxpeca(returnItemModel, cf1cliente, consigned, lstItems, qty);
                }
                else
                {
                    infosForReturn = await ProductService.ValidateVdlpeca(returnItemModel, cf1cliente, consigned);
                }
            }
            else
            {
                returnItemModel.Preco = await PriceService.GetPrice(returnItemModel.Pr0produto, lxd.Lxdlista, returnItemModel.Pr3tamanho, returnItemModel.Pr2opcao, false);
            }
            if (infosForReturn == null)
            {
                return returnItemModel;
            }

            //pegar o preço da nota de venda e pegar os dados da venda: cv5doc, desconto do item e preço com desconto e sem desconto
            await ProductService.FindInfosForReturn(returnItemModel, infosForReturn, cf1cliente);

            return returnItemModel;
        }

        public async Task<IList<ProductCartModel>> FindForPriceQuery(string text, string orderBy)
        {
            text = text.Trim();

            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            LX1 lx1 = await ProductionOrderParametersRepository.Find();
            if (lx1 == null)
                throw new Exception("Parâmetros ordem de produção não cadastrados");

            bool fillZerosToLeftInProductCode = bool.Parse(Configuration["FillZerosToLeftInProductCode"]);

            ProductCartModel productCartModel;
            if (lxd.Lxdean)
            {
                string eancode = text;
                int eancodigoSize = await FormatterService.FindFieldLength("EANCODIGO");
                if ((new Regex(@"[^\d]").Replace(text, "")).Length == 13)
                {
                    string eandigit = SisService.EanDigito(text.Substring(0, eancodigoSize));
                    if (!text.Substring(12, 1).Equals(eandigit))
                    {
                        if (!fillZerosToLeftInProductCode)
                            throw new Exception("Dígito EAN 13 inválido");
                    }
                    else
                    {
                        eancode = text.Substring(0, eancodigoSize);
                    }
                }

                productCartModel = await ProductRepository.FindByEanTable<ProductCartModel>(eancode);
                if (productCartModel == null)
                {
                    productCartModel = await ProductRepository.FindByEan<ProductCartModel>(eancode);
                }

                //colocando dígito verificador no EAN
                if (productCartModel != null && !string.IsNullOrWhiteSpace(productCartModel.Eancodigo))
                    productCartModel.Eancodigo += SisService.EanDigito(productCartModel.Eancodigo);

            }
            else
            {
                if (lx1.Lx1rfid)
                {
                    OF3 of3 = await PiecesRecordRepository.FindByRfidOrOf3peca(text);
                    if (of3 != null)
                        text = of3.Of3peca;
                }

                bool existCodExt = await DictionaryService.ExistField("OF3CODEXT");
                if (existCodExt)
                {
                    OF3 of3 = await PiecesRecordRepository.FindByOf3codext(text);
                    if (of3 != null)
                        text = of3.Of3peca;
                }

                int lengthOf3peca = await FormatterService.FindFieldLength("of3peca");

                string of3peca = text;

                if (of3peca.Length > lengthOf3peca)
                {
                    int difference = of3peca.Length - lengthOf3peca;
                    of3peca = of3peca.Substring(difference, of3peca.Length - difference);
                }

                productCartModel = await ProductRepository.FindByOF3Table<ProductCartModel>(of3peca);
            }

            if (productCartModel != null)
            {
                if (productCartModel.Eancodigo == null)
                    productCartModel.Eancodigo = string.Empty;
                if (productCartModel.Pr2opcao == null)
                    productCartModel.Pr2opcao = string.Empty;
                if (productCartModel.Pr3tamanho == null)
                    productCartModel.Pr3tamanho = string.Empty;
                return new List<ProductCartModel>() { productCartModel };
            }

            return await ProductRepository.FindForPriceQuery<ProductCartModel>(text, orderBy);
        }

        public async Task<bool> FindByAlterCode(string pr0produto, string pr0refer)
        {
            return await ProductRepository.FindByAlterCode(pr0produto, pr0refer) == null ? false : true;
        }
    }
}
