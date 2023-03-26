using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using Printing.Models;
using Printing.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Printing.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class ReportController : Controller
    {
        private readonly ReportService ReportService;

        public ReportController(IConfiguration Configuration)
        {
            ReportService = new ReportService(Configuration);
        }

        [HttpPost]
        public async Task<ReturnModel<string>> PrintReportProductTurnover(ReportProductTurnoverModel model)
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            try
            {
                string authorization = Request.Headers["Authorization"];

                ClaimsPrincipal claimPrincipal = HttpContext.User;
                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await ReportService.PrintReportProductTurnover(model, terminal, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
