using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dictionary.Services
{
    public class DictionaryService
    {
        readonly IConfiguration Configuration;
        readonly DictionaryRepository DictionaryRepository;
        private readonly FormatterService FormatterService;
        public DictionaryService(IConfiguration configuration)
        {
            Configuration = configuration;
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
            FormatterService = new FormatterService(Configuration);
        }

        public async Task<IList<DC1>> FindDC1ByDc1arquivo(string dc1arquivo)
        {
            IList<DC1> lstDc1 = await DictionaryRepository.FindDC1ByDc1arquivo(dc1arquivo);
            if (lstDc1 == null || lstDc1.Count == 0)
            {
                throw new Exception(message: string.Format("Dicionário dos campos da tela {0} não cadastrados.", dc1arquivo));
            }
            return lstDc1;
        }

        public async Task<DC0> FindDC0ByDc0arquivo(string dc0arquivo)
        {
            DC0 dc0 = await DictionaryRepository.FindDC0ByDc0arquivo(dc0arquivo);
            if (dc0 == null)
            {
                throw new Exception(message: string.Format("Dicionário da tela {0} não cadastrados.", dc0arquivo));
            }
            return dc0;
        }

        public async Task<IList<DC1>> FindDC1ByLstDc1arquivo(IList<string> lstDc1arquivo)
        {
            if (lstDc1arquivo == null || lstDc1arquivo.Count == 0)
            {
                return new List<DC1>();
            }
            IList<DC1> lstDc1 = await DictionaryRepository.FindDC1ByLstDc1arquivo(lstDc1arquivo);
            if (lstDc1 == null || lstDc1.Count == 0)
            {
                //throw new Exception(message: string.Format("Dicionários dos campos da tela {0} não cadastrados.", string.Join(", ", lstDc1arquivo)));
            }
            return lstDc1;
        }

        public async Task<IList<DC0>> FindDC0ByLstDc0arquivo(IList<string> lstDc0arquivo)
        {
            if (lstDc0arquivo == null || lstDc0arquivo.Count == 0)
            {
                return new List<DC0>();
            }
            IList<DC0> dc0 = await DictionaryRepository.FindDC0ByLstDc0arquivo(lstDc0arquivo);
            if (dc0 == null)
            {
                throw new Exception(message: string.Format("Dicionários da tela {0} não cadastrados.", string.Join(", ", lstDc0arquivo)));
            }
            return dc0;
        }
        
        public async Task<string> GetNextNumber(string field, bool findLastUsed)
        {
            DC1 dc1 = await DictionaryRepository.FindDC1ByDc1campo(field);
            if (dc1 == null)
                throw new Exception("Campo não cadastrado");

            if (dc1.Dc1ultimo.Equals("9".PadLeft(dc1.Dc1tamanho, '9')))
                throw new Exception(string.Format("Sequência de campo {0} esgotada.", field));

            //verificando o ultimo numero usado
            string nextNumber = await FormatterService.GetNextNumber(field);


            if (findLastUsed)
            {
                string lastUsed = await DictionaryRepository.FindLasUsed(field, dc1.Dc1arquivo);
                if (!string.IsNullOrWhiteSpace(lastUsed))
                {
                    Int64 number = Convert.ToInt64(lastUsed);
                    number++;
                    nextNumber = Formatters.FormatId(number.ToString(), dc1.Dc1tamanho);
                }
            }

            await DictionaryRepository.SaveNextNumber(field, nextNumber);

            return nextNumber;
        }
    }
}
