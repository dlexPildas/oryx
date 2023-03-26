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
    public class ParametersController
    {
        readonly ParametersService ParametersService;
        public ParametersController(IConfiguration Configuration)
        {
            ParametersService = new ParametersService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnModel<LX0>> Find()
        {
            ReturnModel<LX0> returnModel = new ReturnModel<LX0>();
            try
            {
                returnModel.ObjectModel = await ParametersService.Find();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
