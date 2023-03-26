using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Threading.Tasks;

namespace Oryx.Services
{
    public class APIParametersService
    {
        private readonly IConfiguration Configuration;
        private readonly APIParametersRepository APIParametersRepository;

        public APIParametersService(IConfiguration configuration)
        {
            Configuration = configuration;
            APIParametersRepository = new APIParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<LXE> Find()
        {
            LXE lxe = await APIParametersRepository.Find();

            return lxe;
        }

        public async Task<bool> Save(LXE lxe, bool forUpdate = false)
        {
            lxe.Lxepadrao = "1";
            int affectedRows;
            await new FormatterService(Configuration).ValidateFormatBasicByDC1(lxe);
            if (!forUpdate)
            {
                LXE existsLxe = await APIParametersRepository.Find();
                if (existsLxe != null)
                    throw new Exception("Parâmetros de API já cadastrados");

                affectedRows = await APIParametersRepository.Insert(lxe);
            }
            else
            {
                affectedRows = await APIParametersRepository.Update(lxe);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete()
        {
            int affectedRows = await APIParametersRepository.Delete();

            return affectedRows == 1;
        }
    }
}
