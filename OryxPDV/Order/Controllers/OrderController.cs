using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Order.Models;
using Order.Services;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Order.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("[controller]/[Action]")]
    public class OrderController : ControllerBase
    {
        readonly OrderService OrderService;
        readonly OryxDomain.Services.LogServices LogServices;
        readonly ILogger Logger;

        public OrderController(IConfiguration Configuration, ILogger<OrderService> logger)
        {
            OrderService = new OrderService(Configuration, logger);
            LogServices = new OryxDomain.Services.LogServices(Configuration);
            Logger = logger;
        }

        [HttpGet("{vd1pedido}")]
        public async Task<ReturnModel<VD1>> Find(string vd1pedido)
        {
            ReturnModel<VD1> returnModel = new ReturnModel<VD1>();
            try
            {
                returnModel.ObjectModel = await OrderService.Find(vd1pedido);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{vd1pedido}")]
        public async Task<ReturnModel<SaveVd1Model>> Recover(string vd1pedido)
        {
            ReturnModel<SaveVd1Model> returnModel = new ReturnModel<SaveVd1Model>();
            try
            {
                returnModel.ObjectModel = await OrderService.Recover(vd1pedido);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<PayloadOrderModel>> Insert(SaveVd1Model model)
        {
            ReturnModel<PayloadOrderModel> returnModel = new ReturnModel<PayloadOrderModel>();
            string authorization = Request.Headers["Authorization"];

            string lx9acesso = User.FindFirst(ClaimTypes.Name)?.Value;

            try
            {
                ClaimsPrincipal claimPrincipal = HttpContext.User;

                string module = (from c in claimPrincipal.Identities.First().Claims
                                 where c.Type == "module"
                                 select c.Value).FirstOrDefault();
                OryxModuleType oryxModule = OryxModuleType.ORYX_GESTAO;
                oryxModule = (OryxModuleType)Enum.Parse(oryxModule.GetType(), module);

                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                 where c.Type == "terminal"
                                 select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await OrderService.Save(model, authorization, oryxModule, terminal, lx9acesso);
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

                await LogServices.Register(new LX8() 
                {
                    Lx8acao = "I",
                    Lx8acesso = lx9acesso,
                    Lx8arquivo = "VD1",
                    Lx8chave = model.Vd1.Vd1pedido,
                    Lx8datah = DateTime.Now,
                    Lx8usuario = model.Vd1.Vd1usuario,
                    Lx8info = string.Format("ERRO|MESSAGE: {0} \n ERRO|STACKTRACE: {1}", message, ex.StackTrace),
                });
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<PayloadOrderModel>> Update(SaveVd1Model model)
        {
            ReturnModel<PayloadOrderModel> returnModel = new ReturnModel<PayloadOrderModel>();
            string authorization = Request.Headers["Authorization"];

            string lx9acesso = User.FindFirst(ClaimTypes.Name)?.Value;
            try
            {
                ClaimsPrincipal claimPrincipal = HttpContext.User;

                var module = (from c in claimPrincipal.Identities.First().Claims
                              where c.Type == "module"
                              select c.Value).FirstOrDefault();

                OryxModuleType oryxModule = OryxModuleType.ORYX_GESTAO;
                oryxModule = (OryxModuleType)Enum.Parse(oryxModule.GetType(), module);

                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();


                returnModel.ObjectModel = await OrderService.Save(model, authorization, oryxModule, terminal, lx9acesso, true);
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

                await LogServices.Register(new LX8()
                {
                    Lx8acao = "A",
                    Lx8acesso = lx9acesso,
                    Lx8arquivo = "VD1",
                    Lx8chave = model.Vd1.Vd1pedido,
                    Lx8datah = DateTime.Now,
                    Lx8usuario = model.Vd1.Vd1usuario,
                    Lx8info = string.Format("ERRO|MESSAGE: {0} \n ERRO|STACKTRACE: {1}", message, ex.StackTrace),
                });
            }
            return returnModel;
        }

        [HttpDelete("{vd1pedido}")]
        public async Task<ReturnModel<bool>> Delete(string vd1pedido)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                string lx9acesso = User.FindFirst(ClaimTypes.Name)?.Value;
                returnModel.ObjectModel = await OrderService.Delete(vd1pedido, lx9acesso, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<SearchPayloadModel<VD1>>> Search(
            [FromQuery] string search,
            [FromQuery] int limit,
            [FromQuery] int page,
            [FromQuery] bool pending,
            [FromQuery] DateTime? since,
            [FromQuery] DateTime? until,
            [FromQuery] string orderBy
        )
        {
            ReturnModel<SearchPayloadModel<VD1>> returnModel = new ReturnModel<SearchPayloadModel<VD1>>();
            try
            {
                returnModel.ObjectModel = await OrderService.Search(search, limit, page, pending, since, until, orderBy);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        
        [HttpGet("{cf1cliente}")]
        public async Task<ReturnModel<VD1>> FindConsignedByCf1cliente(string cf1cliente)
        {
            ReturnModel<VD1> returnModel = new ReturnModel<VD1>();
            try
            {
                returnModel.ObjectModel = await OrderService.FindConsignedByCf1cliente(cf1cliente);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost("{cv5pedido}/{cv5embarq}")]
        public async Task<ReturnListModel<string>> Print(string cv5pedido, string cv5embarq)
        {
            ReturnListModel<string> returnModel = new ReturnListModel<string>();
            string authorization = Request.Headers["Authorization"];
            try
            {
                ClaimsPrincipal claimPrincipal = HttpContext.User;

                var module = (from c in claimPrincipal.Identities.First().Claims
                              where c.Type == "module"
                              select c.Value).FirstOrDefault();

                OryxModuleType oryxModule = OryxModuleType.ORYX_GESTAO;
                oryxModule = (OryxModuleType)Enum.Parse(oryxModule.GetType(), module);

                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await OrderService.Print(cv5pedido, cv5embarq, oryxModule, authorization, terminal);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<PayloadEmitNFModel>> EmitFiscalDocument(EmitNFVd1Model model)
        {
            ReturnModel<PayloadEmitNFModel> returnModel = new ReturnModel<PayloadEmitNFModel>();
            string authorization = Request.Headers["Authorization"];

            string lx9acesso = User.FindFirst(ClaimTypes.Name)?.Value;

            try
            {
                ClaimsPrincipal claimPrincipal = HttpContext.User;

                string module = (from c in claimPrincipal.Identities.First().Claims
                                 where c.Type == "module"
                                 select c.Value).FirstOrDefault();
                OryxModuleType oryxModule = OryxModuleType.ORYX_GESTAO;
                oryxModule = (OryxModuleType)Enum.Parse(oryxModule.GetType(), module);

                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await OrderService.EmitFiscalDocument(model, authorization, oryxModule, terminal);
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
                
                await LogServices.Register(new LX8()
                {
                    Lx8acao = "F",
                    Lx8acesso = lx9acesso,
                    Lx8arquivo = "VD1",
                    Lx8chave = model.Vd1pedido,
                    Lx8datah = DateTime.Now,
                    Lx8usuario = "",
                    Lx8info = string.Format("ERRO|MESSAGE: {0} \n ERRO|STACKTRACE: {1}", message, ex.StackTrace),
                });
            }
            return returnModel;
        }

        [HttpGet("{vd1pedido}")]
        public async Task<ReturnModel<bool>> HasFiscalDocument(string vd1pedido)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await OrderService.HasFiscalDocument(vd1pedido);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{vd1pedido}")]
        public async Task<ReturnListModel<CV5>> FindAllCv5(string vd1pedido)
        {
            ReturnListModel<CV5> returnModel = new ReturnListModel<CV5>();
            try
            {
                returnModel.ObjectModel = await OrderService.FindAllCv5(vd1pedido);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<CancelPayloadModel>> Cancel(CancelOrderModel model)
        {
            ReturnModel<CancelPayloadModel> returnModel = new ReturnModel<CancelPayloadModel>();
            try
            {
                string authorization = Request.Headers["Authorization"];
                string lx9acesso = User.FindFirst(ClaimTypes.Name)?.Value;
                returnModel.ObjectModel = await OrderService.Cancel(model.Vd1pedido, model.Reason, model.Resale, authorization, lx9acesso);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        
        [HttpPost]
        public async Task<ReturnModel<string>> PrintReportItensByOrder(PrintReportItensByOrder model)
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            try
            {
                string authorization = Request.Headers["Authorization"];

                ClaimsPrincipal claimPrincipal = HttpContext.User;
                string terminal = (from c in claimPrincipal.Identities.First().Claims
                                   where c.Type == "terminal"
                                   select c.Value).FirstOrDefault();

                returnModel.ObjectModel = await OrderService.PrintReportItensByOrder(model, terminal, false, authorization);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet]
        public async Task<ReturnModel<string>> GetNextNumber()
        {
            ReturnModel<string> returnModel = new ReturnModel<string>();
            try
            {
                returnModel.ObjectModel = await OrderService.GetNextNumber();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
