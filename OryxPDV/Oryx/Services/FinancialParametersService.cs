using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Threading.Tasks;

namespace Oryx.Services
{
    public class FinancialParametersService
    {
        private readonly IConfiguration Configuration;
        private readonly FinancialParametersRepository FinancialParametersRepository;

        public FinancialParametersService(IConfiguration configuration)
        {
            Configuration = configuration;
            FinancialParametersRepository = new FinancialParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<LXA> Find()
        {
            LXA lxa = await FinancialParametersRepository.Find();

            return lxa;
        }

        public async Task<bool> Save(LXA lxa, bool forUpdate = false)
        {
            int affectedRows;
            await new FormatterService(Configuration).ValidateFormatBasicByDC1(lxa);
            if (!forUpdate)
            {
                LXA existsLxa = await FinancialParametersRepository.Find();
                if (existsLxa != null)
                {
                    throw new Exception(message: "Parâmetros financeiros já cadastrados.");
                }
                affectedRows = await FinancialParametersRepository.Insert(lxa);
            }
            else
            {
                affectedRows = await FinancialParametersRepository.Update(lxa);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete()
        {
            int affectedRows = await FinancialParametersRepository.Delete();

            return affectedRows == 1;
        }
    }
}
