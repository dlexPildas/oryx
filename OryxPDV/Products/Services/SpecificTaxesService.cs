using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using OryxDomain.Utilities;
using Products.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Services
{
    public class SpecificTaxesService
    {
        private readonly IConfiguration Configuration;
        private readonly SpecificTaxesRepository SpecificTaxesRepository;
        private readonly BusinessOperationRepository BusinessOperationRepository;
        private readonly ParametersRepository ParametersRepository;

        public SpecificTaxesService(IConfiguration configuration)
        {
            Configuration = configuration;
            SpecificTaxesRepository = new SpecificTaxesRepository(Configuration["OryxPath"] + "oryx.ini");
            BusinessOperationRepository = new BusinessOperationRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<CVN>> FindList(string cvnproduto)
        {
            IList<CVN> lstcvn = await SpecificTaxesRepository.FindList(cvnproduto);

            return lstcvn;
        }

        public async Task<CVN> Find(string cvnopercom, string cvnproduto = "", string cvninsumo = "")
        {
            CVN cvn = await SpecificTaxesRepository.Find(cvnopercom, cvnproduto, cvninsumo);

            if (cvn == null)
            {
                throw new Exception(message: "Tributação específica não cadastrada.");
            }
            return cvn;
        }

        public async Task<bool> Save(CVN cvn, bool forUpdate = false)
        {
            int affectedRows;
            await ValidateCvn(cvn);

            if (!forUpdate)
            {
                affectedRows = await SpecificTaxesRepository.Insert(cvn);
            }
            else
            {
                affectedRows = await SpecificTaxesRepository.Update(cvn);
            }

            return affectedRows == 1;
        }

        public async Task<bool> Delete(string cvnopercom, string cvnproduto = "", string cvninsumo = "")
        {
            int affectedRows = await SpecificTaxesRepository.Delete(cvnopercom, cvnproduto, cvninsumo);
            return affectedRows == 1;
        }

        public async Task<SearchPayloadModel<CVN>> Search(string search, int limit, int page)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;

            IList<CVN> specifictaxes = await SpecificTaxesRepository.Search(search, limit, (page - 1) * limit);
            IList<CVN> nextspecifictaxes = await SpecificTaxesRepository.Search(search, limit, page * limit);

            return new SearchPayloadModel<CVN>()
            {
                Items = specifictaxes,
                Limit = limit,
                HasNext = nextspecifictaxes != null && nextspecifictaxes.Count > 0
            };
        }

        public async Task<IList<GenericItemModel>> GetGenericItens(string cvnopercom)
        {
            var cv3 = await BusinessOperationRepository.Find(cvnopercom);

            if (cv3.Cv3itemfab != SalesItemType.MATERIALS)
            {
                return await SpecificTaxesRepository.GetGenericProductsList(cvnopercom);
            }
            else
            {
                return await SpecificTaxesRepository.GetGenericSupplyList(cvnopercom);
            }
        }

        private async Task ValidateCvn(CVN cvn)
        {
            await new FormatterService(Configuration).ValidateFormatBasicByDC1(cvn);

            CVN existsCVN = await SpecificTaxesRepository.Find(cvn.Cvnopercom, cvn.Cvnproduto, cvn.Cvninsumo);

            if (existsCVN != null)
            {
                throw new Exception(message: "Tributação Específica já cadastrada.");
            }

            LX0 lx0 = await ParametersRepository.GetLx0();
            if (lx0 == null)
                throw new Exception("Parâmetros Gerais não cadastrados.");

            var cv3 = await BusinessOperationRepository.Find(cvn.Cvnopercom);

            if (cv3.Cv3emissao == true)
            {
                if (lx0.Lx0crt == TaxRegimeType.SIMPLE_NATIONAL && cvn.Cvncst.Trim().Length != 3)
                {
                    throw new Exception("CST não é compatível ao Regime Tributário.");
                }

                if (cvn.Cvncst.Trim().Length != 2
                    && lx0.Lx0crt != TaxRegimeType.NONE
                    && lx0.Lx0crt != TaxRegimeType.SIMPLE_NATIONAL
                    && lx0.Lx0crt != TaxRegimeType.SIMPLE_NATIONAL_EXCESS)
                {
                    throw new Exception("CST não é compatível ao Regime Tributário.");
                }
            }

            if (cvn.Cvncst == Constants.CstCson.GetValueOrDefault("20") && cvn.Cvnbaseicm >= Convert.ToInt16(Constants.CstCson.GetValueOrDefault("100")))
            {
                throw new Exception("Base ICMS incompatível com CST (Redução).");
            }

            if ((int)cv3.Cv3entsai <= 2 && string.IsNullOrEmpty(cvn.Cvncfop))
            {
                throw new Exception("Natureza da Operação é obrigatória para Operações com fluxo de Mercadorias.");
            }

            if ((int)cv3.Cv3entsai > 2 && string.IsNullOrEmpty(cvn.Cvncfop))
            {
                throw new Exception("Não existe CFOP para operações exclusivamente financeiras.");
            }

            if (cv3.Cv3cfop.Substring(1,1) != cvn.Cvncfop.Substring(1,1))
            {
                throw new Exception("CFOP incompatível com operação principal.");
            }

            var tipotes = cvn.Cvncfop.Substring(1, 1);

            if (cv3.Cv3entsai == GoodsFlowType.GOODS_EXIT
                && tipotes != "5"
                && tipotes != "6"
                && tipotes != "7")
            {
                throw new Exception("CFOP inválido para Saídas.");
            }

            if (cv3.Cv3entsai == GoodsFlowType.GOODS_ENTRY
                && tipotes != "1"
                && tipotes != "2"
                && tipotes != "3")
            {
                throw new Exception("CFOP inválido para Entradas.");
            }

            if (cvn.Cvnbaseicm > 0 && (cvn.Cvncst == Constants.CstCson.GetValueOrDefault("40") ||
                cvn.Cvncst == Constants.CstCson.GetValueOrDefault("41") ||
                cvn.Cvncst == Constants.CstCson.GetValueOrDefault("50") ||
                cvn.Cvncst == Constants.CstCson.GetValueOrDefault("60")))
            {
                throw new Exception("Base de ICMS informada não é compatível com a CST.");
            }

            if (cvn.Cvnbaseicm > 0 && (cvn.Cvncst == Constants.CstCson.GetValueOrDefault("101") ||
                cvn.Cvncst == Constants.CstCson.GetValueOrDefault("102") ||
                cvn.Cvncst == Constants.CstCson.GetValueOrDefault("201") ||
                cvn.Cvncst == Constants.CstCson.GetValueOrDefault("202") ||
                cvn.Cvncst == Constants.CstCson.GetValueOrDefault("500")))
            {
                throw new Exception("Base de ICMS informada não é compatível com a CSOSN.");
            }

            if (cvn.Cvnbasesub > 0 && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("10")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("30")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("60")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("70")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("90")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("201")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("202")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("203")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("500")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("90")
                )
            {
                throw new Exception("Substituição tributária não é compatível com a CST/CSOSN.");
            }

            if (cv3.Cv3credsim > 0 && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("101")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("201")
                && cvn.Cvncst != Constants.CstCson.GetValueOrDefault("900")
                && !cv3.Cv3cupom)
            {
                throw new Exception("Alíquota de Crédito Simples não aplicável a CST/CSOSN.");
            }

            if (cvn.Cvnbaseicm + cvn.Cvnbaseise + cvn.Cvnbaseout != 100)
            {
                throw new Exception("Total de bases de cálculo difere de 100%.");
            }

            if (cvn.Cvnaliqipi > 0 && cvn.Cvncstipi != Constants.CstIpi.GetValueOrDefault("00")
                && cvn.Cvncstipi != Constants.CstIpi.GetValueOrDefault("49")
                && cvn.Cvncstipi != Constants.CstIpi.GetValueOrDefault("50")
                && cvn.Cvncstipi != Constants.CstIpi.GetValueOrDefault("99"))
            {
                throw new Exception("Alíquota IPI incompatível com CST.");
            }

            if(!string.IsNullOrEmpty(cvn.Cvncontad) && string.IsNullOrEmpty(cvn.Cvncontac))
            {
                throw new Exception("Faltou informar conta crédito");
            }

            if (!string.IsNullOrEmpty(cvn.Cvncontac) && string.IsNullOrEmpty(cvn.Cvncontad))
            {
                throw new Exception("Faltou informar conta débito");
            }

            if (!string.IsNullOrEmpty(cvn.Cvncontad) && !string.IsNullOrEmpty(cv3.Cv3contad))
            {
                throw new Exception("Conta débito já informada no cadastro da operação.");
            }

            if (!string.IsNullOrEmpty(cvn.Cvncontad))
            {
                if (cv3.Cv3entsai == GoodsFlowType.GOODS_ENTRY)
                {
                    if (await SpecificTaxesRepository.FindByContad(cvn) != null)
                    {
                        throw new Exception($"Já existe outro item com conta débito diferente {cvn.Cvncontad} na liquidação.");
                    }
                }
            }

            if (!string.IsNullOrEmpty(cvn.Cvncontac) && !string.IsNullOrEmpty(cv3.Cv3contac))
            {
                throw new Exception("Conta crédito já informada no cadastro da operação.");
            }
            
            if (!string.IsNullOrEmpty(cvn.Cvncontac))
            {
                if (cv3.Cv3entsai == GoodsFlowType.GOODS_EXIT)
                {
                    if (await SpecificTaxesRepository.FindByContac(cvn) != null)
                    {
                        throw new Exception($"Já existe outro item com conta crédito diferente {cvn.Cvncontad} na liquidação.");
                    }
                }
            }
        }
        
        public async Task<bool> RepeatSpecificTaxe(RepeatSpecificTaxeModel repeatspecifictaxe)
        {
            CVN findedcvn = await Find(repeatspecifictaxe.Cvnopercom, repeatspecifictaxe.Cvnproduto, repeatspecifictaxe.Cvninsumo);

            CV3 cv3 = await BusinessOperationRepository.Find(findedcvn.Cvnopercom);
            if(cv3 == null)
            {
                throw new Exception("Operação comercial não encontrada.");
            }

            IList<CVN> specifictaxetoinsert = new List<CVN>();

            foreach (var code in repeatspecifictaxe.CodeLst)
            {
                if (cv3.Cv3itemfab != SalesItemType.MATERIALS)
                {
                    findedcvn.Cvnproduto = code;
                }
                else
                {
                    findedcvn.Cvninsumo = code;
                }

                await ValidateCvn(findedcvn);
                specifictaxetoinsert.Add(findedcvn);
            }

            foreach (var specifictaxe in specifictaxetoinsert)
            {
                await SpecificTaxesRepository.Insert(specifictaxe);
            }

            return true;
        }
    }
}
