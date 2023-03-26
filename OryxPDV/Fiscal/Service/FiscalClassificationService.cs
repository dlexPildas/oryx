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
    public class FiscalClassificationService
    {
        private readonly IConfiguration Configuration;
        private readonly FiscalClassificationRepository FiscalClassificationRepository;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;

        public FiscalClassificationService(IConfiguration configuration)
        {
            Configuration = configuration;
            FiscalClassificationRepository = new FiscalClassificationRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<FI0>> FindList()
        {
            IList<FI0> lstfi0 = await FiscalClassificationRepository.FindList();

            return lstfi0;
        }

        public async Task<FI0> Find(string fi0id)
        {
            FI0 fi0 = await FiscalClassificationRepository.Find(fi0id);

            if (fi0 == null)
            {
                throw new Exception(message: "Classificação fiscal não cadastrada.");
            }
            return fi0;
        }

        public async Task<bool> Save(FI0 fi0, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                if (!string.IsNullOrEmpty(fi0.Fi0id))
                {
                    FI0 existsFi0 = await FiscalClassificationRepository.Find(fi0.Fi0id);

                    if (existsFi0 != null)
                    {
                        throw new Exception(message: "Classificação fiscal já cadastrada.");
                    }
                    await new FormatterService(Configuration).ValidateFormatBasicByDC1(fi0);

                    affectedRows = await FiscalClassificationRepository.Insert(fi0);
                }
                else
                {
                    fi0.Fi0id = await DictionaryService.GetNextNumber(nameof(fi0.Fi0id), authorization);
                    await new FormatterService(Configuration).ValidateFormatBasicByDC1(fi0);

                    affectedRows = await FiscalClassificationRepository.Insert(fi0);
                    if (affectedRows == 1)
                        affectedRows = await DictionaryRepository.SaveNextNumber(nameof(fi0.Fi0id), fi0.Fi0id);
                }
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(fi0);
                affectedRows = await FiscalClassificationRepository.Update(fi0);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string fi0)
        {
            int affectedRows = await FiscalClassificationRepository.Delete(fi0);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<FI0>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<FI0> fiscalClassifications = await FiscalClassificationRepository.Search(search, limit, (page - 1) * limit);
            IList<FI0> nextFiscalClassifications = await FiscalClassificationRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<FI0>()
            {
                Items = fiscalClassifications,
                Limit = limit,
                HasNext = nextFiscalClassifications != null && nextFiscalClassifications.Count > 0
            };
        }
    }
}