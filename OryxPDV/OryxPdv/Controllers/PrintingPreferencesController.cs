using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxPdv.Services;
using System;
using System.Threading.Tasks;

namespace OryxPdv.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class PrintingPreferencesController : ControllerBase
    {
        readonly PrintingPreferencesService PrintingPreferencesService;
        public PrintingPreferencesController(IConfiguration Configuration)
        {
            PrintingPreferencesService = new PrintingPreferencesService(Configuration);
        }

        [HttpGet("{pd1codigo}")]
        public async Task<ReturnListModel<PD1>> FindList(string pd1codigo)
        {
            ReturnListModel<PD1> returnModel = new ReturnListModel<PD1>();
            try
            {
                returnModel.ObjectModel = await PrintingPreferencesService.FindList(pd1codigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{pd1codigo}/{pd1relat}")]
        public async Task<ReturnModel<PD1>> Find(string pd1codigo, string pd1relat)
        {
            ReturnModel<PD1> returnModel = new ReturnModel<PD1>();
            try
            {
                returnModel.ObjectModel = await PrintingPreferencesService.Find(pd1codigo, pd1relat);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PD1 pd1)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await PrintingPreferencesService.Save(pd1);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PD1 pd1)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await PrintingPreferencesService.Save(pd1, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{pd1codigo}/{pd1relat}")]
        public async Task<ReturnModel<bool>> Delete(string pd1codigo, string pd1relat)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await PrintingPreferencesService.Delete(pd1codigo, pd1relat);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<PD1>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<PD1>> returnModel = new ReturnModel<SearchPayloadModel<PD1>>();
            try
            {
                returnModel.ObjectModel = await PrintingPreferencesService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
