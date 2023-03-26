using Microsoft.Extensions.Configuration;
using Order.Models;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Services;
using System;
using System.Threading.Tasks;

namespace Order.Services
{
    public class SalesService
    {
        private readonly IConfiguration Configuration;
        private readonly PrintingPreferencesRepository PrintingPreferencesRepository;
        private readonly PrintService PrintService;
        private readonly FormatterService FormatterService;
        public SalesService(IConfiguration configuration)
        {
            Configuration = configuration;
            PrintingPreferencesRepository = new PrintingPreferencesRepository(Configuration["OryxPath"] + "oryx.ini");
            PrintService = new PrintService(Configuration);
            FormatterService = new FormatterService(Configuration);
        }

        public async Task<string> PrintReportSalesByPeriod(PrintReportSalesByPeriod model, string terminal, bool print, string authorization)
        {
            string dc9relat = "329";
            string pd1impres = "Bullzip PDF Printer";

            int cv5represSize = await FormatterService.FindFieldLength("cv5repres");
            int cv8tipotitSize = await FormatterService.FindFieldLength("pd0codigo");

            if (print)
            {
                PD1 pd1 = await PrintingPreferencesRepository.Find(terminal, dc9relat);
                if (pd1 == null)
                {
                    print = false;
                    //throw new Exception(string.Format("Impressora não definida para o terminal {0} e relatório {1}", terminal, dc9relat));
                }
                else
                {
                    if (pd1.Pd1vias == 0)
                        throw new Exception(string.Format("Número de vias não definido para o terminal {0} e relatório {1}", terminal, dc9relat));
                    pd1impres = pd1.Pd1impres;
                }
            }

            model.FromCv5repres = string.IsNullOrWhiteSpace(model.FromCv5repres) ? "EMPTY" : model.FromCv5repres;
            model.ToCv5repres = string.IsNullOrWhiteSpace(model.ToCv5repres) ? "9".PadLeft(cv5represSize, '9') : model.ToCv5repres;

            model.FromCv5tipo = string.IsNullOrWhiteSpace(model.FromCv5tipo) ? "EMPTY" : model.FromCv5tipo;
            model.ToCv5tipo = string.IsNullOrWhiteSpace(model.ToCv5tipo) ? "Z".PadLeft(cv8tipotitSize, 'Z') : model.ToCv5tipo;

            if (string.IsNullOrWhiteSpace(model.Cv5clinome))
                model.Cv5clinome = "EMPTY";

            string pdfName = string.Format("RELAT_{0}_{1}.pdf", dc9relat, DateTime.Now.Ticks.ToString());

            PrintModel printModel = new PrintModel()
            {
                Relat = dc9relat,
                Params = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", pd1impres, "S", model.FromCv5emissao.ToString("dd/MM/yyyy"), model.ToCv5emissao.ToString("dd/MM/yyyy"), model.FromCv5tipo, model.ToCv5tipo, model.FromCv5repres, model.ToCv5repres, model.Cv5clinome, model.OnlyCashier ? "1" : "0", pdfName)
            };

            await PrintService.Print(printModel, authorization);

            return "/Printing/Print/Document/" + pdfName;
        }

