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
    public class FamilyController : ControllerBase
    {
        readonly FamilyService FamilyService;
        public FamilyController(IConfiguration Configuration)
        {
            FamilyService = new FamilyService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<PRB>> FindList()
        {
            ReturnListModel<PRB> returnModel = new ReturnListModel<PRB>();
            try
            {
                returnModel.ObjectModel = await FamilyService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{prbfamilia}")]
        public async Task<ReturnModel<PRB>> Find(string prbfamilia)
        {
            ReturnModel<PRB> returnModel = new ReturnModel<PRB>();
            try
            {
                returnModel.ObjectModel = await FamilyService.Find(prbfamilia);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PRB prb)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await FamilyService.Save(prb, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PRB prb)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await FamilyService.Save(prb, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{prbfamilia}")]
        public async Task<ReturnModel<bool>> Delete(string prbfamilia)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await FamilyService.Delete(prbfamilia);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<PRB>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<PRB>> returnModel = new ReturnModel<SearchPayloadModel<PRB>>();
            try
            {
                returnModel.ObjectModel = await FamilyService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    
    }
}
