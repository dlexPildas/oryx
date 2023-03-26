using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OryxDomain.Http;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class PrintService
    {
        private readonly APIParametersRepository APIParametersRepository;
        public PrintService(IConfiguration configuration)
        {
            APIParametersRepository = new APIParametersRepository(configuration["OryxPath"] + "Oryx.ini");
        }
        public async Task Print(PrintModel printModel, string authorization)
        {
            LXE lxe = await APIParametersRepository.Find();

            string response = await HttpUtilities.CallPostAsync(
                lxe.Lxebaseurl,
                "Printing/Print/Print",
                JsonConvert.SerializeObject(printModel),
                authorization
            );
            ReturnModel<bool> returnModel = JsonConvert.DeserializeObject<ReturnModel<bool>>(response);

            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }
        }
    }
}
