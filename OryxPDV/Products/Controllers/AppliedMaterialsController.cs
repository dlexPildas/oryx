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
    public class AppliedMaterialsController : ControllerBase
    {
        readonly AppliedMaterialsService AppliedMaterialsService;
        public AppliedMaterialsController(IConfiguration Configuration)
        {
            AppliedMaterialsService = new AppliedMaterialsService(Configuration);
        }

        [HttpGet()]
        public async Task<ReturnListModel<PR8>> Find([FromQuery] string pr0produto, [FromQuery] string pr3tamanho)
        {
            ReturnListModel<PR8> returnModel = new ReturnListModel<PR8>();
            try
            {
                returnModel.ObjectModel = await AppliedMaterialsService.FindList(pr0produto, pr3tamanho);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
