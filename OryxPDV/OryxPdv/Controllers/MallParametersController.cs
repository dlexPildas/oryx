using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
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
    public class MallParametersController : ControllerBase
    {
        readonly MallParametersService MallParametersService;
        public MallParametersController(IConfiguration Configuration)
        {
            MallParametersService = new MallParametersService(Configuration);
        }

        [HttpGet]
        public async Task<ReturnListModel<PD5>> FindAll()
        {
            ReturnListModel<PD5> returnModel = new ReturnListModel<PD5>();
            try
            {
                returnModel.ObjectModel = await MallParametersService.FindAll();
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpGet("{pd5codigo}")]
        public async Task<ReturnModel<PD5>> Find(int pd5codigo)
        {
            ReturnModel<PD5> returnModel = new ReturnModel<PD5>();
            try
            {
                returnModel.ObjectModel = await MallParametersService.Find(pd5codigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPost]
        public async Task<ReturnModel<bool>> Insert(PD5 pd5)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await MallParametersService.Save(pd5);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpPut]
        public async Task<ReturnModel<bool>> Update(PD5 pd5)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await MallParametersService.Save(pd5, true);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }

        [HttpDelete("{pd5codigo}")]
        public async Task<ReturnModel<bool>> Delete(int pd5codigo)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = await MallParametersService.Delete(pd5codigo);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
