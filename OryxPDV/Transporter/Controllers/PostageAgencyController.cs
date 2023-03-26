using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using System;
using System.Threading.Tasks;
using Transporter.Services;

namespace Transporter.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class PostageAgencyController : ControllerBase
    {
        readonly PostageAgencyService PostageAgencyService;
        public PostageAgencyController(IConfiguration Configuration)
        {
            PostageAgencyService = new PostageAgencyService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<LXG>> FindList()
        {
            ReturnListModel<LXG> returnModel = new ReturnListModel<LXG>();
            try
            {
                returnModel.ObjectModel = await PostageAgencyService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{lxgtransp}")]
        public async Task<ReturnModel<LXG>> Find(string lxgtransp)
        {
            ReturnModel<LXG> returnModel = new ReturnModel<LXG>();
            try
            {
                returnModel.ObjectModel = await PostageAgencyService.Find(lxgtransp);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(LXG lxg)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await PostageAgencyService.Save(lxg, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(LXG lxg)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await PostageAgencyService.Save(lxg, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{lxgtransp}")]
        public async Task<ReturnModel<bool>> Delete(string lxgtransp)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await PostageAgencyService.Delete(lxgtransp);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<LXG>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<LXG>> returnModel = new ReturnModel<SearchPayloadModel<LXG>>();
            try
            {
                returnModel.ObjectModel = await PostageAgencyService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
