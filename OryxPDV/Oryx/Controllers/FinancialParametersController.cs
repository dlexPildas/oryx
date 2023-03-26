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
    public class FinancialParametersController : Controller
    {
        readonly FinancialParametersService FinancialParametersService;
        public FinancialParametersController(IConfiguration Configuration)
        {
            FinancialParametersService = new FinancialParametersService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnModel<LXA>> Find()
        {
            ReturnModel<LXA> returnModel = new ReturnModel<LXA>();
            try
            {
                returnModel.ObjectModel = await FinancialParametersService.Find();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(LXA lxa)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await FinancialParametersService.Save(lxa);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(LXA lxa)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await FinancialParametersService.Save(lxa, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete]
        public async Task<ReturnModel<bool>> Delete()
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await FinancialParametersService.Delete();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
