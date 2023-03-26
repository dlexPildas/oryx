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
    public class SizeGridController : ControllerBase
    {
        readonly SizeGridService SizeGridService;
        public SizeGridController(IConfiguration Configuration)
        {
            SizeGridService = new SizeGridService(Configuration);
        }

        [HttpGet("{gr0grade}")]
        public async Task<ReturnModel<GR0>> Find(string gr0grade)
        {
            ReturnModel<GR0> returnModel = new ReturnModel<GR0>();
            try
            {
                returnModel.ObjectModel = await SizeGridService.Find(gr0grade);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Save(GR0 gr0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await SizeGridService.Save(gr0, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{gr0grade}")]
        public async Task<ReturnModel<bool>> Delete(string gr0grade)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await SizeGridService.Delete(gr0grade);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<GR0>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<GR0>> returnModel = new ReturnModel<SearchPayloadModel<GR0>>();
            try
            {
                returnModel.ObjectModel = await SizeGridService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
