using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Services;
using Printing.Models;
using System;
using System.Threading.Tasks;

namespace Printing.Services
{
    public class ReportService
    {
        private readonly IConfiguration Configuration;
        private FormatterService FormatterService;
        private readonly OryxDomain.Services.PrintService PrintService;

        public ReportService(IConfiguration configuration)
        {
            this.Configuration = configuration;
            FormatterService = new FormatterService(configuration);
            PrintService = new OryxDomain.Services.PrintService(configuration);
        }

        public async Task<string> PrintReportProductTurnover(ReportProductTurnoverModel model, string terminal, string authorization)
        {
            string dc9relat = "331";
            string pd1impres = "Bullzip PDF Printer";

            int pr0produtoSize = await FormatterService.FindFieldLength("pr0produto");
            int pr0colecaoSize = await FormatterService.FindFieldLength("pr0colecao");
            int pr0etiqSize = await FormatterService.FindFieldLength("pr0etiq");

            model.FromCv7codigo = string.IsNullOrWhiteSpace(model.FromCv7codigo) ? "EMPTY" : model.FromCv7codigo;
            model.ToCv7codigo = string.IsNullOrWhiteSpace(model.ToCv7codigo) ? "Z".PadLeft(pr0produtoSize, 'Z') : model.ToCv7codigo;

            model.FromPr0colecao = string.IsNullOrWhiteSpace(model.FromPr0colecao) ? "EMPTY" : model.FromPr0colecao;
            model.ToPr0colecao = string.IsNullOrWhiteSpace(model.ToPr0colecao) ? "9".PadLeft(pr0colecaoSize, '9') : model.ToPr0colecao;

            model.FromPr0etiq = string.IsNullOrWhiteSpace(model.FromPr0etiq) ? "EMPTY" : model.FromPr0etiq;
            model.ToPr0etiq = string.IsNullOrWhiteSpace(model.ToPr0etiq) ? "9".PadLeft(pr0etiqSize, '9') : model.ToPr0etiq;

            if(string.IsNullOrWhiteSpace(model.Cv5clinome))
                model.Cv5clinome = "EMPTY";

            string pdfName = string.Format("RELAT_{0}_{1}.pdf", dc9relat, DateTime.Now.Ticks.ToString());

            PrintModel printModel = new PrintModel()
            {
                Relat = dc9relat,
                TimeOutCallOryx = Convert.ToInt32(Configuration["TimeOutCallOryx"]),
                Params = string.Format(
                      "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13}"
                    , pd1impres
                    , "S"
                    , model.FromCv5emissao.ToString("dd/MM/yyyy")
                    , model.ToCv5emissao.ToString("dd/MM/yyyy")
                    , model.FromCv7codigo
                    , model.ToCv7codigo
                    , model.FromPr0colecao
                    , model.ToPr0colecao
                    , model.FromPr0etiq
                    , model.ToPr0etiq
                    , model.Cv5clinome
                    , model.Entsai
                    , model.OnlySalesItems ? "1" : "0"
                    , pdfName)
            };

            await PrintService.Print(printModel, authorization);

            return "/Printing/Print/Document/" + pdfName;
        }
    }
}
