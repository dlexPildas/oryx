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
    public class GroupService
    {
        private readonly IConfiguration Configuration;
        private readonly GroupRepository GroupRepository;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        public GroupService(IConfiguration configuration)
        {
            Configuration = configuration;
            GroupRepository = new GroupRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
        }
        public async Task<IList<PRS>> FindList()
        {
            IList<PRS> lstprs = await GroupRepository.FindList();

            return lstprs;
        }

        public async Task<PRS> Find(string prsgrupo)
        {
            PRS prs = await GroupRepository.Find(prsgrupo);

            if (prs == null)
            {
                throw new Exception(message: "Grupo não cadastrado.");
            }
            return prs;
        }

        public async Task<bool> Save(PRS prs, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                prs.Prsgrupo = await DictionaryService.GetNextNumber(nameof(prs.Prsgrupo), authorization);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(prs);
                PRS existsPRS = await GroupRepository.Find(prs.Prsgrupo);
                if (existsPRS != null)
                {
                    throw new Exception(message: "Grupo já cadastrado.");
                }

                affectedRows = await GroupRepository.Insert(prs);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(prs.Prsgrupo), prs.Prsgrupo);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(prs);
                affectedRows = await GroupRepository.Update(prs);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string prsgrupo)
        {
            int affectedRows = await GroupRepository.Delete(prsgrupo);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<PRS>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<PRS> groups = await GroupRepository.Search(search, limit, (page - 1) * limit);
            IList<PRS> nextGroups = await GroupRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<PRS>()
            {
                Items = groups,
                Limit = limit,
                HasNext = nextGroups != null && nextGroups.Count > 0
            };
        }
    }
}
