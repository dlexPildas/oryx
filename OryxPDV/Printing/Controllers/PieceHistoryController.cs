using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using Printing.Models;
using Printing.Services;
using System;
using System.Threading.Tasks;

namespace Printing.Controllers
{
    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Route("[controller]/[Action]")]
    public class PieceHistoryController : ControllerBase
    {
        readonly PieceHistoryService PieceHistoryService;
        private readonly IConfiguration Configuration;
        public PieceHistoryController(IConfiguration Configuration)
        {
            PieceHistoryService = new PieceHistoryService(Configuration);
            this.Configuration = Configuration;
        }

        [HttpGet("{of3peca}")]
        public async Task<ReturnModel<PieceHistoryModel>> Get(string of3peca)
        {
            ReturnModel<PieceHistoryModel> returnModel = new ReturnModel<PieceHistoryModel>();
            try
            {
                returnModel.ObjectModel = await PieceHistoryService.GetPieceHistory(of3peca);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }
            return returnModel;
        }
    }
}
