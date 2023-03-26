using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Order.Models;
using Order.Services;
using OryxDomain.Models;
using System.Threading.Tasks;

namespace Order.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class CartController : ControllerBase
    {
        readonly CartService CartService;
        public CartController(IConfiguration Configuration)
        {
            CartService = new CartService(Configuration);
        }

        [HttpPost]
        public async Task<ReturnListModel<Vd7CartModel>> RecalculatePrices(RecalculatePricesModel model)
        {
            ReturnListModel<Vd7CartModel> returnModel = new ReturnListModel<Vd7CartModel>();
            string authorization = Request.Headers["Authorization"];
            returnModel.ObjectModel = await CartService.RecalculatePrices(model, authorization);
            return returnModel;
        }
    }
}
