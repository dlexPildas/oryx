using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Threading.Tasks;

namespace Oryx.Services
{
    public class OrderParametersService
    {
        private IConfiguration Configuration;
        private readonly OrderParametersRepository OrderParametersRepository;

        public OrderParametersService(IConfiguration configuration)
        {
            Configuration = configuration;
            OrderParametersRepository = new OrderParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<LX2> Find()
        {
            LX2 lx2 = await OrderParametersRepository.Find();

            return lx2;
        }

        public async Task<bool> Save(LX2 lx2, bool forUpdate = false)
        {
            int affectedRows;
            await new FormatterService(Configuration).ValidateFormatBasicByDC1(lx2);
            if (!forUpdate)
            {
                LX2 existsLx2 = await OrderParametersRepository.Find();
                if (existsLx2 != null)
                {
                    throw new Exception(message: "Parâmetros para pedidos já cadastrados.");
                }
                lx2.Lx2padrao = "1";
                affectedRows = await OrderParametersRepository.Insert(lx2);
            }
            else
            {
                affectedRows = await OrderParametersRepository.Update(lx2);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete()
        {
            int affectedRows = await OrderParametersRepository.Delete();

            return affectedRows == 1;
        }
    }
}
