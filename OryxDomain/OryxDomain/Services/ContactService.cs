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
    public class ContactService
    {
        private readonly IConfiguration Configuration;
        private readonly APIParametersRepository APIParametersRepository;
        public ContactService(IConfiguration configuration)
        {
            Configuration = configuration;
            APIParametersRepository = new APIParametersRepository(Configuration["OryxPath"] + "oryx.ini");

        }

        public async Task<string> FindEmailDocFis(string cf1cliente, string authorization)
        {
            LXE lxe = await APIParametersRepository.Find();
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");

            string jsonReponse = await HttpUtilities.GetAsync(
                  lxe.Lxebaseurl
                , "/Customer/Contact/FindEmailDocFis"
                , cf1cliente
                , authorization);

            ReturnModel<string> returnModel = JsonConvert.DeserializeObject<ReturnModel<string>>(jsonReponse);

            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }

            return returnModel.ObjectModel;
        }
    }
}
