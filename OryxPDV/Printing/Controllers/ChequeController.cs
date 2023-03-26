using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using Printing.Services;
using System;
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
    public class ChequeController : ControllerBase
    {
        readonly ChequeServices ChequeServices;
        public ChequeController(IConfiguration Configuration)
        {
            ChequeServices = new ChequeServices(Configuration);
        }

        [HttpPost]
        public async Task<ReturnModel<string>> Print(CV8 cv8)
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            try
            {
                ClaimsPrincipal claimPrincipal = HttpContext.User;

                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await ChequeServices.Print(cv8, terminal);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }

            return returnModel;
        }
    }
}
