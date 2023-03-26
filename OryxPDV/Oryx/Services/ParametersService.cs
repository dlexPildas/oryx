using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Threading.Tasks;

namespace Oryx.Services
{
    public class ParametersService
    {
        private readonly IConfiguration Configuration;
        private readonly ParametersRepository ParametersRepository;
        public ParametersService(IConfiguration configuration)
        {
            Configuration = configuration;
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<LX0> Find()
        {
            LX0 lx0 = await ParametersRepository.GetLx0();

            if (lx0 == null)
            {
                throw new Exception("Parâmetros Gerais não cadastrados.");
            }

            return lx0;
        }
    }
}
