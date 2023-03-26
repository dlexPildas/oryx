using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxPdv.Services;
using System;
using System.Threading.Tasks;

namespace OryxPdv.Controller
{
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class ParametersController : ControllerBase
    {
        readonly ParametersService ParametersService;
        public ParametersController(IConfiguration Configuration)
        {
            ParametersService = new ParametersService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnModel<LXD>> Find()
        {
            ReturnModel<LXD> returnModel = new ReturnModel<LXD>();
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

        [Authorize]
        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(LXD lxd)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await ParametersService.Save(lxd);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
        [Authorize]
        [HttpPut]
        public async Task<ReturnModel<bool>> Update(LXD lxd)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await ParametersService.Save(lxd, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
        [Authorize]
        [HttpDelete]
        public async Task<ReturnModel<bool>> Delete()
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await ParametersService.Delete();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
