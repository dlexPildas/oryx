using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using Products.Models;
using Products.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[Action]")]
    public class SpecificTaxesController : ControllerBase
    {
        readonly SpecificTaxesService SpecificTaxesService;
        public SpecificTaxesController(IConfiguration Configuration)
        {
            SpecificTaxesService = new SpecificTaxesService(Configuration);
        }

        [HttpGet("{cvnproduto}")]
        public async Task<ReturnListModel<CVN>> FindList(string cvnproduto)
        {
            ReturnListModel<CVN> returnModel = new ReturnListModel<CVN>();
            try
            {
                returnModel.ObjectModel = await SpecificTaxesService.FindList(cvnproduto);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<CVN>> Find([FromQuery] string cvnopercom, [FromQuery] string cvnproduto, [FromQuery] string cvninsumo)
        {
            ReturnModel<CVN> returnModel = new ReturnModel<CVN>();
            try
            {
                returnModel.ObjectModel = await SpecificTaxesService.Find(cvnopercom,cvnproduto,cvninsumo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(CVN cvn)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await SpecificTaxesService.Save(cvn);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(CVN cvn)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await SpecificTaxesService.Save(cvn, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete()]
        public async Task<ReturnModel<bool>> Delete([FromQuery] string cvnopercom, [FromQuery] string cvnproduto, [FromQuery] string cvninsumo)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await SpecificTaxesService.Delete(cvnopercom,cvnproduto,cvninsumo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<CVN>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<CVN>> returnModel = new ReturnModel<SearchPayloadModel<CVN>>();
            try
            {
                returnModel.ObjectModel = await SpecificTaxesService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{cvnopercom}")]
        public async Task<ReturnModel<IList<GenericItemModel>>> FindGenericProducts(string cvnopercom)
        {
            ReturnModel<IList<GenericItemModel>> returnModel = new ReturnModel<IList<GenericItemModel>>();
            try
            {
                returnModel.ObjectModel = await SpecificTaxesService.GetGenericItens(cvnopercom);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> RepeatSpecificTaxe(RepeatSpecificTaxeModel repeatspecifictaxe)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await SpecificTaxesService.RepeatSpecificTaxe(repeatspecifictaxe);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
