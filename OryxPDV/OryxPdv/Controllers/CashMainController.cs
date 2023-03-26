using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxPdv.Services;
using System;
using System.Threading.Tasks;

namespace OryxPdv.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class CashMainController : ControllerBase
    {
        readonly CashMainService CashMainService;
        public CashMainController(IConfiguration Configuration)
        {
            CashMainService = new CashMainService(Configuration);
        }
        [HttpGet]
        public async Task<ReturnModel<PD3>> Find()
        {
            ReturnModel<PD3> returnModel = new ReturnModel<PD3>();
            try
            {
                returnModel.ObjectModel = await CashMainService.Find();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PD3 pd3)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await CashMainService.Save(pd3);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
        
        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PD3 pd3)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await CashMainService.Save(pd3, true);
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
                returnModel.ObjectModel = await CashMainService.Delete();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
