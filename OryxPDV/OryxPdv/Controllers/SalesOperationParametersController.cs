using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxPdv.Services;
using System;
using System.Threading.Tasks;

namespace OryxPdv.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class SalesOperationParametersController : ControllerBase
    {
        readonly SalesOperationParametersService SalesOperationParametersService;
        public SalesOperationParametersController(IConfiguration Configuration)
        {
            SalesOperationParametersService = new SalesOperationParametersService(Configuration);
        }

        [HttpGet("{pd2codigo}")]
        public async Task<ReturnModel<PD2>> Find(int pd2codigo)
        {
            ReturnModel<PD2> returnModel = new ReturnModel<PD2>();
            try
            {
                returnModel.ObjectModel = await SalesOperationParametersService.Find(pd2codigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnListModel<PD2>> FindAll()
        {
            ReturnListModel<PD2> returnModel = new ReturnListModel<PD2>();
            try
            {
                returnModel.ObjectModel = await SalesOperationParametersService.FindAll();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<string>> FindOperCom(SalesOperationType pd2tipoope, string pd2tipo, OperationType pd2estadua, bool pd2contrib, bool pd2emispro, string cv4estado)
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            try
            {
                returnModel.ObjectModel = await SalesOperationParametersService.FindOperCom(pd2tipoope, pd2tipo, pd2estadua, pd2contrib, pd2emispro, cv4estado);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PD2 pd2)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await SalesOperationParametersService.Save(pd2);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PD2 pd2)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await SalesOperationParametersService.Save(pd2, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{pd2codigo}")]
        public async Task<ReturnModel<bool>> Delete(int pd2codigo)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await SalesOperationParametersService.Delete(pd2codigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<PD2>>> Search(
            [FromQuery] int? pd2codigo,
            [FromQuery]SalesOperationType? pd2tipoope,
            [FromQuery] string pd2tipo,
            [FromQuery] OperationType? pd2estadua,
            [FromQuery] bool? pd2contrib,
            [FromQuery] string pd2opercom,
            [FromQuery] bool? pd2emispro,
            [FromQuery] int limit,
            [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<PD2>> returnModel = new ReturnModel<SearchPayloadModel<PD2>>();
            try
            {
                returnModel.ObjectModel = await SalesOperationParametersService.Search(pd2codigo, pd2tipoope, pd2tipo, pd2estadua, pd2contrib, pd2opercom, pd2emispro, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
