using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System.Threading.Tasks;

namespace Oryx.Services
{
    public class BusinessOperationService
    {
        private IConfiguration Configuration;
        private readonly BusinessOperationRepository BusinessOperationRepository;

        public BusinessOperationService(IConfiguration configuration)
        {
            Configuration = configuration;
            BusinessOperationRepository = new BusinessOperationRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<CV3> Find(string cv3opercom)
        {
            CV3 cv3 = await BusinessOperationRepository.Find(cv3opercom);

            return cv3;
        }
    }
}
