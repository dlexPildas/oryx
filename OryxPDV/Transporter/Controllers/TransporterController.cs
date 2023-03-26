using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.MelhorEnvio;
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
    public class TransporterController : ControllerBase
    {
        readonly TransporterService TransporterService;
        public TransporterController(IConfiguration Configuration)
        {
            TransporterService = new TransporterService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<CF7>> FindList()
        {
            ReturnListModel<CF7> returnModel = new ReturnListModel<CF7>();
            try
            {
                returnModel.ObjectModel = await TransporterService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{cf7transp}")]
        public async Task<ReturnModel<CF7>> Find(string cf7transp)
        {
            ReturnModel<CF7> returnModel = new ReturnModel<CF7>();
            try
            {
                returnModel.ObjectModel = await TransporterService.Find(cf7transp);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(CF7 cf7)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await TransporterService.Save(cf7, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(CF7 cf7)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await TransporterService.Save(cf7, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{cf7transp}")]
        public async Task<ReturnModel<bool>> Delete(string cf7transp)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await TransporterService.Delete(cf7transp);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<CF7>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<CF7>> returnModel = new ReturnModel<SearchPayloadModel<CF7>>();
            try
            {
                returnModel.ObjectModel = await TransporterService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{cf7codext}")]
        public async Task<ReturnListModel<TextValue>> FindPostageAgencies(string cf7codext)
        {
            ReturnListModel<TextValue> returnModel = new ReturnListModel<TextValue>();
            try
            {
                returnModel.ObjectModel = await TransporterService.FindPostageAgencies(cf7codext);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
    
}
