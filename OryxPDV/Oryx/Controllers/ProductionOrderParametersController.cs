using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oryx.Services;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using System;
using System.Threading.Tasks;

namespace Oryx.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class ProductionOrderParametersController : Controller
    {
        readonly ProductionOrderParametersService ProductionOrderParametersService;
        public ProductionOrderParametersController(IConfiguration Configuration)
        {
            ProductionOrderParametersService = new ProductionOrderParametersService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnModel<LX1>> Find()
        {
            ReturnModel<LX1> returnModel = new ReturnModel<LX1>();
            try
            {
                returnModel.ObjectModel = await ProductionOrderParametersService.Find();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
