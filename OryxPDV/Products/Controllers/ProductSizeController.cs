using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using Products.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Products.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[Action]")]
    public class ProductSizeController : ControllerBase
    {
        readonly ProductSizeService ProductSizeService;
        public ProductSizeController(IConfiguration Configuration)
        {
            ProductSizeService = new ProductSizeService(Configuration);
        }

        [HttpGet("{pr3produto}")]
        public async Task<ReturnListModel<PR3>> FindList(string pr3produto)
        {
            ReturnListModel<PR3> returnModel = new ReturnListModel<PR3>();
            try
            {
                returnModel.ObjectModel = await ProductSizeService.FindList(pr3produto);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<PR3>> Find([FromQuery] string pr3produto, [FromQuery] string pr3tamanho)
        {
            ReturnModel<PR3> returnModel = new ReturnModel<PR3>();
            try
            {
                returnModel.ObjectModel = await ProductSizeService.Find(pr3produto, pr3tamanho);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PR3 pr3)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await ProductSizeService.Save(pr3);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PR3 pr3)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await ProductSizeService.Save(pr3, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete]
        public async Task<ReturnModel<bool>> Delete([FromQuery] string pr3produto, [FromQuery] string pr3tamanho)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                string lx9acesso = User.FindFirst(ClaimTypes.Name)?.Value;

                returnModel.ObjectModel = await ProductSizeService.Delete(pr3produto, pr3tamanho,authorization,lx9acesso);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<PR3>>> Search([FromQuery] string search, [FromQuery] int limit, [FromQuery] int page)
        {
            ReturnModel<SearchPayloadModel<PR3>> returnModel = new ReturnModel<SearchPayloadModel<PR3>>();
            try
            {
                returnModel.ObjectModel = await ProductSizeService.Search(search, limit, page);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
