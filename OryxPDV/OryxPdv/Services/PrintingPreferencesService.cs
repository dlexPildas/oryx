using OryxDomain.Repository;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OryxPdv.Services
{
    public class PrintingPreferencesService
    {
        private readonly IConfiguration Configuration;
        private readonly PrintingPreferencesRepository PrintingPreferencesRepository;
        public PrintingPreferencesService(IConfiguration configuration)
        {
            Configuration = configuration;
            PrintingPreferencesRepository = new PrintingPreferencesRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<PD1>> FindList(string pd1codigo)
        {
            IList<PD1> lstPd1 = await PrintingPreferencesRepository.FindList(pd1codigo);
            return lstPd1;
        }

        public async Task<PD1> Find(string pd1codigo, string pd1relat)
        {
            PD1 pd1 = await PrintingPreferencesRepository.Find(pd1codigo, pd1relat);

            if (pd1 == null)
            {
                throw new Exception(message: string.Format("Impressora para o terminal {0} e relatório {1} não cadastrada.", pd1codigo, pd1relat));
            }
            return pd1;
        }

        public async Task<bool> Save(PD1 pd1, bool forUpdate = false)
        {
            int affectedRows;
            await new FormatterService(Configuration).ValidateFormatBasicByDC1(pd1);
            if (!forUpdate)
            {
                PD1 existsPd1 = await PrintingPreferencesRepository.Find(pd1.Pd1codigo, pd1.Pd1relat);
                if (existsPd1 != null)
                {
                    throw new Exception(message: string.Format("Impressora para o terminal {0} e relatório {1} já cadastrada.", pd1.Pd1codigo, pd1.Pd1relat));
                }

                affectedRows = await PrintingPreferencesRepository.Insert(pd1);
            }
            else
            {
                affectedRows = await PrintingPreferencesRepository.Update(pd1);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string pd1codigo, string pd1relat)
        {
            int affectedRows = await PrintingPreferencesRepository.Delete(pd1codigo, pd1relat);
            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<PD1>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<PD1> lstPd1 = await PrintingPreferencesRepository.Search(search, limit, (page - 1) * limit);
            IList<PD1> nextLstPd1 = await PrintingPreferencesRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<PD1>()
            {
                Items = lstPd1,
                Limit = limit,
                HasNext = nextLstPd1 != null && nextLstPd1.Count > 0
            };
        }
    }
}
