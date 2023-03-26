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
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class APIParametersController : ControllerBase
    {
        readonly APIParametersService APIParametersService;
        public APIParametersController(IConfiguration Configuration)
        {
            APIParametersService = new APIParametersService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnModel<LXE>> Find()
        {
            ReturnModel<LXE> returnModel = new ReturnModel<LXE>();
            try
            {
                returnModel.ObjectModel = await APIParametersService.Find();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
        [Authorize]
        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(LXE lxe)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await APIParametersService.Save(lxe);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [Authorize]
        [HttpPut]
        public async Task<ReturnModel<bool>> Update(LXE lxe)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await APIParametersService.Save(lxe, true);
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
                returnModel.ObjectModel = await APIParametersService.Delete();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
