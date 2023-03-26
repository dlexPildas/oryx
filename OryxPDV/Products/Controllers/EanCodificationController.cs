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
    public class EanCodificationController : ControllerBase
    {
        readonly EanCodificationService EanCodificationService;
        public EanCodificationController(IConfiguration Configuration)
        {
            EanCodificationService = new EanCodificationService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<EAN>> FindList()
        {
            ReturnListModel<EAN> returnModel = new ReturnListModel<EAN>();
            try
            {
                returnModel.ObjectModel = await EanCodificationService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{eancodigo}")]
        public async Task<ReturnModel<EAN>> Find(string eancodigo)
        {
            ReturnModel<EAN> returnModel = new ReturnModel<EAN>();
            try
            {
                returnModel.ObjectModel = await EanCodificationService.Find(eancodigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(EAN ean)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await EanCodificationService.Save(ean, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(EAN ean)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await EanCodificationService.Save(ean, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{eancodigo}")]
        public async Task<ReturnModel<bool>> Delete(string eancodigo)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await EanCodificationService.Delete(eancodigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<EAN>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<EAN>> returnModel = new ReturnModel<SearchPayloadModel<EAN>>();
            try
            {
                returnModel.ObjectModel = await EanCodificationService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
