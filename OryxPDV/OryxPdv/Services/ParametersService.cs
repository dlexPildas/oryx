using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Threading.Tasks;

namespace OryxPdv.Services
{
    public class ParametersService
    {
        private readonly IConfiguration Configuration;
        private readonly PDVParametersRepository PDVParametersRepository;
        private readonly ParametersRepository ParametersRepository;

        public ParametersService(IConfiguration configuration)
        {
            Configuration = configuration;
            PDVParametersRepository = new PDVParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<LXD> Find()
        {
            LXD lxd = await PDVParametersRepository.Find();

            return lxd;
        }

        public async Task<bool> Save(LXD lxd, bool forUpdate = false)
        {
            lxd.Lxdpadrao = "1";
            int affectedRows;

            if (!string.IsNullOrWhiteSpace(lxd.Lxddocven4) &&
                !string.IsNullOrWhiteSpace(lxd.Lxddocven5) &&
                lxd.Lxddocven4.Equals(lxd.Lxddocven5))
            {
                throw new Exception("Tipo de documento fiscal para saídas de notas fiscais de venda e consignado precisam ser diferentes.");
            }

            if (!string.IsNullOrWhiteSpace(lxd.Lxddocdev3) &&
                !string.IsNullOrWhiteSpace(lxd.Lxddocdev4) &&
                lxd.Lxddocdev3.Equals(lxd.Lxddocdev4))
            {
                throw new Exception("Tipo de documento fiscal para devolução de notas fiscais de venda e consignado precisam ser diferentes.");
            }

            await new FormatterService(Configuration).ValidateFormatBasicByDC1(lxd);
            if (!forUpdate)
            {
                LXD existsLxd = await PDVParametersRepository.Find();
                if (existsLxd != null)
                {
                    throw new Exception(message: "Parâmetros de PDV já cadastrados.");
                }
                affectedRows = await PDVParametersRepository.Insert(lxd);
            }
            else
            {
                affectedRows = await PDVParametersRepository.Update(lxd);
            }

            if (lxd.Lxdintesho && lxd.Lxdtipinsh != MallIntegrationType.None)
            {
                await ParametersRepository.CreateTableTecdatasoft();
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete()
        {
            int affectedRows = await PDVParametersRepository.Delete();

            return affectedRows == 1;
        }
    }
}
