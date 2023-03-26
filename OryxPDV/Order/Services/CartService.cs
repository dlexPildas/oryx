using Microsoft.Extensions.Configuration;
using Order.Models;
using OryxDomain.Models;
using OryxDomain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Services
{
    public class CartService
    {
        private readonly IConfiguration Configuration;
        private readonly PriceService PriceService;

        public CartService(IConfiguration configuration)
        {
            Configuration = configuration;
            PriceService = new PriceService(Configuration);
        }

        public async Task<IList<Vd7CartModel>> RecalculatePrices(RecalculatePricesModel model, string authorization)
        {
            foreach (Vd7CartModel vd7 in model.LstVd7)
            {
                foreach (SalesItemModel product in vd7.Items)
                {
                    ProductCartModel productCartMode = new ProductCartModel()
                    {
                        Pr0produto = product.Vd2produto,
                        Pr3tamanho = product.Vd3tamanho,
                        Pr2opcao = product.Vd3opcao,
                    };
                    product.Vd5preco = await PriceService.Find(productCartMode.Pr0produto, model.List, authorization, productCartMode.Pr3tamanho, productCartMode.Pr2opcao);
                    product.PriceList = product.Vd5preco;
                    product.Total = product.Vd5preco * product.Vd3qtde;
                }
            }
            return model.LstVd7;
        }
    }
}
