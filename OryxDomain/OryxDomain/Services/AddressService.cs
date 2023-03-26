using Microsoft.Extensions.Configuration;
using OryxDomain.Http;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class AddressService
    {
        private readonly FormatterService FormatterService;
        private readonly AddressRepository AddressRepository;
        private readonly IConfiguration Configuration;
        private readonly APIParametersRepository APIParametersRepository;
        public AddressService(IConfiguration configuration)
        {
            Configuration = configuration;
            FormatterService = new FormatterService(configuration);
            AddressRepository = new AddressRepository(Configuration["OryxPath"] + "oryx.ini");
            APIParametersRepository = new APIParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<bool> VerifyAndSaveAddress(string cep)
        {
            LXE lxe = await APIParametersRepository.Find();
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");

            string cepClient = cep;

            CF2 cF2Oryx = await AddressRepository.FindCf2ByCep(cep);
            CF3 cF3 = null;
            CF2 newCf2 = null;

            if (cF2Oryx == null)
            {

                try
                {
                    FindCf2CF3ByCep(lxe.Lxeapicep, in cepClient, out newCf2, out cF3);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (newCf2 != null)
                {
                    newCf2.Cf2cep = newCf2.Cf2cep.Replace("-", "");

                    await SaveAddress(newCf2, cF3);
                    return true;
                }
                return false;
            }
            //comentado validação do CEP pela API
            //if (newCf2 == null)
            //{
            //    throw new Exception(string.Format("CEP {0} não localizado ou inválido", cep));
            //}
            return true;
        }

        public async Task<bool> VerifyAndSaveAddressFromShopping(CF2 cf2, CF3 cf3)
        {
            await VerifyAndSaveAddress(cf2.Cf2cep);
            CF2 cF2Oryx = await AddressRepository.FindCf2ByCep(cf2.Cf2cep);
            if (cF2Oryx == null)
                await SaveAddress(cf2, cf3);
            return true;
        }

        private void FindCf2CF3ByCep(string urlapicep, in string cep, out CF2 cf2, out CF3 cf3)
        {
            Validators.ValidateCep(cep);

            if (!urlapicep.EndsWith("/"))
                urlapicep += "/";

            string cepReturn = HttpUtilities.GetAsync(urlapicep, string.Format("{0}/json/", cep)).Result;

            CepModel cepModel = JsonSerializer.Deserialize<CepModel>(cepReturn);
            cf2 = null;
            cf3 = null;
            if (!cepModel.erro)
            {
                cf2 = new CF2();
                cf2.Cf2cep = cepModel.cep;
                cf2.Cf2local = cepModel.ibge;
                cf2.Cf2logra = cepModel.logradouro;

                cf3 = new CF3();
                cf3.Cf3estado = cepModel.uf;
                cf3.Cf3local = cepModel.ibge;
                cf3.Cf3nome = cepModel.localidade;
            }
        }

        private async Task SaveAddress(CF2 cf2, CF3 cf3)
        {
            cf2.Cf2logra = Formatters.RemoveSpecialCharacterByNames(cf2.Cf2logra);

            await FormatterService.ValidateFormatBasicByDC1(cf2);
            _ = await AddressRepository.InsertCf2(cf2);

            CF3 existsCf3 = await AddressRepository.FindCf3ByIbge(cf2.Cf2local);
            if (existsCf3 == null)
            {
                cf3.Cf3nome = Formatters.RemoveSpecialCharacterByNames(cf3.Cf3nome);

                await FormatterService.ValidateFormatBasicByDC1(cf3);

                _ = await AddressRepository.InsertCf3(cf3);
            }
        }
    }
}
