using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Services
{
    public class CollectionService
    {
        private readonly IConfiguration Configuration;
        private readonly CollectionRepository CollectionRepository;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        public CollectionService(IConfiguration configuration)
        {
            Configuration = configuration;
            CollectionRepository = new CollectionRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<CO0>> FindList()
        {
            IList<CO0> lstco0 = await CollectionRepository.FindList();

            return lstco0;
        }

        public async Task<CO0> Find(string co0codigo)
        {
            CO0 co0 = await CollectionRepository.Find(co0codigo);

            if (co0 == null)
            {
                throw new Exception(message: "Coleção não cadastrada.");
            }
            return co0;
        }

        public async Task<bool> Save(CO0 co0, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                co0.Co0colecao = await DictionaryService.GetNextNumber(nameof(co0.Co0colecao), authorization);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(co0);
                CO0 existsCo0 = await CollectionRepository.Find(co0.Co0colecao);
                if (existsCo0 != null)
                {
                    throw new Exception(message: "Coleção já cadastrada.");
                }

                affectedRows = await CollectionRepository.Insert(co0);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(co0.Co0colecao), co0.Co0colecao);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(co0);
                affectedRows = await CollectionRepository.Update(co0);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string co0colecao)
        {
            int affectedRows = await CollectionRepository.Delete(co0colecao);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<CO0>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<CO0> collections = await CollectionRepository.Search(search, limit, (page - 1) * limit);
            IList<CO0> nextCollections = await CollectionRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<CO0>()
            {
                Items = collections,
                Limit = limit,
                HasNext = nextCollections != null && nextCollections.Count > 0
            };
        }
    }
}
