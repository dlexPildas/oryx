using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OryxDomain.Http;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class PriceService
    {
        private readonly IConfiguration Configuration;
        private readonly APIParametersRepository APIParametersRepository;
        private readonly ProductRepository ProductRepository;
        private readonly PriceListRepository PriceListRepository;

        public PriceService(IConfiguration configuration)
        {
            Configuration = configuration;
            APIParametersRepository = new APIParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductRepository = new ProductRepository(Configuration["OryxPath"] + "oryx.ini");
            PriceListRepository = new PriceListRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<decimal> FindMaterialPrice(string product, string list, string cpfCnpj, string authorization)
        {
            //TODO ajustar a API do preço para seguir o padrão de controllers e implementar o get
            throw new NotImplementedException();
        }

        public async Task<decimal> FindEsquadrias(string product, string list, string color, string authorization)
        {
            //TODO ajustar a API do preço para seguir o padrão de controllers e implementar o get
            throw new NotImplementedException();
        }

        public async Task<decimal> Find(string product, string list, string authorization, string size = "", string color = "")
        {
            LXE lxe = await APIParametersRepository.Find();
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");

            Dictionary<string, string> querie = null;

            string url = "/Price/Price/PriceHierarchy/"
                + product
                + "/" + list;

            if (!string.IsNullOrWhiteSpace(size))
            {
                url = url + "/" + size;
                if (!string.IsNullOrWhiteSpace(color))
                {
                    url = url + "/" + color;
                    querie = new Dictionary<string, string>()
                        {
                            { "precopadrao", false.ToString() }
                        };
                }
            }

            string jsonResponse = await HttpUtilities.GetAsync(
                  lxe.Lxebaseurl
                , url
                , string.Empty
                , authorization
                , querie);

            ReturnModel<decimal> price = JsonConvert.DeserializeObject<ReturnModel<decimal>>(jsonResponse);
            if (price.IsError)
            {
                return 0;
            }
            return price.ObjectModel;
        }

        public async Task<decimal> GetPrice(string produto, string lista, string tam = "", string cor = "", bool precopadrao = false)
        {
            decimal price = 0;

            price = string.IsNullOrWhiteSpace(tam)
                                    ? await ProductRepository.FindPrice(produto)
                                    : await ProductRepository.FindPrice(produto, tam);

            price = await RoundPriceByList(price, lista);

            if (precopadrao)
                return price;

            decimal tempPrice;

            tempPrice = string.IsNullOrWhiteSpace(tam)
                                    ? await ProductRepository.FindSpecialPrice(produto, lista)
                                    : await ProductRepository.FindSpecialPrice(produto, lista, tam);

            if (tempPrice > 0)
                price = tempPrice;

            if (!string.IsNullOrWhiteSpace(cor))
            {
                tempPrice = await ProductRepository.FindSpecialPrice(produto, lista, tam, cor);
                if (tempPrice > 0)
                    price = tempPrice;
            }

            return price;
        }

        public async Task<decimal> RoundPriceByList(decimal price, string lista)
        {
            CV6 cv6 = await PriceListRepository.Find(lista);
            if (cv6 == null)
            {
                throw new Exception("Parametros inválidos para lista de preço.");
            }
            price = price * (cv6.Cv6percent / 100);
            switch (cv6.Cv6arredon)
            {
                case 2:
                    price = Math.Round(price, 1);
                    break;
                case 3:
                    price = Math.Round(price, 0);
                    break;
                case 4:
                    price = ((int)price) + 1;
                    break;
                default:
                    break;
            }

            return price;
        }
    }
}
