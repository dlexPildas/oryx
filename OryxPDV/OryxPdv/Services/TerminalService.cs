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
    public class TerminalService
    {
        private readonly IConfiguration Configuration;
        private readonly TerminalRepository TerminalRepository;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        private readonly PrintingPreferencesRepository PrintingPreferencesRepository;
        public TerminalService(IConfiguration configuration)
        {
            Configuration = configuration;
            TerminalRepository = new TerminalRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
            PrintingPreferencesRepository = new PrintingPreferencesRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<PD0>> FindList()
        {
            IList<PD0> lstpd0 = await TerminalRepository.FindList();

            return lstpd0;
        }

        public async Task<PD0> Find(string pd0codigo)
        {
            PD0 pd0 = await TerminalRepository.Find(pd0codigo);

            if (pd0 == null)
            {
                throw new Exception(message: "Terminal não cadastrado.");
            }
            return pd0;
        }

        public async Task<bool> Save(PD0 pd0, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                pd0.Pd0codigo = await DictionaryService.GetNextNumber(nameof(pd0.Pd0codigo), authorization);
                await new  FormatterService(Configuration).ValidateFormatBasicByDC1(pd0);
                PD0 existsPd0 = await TerminalRepository.Find(pd0.Pd0codigo);
                if (existsPd0 != null)
                {
                    throw new Exception(message: "Terminal já cadastrado.");
                }
                
                affectedRows = await TerminalRepository.Insert(pd0);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(pd0.Pd0codigo), pd0.Pd0codigo);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(pd0);
                affectedRows = await TerminalRepository.Update(pd0);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string pd0codigo)
        {
            int affectedRows = await TerminalRepository.Delete(pd0codigo);

            return affectedRows == 1;
        }

        public async Task<PD0> Reload(string pd0codigo)
        {
            if (string.IsNullOrWhiteSpace(pd0codigo))
            {
                throw new Exception("Terminal não informado");
            }
            PD0 pd0 = await Find(pd0codigo);
            pd0.PrinterPreferences = await PrintingPreferencesRepository.FindList(pd0codigo);
            return pd0;
        }

        public async Task<SearchPayloadModel<PD0>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;
            
            IList<PD0> terminals = await TerminalRepository.Search(search, limit, (page - 1) * limit);
            IList<PD0> nextTerminals = await TerminalRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<PD0>()
            {
                Items = terminals,
                Limit = limit,
                HasNext = nextTerminals != null && nextTerminals.Count > 0
            };
        }
    }
}
