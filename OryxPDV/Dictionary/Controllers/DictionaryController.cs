using Dictionary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dictionary.Controllers
{
    //[Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class DictionaryController : ControllerBase
    {
        readonly DictionaryService DictionaryService;
        readonly ILogger Logger;
        public DictionaryController(IConfiguration Configuration, ILogger<DictionaryService> logger)
        {
            DictionaryService = new DictionaryService(Configuration);
            Logger = logger;
        }

        [HttpGet("{dc1arquivo}")]
        public async Task<ReturnListModel<DC1>> FindDC1ByDc1arquivo(string dc1arquivo)
        {
            ReturnListModel<DC1> returnModel = new ReturnListModel<DC1>();
            try
            {
                returnModel.ObjectModel = await DictionaryService.FindDC1ByDc1arquivo(dc1arquivo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{dc0arquivo}")]
        public async Task<ReturnModel<DC0>> FindDC0ByDc0arquivo(string dc0arquivo)
        {
            ReturnModel<DC0> returnModel = new ReturnModel<DC0>();
            try
            {
                returnModel.ObjectModel = await DictionaryService.FindDC0ByDc0arquivo(dc0arquivo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnListModel<DC1>> FindDC1ByLstDc1arquivo([FromQuery]IList<string> dc1arquivo)
        {
            ReturnListModel<DC1> returnModel = new ReturnListModel<DC1>();
            try
            {
                returnModel.ObjectModel = await DictionaryService.FindDC1ByLstDc1arquivo(dc1arquivo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnListModel<DC0>> FindDC0ByLstDc0arquivo([FromQuery]IList<string> dc0arquivo)
        {
            ReturnListModel<DC0> returnModel = new ReturnListModel<DC0>();
            try
            {
                returnModel.ObjectModel = await DictionaryService.FindDC0ByLstDc0arquivo(dc0arquivo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{field}")]
        public async Task<ReturnModel<string>> GetNextNumber(string field, bool findLastUsed = false)
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            try
            {
                returnModel.ObjectModel = await DictionaryService.GetNextNumber(field, findLastUsed);
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
    }
}
