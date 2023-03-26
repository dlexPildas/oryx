using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Services
{
    public class AppliedMaterialsService
    {
        private readonly IConfiguration Configuration;
        private readonly AppliedMaterialsRepository AppliedMaterialsRepository;
        public AppliedMaterialsService(IConfiguration configuration)
        {
            Configuration = configuration;
            AppliedMaterialsRepository = new AppliedMaterialsRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<List<PR8>> FindList(string pr0produto, string pr3tamanho)
        {
            return await AppliedMaterialsRepository.FindList(pr0produto, pr3tamanho);
        }
    }
}
