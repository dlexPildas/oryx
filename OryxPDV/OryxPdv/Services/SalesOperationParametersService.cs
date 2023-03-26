using OryxDomain.Repository;
using OryxDomain.Utilities;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OryxPdv.Services
{
    public class SalesOperationParametersService
    {
        private readonly IConfiguration Configuration;
        private readonly SalesOperationParametersRepository SalesOperationParametersRepository;
        private readonly FiscalDocumentTypeRepository FiscalDocumentTypeRepository;

        public SalesOperationParametersService(IConfiguration configuration)
        {
            Configuration = configuration;
            SalesOperationParametersRepository = new SalesOperationParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalDocumentTypeRepository = new FiscalDocumentTypeRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<PD2> Find(int pd2codigo)
        {
            PD2 pd2 = await SalesOperationParametersRepository.Find(pd2codigo);

            if (pd2 == null)
            {
                throw new Exception(message: "Parâmetro operacional de venda não encontrado.");
            }
            return pd2;
        }

        public async Task<IList<PD2>> FindAll()
        {
            IList<PD2> lstPd1 = await SalesOperationParametersRepository.FindAll();
            return lstPd1;
        }

        public async Task<string> FindOperCom(SalesOperationType pd2tipoope, string pd2tipo, OperationType pd2estadua, bool pd2contrib, bool pd2emispro, string cv4estado)
        {
            string opercom;

            CV1 model = await FiscalDocumentTypeRepository.Find(pd2tipo);
            if (model.Cv1modelo.Equals(Constants.FiscalModels.GetValueOrDefault("DOCUMENTO_INTERNO")))
            {
                opercom = await SalesOperationParametersRepository.FindOperCom(pd2tipoope, pd2tipo, pd2estadua, pd2contrib, pd2emispro);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cv4estado))
                {
                    throw new Exception("Cadastro do cep do cliente inválido. Revise o cadastro do cliente e tente novamente.");
                }
                opercom = await SalesOperationParametersRepository.FindOperCom(pd2tipoope, pd2tipo, pd2estadua, pd2contrib, pd2emispro, cv4estado);
            }

            if (string.IsNullOrWhiteSpace(opercom))
            {
                throw new Exception(message: string.Format("Operação não permitida ou não cadastrada para o registro atual: {0}, {1}, {2}, {3}, {4}, {5}.", pd2tipoope.GetEnumDescription(), pd2tipo, pd2estadua.GetEnumDescription(), pd2contrib ? "Contribuinte" : "Não contribuinte", pd2emispro ? "Emissão própria" : "Emissão de terceiros", cv4estado));
            }
            return opercom;
        }

        public async Task<bool> Save(PD2 pd2, bool forUpdate = false)
        {
            int affectedRows;
            await new FormatterService(Configuration).ValidateFormatBasicByDC1(pd2);
            //string opercom = await SalesOperationParametersRepository.FindOperCom(pd2.Pd2tipoope, pd2.Pd2tipo, pd2.Pd2estadua, pd2.Pd2contrib, pd2.Pd2emispro ,pd2.Pd2codigo);
            //if (!string.IsNullOrWhiteSpace(opercom))
            //{
            //    throw new Exception(message: string.Format("Parâmetros operacionais de venda já cadastrados para a combinação: {0}, {1}, {2}, {3}, {4}.", pd2.Pd2tipoope.GetEnumDescription(), pd2.Pd2tipo, pd2.Pd2estadua.GetEnumDescription(), pd2.Pd2contrib ? "Contribuinte" : "Não contribuinte", pd2.Pd2emispro ? "Emissão própria" : "Emissão de terceiros"));
            //}
            if (!forUpdate)
            {
                affectedRows = await SalesOperationParametersRepository.Insert(pd2);
            }
            else
            {
                affectedRows = await SalesOperationParametersRepository.Update(pd2);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(int pd2codigo)
        {
            int affectedRows = await SalesOperationParametersRepository.Delete(pd2codigo);

            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<PD2>> Search(int? pd2codigo, SalesOperationType? pd2tipoope, string pd2tipo, OperationType? pd2estadua, bool? pd2contrib, string pd2opercom, bool? pd2emispro, int limit, int page)
        {
            IList<PD2> lstPd2 = await SalesOperationParametersRepository.Search(pd2codigo, pd2tipoope, pd2tipo, pd2estadua, pd2contrib, pd2emispro, pd2opercom, limit, (page - 1) * limit);
            IList<PD2> nextPd2 = await SalesOperationParametersRepository.Search(pd2codigo, pd2tipoope, pd2tipo, pd2estadua, pd2contrib, pd2emispro, pd2opercom, limit, page * limit);
            return new SearchPayloadModel<PD2>()
            {
                Items = lstPd2,
                Limit = limit,
                HasNext = nextPd2 != null && nextPd2.Count > 0
            };
        }
    }
}
