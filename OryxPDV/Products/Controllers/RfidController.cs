using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using Products.Models;
using Products.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Products.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[Action]")]
    public class RfidController : ControllerBase
    {
        readonly RfidService RfidService;
        public RfidController(IConfiguration Configuration)
        {
            RfidService = new RfidService(Configuration);
        }

        [HttpPost]
        public async Task<ReturnListModel<string>> FindBySales(RfidPostModel model)
        {
            ReturnListModel<string> returnModel = new ReturnListModel<string>();
            try
            {
                ClaimsPrincipal claimPrincipal = HttpContext.User;

                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                 where c.Type == "terminal"
                                 select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await RfidService.FindBySales(terminal, model.LstSalesItems);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnListModel<string>> FindByReturn(RfidPostModel model)
        {
            ReturnListModel<string> returnModel = new ReturnListModel<string>();
            try
            {
                ClaimsPrincipal claimPrincipal = HttpContext.User;

                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await RfidService.FindByReturn(terminal, model.LstReturnItems);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnListModel<string>> Find()
        {
            ReturnListModel<string> returnModel = new ReturnListModel<string>();
            try
            {
                ClaimsPrincipal claimPrincipal = HttpContext.User;
                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();
                returnModel.ObjectModel = await RfidService.FindAsync(terminal);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}