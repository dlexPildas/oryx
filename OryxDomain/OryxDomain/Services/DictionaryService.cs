using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OryxDomain.Http;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class DictionaryService
    {
        private readonly IConfiguration Configuration;
        private readonly APIParametersRepository APIParametersRepository;
        private readonly DictionaryRepository DictionaryRepository;
        public DictionaryService(IConfiguration configuration)
        {
            Configuration = configuration;
            APIParametersRepository = new APIParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<string> GetNextNumber(string field, string authorization, bool findLastUsed = true)
        {
            LXE lxe = await APIParametersRepository.Find();
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados.");

            Dictionary<string, string> queries = new Dictionary<string, string>()
            {
                { "findLastUsed",  findLastUsed.ToString() }
            };

            string response = await HttpUtilities.GetAsync(
                lxe.Lxebaseurl,
                "Dictionary/Dictionary/GetNextNumber/"+field,
                string.Empty,
                authorization,
                queries);

            ReturnModel<string> returnModel = JsonConvert.DeserializeObject<ReturnModel<string>>(response);

            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }

            return returnModel.ObjectModel;
        }

        public async Task<bool> ExistField(string field)
        {
            DC1 dc1 = await DictionaryRepository.FindDC1ByDc1campo(field);
            if (dc1 == null)
                return false;

            return true;
        }
    }
}
