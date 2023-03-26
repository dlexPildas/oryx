using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transporter.Services
{
    public class PostageAgencyService
    {
        private IConfiguration Configuration;
        private readonly PostageAgencyRepository PostageAgencyRepository;

        public PostageAgencyService(IConfiguration configuration)
        {
            Configuration = configuration;
            PostageAgencyRepository = new PostageAgencyRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<LXG>> FindList()
        {
            IList<LXG> lstlxg = await PostageAgencyRepository.FindList();

            return lstlxg;
        }

        public async Task<LXG> Find(string lxgtransp)
        {
            LXG lxg = await PostageAgencyRepository.Find(lxgtransp);

            if (lxg == null)
            {
                throw new Exception(message: "Agência de postagem não cadastrada.");
            }
            return lxg;
        }

        public async Task<bool> Save(LXG lxg, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(lxg);
                LXG existslxg = await PostageAgencyRepository.Find(lxg.Lxgtransp);
                if (existslxg != null)
                {
                    throw new Exception(message: "Agência de postagem já cadastrada.");
                }

                affectedRows = await PostageAgencyRepository.Insert(lxg);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(lxg);
                affectedRows = await PostageAgencyRepository.Update(lxg);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string lxgtransp)
        {
            int affectedRows = await PostageAgencyRepository.Delete(lxgtransp);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<LXG>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<LXG> PostageAgencies = await PostageAgencyRepository.Search(search, limit, (page - 1) * limit);
            IList<LXG> nextPostageAgencies = await PostageAgencyRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<LXG>()
            {
                Items = PostageAgencies,
                Limit = limit,
                HasNext = nextPostageAgencies != null && nextPostageAgencies.Count > 0
            };
        }
    }
}
