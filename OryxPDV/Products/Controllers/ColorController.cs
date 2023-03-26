using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using Products.Models;
using Products.Services;
using System;
using System.Threading.Tasks;

namespace Products.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[Action]")]
    public class ColorController : ControllerBase
    {
        readonly ColorService ColorService;
        public ColorController(IConfiguration Configuration)
        {
            ColorService = new ColorService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<CR1>> FindList()
        {
            ReturnListModel<CR1> returnModel = new ReturnListModel<CR1>();
            try
            {
                returnModel.ObjectModel = await ColorService.FindList();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{cr1cor}")]
        public async Task<ReturnModel<CR1>> Find(string cr1cor)
        {
            ReturnModel<CR1> returnModel = new ReturnModel<CR1>();
            try
            {
                returnModel.ObjectModel = await ColorService.Find(cr1cor);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(CR1 cr1)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await ColorService.Save(cr1, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(CR1 cr1)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await ColorService.Save(cr1, authorization, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{cr1cor}")]
        public async Task<ReturnModel<bool>> Delete(string cr1cor)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await ColorService.Delete(cr1cor);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<CR1>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<CR1>> returnModel = new ReturnModel<SearchPayloadModel<CR1>>();
            try
            {
                returnModel.ObjectModel = await ColorService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }


        [HttpPost]
        public ReturnModel<GetArgbResponseModel> GetArgbColor(GetArgbModel model)
        {
            ReturnModel<GetArgbResponseModel> returnModel = new ReturnModel<GetArgbResponseModel>();
            try
            {
                returnModel.ObjectModel = ColorService.GetArgb(model);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
