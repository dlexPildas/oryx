using Microsoft.Extensions.Configuration;
using Order.Models;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Services
{
    public class ShipmentService
    {
        private readonly IConfiguration Configuration;
        private readonly OrderRepository OrderRepository;

        public ShipmentService(IConfiguration configuration)
        {
            Configuration = configuration;
            OrderRepository = new OrderRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<PayloadShipForSaleConfirmationModel> FindForSaleConfirmation(string vd1pedido)
        {
            VD1 vd1 = await OrderRepository.Find(vd1pedido);

            if (vd1 == null)
                throw new Exception(string.Format("Pedido {0} não encontrado.", vd1pedido));

            IList<ShipSaleConfirmationModel> lstShip = await OrderRepository.FindShipForSaleConfirmation(vd1pedido);

            return new PayloadShipForSaleConfirmationModel()
            {
                Cf1cliente = vd1.Vd1cliente,
                LstShip = lstShip
            };
        }
    }
}
