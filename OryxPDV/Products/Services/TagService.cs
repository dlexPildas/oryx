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
    public class TagService
    {
        private readonly IConfiguration Configuration;
        private readonly TagRepository TagRepository;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        public TagService(IConfiguration configuration)
        {
            Configuration = configuration;
            TagRepository = new TagRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<ET0>> FindList()
        {
            IList<ET0> lstet0 = await TagRepository.FindList();

            return lstet0;
        }

        public async Task<ET0> Find(string et0etiq)
        {
            ET0 et0 = await TagRepository.Find(et0etiq);

            if (et0 == null)
            {
                throw new Exception(message: "Etiqueta não cadastrada.");
            }
            return et0;
        }

        public async Task<bool> Save(ET0 et0, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                et0.Et0etiq = await DictionaryService.GetNextNumber(nameof(et0.Et0etiq), authorization);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(et0);
                ET0 existset0 = await TagRepository.Find(et0.Et0etiq);
                if (existset0 != null)
                {
                    throw new Exception(message: "Etiqueta já cadastrada.");
                }

                affectedRows = await TagRepository.Insert(et0);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(et0.Et0etiq), et0.Et0etiq);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(et0);
                affectedRows = await TagRepository.Update(et0);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string et0etiq)
        {
            int affectedRows = await TagRepository.Delete(et0etiq);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<ET0>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<ET0> tags = await TagRepository.Search(search, limit, (page - 1) * limit);
            IList<ET0> nextTags = await TagRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<ET0>()
            {
                Items = tags,
                Limit = limit,
                HasNext = nextTags != null && nextTags.Count > 0
            };
        }
    }
}
