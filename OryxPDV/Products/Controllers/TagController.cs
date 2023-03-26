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
    public class TagController : ControllerBase
    {
        readonly TagService TagService;
        public TagController(IConfiguration Configuration)
        {
            TagService = new TagService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<ET0>> FindList()
        {
            ReturnListModel<ET0> returnModel = new ReturnListModel<ET0>();
            try
            {
                returnModel.ObjectModel = await TagService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{et0etiq}")]
        public async Task<ReturnModel<ET0>> Find(string et0etiq)
        {
            ReturnModel<ET0> returnModel = new ReturnModel<ET0>();
            try
            {
                returnModel.ObjectModel = await TagService.Find(et0etiq);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(ET0 et0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await TagService.Save(et0, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(ET0 et0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await TagService.Save(et0, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{et0etiq}")]
        public async Task<ReturnModel<bool>> Delete(string et0etiq)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await TagService.Delete(et0etiq);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<ET0>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<ET0>> returnModel = new ReturnModel<SearchPayloadModel<ET0>>();
            try
            {
                returnModel.ObjectModel = await TagService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