        public async Task<string> PrintReportProductsSoldByPeriod(ProductsSoldByPeriod model, string terminal, bool print, string authorization)
        {
            string dc9relat = "328";
            string pd1impres = "Bullzip PDF Printer";

            int cv5tipoSize = await FormatterService.FindFieldLength("cv5tipo");
            int pr0colecaoSize = await FormatterService.FindFieldLength("pr0colecao");
            int pr0etiqSize = await FormatterService.FindFieldLength("pr0etiq");
            int pr0familiaSize = await FormatterService.FindFieldLength("pr0familia");
            int cv7codigoSize = await FormatterService.FindFieldLength("cv7codigo");
            int cv5clienteSize = await FormatterService.FindFieldLength("cv5cliente");

            if (print)
            {
                PD1 pd1 = await PrintingPreferencesRepository.Find(terminal, dc9relat);
                if (pd1 == null)
                {
                    print = false;
                    //throw new Exception(string.Format("Impressora não definida para o terminal {0} e relatório {1}", terminal, dc9relat));
                }
                else
                {
                    if (pd1.Pd1vias == 0)
                        throw new Exception(string.Format("Número de vias não definido para o terminal {0} e relatório {1}", terminal, dc9relat));
                    pd1impres = pd1.Pd1impres;
                }
            }

            model.FromCv5tipo = string.IsNullOrWhiteSpace(model.FromCv5tipo) ? "EMPTY" : model.FromCv5tipo;
            model.ToCv5tipo = string.IsNullOrWhiteSpace(model.ToCv5tipo) ? "Z".PadLeft(cv5tipoSize, 'Z') : model.ToCv5tipo;

            model.FromPr0colecao = string.IsNullOrWhiteSpace(model.FromPr0colecao) ? "EMPTY" : model.FromPr0colecao;
            model.ToPr0colecao = string.IsNullOrWhiteSpace(model.ToPr0colecao) ? "9".PadLeft(pr0colecaoSize, '9') : model.ToPr0colecao;

            model.FromPr0etiq = string.IsNullOrWhiteSpace(model.FromPr0etiq) ? "EMPTY" : model.FromPr0etiq;
            model.ToPr0etiq = string.IsNullOrWhiteSpace(model.ToPr0etiq) ? "9".PadLeft(pr0etiqSize, '9') : model.ToPr0etiq;

            model.FromPr0familia = string.IsNullOrWhiteSpace(model.FromPr0familia) ? "EMPTY" : model.FromPr0familia;
            model.ToPr0familia = string.IsNullOrWhiteSpace(model.ToPr0familia) ? "9".PadLeft(pr0familiaSize, '9') : model.ToPr0familia;

            model.FromCv7codigo = string.IsNullOrWhiteSpace(model.FromCv7codigo) ? "EMPTY" : model.FromCv7codigo;
            model.ToCv7codigo = string.IsNullOrWhiteSpace(model.ToCv7codigo) ? "Z".PadLeft(cv7codigoSize, 'Z') : model.ToCv7codigo;

            model.FromCv5cliente = string.IsNullOrWhiteSpace(model.FromCv5cliente) ? "EMPTY" : model.FromCv5cliente;
            model.ToCv5cliente = string.IsNullOrWhiteSpace(model.ToCv5cliente) ? "Z".PadLeft(cv5clienteSize, 'Z') : model.ToCv5cliente;

            if (string.IsNullOrWhiteSpace(model.Cf3regiao))
                model.Cf3regiao = "EMPTY";

            string pdfName = string.Format("RELAT_{0}_{1}.html", dc9relat, DateTime.Now.Ticks.ToString());

            PrintModel printModel = new PrintModel()
            {
                Relat = dc9relat,
                Params = string.Format(
                      "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22}"
                    , pd1impres
                    , "S"
                    , model.FromCv5emissao.ToString("dd/MM/yyyy")
                    , model.ToCv5emissao.ToString("dd/MM/yyyy")
                    , model.FromCv5tipo
                    , model.ToCv5tipo
                    , model.FromPr0colecao
                    , model.ToPr0colecao
                    , model.FromPr0etiq
                    , model.ToPr0etiq
                    , model.FromPr0familia
                    , model.ToPr0familia
                    , model.FromCv7codigo
                    , model.ToCv7codigo
                    , model.FromCv5cliente
                    , model.ToCv5cliente
                    , model.DetailGrid ? "1" : "0"
                    , model.Cost ? "1" : "0"
                    , model.People
                    , model.Products
                    , model.Order
                    , model.Cf3regiao
                    , "html\\" + pdfName)
            };

            await PrintService.Print(printModel, authorization);

            return "/Printing/Print/Document/" + pdfName;
        }

        public async Task<string> PrintReportSalesRanking(SalesRankingModel model, string terminal, bool print, string authorization)
        {
            string dc9relat = "330";
            string pd1impres = "Bullzip PDF Printer";

            int cv5tipoSize = await FormatterService.FindFieldLength("cv5tipo");
            int cv5repreSize = await FormatterService.FindFieldLength("cv5repres");

            if (print)
            {
                PD1 pd1 = await PrintingPreferencesRepository.Find(terminal, dc9relat);
                if (pd1 == null)
                {
                    print = false;
                    //throw new Exception(string.Format("Impressora não definida para o terminal {0} e relatório {1}", terminal, dc9relat));
                }
                else
                {
                    if (pd1.Pd1vias == 0)
                        throw new Exception(string.Format("Número de vias não definido para o terminal {0} e relatório {1}", terminal, dc9relat));
                    pd1impres = pd1.Pd1impres;
                }
            }

            model.FromCv5tipo = string.IsNullOrWhiteSpace(model.FromCv5tipo) ? "EMPTY" : model.FromCv5tipo;
            model.ToCv5tipo = string.IsNullOrWhiteSpace(model.ToCv5tipo) ? "Z".PadLeft(cv5tipoSize, 'Z') : model.ToCv5tipo;

            model.FromCv5repres = string.IsNullOrWhiteSpace(model.FromCv5repres) ? "EMPTY" : model.FromCv5repres;
            model.ToCv5repres = string.IsNullOrWhiteSpace(model.ToCv5repres) ? "9".PadLeft(cv5repreSize, '9') : model.ToCv5repres;

            if (string.IsNullOrWhiteSpace(model.Cv5nomeloc))
                model.Cv5nomeloc = "EMPTY";

            string pdfName = string.Format("RELAT_{0}_{1}.pdf", dc9relat, DateTime.Now.Ticks.ToString());

            PrintModel printModel = new PrintModel()
            {
                Relat = dc9relat,
                Params = string.Format(
                      "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11}"
                    , pd1impres
                    , "S"
                    , model.FromCv5emissao.ToString("dd/MM/yyyy")
                    , model.ToCv5emissao.ToString("dd/MM/yyyy")
                    , model.FromCv5tipo
                    , model.ToCv5tipo
                    , model.FromCv5repres
                    , model.ToCv5repres
                    , model.Cv5nomeloc
                    , model.Limit
                    , model.Group
                    , pdfName)
            };

            await PrintService.Print(printModel, authorization);

            return "/Printing/Print/Document/" + pdfName;
        }
    }
}
