using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Services;
using System;
using System.Threading.Tasks;

namespace Transporter.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class LogisticIntegrationController : ControllerBase
    {
        readonly LogisticIntegrationService LogisticIntegrationService;
        public LogisticIntegrationController(IConfiguration Configuration)
        {
            LogisticIntegrationService = new LogisticIntegrationService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<LXF>> FindList()
        {
            ReturnListModel<LXF> returnModel = new ReturnListModel<LXF>();
            try
            {
                returnModel.ObjectModel = await LogisticIntegrationService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{lxfcodigo}")]
        public async Task<ReturnModel<LXF>> Find(string lxfcodigo)
        {
            ReturnModel<LXF> returnModel = new ReturnModel<LXF>();
            try
            {
                returnModel.ObjectModel = await LogisticIntegrationService.Find(lxfcodigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(LXF lxf)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await LogisticIntegrationService.Save(lxf, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(LXF lxf)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await LogisticIntegrationService.Save(lxf, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{lxfcodigo}")]
        public async Task<ReturnModel<bool>> Delete(string lxfcodigo)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await LogisticIntegrationService.Delete(lxfcodigo);
            }   
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<LXF>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<LXF>> returnModel = new ReturnModel<SearchPayloadModel<LXF>>();
            try
            {
                returnModel.ObjectModel = await LogisticIntegrationService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{lxftipo}")]
        public async Task<ReturnModel<LXF>> FindByType(LogisticIntegrationType lxftipo)
        {
            ReturnModel<LXF> returnModel = new ReturnModel<LXF>();
            try
            {
                returnModel.ObjectModel = await LogisticIntegrationService.FindByType(lxftipo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<LXF>> GenerateToken(LXF lxf)
        {
            ReturnModel<LXF> returnModel = new ReturnModel<LXF>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await LogisticIntegrationService.GenerateNewToken(lxf,authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
