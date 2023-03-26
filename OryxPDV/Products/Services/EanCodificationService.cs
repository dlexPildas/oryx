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
    public class EanCodificationService
    {
        private readonly IConfiguration Configuration;
        private readonly EanCodificationRepository EanCodificationRepository;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        private readonly ProductSizesRepository ProductSizesRepository;
        private readonly ColorOptionsRepository ColorOptionsRepository;
        private readonly ProductRepository ProductRepository;

        public EanCodificationService(IConfiguration configuration)
        {
            Configuration = configuration;
            EanCodificationRepository = new EanCodificationRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryService = new DictionaryService(Configuration);
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductSizesRepository = new ProductSizesRepository(Configuration["OryxPath"] + "oryx.ini");
            ColorOptionsRepository = new ColorOptionsRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductRepository = new ProductRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<EAN>> FindList()
        {
            IList<EAN> lstean = await EanCodificationRepository.FindList();

            return lstean;
        }

        public async Task<EAN> Find(string eancodigo)
        {
            EAN ean = await EanCodificationRepository.Find(eancodigo);

            if (ean == null)
            {
                throw new Exception(message: "Ean não cadastrado.");
            }
            return ean;
        }

        public async Task<bool> Save(EAN ean, string authorization, bool forUpdate = false)
        {
            int affectedRows;

            if (!forUpdate)
            {
                ean.Eancodigo = await DictionaryService.GetNextNumber(nameof(ean.Eancodigo), authorization);
                await ValidateEan(ean);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(ean);
                
                if (await EanCodificationRepository.Find(ean.Eancodigo) != null)
                {
                    throw new Exception(message: "EAN já cadastrado.");
                }

                affectedRows = await EanCodificationRepository.Insert(ean);
                if (affectedRows == 1)
                    affectedRows = await DictionaryRepository.SaveNextNumber(nameof(ean.Eancodigo), ean.Eancodigo);
            }
            else
            {
                await ValidateEan(ean);
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(ean);
                affectedRows = await EanCodificationRepository.Update(ean);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string eancodigo)
        {
            int affectedRows = await EanCodificationRepository.Delete(eancodigo);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<EAN>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<EAN> eans = await EanCodificationRepository.Search(search, limit, (page - 1) * limit);
            IList<EAN> nextEans = await EanCodificationRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<EAN>()
            {
                Items = eans,
                Limit = limit,
                HasNext = nextEans != null && nextEans.Count > 0
            };
        }

        private async Task ValidateEan(EAN ean)
        {
            var productByEan = await ProductRepository.Find<PR0>(ean.Eanproduto);
            
            if (productByEan == null)
            {
                throw new Exception(message: $"O produto informado no cadastro de código de EAN não existe.");
            }

            if (productByEan != null && !string.IsNullOrEmpty(productByEan.Pr0ean))
            {
                throw new Exception(message: $"O produto: {ean.Eanproduto}, já possui código EAN definido.");
            }

            if (await ColorOptionsRepository.FindByEan(ean.Eanproduto, ean.Eanopcao) == null)
            {
                throw new Exception(message: $"A opção de cor {ean.Eanopcao}, não existe para o produto {ean.Eanproduto}.");
            }

            if (await ProductSizesRepository.Find(ean.Eanproduto, ean.Eantamanho) == null)
            {
                throw new Exception(message: $"A opção de tamanho {ean.Eantamanho}, não existe para o produto {ean.Eanproduto}.");
            }

            var eanByCodext = await EanCodificationRepository.FindEanByCodext(ean.Eancodext);
            if (eanByCodext != null && eanByCodext.Eancodigo != ean.Eancodigo)
            {
                throw new Exception(message: $"Código alternativo {ean.Eancodext} já cadastrado para o EAN {ean.Eancodigo}.");
            }

            if (await EanCodificationRepository.FindEanByParameters(ean.Eancodigo, ean.Eanproduto, ean.Eanopcao, ean.Eantamanho) != null)
            {
                throw new Exception(message: $"Já existe código EAN para o mesmo item {ean.Eancodigo}.");
            }
        }
    }
}