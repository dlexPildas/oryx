using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Services
{
    public class SizeGridService
    {
        private readonly IConfiguration Configuration;
        private readonly SizeGridRepository SizeGridRepository;
        private readonly SizeService SizeService;
        private readonly DictionaryService DictionaryService;
        public SizeGridService(IConfiguration configuration)
        {
            Configuration = configuration;
            SizeGridRepository = new SizeGridRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            SizeService = new SizeService(Configuration);
        }

        public async Task<GR0> Find(string gr0grade)
        {
            GR0 gr0 = await SizeGridRepository.Find(gr0grade);

            if (gr0 == null)
            {
                throw new Exception(message: "Grade não cadastrada.");
            }

            var gr1list = await SizeService.FindSizesByGR0(gr0grade);
            gr0.LstGr1 = gr1list.OrderBy(x => x.Gr1posicao).ToList();

            return gr0;
        }

        public async Task<bool> Save(GR0 gr0, string authorization)
        {
            var existgr0 = await SizeGridRepository.Find(gr0.Gr0grade);

            await ValidateSizes(gr0.LstGr1);

            if (existgr0 == null)
            {
                return await Insert(gr0, authorization);
            }
            else
            {
                if (await Update(gr0))
                {
                    return await UpdateSizesFromSizeGrid(existgr0, gr0.LstGr1);
                }
                else
                {
                    throw new Exception("Houve erro ao atualizar grade de tamanhos");
                }
            }
        }

        public async Task<bool> Insert(GR0 gr0, string authorization)
        {
            gr0.Gr0grade = await DictionaryService.GetNextNumber(nameof(GR0.Gr0grade), authorization);
            await new FormatterService(Configuration).ValidateFormatBasicByDC1(gr0);

            int affectedRows = await SizeGridRepository.Insert(gr0);

            if (affectedRows == 1)
            {                
                await InsertSizes(gr0.LstGr1, gr0.Gr0grade, authorization);
                return true;
            }
            else
            {
                throw new Exception("Houve erro ao inserir a grade de tamanhos");
            }
        }

        private async Task<bool> Update(GR0 gr0)
        {
            await new FormatterService(Configuration).ValidateFormatBasicByDC1(gr0);
            if (await SizeGridRepository.Update(gr0) == 1)
                return true;

            return false;
        }

        private async Task<bool> UpdateSizesFromSizeGrid(GR0 existedsizegrade, IList<GR1> sizes)
        {
            List<GR1> existedsizes = new List<GR1>();
            existedsizes = await SizeService.FindSizesByGR0(existedsizegrade.Gr0grade);

            await VerifySizeToDelete(existedsizes, sizes);

            foreach (var item in sizes)
            {
                GR1 existedSize = existedsizes.Where(x => x.Gr1tamanho == item.Gr1tamanho).FirstOrDefault();

                if (existedSize == null)
                {
                    await SizeService.Save(existedSize, false,true);
                }
                else
                {
                    existedSize.Gr1desc = item.Gr1desc;
                    existedSize.Gr1especif = item.Gr1especif;
                    existedSize.Gr1grade = existedsizegrade.Gr0grade;
                    existedSize.Gr1posicao = item.Gr1posicao.Trim();
                    existedSize.Gr1tamanho = existedSize.Gr1tamanho;
                    existedSize.Gr1tamext = item.Gr1tamext;

                    await SizeService.Save(existedSize, true,true);
                }
            }
            return true;
        }

        private async Task VerifySizeToDelete(IList<GR1> sizesexisted, IList<GR1> sizes)
        {
            foreach (var item in sizesexisted)
            {
                if (sizes.Where(x => x.Gr1tamanho == item.Gr1tamanho).FirstOrDefault() == null)
                {
                    //TODO: VerificaDependencias()
                    await SizeService.Delete(item.Gr1tamanho);
                }
            }
        }

        private async Task InsertSizes(IList<GR1> sizes, string newgr0grade,string authorization)
        {
            foreach (var item in sizes)
            {
                item.Gr1grade = newgr0grade;
                item.Gr1posicao.Trim();

                await SizeService.Save(item, false,true);
            }
        }

        public async Task<bool> Delete(string gr0grade)
        {
            //TODO: VerificarDependencias()
            int affectedRows = await SizeGridRepository.Delete(gr0grade);

            return affectedRows == 1;
        }

        public async Task ValidateSizes(IList<GR1> lstGr1)
        {
            foreach (var item in lstGr1)    
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(item);
            }
        }

        public async Task<SearchPayloadModel<GR0>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<GR0> sizegrids = await SizeGridRepository.Search(search, limit, (page - 1) * limit);
            IList<GR0> nextsizegrids = await SizeGridRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<GR0>()
            {
                Items = sizegrids,
                Limit = limit,
                HasNext = nextsizegrids != null && nextsizegrids.Count > 0
            };
        }
    }
}
