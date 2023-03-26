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
    public class CollectionController : ControllerBase
    {
        readonly CollectionService CollectionService;
        public CollectionController(IConfiguration Configuration)
        {
            CollectionService = new CollectionService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<CO0>> FindList()
        {
            ReturnListModel<CO0> returnModel = new ReturnListModel<CO0>();
            try
            {
                returnModel.ObjectModel = await CollectionService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{co0colecao}")]
        public async Task<ReturnModel<CO0>> Find(string co0colecao)
        {
            ReturnModel<CO0> returnModel = new ReturnModel<CO0>();
            try
            {
                returnModel.ObjectModel = await CollectionService.Find(co0colecao);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(CO0 co0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await CollectionService.Save(co0, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(CO0 co0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await CollectionService.Save(co0, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{co0colecao}")]
        public async Task<ReturnModel<bool>> Delete(string co0colecao)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await CollectionService.Delete(co0colecao);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<CO0>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<CO0>> returnModel = new ReturnModel<SearchPayloadModel<CO0>>();
            try
            {
                returnModel.ObjectModel = await CollectionService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
