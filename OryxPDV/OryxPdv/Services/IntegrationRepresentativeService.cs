using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OryxPdv.Services
{
    public class IntegrationRepresentativeService
    {
        private readonly IConfiguration Configuration;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        private readonly IntegrationRepresentativeRepository IntegrationRepresentativeRepository;

        public IntegrationRepresentativeService(IConfiguration configuration)
        {
            Configuration = configuration;
            DictionaryService = new DictionaryService(Configuration);
            IntegrationRepresentativeRepository = new IntegrationRepresentativeRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<PD8> Find(string pd8codigo)
        {
            PD8 pd8 = await IntegrationRepresentativeRepository.Find(pd8codigo);

            if (pd8 == null)
                throw new Exception(message: "Parâmetro de Integração de guias não encontrado.");
            
            return pd8;
        }

        public async Task<IList<PD8>> FindAll()
        {
            IList<PD8> lstPd8 = await IntegrationRepresentativeRepository.FindAll();
            return lstPd8;
        }


        public async Task<bool> Save(PD8 pd8, string authorization, bool forUpdate = false)
        {
            int affectedRows;

            if (!forUpdate)
            {
                pd8.Pd8codigo = await DictionaryService.GetNextNumber(nameof(pd8.Pd8codigo), authorization);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(pd8);
                PD8 existsPd8 = await IntegrationRepresentativeRepository.Find(pd8.Pd8codigo);
                if (existsPd8 != null)
                {
                    throw new Exception(message: "Parâmetro de integração de guias já cadastrado.");
                }

                affectedRows = await IntegrationRepresentativeRepository.Insert(pd8);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(pd8.Pd8codigo), pd8.Pd8codigo);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(pd8);
                affectedRows = await IntegrationRepresentativeRepository.Update(pd8);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string pd8codigo)
        {
            int affectedRows = await IntegrationRepresentativeRepository.Delete(pd8codigo);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<PD8>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<PD8> parmas = await IntegrationRepresentativeRepository.Search(search, limit, (page - 1) * limit);
            IList<PD8> nextParams = await IntegrationRepresentativeRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<PD8>()
            {
                Items = parmas,
                Limit = limit,
                HasNext = nextParams != null && nextParams.Count > 0
            };
        }
    }
}
