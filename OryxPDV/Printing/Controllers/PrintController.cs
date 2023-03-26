using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using Printing.Services;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Printing.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Route("[controller]/[Action]")]
    public class PrintController : ControllerBase
    {
        readonly PrintService PrintService;
        private readonly IConfiguration Configuration;
        public PrintController(IConfiguration Configuration)
        {
            PrintService = new PrintService(Configuration);
            this.Configuration = Configuration;
        }

        [Authorize]
        [HttpPost]
        public ReturnModel<bool> Print(PrintModel model)
        {
            ReturnModel<bool> returnModel = new ReturnModel<bool>();
            try
            {
                returnModel.ObjectModel = PrintService.Print(model);
            }
            catch (Exception ex)
            {
                returnModel.SetError(ex);
            }

            return returnModel;
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> Document(string fileName, string authorization)
        {
            string extension = "*";
            try
            {
                Stream stream = await PrintService.FindDocument(fileName, authorization);
                if (fileName.ToUpper().EndsWith(".HTML"))
                    extension = "html";
                if (fileName.ToUpper().EndsWith(".PDF"))
                    extension = "pdf";

                return new FileStreamResult(stream, "application/"+ extension);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportedFile(string pathFile, string authorization)
        {
            try
            {
                Stream stream = await PrintService.ExportedFile(pathFile, authorization);

                return new FileStreamResult(stream, "text/plain");
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Content(ex.Message);
            }
        }
    }
}
