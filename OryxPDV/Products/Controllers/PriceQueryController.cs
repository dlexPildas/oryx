using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using Products.Models;
using Products.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class PriceQueryController : Controller
    {
        private readonly PriceQueryService PriceQueryService;
        public PriceQueryController(IConfiguration Configuration)
        {
            PriceQueryService = new PriceQueryService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnModel<PriceQueryModel>> Find([FromQuery] string pr0produto, [FromQuery] string pr2opcao, [FromQuery] string pr3tamanho, [FromQuery] IList<string> cv6lista)
        {
            ReturnModel<PriceQueryModel> returnModel = new ReturnModel<PriceQueryModel>();
            try
            {
                returnModel.ObjectModel = await PriceQueryService.Find(pr0produto, pr2opcao, pr3tamanho, cv6lista);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}