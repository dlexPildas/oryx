using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Order.Services;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using System;
using System.Threading.Tasks;

namespace Products.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[Action]")]
    public class PriceOrder : ControllerBase
    {
        readonly PriceOrderService PriceOrderService;
        public PriceOrder(IConfiguration Configuration)
        {
            PriceOrderService = new PriceOrderService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnModel<VD5>> Find([FromQuery] string vd5pedido, [FromQuery] string vd5produto, [FromQuery] string vd5tamanho, [FromQuery] string vd5opcao)
        {
            ReturnModel<VD5> returnModel = new ReturnModel<VD5>();
            try
            {
                returnModel.ObjectModel = await PriceOrderService.Find(vd5pedido, vd5produto, vd5tamanho, vd5opcao);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
