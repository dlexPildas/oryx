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
    public class GroupController : ControllerBase
    {
        readonly GroupService GroupService;
        public GroupController(IConfiguration Configuration)
        {
            GroupService = new GroupService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<PRS>> FindList()
        {
            ReturnListModel<PRS> returnModel = new ReturnListModel<PRS>();
            try
            {
                returnModel.ObjectModel = await GroupService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{prsgrupo}")]
        public async Task<ReturnModel<PRS>> Find(string prsgrupo)
        {
            ReturnModel<PRS> returnModel = new ReturnModel<PRS>();
            try
            {
                returnModel.ObjectModel = await GroupService.Find(prsgrupo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PRS prs)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await GroupService.Save(prs, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PRS prs)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await GroupService.Save(prs, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{prsgrupo}")]
        public async Task<ReturnModel<bool>> Delete(string prsgrupo)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await GroupService.Delete(prsgrupo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<PRS>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<PRS>> returnModel = new ReturnModel<SearchPayloadModel<PRS>>();
            try
            {
                returnModel.ObjectModel = await GroupService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
