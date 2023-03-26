using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OryxDomain.Services;
using System;
using System.Threading.Tasks;

namespace Oryx.Services
{
    public class FiscalDocumentTypeService
    {
        private readonly IConfiguration Configuration;
        private readonly FiscalService FiscalService;

        public FiscalDocumentTypeService(IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration;
            FiscalService = new FiscalService(Configuration, logger);
        }

        public async Task<string> ProximoNumero(string cv1docfis)
        {
            if (string.IsNullOrWhiteSpace(cv1docfis))
                throw new Exception("Tipo de documento fiscal não informado.");

            return await FiscalService.ProximoNumero(new OryxDomain.Models.Oryx.CV5() { Cv5tipo =  cv1docfis });
        }
    }
}
