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
    public class ColorOptionsController : ControllerBase
    {
        readonly ColorOptionsService ColorOptionsService;
        public ColorOptionsController(IConfiguration Configuration)
        {
            ColorOptionsService = new ColorOptionsService(Configuration);
        }

        //[HttpGet]
        //public async Task<ReturnListModel<PR2>> FindList()
        //{
        //    ReturnListModel<PR2> returnModel = new ReturnListModel<PR2>();
        //    try
        //    {
        //        returnModel.ObjectModel = await ColorOptionsService.FindList();
        //    }
        //    catch (Exception ex)
        //    {
        //        returnModel.SetError(ex);
        //    }
        //    return returnModel;
        //}

        [HttpGet("{pr2produto}")]
        public async Task<ReturnListModel<PR2>> FindList(string pr2produto)
        {
            ReturnListModel<PR2> returnModel = new ReturnListModel<PR2>();
            try
            {
                returnModel.ObjectModel = await ColorOptionsService.FindList(pr2produto);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<PR2>> Find([FromQuery] string pr2produto, [FromQuery] string pr2opcao)
        {
            ReturnModel<PR2> returnModel = new ReturnModel<PR2>();
            try
            {
                returnModel.ObjectModel = await ColorOptionsService.Find(pr2produto,pr2opcao);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PR2 pr2)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await ColorOptionsService.Save(pr2);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PR2 pr2)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await ColorOptionsService.Save(pr2, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete()]
        public async Task<ReturnModel<bool>> Delete([FromQuery] string pr2produto, [FromQuery] string pr2opcao)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await ColorOptionsService.Delete(pr2produto, pr2opcao);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<PR2>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<PR2>> returnModel = new ReturnModel<SearchPayloadModel<PR2>>();
            try
            {
                returnModel.ObjectModel = await ColorOptionsService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
