using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Threading.Tasks;

namespace Oryx.Services
{
    public class FiscalParametersService
    {
        private readonly IConfiguration Configuration;
        private readonly FiscalParametersRepository FiscalParametersRepository;

        public FiscalParametersService(IConfiguration configuration)
        {
            Configuration = configuration;
            FiscalParametersRepository = new FiscalParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<LX3> Find()
        {
            LX3 lx3 = await FiscalParametersRepository.Find();

            return lx3;
        }

        public async Task<bool> Save(LX3 lx3, bool forUpdate = false)
        {
            int affectedRows;
            await new FormatterService(Configuration).ValidateFormatBasicByDC1(lx3);
            if (!forUpdate)
            {
                LX3 existsLX3 = await FiscalParametersRepository.Find();
                if (existsLX3 != null)
                {
                    throw new Exception(message: "Parâmetros fiscais já cadastrados.");
                }
                affectedRows = await FiscalParametersRepository.Insert(lx3);
            }
            else
            {
                affectedRows = await FiscalParametersRepository.Update(lx3);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete()
        {
            int affectedRows = await FiscalParametersRepository.Delete();

            return affectedRows == 1;
        }
    }
}
