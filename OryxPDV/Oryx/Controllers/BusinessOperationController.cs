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
    public class BusinessOperationController : Controller
    {
        readonly BusinessOperationService BusinessOperationService;
        public BusinessOperationController(IConfiguration Configuration)
        {
            BusinessOperationService = new BusinessOperationService(Configuration);
        }

        [HttpGet("{cv3opercom}")]
        public async Task<ReturnModel<CV3>> Find(string cv3opercom)
        {
            ReturnModel<CV3> returnModel = new ReturnModel<CV3>();
            try
            {
                returnModel.ObjectModel = await BusinessOperationService.Find(cv3opercom);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
