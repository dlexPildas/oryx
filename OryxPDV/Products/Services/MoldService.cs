using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Threading.Tasks;

namespace Products.Services
{
    public class MoldService
    {
        private readonly IConfiguration Configuration;
        private readonly MoldRepository MoldRepository;
        public MoldService(IConfiguration configuration)
        {
            Configuration = configuration;
            MoldRepository = new MoldRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<GR3> Find(string gr3molde)
        {
            GR3 gr3 = await MoldRepository.Find(gr3molde);

            if (gr3 == null)
            {
                throw new Exception(message: "Molde não cadastrado.");
            }
            return gr3;
        }
    }
}
