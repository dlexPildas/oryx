using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Threading.Tasks;

namespace OryxPdv.Services
{

    public class CashMainService
    {
        readonly private IConfiguration Configuration;
        readonly private CashMainRepository CashMainRepository;
        public CashMainService(IConfiguration configuration)
        {
            Configuration = configuration;
            CashMainRepository = new CashMainRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<PD3> Find()
        {
            PD3 pd3 = await CashMainRepository.Find();

            return pd3;
        }

        public async Task<bool> Save(PD3 pd3, bool forUpdate = false)
        {
            int affectedRows;

            await new FormatterService(Configuration).ValidateFormatBasicByDC1(pd3);
            if (!forUpdate)
            {
                PD3 existsPd3 = await CashMainRepository.Find();
                if (existsPd3 != null)
                {
                    throw new Exception(message: "Parâmetros da tela principal de caixa já cadastrados.");
                }
                affectedRows = await CashMainRepository.Insert(pd3);
            }
            else
            {
                affectedRows = await CashMainRepository.Update(pd3);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete()
        {
            int affectedRows = await CashMainRepository.Delete();

            return affectedRows == 1;
        }
    }
}
