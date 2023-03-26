using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Services
{
    public class SizeService
    {
        private readonly IConfiguration Configuration;
        private readonly SizesRepository SizesRepository;
        
        public SizeService(IConfiguration configuration)
        {
            Configuration = configuration;
            SizesRepository = new SizesRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<bool> Save(GR1 gr1, bool forUpdate, bool isvalidated = false)
        {
            int affectedRows;
            await ValidateGR1(gr1);

            if(!isvalidated)
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(gr1);

            if (!forUpdate)
                affectedRows = await SizesRepository.Insert(gr1);
            else
                affectedRows = await SizesRepository.Update(gr1);

            return affectedRows == 1;
        }

        public async Task ValidateGR1(GR1 gr1)
        {
            var existedsize = await SizesRepository.Find(gr1.Gr1tamanho);
            if (existedsize != null && existedsize.Gr1grade != gr1.Gr1grade)
            {
                throw new Exception($"O tamanho {gr1.Gr1tamanho} já existe em outra grade.");
            }
        }

        public async Task<bool> Delete(string gr1tamanho)
        {
            int affectedRows = await SizesRepository.Delete(gr1tamanho);

            return affectedRows == 1;
        }

        public async Task<List<GR1>> FindSizesByGR0(string gr1grade)
        {
            var list = await SizesRepository.FindByGR0(gr1grade);

            return list;
        }

    }
}
