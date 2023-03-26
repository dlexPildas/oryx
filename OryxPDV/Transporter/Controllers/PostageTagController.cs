using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transporter.Services;

namespace Transporter.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class PostageTagController : ControllerBase
    {
        readonly PostageTagService PostageTagService;
        public PostageTagController(IConfiguration Configuration)
        {
            PostageTagService = new PostageTagService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<ET3>> FindList()
        {
            ReturnListModel<ET3> returnModel = new ReturnListModel<ET3>();
            try
            {
                returnModel.ObjectModel = await PostageTagService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{et3etiquet}")]
        public async Task<ReturnModel<ET3>> Find(string et3etiquet)
        {
            ReturnModel<ET3> returnModel = new ReturnModel<ET3>();
            try
            {
                returnModel.ObjectModel = await PostageTagService.Find(et3etiquet);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(ET3 et3)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await PostageTagService.Save(et3, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(ET3 et3)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await PostageTagService.Save(et3, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{et3etiquet}")]
        public async Task<ReturnModel<bool>> Delete(string et3etiquet)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await PostageTagService.Delete(et3etiquet);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<ET3>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page, 
            [FromQuery] IList<PostageTagStatusType> statuslst)
        {
            ReturnModel<SearchPayloadModel<ET3>> returnModel = new ReturnModel<SearchPayloadModel<ET3>>();
            try
            {
                returnModel.ObjectModel = await PostageTagService.Search(search, limit, page, statuslst);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
        [HttpPut]
        public async Task UpdateStatus([FromBody]IList<string> idlst)
        {
            string authorization = Request.Headers["Authorization"];

            await PostageTagService.UpdateStatus(idlst,authorization);
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> SendToCart([FromBody]IList<string> etiquetlst)
        {
            string authorization = Request.Headers["Authorization"];
            ReturnModel<bool> returnModel = new ReturnModel<bool>();

            returnModel.ObjectModel = await PostageTagService.SendToCart(etiquetlst,authorization);
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Generate([FromBody] IList<string> etiquetlst)
        {
            string authorization = Request.Headers["Authorization"];
            ReturnModel<bool> returnModel = new ReturnModel<bool>();

            returnModel.ObjectModel = await PostageTagService.SendToCart(etiquetlst, authorization);
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnListModel<string>> Print([FromBody] IList<string> etiquetlst)
        {
            string authorization = Request.Headers["Authorization"];

            ReturnListModel<string> returnModel = new ReturnListModel<string>();

            try
            {
                returnModel.ObjectModel = await PostageTagService.Print(etiquetlst, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;

        }

        public async Task<ReturnModel<bool>> Cancel([FromQuery] string et3etiq)
        {
            string authorization = Request.Headers["Authorization"];

            ReturnModel<bool> returnModel = new ReturnModel<bool>();

            try
            {
                returnModel.ObjectModel = await PostageTagService.Cancel(et3etiq, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;

        }
    }
}
