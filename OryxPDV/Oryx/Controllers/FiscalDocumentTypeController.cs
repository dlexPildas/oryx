using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oryx.Services;
using OryxDomain.Models;
using System;
using System.Threading.Tasks;

namespace Oryx.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class FiscalDocumentTypeController : Controller
    {
        private readonly FiscalDocumentTypeService FiscalDocumentTypeService;

        public FiscalDocumentTypeController(IConfiguration configuration, ILogger<FiscalDocumentTypeController> logger)
        {
            FiscalDocumentTypeService = new FiscalDocumentTypeService(configuration, logger);
        }

        [HttpGet("{cv1docfis}")]
        public async Task<ReturnModel<string>> ProximoNumero(string cv1docfis)
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            try
            {
                returnModel.ObjectModel = await FiscalDocumentTypeService.ProximoNumero(cv1docfis);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
