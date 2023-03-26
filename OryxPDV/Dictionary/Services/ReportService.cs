using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Threading.Tasks;

namespace Dictionary.Services
{
    public class ReportService
    {
        private IConfiguration Configuration;
        private readonly ReportsRepository ReportsRepository;

        public ReportService(IConfiguration configuration)
        {
            Configuration = configuration;
            ReportsRepository = new ReportsRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<DC9> Find(string dc9relat)
        {
            if (string.IsNullOrWhiteSpace(dc9relat))
                throw new Exception("Relatório não informado");

            DC9 dc9 = await ReportsRepository.Find(dc9relat);
            if(dc9 == null)
                throw new Exception("Relatório " + dc9relat + " não encontrado");

            return dc9;
        }
    }
}
