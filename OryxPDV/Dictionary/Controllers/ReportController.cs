using Dictionary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using System;
using System.Threading.Tasks;

namespace Dictionary.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class ReportController : ControllerBase
    {
        readonly ReportService ReportService;

        public ReportController(IConfiguration Configuration)
        {
            ReportService = new ReportService(Configuration);
        }

        [HttpGet("{dc9relat}")]
        public async Task<ReturnModel<DC9>> Find(string dc9relat)
        {
            ReturnModel<DC9> returnModel = new ReturnModel<DC9>();
            try
            {
                returnModel.ObjectModel = await ReportService.Find(dc9relat);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
