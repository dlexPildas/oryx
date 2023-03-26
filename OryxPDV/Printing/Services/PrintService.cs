using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OryxDomain.Http;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Printing.Services
{
    public class PrintService
    {
        private IConfiguration Configuration;
        private readonly APIParametersRepository APIParametersRepository;

        public PrintService(IConfiguration configuration)
        {
            Configuration = configuration;
            APIParametersRepository = new APIParametersRepository(Configuration["OryxPath"]);
        }

        public bool Print(PrintModel model)
        {
            string pathEmpty = Configuration["OryxPath"] + "empty" + model.Relat + ".txt";
            File.Delete(pathEmpty);
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = Configuration["OryxPath"] + Configuration["ModuleOryx"];
                p.StartInfo.WorkingDirectory = Configuration["OryxPath"];
                p.StartInfo.Arguments = string.Format("print \"do form {0} with '{0}'\"  \"'{1}'\"", model.Relat, model.Params);
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.WaitForInputIdle();
                if (model.TimeOutCallOryx > 0)
                    p.WaitForExit(Convert.ToInt32(model.TimeOutCallOryx));
                else
                    p.WaitForExit(Convert.ToInt32(Configuration["TimeOutCallOryx"]));

                if (!p.HasExited)
                {
                    p.Kill();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //throw ex;
            }
            
            if (File.Exists(pathEmpty))
                throw new Exception("Não há dados para o relatório.");
            
            return true;
        }

        public async Task<Stream> FindDocument(string fileName, string authorization)
        {
            await ValidateGetDocument(authorization);

            if (fileName.EndsWith(".html"))
                fileName = "html\\" + fileName;

            string file = string.Format("{0}temp\\{1}", Configuration["OryxPath"], fileName);

            return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public async Task<Stream> ExportedFile(string pathFile, string authorization)
        {
            await ValidateGetDocument(authorization);

            string file = string.Format("{0}{1}", Configuration["OryxPath"], pathFile);

            return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        #region private methods
        private async Task ValidateGetDocument(string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
            {
                throw new Exception("Não autorizado");
            }

            LXE lxe = await APIParametersRepository.Find();
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");

            string response = await HttpUtilities.CallPostAsync(
                  lxe.Lxebaseurl
                , "Authentication/Authentication/Authenticated"
                , string.Empty
                , authorization);

            ReturnModel<DC4> returnModel = JsonConvert.DeserializeObject<ReturnModel<DC4>>(response);

            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }
        }
        #endregion
    }
}
