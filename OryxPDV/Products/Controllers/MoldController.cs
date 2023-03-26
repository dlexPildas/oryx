using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using Products.Services;
using System;
using System.Threading.Tasks;

namespace Products.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[Action]")]
    public class MoldController : ControllerBase
    {
        readonly MoldService MoldService;
        public MoldController(IConfiguration Configuration)
        {
            MoldService = new MoldService(Configuration);
        }

        [HttpGet("{gr3molde}")]
        public async Task<ReturnModel<GR3>> Find(string gr3molde)
        {
            ReturnModel<GR3> returnModel = new ReturnModel<GR3>();
            try
            {
                returnModel.ObjectModel = await MoldService.Find(gr3molde);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
