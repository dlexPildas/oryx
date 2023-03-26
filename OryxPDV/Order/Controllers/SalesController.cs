using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Order.Models;
using Order.Services;
using OryxDomain.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Order.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class SalesController : ControllerBase
    {
        readonly SalesService SalesService;
        public SalesController(IConfiguration Configuration)
        {
            SalesService = new SalesService(Configuration);
        }

        [HttpPost]
        public async Task<ReturnModel<string>> PrintReportSalesByPeriod(PrintReportSalesByPeriod model)
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            try
            {
                string authorization = Request.Headers["Authorization"];

                ClaimsPrincipal claimPrincipal = HttpContext.User;
                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await SalesService.PrintReportSalesByPeriod(model, terminal, false, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<string>> PrintReportProductsSoldByPeriod(ProductsSoldByPeriod model)
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            try
            {
                string authorization = Request.Headers["Authorization"];

                ClaimsPrincipal claimPrincipal = HttpContext.User;
                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await SalesService.PrintReportProductsSoldByPeriod(model, terminal, false, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<string>> PrintReportSalesRanking(SalesRankingModel model)
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            try
            {
                string authorization = Request.Headers["Authorization"];

                ClaimsPrincipal claimPrincipal = HttpContext.User;
                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await SalesService.PrintReportSalesRanking(model, terminal, false, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
