using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Exceptions;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class SisService
    {
        private readonly IConfiguration Configuration;
        private readonly PaymentConditionRepository PaymentConditionRepository;
        private readonly LogisticIntegrationService FinancialParametersRepository;
        private readonly ParametersRepository ParametersRepository;
        private readonly PDVParametersRepository PDVParametersRepository;
        private readonly CustomerRepository CustomerRepository;
        private readonly SisRepository SisRepository;
        private readonly ProductRepository ProductRepository;
        private readonly EanCodificationRepository EanCodificationRepository;
        private readonly FormatterService FormatterService;
        public SisService(IConfiguration configuration)
        {
            Configuration = configuration;
            PaymentConditionRepository = new PaymentConditionRepository(Configuration["OryxPath"] + "oryx.ini");
            FinancialParametersRepository = new FinancialParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            CustomerRepository = new CustomerRepository(Configuration["OryxPath"] + "oryx.ini");
            SisRepository = new SisRepository(Configuration["OryxPath"] + "oryx.ini");
            ProductRepository = new ProductRepository(Configuration["OryxPath"] + "oryx.ini");
            EanCodificationRepository = new EanCodificationRepository(Configuration["OryxPath"] + "oryx.ini");
            PDVParametersRepository = new PDVParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            FormatterService = new FormatterService(configuration);
        }

        public async Task<int> PrazoMedio(string conpgto)
        {
            int somadias = 0;
            int nParcelas = 0;
            int prazoDaParcela = 0;

            IList<VD9> lstVd9 = await PaymentConditionRepository.FindVd9List(conpgto);
            foreach (var vd9 in lstVd9)
            {
                prazoDaParcela += vd9.Vd9caren;
                somadias += prazoDaParcela;
                nParcelas++;
            }

            if (nParcelas > 0 && somadias > 0)
            {
                return somadias / nParcelas;
            }
            return 0;
        }

        public async Task<string> ClienteOk(string cpfCnpj, bool silent = true)
        {
            LXA lxa = await FinancialParametersRepository.Find();
            if (lxa == null)
                throw new Exception("Parâmetros financeiros não cadastrados");
            LXD lxd = await PDVParametersRepository.Find();
            if (lxd == null)
                throw new Exception("Parâmetros de PDV não cadastrados");

            string payload = "";
            if (!lxa.Lxanaocons && !lxd.Lxdcliente.Equals(cpfCnpj))
            {
                //verificando titulos em aberto
                IList<CV8> lstOpenTitles = await CustomerRepository.FindOpenTitles(cpfCnpj, DateTime.Now);
                foreach (CV8 cv8 in lstOpenTitles)
                {
                    cv8.Cv8valor = await SaldoTitulo(cv8.Cv8emissor, cv8.Cv8tipo, cv8.Cv8doc, cv8.Cv8parcela, cv8.Cv8valor);
                }

                lstOpenTitles = lstOpenTitles.Where(a => a.Cv8valor > 0).ToList();
                if (lstOpenTitles.Any())
                {
                    CV8 cv8OpenTitle = lstOpenTitles.First();
                    if (!silent)
                    {
                        throw new WarningException(string.Format("Titulo {0}-{1} em aberto desde {2}", cv8OpenTitle.Cv8doc, cv8OpenTitle.Cv8parcela, cv8OpenTitle.Cv8vencim.ToString("dd/MM/yyyy")));
                    }
                    payload = string.Format("Título {0}-{1} vencido em {2}", cv8OpenTitle.Cv8doc, cv8OpenTitle.Cv8parcela, cv8OpenTitle.Cv8vencim.ToString("dd/MM/yyyy"));
                }
            }

            IList<CVS> blocks = await CustomerRepository.FindBlocksByCvsfatur(cpfCnpj);
            if (blocks.Count > 0)
            {
                CVS cvs = blocks.First();
                payload += string.Format("{0}Bloqueio ativado por {1} em {2}. Motivo: {3}", string.IsNullOrWhiteSpace(payload) ? "" : "\n", cvs.Cvsusuario, cvs.Cvsabert.ToString("dd/MM/yyyy"), cvs.Cvsmotivob);
                if (!silent)
                    throw new BlockException(payload);
            }

            return payload;
        }

        public string EanDigito(string eancodigo)
        {
            int nTam = eancodigo.Length;
            string sequencia = "1313131313131";
            sequencia = sequencia.Substring(0, nTam);
            decimal soma = 0;
            for (int i = 0; i < eancodigo.Length; i++)
            {
                soma += Convert.ToInt32(eancodigo.Substring(i,1)) * Convert.ToInt32(sequencia.Substring(i,1));
            }
            decimal decimosuperior = soma / 10;
            if ((int)decimosuperior != decimosuperior)
            {
                decimosuperior = ((int)decimosuperior * 10) + 10;
            }
            else
            {
                decimosuperior *= 10;
            }

            return ((int)(decimosuperior - soma)).ToString();
        }

        public async Task<decimal> SaldoTitulo(string cv8emissor, string cv8tipo, string cv8doc, string cv8parcela, decimal cv8valor, DateTime? dateBase = null, bool recebivel = true)
        {
            decimal valorinicial = cv8valor;

            IList<BondSettled> lstBondsSettled = await SisRepository.FindAllBondsSettled(cv8emissor, cv8tipo, cv8doc, cv8parcela, recebivel ? "" : " AND FN5TIPO<>1 ");

            if (dateBase.HasValue)
            {
                lstBondsSettled = lstBondsSettled.Where(a => a.Fn5abert <= dateBase.Value).ToList();
            }

            valorinicial -= lstBondsSettled.Sum(a => a.Fn6valor);

            return valorinicial;
        }

        public async Task<string> Ean(string produto, string opcao, string tamanho, bool interno = false, string prefixo = "", int codFinal = 0)
        {
            string code = string.Empty;
            PR0 pr0 = await ProductRepository.Find<PR0>(produto);
            if (pr0 != null && !string.IsNullOrWhiteSpace(pr0.Pr0ean))
                return pr0.Pr0ean;
            
            EAN ean = await EanCodificationRepository.FindEan(produto, opcao, tamanho);
            if (ean != null)
                return ean.Eancodigo;

            if (string.IsNullOrWhiteSpace(prefixo))
            {
                LX0 lx0 = await ParametersRepository.GetLx0();
                prefixo = lx0.Lx0ean.Trim();
            }
            int eansize = await FormatterService.FindFieldLength("EANCODIGO");
            if (!interno && !string.IsNullOrWhiteSpace(prefixo))
            {
                IList<string> lstCodes = (await EanCodificationRepository.FindAllFixedCodes(prefixo)).Concat(await EanCodificationRepository.FindAllAutomaticCodes(prefixo)).ToList();
                codFinal = Convert.ToInt32(string.Format("1{0}", new string('0', eansize)))-1;
                for (int i = 1; i < codFinal; i++)
                {
                    code = string.Format("{0}{1}", prefixo, i.ToString().PadLeft(eansize-prefixo.Length, '0'));
                    if (lstCodes.Where(c => c.Equals(code)).Any())
                        break;
                    code = "";
                }
            }
            else
            {
                //criando codigo interno
                IList<string> lstCodes = await EanCodificationRepository.FindAllInternalEan(eansize);
                for (int i = 1; i < 999999; i++)
                {
                    code = string.Format("2{0}{1}", new string('0', eansize-7), (i+ 1000000).ToString().Substring(1,6));
                    if (lstCodes.Where(c => c.Equals(code)).Any())
                        break;
                    code = "";
                }
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                return code;
                //throw new Exception(string.Format("Sequência de Código Ean esgotada. {0}", string.IsNullOrWhiteSpace(prefixo) ? "" : "Prefixo: "+prefixo));
            }
            //incluindo codigo
            await EanCodificationRepository.Insert(new EAN()
            {
                Eancodigo = code,
                Eanproduto = produto,
                Eanopcao = opcao,
                Eantamanho = tamanho
            });
            return code;
        }

        public async Task<string> CodigoDFE(string cv5doc)
        {
            int cv5docSize = await FormatterService.FindFieldLength(nameof(cv5doc));
            string result = (Convert.ToInt64(new string('9', cv5docSize)) - Convert.ToInt64(cv5doc)).ToString().PadLeft(cv5docSize, '0');
            result = Modulo11(result) + result;
            result += Modulo11(result);
            return result;
        }

        public string Modulo11(string number, bool febraban = false)
        {
            int produto = 0, multiplicador = 2, payload;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                produto += Convert.ToInt32(number.Substring(i, 1)) * multiplicador;
                multiplicador = multiplicador == 9 ? 2 : multiplicador + 1;
            }

            //EXCEÇÃO
            produto = produto % 11;
            payload = 11 - produto;
            if (febraban)
            {
                payload = produto == 11 || produto == 10 || produto == 1 || produto == 0 ? 1 : payload;
            }
            else
            {
                payload = payload == 10 || payload == 11 ? 0 : payload;
            }
            return payload.ToString();
        }

        public void EmailValido(string receiver)
        {
            IList<string> lstMails = receiver.Split(';');
            foreach (string mail in lstMails)
            {
                if (!Validators.IsEMail(mail))
                    throw new Exception(string.Format("E-mail {0} inválido", mail));
            }
        }
    }
}
