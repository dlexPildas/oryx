using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OryxPdv.Services
{
    public class MallParametersService
    {
        private IConfiguration Configuration;
        private readonly MallParametersRepository MallParametersRepository;
        public MallParametersService(IConfiguration configuration)
        {
            Configuration = configuration;
            MallParametersRepository = new MallParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<PD5>> FindAll()
        {
            IList<PD5> lstpd5 = await MallParametersRepository.FindAll();

            return lstpd5;
        }

        public async Task<PD5> Find(int pd5codigo)
        {
            PD5 pd5 = await MallParametersRepository.Find(pd5codigo);

            return pd5;
        }

        public async Task<bool> Save(PD5 pd5, bool forUpdate = false)
        {
            int affectedRows;
            await new FormatterService(Configuration).ValidateFormatBasicByDC1(pd5);
            if (!forUpdate)
            {
                PD5 existsPd5 = await MallParametersRepository.Find(pd5.Pd5tipinsh);
                if (existsPd5 != null)
                {
                    throw new Exception(message: "Parâmetros de integração com shopping já cadastrada.");
                }

                affectedRows = await MallParametersRepository.Insert(pd5);
            }
            else
            {
                affectedRows = await MallParametersRepository.Update(pd5);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(int pd0codigo)
        {
            int affectedRows = await MallParametersRepository.Delete(pd0codigo);

            return affectedRows == 1;
        }
    }
}
