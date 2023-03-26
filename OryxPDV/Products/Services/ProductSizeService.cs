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
    public class ProductSizeService
    {
        private readonly IConfiguration Configuration;
        private readonly ProductSizesRepository ProductSizesRepository;
        private readonly ProductRepository ProductRepository;
        private readonly SizesRepository SizesRepository;
        private readonly UserRepository UserRepository;
        private readonly AppliedMaterialsRepository AppliedMaterialsRepository;
        private readonly PriceOrderRepository PriceOrderRepository;
        private readonly SecurityRepository SecurityRepository;


        public ProductSizeService(IConfiguration configuration)
        {
            Configuration = configuration;
            ProductSizesRepository = new ProductSizesRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductRepository = new ProductRepository(Configuration["OryxPath"] + "oryx.ini");
            SizesRepository = new SizesRepository(Configuration["OryxPath"] + "oryx.ini");
            AppliedMaterialsRepository = new AppliedMaterialsRepository(Configuration["OryxPath"] + "oryx.ini");
            PriceOrderRepository = new PriceOrderRepository(Configuration["OryxPath"] + "oryx.ini");
            UserRepository = new UserRepository(Configuration["OryxPath"] + "oryx.ini");
            SecurityRepository = new SecurityRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<PR3>> FindList(string pr3produto)
        {
            IList<PR3> lstpr3 = await ProductSizesRepository.FindList(pr3produto);

            return lstpr3;
        }

        public async Task<PR3> Find(string pr3produto, string pr3tamanho)
        {
            PR3 pr3 = await ProductSizesRepository.Find(pr3produto, pr3tamanho);

            if (pr3 == null)
            {
                throw new Exception(message: "Opção de tamanho não cadastrada.");
            }
            return pr3;
        }

        public async Task<bool> Save(PR3 pr3, bool forUpdate = false)
        {
            int affectedRows;

            await Validate(pr3);

            if (!forUpdate)
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(pr3);
                PR3 existsPr3 = await ProductSizesRepository.Find(pr3.Pr3produto, pr3.Pr3tamanho);
                if (existsPr3 != null)
                {
                    throw new Exception(message: "Opção de tamanho já cadastrada.");
                }

                affectedRows = await ProductSizesRepository.Insert(pr3);
            }
            else
            {
                await new FormatterService(Configuration).ValidateFormatBasicByDC1(pr3);
                affectedRows = await ProductSizesRepository.Update(pr3);
            }

            return affectedRows == 1;
        }

        private async Task Validate(PR3 pr3)
        {
            var pr0 = await ProductRepository.Find<PR0>(pr3.Pr3produto);

            if (pr3.Pr3lotemul > pr0.Pr0lotemax)
            {
                throw new Exception($"Valor superior ao lote máximo {pr0.Pr0lotemax}.");
            }

            if (pr3.Pr3lotemul % pr0.Pr0lotemax > 0)
            {
                throw new Exception($"Lote múltiplo não é divisível pelo lote máximo {pr0.Pr0lotemax}.");
            }
        }

        public async Task<bool> Delete(string pr3produto, string pr3tamanho, string authorization, string lx9acesso)
        {
            //TODO: VerificarDependencias
            string lx9usuario = await SecurityRepository.FindLx9Valid(lx9acesso);

            var dc5 = await UserRepository.GetDeleteArchivePermission(lx9usuario, nameof(PR3));
            if (dc5.Dc5excluir)
            {

                await AppliedMaterialsRepository.Delete(pr3produto, pr3tamanho);
                await PriceOrderRepository.Delete(pr3produto, pr3tamanho);
                int affectedRows = await ProductSizesRepository.Delete(pr3produto, pr3tamanho);
                
                return affectedRows == 1;
            }
            else
            {
                throw new Exception("Você não possui permissão para excluir tamanhos de produtos.");
            }

        }

        public async Task<SearchPayloadModel<PR3>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<PR3> productSizeOptions = await ProductSizesRepository.Search(search, limit, (page - 1) * limit);
            IList<PR3> productSiznextOptions = await ProductSizesRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<PR3>()
            {
                Items = productSizeOptions,
                Limit = limit,
                HasNext = productSiznextOptions != null && productSiznextOptions.Count > 0
            };
        }
    }
}
