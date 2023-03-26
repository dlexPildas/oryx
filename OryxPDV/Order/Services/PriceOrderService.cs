using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Threading.Tasks;

namespace Order.Services
{
    public class PriceOrderService
    {
        private readonly IConfiguration Configuration;
        private readonly PriceOrderRepository PriceOrderRepository;
        public PriceOrderService(IConfiguration configuration)
        {
            Configuration = configuration;
            PriceOrderRepository = new PriceOrderRepository(Configuration["OryxPath"] + "oryx.ini");
        }
        public async Task<VD5> Find(string vd5pedido, string vd5produto, string vd5tamanho, string vd5opcao)
        {
            VD5 vd5 = await PriceOrderRepository.Find( vd5pedido,  vd5produto,  vd5tamanho, vd5opcao);

            if (vd5 == null)
            {
                throw new Exception(message: "Preço pedido não encontrado.");
            }
            return vd5;
        }
    }
}
