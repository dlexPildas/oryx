using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxPdv.Services;
using System;
using System.Threading.Tasks;

namespace OryxPdv.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class IntegrationRepresentativeController : ControllerBase
    {
        readonly IntegrationRepresentativeService IntegrationRepresentativeService;
        public IntegrationRepresentativeController(IConfiguration Configuration)
        {
            IntegrationRepresentativeService = new IntegrationRepresentativeService(Configuration);
        }

        [HttpGet("{pd8codigo}")]
        public async Task<ReturnModel<PD8>> Find(string pd8codigo)
        {
            ReturnModel<PD8> returnModel = new ReturnModel<PD8>();
            try
            {
                returnModel.ObjectModel = await IntegrationRepresentativeService.Find(pd8codigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnListModel<PD8>> FindAll()
        {
            ReturnListModel<PD8> returnModel = new ReturnListModel<PD8>();
            try
            {
                returnModel.ObjectModel = await IntegrationRepresentativeService.FindAll();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PD8 pd8)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await IntegrationRepresentativeService.Save(pd8, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PD8 pd8)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await IntegrationRepresentativeService.Save(pd8, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{pd8codigo}")]
        public async Task<ReturnModel<bool>> Delete(string pd8codigo)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await IntegrationRepresentativeService.Delete(pd8codigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<PD8>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<PD8>> returnModel = new ReturnModel<SearchPayloadModel<PD8>>();
            try
            {
                returnModel.ObjectModel = await IntegrationRepresentativeService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
