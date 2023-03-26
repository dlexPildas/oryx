using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MelhorEnvio.Views;
using OryxDomain.Utilities;
using OryxDomain.Models.MelhorEnvio;
using System.Linq;


namespace Transporter.Services
{
    public class TransporterService
    {
        private IConfiguration Configuration;
        private readonly TransporterRepository TransporterRepository;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        private readonly TransporterView TransporterView;

        public TransporterService(IConfiguration configuration)
        {
            Configuration = configuration;
            TransporterRepository = new TransporterRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            TransporterView = new TransporterView(Parameters.SqlConnection);
        }

        public async Task<IList<CF7>> FindList()
        {
            IList<CF7> lstCf7 = await TransporterRepository.FindList();

            return lstCf7;
        }

        public async Task<CF7> Find(string cf7transp)
        {
            CF7 cf7 = await TransporterRepository.Find(cf7transp);

            if (cf7 == null)
            {
                throw new Exception(message: "Transportadora não cadastrada.");
            }
            return cf7;
        }

        public async Task<bool> Save(CF7 cf7, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                cf7.Cf7transp = await DictionaryService.GetNextNumber(nameof(cf7.Cf7transp), authorization);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(cf7);
                CF7 existsCf7 = await TransporterRepository.Find(cf7.Cf7transp);
                if (existsCf7 != null)
                {
                    throw new Exception(message: "Transportadora já cadastrada.");
                }

                affectedRows = await TransporterRepository.Insert(cf7);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(cf7.Cf7transp), cf7.Cf7transp);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(cf7);
                affectedRows = await TransporterRepository.Update(cf7);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string cf7transp)
        {
            int affectedRows = await TransporterRepository.Delete(cf7transp);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<CF7>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<CF7> transporters = await TransporterRepository.Search(search, limit, (page - 1) * limit);
            IList<CF7> nextTransporters = await TransporterRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<CF7>()
            {
                Items = transporters,
                Limit = limit,
                HasNext = nextTransporters != null && nextTransporters.Count > 0
            };
        }

        public async Task<IList<TextValue>> FindPostageAgencies(string cf7codext)
        {
            return await TransporterView.GetPostageAgencies(cf7codext).ToList();
        }
    }
}
