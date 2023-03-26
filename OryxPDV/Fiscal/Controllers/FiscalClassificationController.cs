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
    public class FiscalClassificationController : ControllerBase
    {
        readonly FiscalClassificationService FiscalClassificationService;
        public FiscalClassificationController(IConfiguration Configuration)
        {
            FiscalClassificationService = new FiscalClassificationService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<FI0>> FindList()
        {
            ReturnListModel<FI0> returnModel = new ReturnListModel<FI0>();
            try
            {
                returnModel.ObjectModel = await FiscalClassificationService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{fi0id}")]
        public async Task<ReturnModel<FI0>> Find(string fi0id)
        {
            ReturnModel<FI0> returnModel = new ReturnModel<FI0>();
            try
            {
                returnModel.ObjectModel = await FiscalClassificationService.Find(fi0id);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(FI0 fi0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await FiscalClassificationService.Save(fi0, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(FI0 fi0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await FiscalClassificationService.Save(fi0, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{fi0id}")]
        public async Task<ReturnModel<bool>> Delete(string fi0id)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await FiscalClassificationService.Delete(fi0id);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<FI0>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<FI0>> returnModel = new ReturnModel<SearchPayloadModel<FI0>>();
            try
            {
                returnModel.ObjectModel = await FiscalClassificationService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
