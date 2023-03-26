using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxPdv.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OryxPdv.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class TerminalController : ControllerBase
    {
        readonly TerminalService TerminalService;
        readonly ILogger Logger;
        public TerminalController(IConfiguration Configuration, ILogger<TerminalService> logger)
        {
            TerminalService = new TerminalService(Configuration);
            Logger = logger;
        }

        [HttpGet]
        public async Task<ReturnListModel<PD0>> FindList()
        {
            ReturnListModel<PD0> returnModel = new ReturnListModel<PD0>();
            try
            {
                returnModel.ObjectModel = await TerminalService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [Authorize]
        [HttpGet("{pd0codigo}")]
        public async Task<ReturnModel<PD0>> Find(string pd0codigo)
        {
            ReturnModel<PD0> returnModel = new ReturnModel<PD0>();
            try
            {
                returnModel.ObjectModel = await TerminalService.Find(pd0codigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [Authorize]
        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PD0 pd0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await TerminalService.Save(pd0, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "<br/>" + ex.InnerException.Message;
                }
                Logger.LogError("ERRO|MESSAGE: " + message);
                Logger.LogError("ERRO|STACKTRACE: " + ex.StackTrace);
            }
            return returnModel;
        }

        [Authorize]
        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PD0 pd0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await TerminalService.Save(pd0, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [Authorize]
        [HttpDelete("{pd0codigo}")]
        public async Task<ReturnModel<bool>> Delete(string pd0codigo)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await TerminalService.Delete(pd0codigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{pd0codigo=}")]
        public async Task<ReturnModel<PD0>> Reload(string pd0codigo)
        {
            ReturnModel<PD0> returnModel = new ReturnModel<PD0>();
            try
            {
                returnModel.ObjectModel = await TerminalService.Reload(pd0codigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [Authorize]
        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<PD0>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<PD0>> returnModel = new ReturnModel<SearchPayloadModel<PD0>>();
            try
            {
                returnModel.ObjectModel = await TerminalService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
