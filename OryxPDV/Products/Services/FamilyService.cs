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
    public class FamilyService
    {
        private readonly IConfiguration Configuration;
        private readonly FamilyRepository FamilyRepository;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        public FamilyService(IConfiguration configuration)
        {
            Configuration = configuration;
            FamilyRepository = new FamilyRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<PRB>> FindList()
        {
            IList<PRB> lstprb = await FamilyRepository.FindList();

            return lstprb;
        }

        public async Task<PRB> Find(string prbfamilia)
        {
            PRB prb = await FamilyRepository.Find(prbfamilia);

            if (prb == null)
            {
                throw new Exception(message: "Familia não cadastrada.");
            }
            return prb;
        }

        public async Task<bool> Save(PRB prb, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                prb.Prbfamilia = await DictionaryService.GetNextNumber(nameof(prb.Prbfamilia), authorization);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(prb);
                PRB existsprb = await FamilyRepository.Find(prb.Prbfamilia);
                if (existsprb != null)
                {
                    throw new Exception(message: "Familia já cadastrada.");
                }

                affectedRows = await FamilyRepository.Insert(prb);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(prb.Prbfamilia), prb.Prbfamilia);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(prb);
                affectedRows = await FamilyRepository.Update(prb);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string prbfamilia)
        {
            int affectedRows = await FamilyRepository.Delete(prbfamilia);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<PRB>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<PRB> families = await FamilyRepository.Search(search, limit, (page - 1) * limit);
            IList<PRB> nextfamilies = await FamilyRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<PRB>()
            {
                Items = families,
                Limit = limit,
                HasNext = nextfamilies != null && nextfamilies.Count > 0
            };
        }
    }
}