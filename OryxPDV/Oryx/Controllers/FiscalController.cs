using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OryxDomain.Models;
using OryxDomain.Services;
using System;
using System.Threading.Tasks;

namespace Oryx.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class FiscalController : Controller
    {
        private readonly FiscalService FiscalService;

        public FiscalController(IConfiguration configuration, ILogger<FiscalController> logger)
        {
            FiscalService = new FiscalService(configuration, logger);
        }

        [HttpGet("{conpgto}/{lista}")]
        public async Task<ReturnModel<bool>> CondicaoValida(string conpgto, string lista)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await FiscalService.CondicaoValida(conpgto, lista);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
