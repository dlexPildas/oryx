using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class LogServices
    {
        private readonly IConfiguration Configuration;
        private readonly LogRegisterRepository LogRegisterRepository;
        private readonly ParametersRepository ParametersRepository;

        public LogServices(IConfiguration configuration)
        {
            Configuration = configuration;
            LogRegisterRepository = new LogRegisterRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<bool> Register(LX8 lx8)
        {
            LX0 lx0 = await ParametersRepository.GetLx0();

            if (string.IsNullOrWhiteSpace(lx8.Lx8usuario))
                lx8.Lx8usuario = "SPV";

            if (lx0.Lx0logs)
            {
                int affectedRows = await LogRegisterRepository.Insert(lx8);
                return affectedRows == 1;
            }

            return true;
        }
    }
}
