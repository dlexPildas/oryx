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
    public class ColorOptionsService
    {
        private readonly IConfiguration Configuration;
        private readonly ColorOptionsRepository ColorOptionsRepository;
        
        public ColorOptionsService(IConfiguration configuration)
        {
            Configuration = configuration;
            ColorOptionsRepository = new ColorOptionsRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<PR2>> FindList(string pr2produto)
        {
            IList<PR2> lstpr2 = await ColorOptionsRepository.FindList(pr2produto);

            return lstpr2;
        }

        public async Task<PR2> Find(string pr2produto, string pr2opcao)
        {
            PR2 pr2 = await ColorOptionsRepository.Find(pr2produto, pr2opcao);

            if (pr2 == null)
            {
                throw new Exception(message: "Opção de cor não cadastrada.");
            }
            return pr2;
        }

        public async Task<bool> Save(PR2 pr2, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(pr2);
                PR2 existsPr2 = await ColorOptionsRepository.Find(pr2.Pr2produto, pr2.Pr2opcao);
                if (existsPr2 != null)
                {
                    throw new Exception(message: "Opção de cor já cadastrada.");
                }

                affectedRows = await ColorOptionsRepository.Insert(pr2);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(pr2);
                affectedRows = await ColorOptionsRepository.Update(pr2);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string pr2produto, string pr2opcao)
        {
            //TODO: Verificar Dependencias
            int affectedRows = await ColorOptionsRepository.Delete(pr2produto, pr2opcao);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<PR2>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<PR2> colorOptions = await ColorOptionsRepository.Search(search, limit, (page - 1) * limit);
            IList<PR2> nextcolorOptions = await ColorOptionsRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<PR2>()
            {
                Items = colorOptions,
                Limit = limit,
                HasNext = nextcolorOptions != null && nextcolorOptions.Count > 0
            };
        }
    }
}
