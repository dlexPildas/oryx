using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using Products.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Products.Services
{
    public class ColorService
    {
        private readonly IConfiguration Configuration;
        private readonly ColorRepository ColorRepository;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        public ColorService(IConfiguration configuration)
        {
            Configuration = configuration;
            ColorRepository = new ColorRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<CR1>> FindList()
        {
            IList<CR1> lstcr1 = await ColorRepository.FindList();

            return lstcr1;
        }

        public async Task<CR1> Find(string cr1cor)
        {
            CR1 cr1 = await ColorRepository.Find(cr1cor);

            if (cr1 == null)
            {
                throw new Exception(message: "Cor não cadastrada.");
            }
            return cr1;
        }

        public async Task<bool> Save(CR1 cr1, string authorization, bool forUpdate = false)
        {
            int affectedRows;
            if (!forUpdate)
            {
                cr1.Cr1cor = await DictionaryService.GetNextNumber(nameof(cr1.Cr1cor), authorization);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(cr1);
                CR1 existscr1 = await ColorRepository.Find(cr1.Cr1cor);
                if (existscr1 != null)
                {
                    throw new Exception(message: "Cor já cadastrada.");
                }

                affectedRows = await ColorRepository.Insert(cr1);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(cr1.Cr1cor), cr1.Cr1cor);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(cr1);
                affectedRows = await ColorRepository.Update(cr1);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string cr1cor)
        {
            int affectedRows = await ColorRepository.Delete(cr1cor);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<CR1>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<CR1> colors = await ColorRepository.Search(search, limit, (page - 1) * limit);
            IList<CR1> nextColors = await ColorRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<CR1>()
            {
                Items = colors,
                Limit = limit,
                HasNext = nextColors != null && nextColors.Count > 0
            };
        }

        public GetArgbResponseModel GetArgb(GetArgbModel getArgbModel)
        {
            ValidateRgb(getArgbModel);

            var alphaDefaultValue = 255;

            var argb = Color.FromArgb(alphaDefaultValue, getArgbModel.Red, getArgbModel.Green, getArgbModel.Blue);

            return new GetArgbResponseModel()
            {
                HexCode = ColorTranslator.ToHtml(argb),
                ArgbCode = argb.Name
            };
        }

        private void ValidateRgb(GetArgbModel getArgbModel)
        {
            if(getArgbModel.Red > 255 
                || getArgbModel.Green > 255
                || getArgbModel.Blue > 255)
                    throw new Exception(message: "Valor inválido.");
        }
    }
}
