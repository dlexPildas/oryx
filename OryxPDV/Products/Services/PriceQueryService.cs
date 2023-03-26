using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Services
{
    public class PriceQueryService
    {
        private readonly IConfiguration Configuration;
        private readonly PriceService PriceService;
        private readonly StockService StockService;
        private readonly PriceListRepository PriceListRepository;
        private readonly PDVParametersRepository PDVParametersRepository;
        private readonly ProductRepository ProductRepository;
        private readonly ProductSizesRepository ProductSizesRepository;

        public PriceQueryService(IConfiguration configuration)
        {
            this.Configuration = configuration;
            PriceService = new PriceService(configuration);
            StockService = new StockService(configuration);
            PriceListRepository = new PriceListRepository(Configuration["OryxPath"] + "oryx.ini");
            PDVParametersRepository = new PDVParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductRepository = new ProductRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductSizesRepository = new ProductSizesRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<PriceQueryModel> Find(string pr0produto, string pr2opcao, string pr3tamanho, IList<string> cv6lista)
        {
            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            if (string.IsNullOrWhiteSpace(pr0produto))
                throw new Exception("Produto não informado");

            if (cv6lista == null || cv6lista.Count == 0)
                throw new Exception("Nenhuma lista de preço informada");

            //colocando lista de preço do PDV como a primeira
            cv6lista.Remove(lxd.Lxdlista);
            cv6lista = cv6lista.Prepend(lxd.Lxdlista).ToList();

            //dados do produto
            PR0 pr0 = await ProductRepository.Find<PR0>(pr0produto);

            if (pr0 == null)
            {
                throw new Exception(string.Format("Produto {0} não encontrado", pr0produto));
            }

            PriceQueryModel priceQueryModel = new PriceQueryModel()
            {
                Pr0produto = pr0produto,
                Pr2opcao = pr2opcao,
                Pr3tamanho = pr3tamanho,
                Pr0pesobru = pr0.Pr0pesobru,
                Pr0pesoliq = pr0.Pr0pesoliq,
                Prices = new List<PriceModel>()
            };

            //popular prices - preços do produto, conforme as listas de preço 
            foreach (string list in cv6lista)
            {
                CV6 cv6 = await PriceListRepository.Find(list);
                
                if (cv6 == null)
                    throw new Exception(string.Format("Lista de preço {0} não encontrada", list));
                
                decimal price = await PriceService.GetPrice(pr0produto, list, pr3tamanho, pr2opcao, false);
                
                priceQueryModel.Prices.Add(new PriceModel()
                {
                    Cv6lista = list,
                    Cv6percent = cv6.Cv6percent,
                    Cv6nome = cv6.Cv6nome,
                    PriceList = price,
                    Vd5preco = price
                });
            }

            //popular vd5preço -  sendo o preço conforme a lista de preço em lxdlista
            priceQueryModel.Vd5preco = await PriceService.GetPrice(pr0produto, lxd.Lxdlista, pr3tamanho, pr2opcao, false);

            //popular stock - estoque do produto selecionado - caso o estoque esteja ativo
            if (lxd.Lxdprecest)
            {
                if (string.IsNullOrWhiteSpace(pr2opcao))
                    pr2opcao = "";
                
                if (string.IsNullOrWhiteSpace(pr3tamanho))
                    pr3tamanho = "";

                priceQueryModel.Stock = await StockService.FindStock(pr0produto, pr2opcao, pr3tamanho);

            }

            //popular variants - caso a consulta rápida esteja desmarcada;
            //buscar todas as variantes da referência, popular com o preço de cada um conforme preço especial por tamanho e a lista de preço padrão PDV
            if (lxd.Lxdprecmod)
            {
                priceQueryModel.Variants = await ProductRepository.FindVariants<VariantModel>(pr0produto);

                foreach (VariantModel variant in priceQueryModel.Variants)
                {
                    variant.Price = await PriceService.GetPrice(variant.Pr0produto, lxd.Lxdlista, variant.Pr3tamanho, variant.Pr2opcao, false);
                    //Caso o parametro de estoque estiver ativo, então consultar o estoque para todas as variantes
                    if (lxd.Lxdprecest)
                        variant.Stock = await StockService.FindStock(variant.Pr0produto, variant.Pr2opcao, variant.Pr3tamanho);
                }
            }

            if (!string.IsNullOrWhiteSpace(priceQueryModel.Pr3tamanho))
            {
                PR3 pr3 = await ProductSizesRepository.Find(priceQueryModel.Pr0produto, priceQueryModel.Pr3tamanho);
                if (pr3 != null && pr3.Pr3pesoliq > 0)
                    priceQueryModel.Pr0pesoliq = pr3.Pr3pesoliq;
            }

            return priceQueryModel;
        }
    }
}
