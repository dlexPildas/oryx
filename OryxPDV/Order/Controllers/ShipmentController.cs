using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Order.Models;
using Order.Services;
using OryxDomain.Models;
using System;
using System.Threading.Tasks;

namespace Order.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class ShipmentController : ControllerBase
    {
        readonly ShipmentService ShipmentService;
        public ShipmentController(IConfiguration Configuration)
        {
            ShipmentService = new ShipmentService(Configuration);
        }

        [HttpGet("{vd1pedido}")]
        public async Task<ReturnModel<PayloadShipForSaleConfirmationModel>> FindForSaleConfirmation(string vd1pedido)
        {
            ReturnModel<PayloadShipForSaleConfirmationModel> returnModel = new ReturnModel<PayloadShipForSaleConfirmationModel>();
            try
            {
                returnModel.ObjectModel = await ShipmentService.FindForSaleConfirmation(vd1pedido);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
