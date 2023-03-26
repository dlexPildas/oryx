using MelhorEnvio.Views;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.MelhorEnvio;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class LogisticIntegrationService
    {
        private IConfiguration Configuration;
        private readonly LogisticIntegrationRepository LogisticIntegrationRepository;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;

        public LogisticIntegrationService(IConfiguration configuration)
        {
            Configuration = configuration;
            LogisticIntegrationRepository = new LogisticIntegrationRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            TokenView tokenView = new TokenView(Parameters.SqlConnection);

        }

        public async Task<IList<LXF>> FindList()
        {
            IList<LXF> lstlxf = await LogisticIntegrationRepository.FindList();

            return lstlxf;
        }

        public async Task<LXF> Find(string lxfcodigo)
        {
            LXF lxf = await LogisticIntegrationRepository.Find(lxfcodigo);

            if (lxf == null)
            {
                throw new Exception(message: "Integração logística não cadastrada.");
            }
            return lxf;
        }

        public async Task<bool> Save(LXF lxf, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                lxf.Lxfcodigo = await DictionaryService.GetNextNumber(nameof(lxf.Lxfcodigo), authorization);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(lxf);
                LXF existslxf = await LogisticIntegrationRepository.Find(lxf.Lxfcodigo);
                if (existslxf != null)
                {
                    throw new Exception(message: "Integração logística já cadastrada.");
                }

                if(await FindByType(lxf.Lxftipo) != null)
                {
                    throw new Exception(message: "Tipo de Integração de logística já cadastrada em outra integração.");
                }

                affectedRows = await LogisticIntegrationRepository.Insert(lxf);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(lxf.Lxfcodigo), lxf.Lxfcodigo);
            }
            else
            {
                var lxfByType = await FindByType(lxf.Lxftipo);
                if (lxfByType != null && lxfByType.Lxfcodigo != lxf.Lxfcodigo)
                {
                    throw new Exception(message: "Tipo de Integração de logística já cadastrada em outra integração.");
                }

                await new FormatterService(Configuration).ValidateFormatBasicByDC1(lxf);
                affectedRows = await LogisticIntegrationRepository.Update(lxf);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string lxfcodigo)
        {
            int affectedRows = await LogisticIntegrationRepository.Delete(lxfcodigo);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<LXF>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<LXF> lxflst = await LogisticIntegrationRepository.Search(search, limit, (page - 1) * limit);
            IList<LXF> nextlxfs = await LogisticIntegrationRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<LXF>()
            {
                Items = lxflst,
                Limit = limit,
                HasNext = nextlxfs != null && nextlxfs.Count > 0
            };
        }

        public async Task<LXF> FindByType(LogisticIntegrationType lxftipo)
        {
            LXF lxf = await LogisticIntegrationRepository.FindByType(lxftipo);
           
            return lxf;
        }

        public async Task<LXF> GenerateNewToken(LXF lxf,string authorization)
        {
            TokenModel newToken = await TokenView.GetTokenAsync(lxf.Lxfaut4);
            LXF oldLxf = await LogisticIntegrationRepository.Find(lxf.Lxfcodigo);

            oldLxf.Lxfaut4 = lxf.Lxfaut4;
            oldLxf.Lxfaut3 = newToken.refresh_token;
            oldLxf.Lxftoken = newToken.access_token;

            if(await Save(oldLxf, authorization, true))
            {
                return oldLxf;
            }
            else
            {
                throw new Exception("Não foi possível gerar um novo token");
            }
        }
    }
}
