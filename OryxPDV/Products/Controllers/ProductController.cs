using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    public class ProductController : ControllerBase
    {
        readonly ProductsService productsService;
        readonly ILogger Logger;
        public ProductController(IConfiguration Configuration, ILogger<ProductController> logger)
        {
            productsService = new ProductsService(Configuration);
            Logger = logger;
        }

        [HttpPost]
        public async Task<ReturnModel<ProductCartModel>> FindByCode(FindByCodeModel model)
        {
            ReturnModel<ProductCartModel> returnModel = new ReturnModel<ProductCartModel>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await productsService.FindByCode(model.Product, model.List, model.Volume, model.Pedido, model.LstVd8);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "<br/>" + ex.InnerException.Message;
                }
                Logger.LogError("ERRO|MESSAGE: " + message);
                Logger.LogError("ERRO|STACKTRACE: " + ex.StackTrace);
            }
            return returnModel;
        }


        [HttpGet("{code}/{list=}")]
        public async Task<ReturnModel<ProductCartModel>> Find(string code, string list)
        {
            ReturnModel<ProductCartModel> returnModel = new ReturnModel<ProductCartModel>();
            string authorization = Request.Headers["Authorization"];
            returnModel.ObjectModel = await productsService.Find(code, list, authorization);
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<string>> FindEan([FromQuery]string pr0produto, [FromQuery] string pr2opcao, [FromQuery] string pr3tamanho)
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            returnModel.ObjectModel = await productsService.FindEan(pr0produto, pr2opcao, pr3tamanho);
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<ReturnItemModel>> FindReturnByCode(FindReturnModel model)
        {
            ReturnModel<ReturnItemModel> returnModel = new ReturnModel<ReturnItemModel>();
            try
            {
                returnModel.ObjectModel = await productsService.FindReturnByCode(model.Product, model.LstItems, model.Cf1cliente, model.Consigned, model.Qty, model.Input, model.Option, model.Size);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "<br/>" + ex.InnerException.Message;
                }
                Logger.LogError("ERRO|MESSAGE: " + message);
                Logger.LogError("ERRO|STACKTRACE: " + ex.StackTrace);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnListModel<ProductCartModel>> FindForPriceQuery([FromQuery] string text, [FromQuery] string orderBy)
        {
            ReturnListModel<ProductCartModel> returnModel = new ReturnListModel<ProductCartModel>();
            try
            {
                returnModel.ObjectModel = await productsService.FindForPriceQuery(text, orderBy);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<bool>> FindByAlterCode([FromQuery] string pr0produto, [FromQuery] string pr0refer)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await productsService.FindByAlterCode(pr0produto, pr0refer);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PR0 pr0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await productsService.Save(pr0, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }


        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PR0 pr0)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                returnModel.ObjectModel = await productsService.Save(pr0, authorization,true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
