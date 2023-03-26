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
    public class FiscalParametersController : Controller
    {
        readonly FiscalParametersService FiscalParametersService;
        public FiscalParametersController(IConfiguration Configuration)
        {
            FiscalParametersService = new FiscalParametersService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnModel<LX3>> Find()
        {
            ReturnModel<LX3> returnModel = new ReturnModel<LX3>();
            try
            {
                returnModel.ObjectModel = await FiscalParametersService.Find();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(LX3 lx3)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await FiscalParametersService.Save(lx3);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(LX3 lx3)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await FiscalParametersService.Save(lx3, true);
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
                returnModel.ObjectModel = await FiscalParametersService.Delete();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
