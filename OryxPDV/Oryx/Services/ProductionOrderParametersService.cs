using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Threading.Tasks;

namespace Oryx.Services
{
    public class ProductionOrderParametersService
    {
        private IConfiguration Configuration;
        private readonly ProductionOrderParametersRepository ProductionOrderParametersRepository;

        public ProductionOrderParametersService(IConfiguration configuration)
        {
            Configuration = configuration;
            ProductionOrderParametersRepository = new ProductionOrderParametersRepository(Configuration["OryxPath"] + "oryx.ini");

        }

        public async Task<LX1> Find()
        {
            LX1 lx1 = await ProductionOrderParametersRepository.Find();

            if (lx1 == null)
            {
                throw new Exception("Parâmetros de ordem de produção não cadastrados.");
            }

            return lx1;
        }
    }
}
