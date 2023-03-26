using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using Printing.Utilities;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Rectangle = iTextSharp.text.Rectangle;

namespace Printing.Services
{
    public class ChequeServices
    {
        private IConfiguration Configuration;
        private readonly ChequeParametersRepository ChequeRepository;
        private readonly ChequePositionsRepository ChequePositionsRepository;
        private readonly ParametersRepository ParametersRepository;
        private readonly CustomerRepository CustomerRepository;
        private readonly PrintingPreferencesRepository PrintingPreferencesRepository;

        public ChequeServices(IConfiguration configuration)
        {
            Configuration = configuration;
            ChequeRepository = new ChequeParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            ChequePositionsRepository = new ChequePositionsRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            CustomerRepository = new CustomerRepository(Configuration["OryxPath"] + "oryx.ini");
            PrintingPreferencesRepository = new PrintingPreferencesRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<string> Print(CV8 cv8, string terminal)
        {
            string relatCode = "332";
            string pathFile = Configuration["OryxPath"]+"temp\\" + @"cheque.pdf";
            
            //validações
            ValidateInstallment(cv8);

            LX0 lx0 = await ParametersRepository.GetLx0();
            if (lx0 == null)
                throw new Exception("Parâmetro gerais não cadastrados");

            CF1 cf1 = await CustomerRepository.FindByCpfCnpj(lx0.Lx0cliente);

            PD1 pd1 = await PrintingPreferencesRepository.Find(terminal, relatCode);

            PD6 pd6 = await ChequeRepository.Find();
            if (pd6 == null)
            {
                throw new Exception("Parâmetro de cheque não cadastrado");
            }
            PD7 pd7 = await ChequePositionsRepository.Find(cv8.Cv8agente, pd6.Pd6emissor);
            if (pd7 == null)
            {
                pd7 = await ChequePositionsRepository.Find(pd6.Pd6agentep, pd6.Pd6emissor);
            }

            if (pd7 == null)
                throw new Exception(string.Format("Parâmetros de cheque não encontrados para o emissor {0} e os agentes {1}, {2}", pd6.Pd6emissor, cv8.Cv8agente, pd6.Pd6agentep));

            if (!File.Exists(@"C:\Program Files\Bullzip\PDF Printer\pdfcmd.exe"))
            {
                throw new Exception("Impressora Bullzip não instalada.");
            }
            //criando pdf
            GenerateChequeReportInPdf(cv8, pathFile, pd6, pd7, cf1.Cf3nome);

            RotatePdfDocument(ref pathFile, pd6.Pd6rotacao);

            string printer = pd1 != null && !!string.IsNullOrWhiteSpace(pd1.Pd1impres) ? pd1.Pd1impres : pd6.Pd6impres;

            if (string.IsNullOrWhiteSpace(printer))
            {
                return "/Printing/Print/Document/cheque.pdf";
            }

            PrintCheque(pathFile, printer);
            
            return "";
        }

        private void PrintCheque(string pathFile, string printer)
        {
            try
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    FileName = @"C:\Program Files\Bullzip\PDF Printer\pdfcmd.exe",
                    Arguments = $"command=printpdf input=\"{pathFile}\" printer=\"{printer}\"",
                };
                process.Start();
                process.WaitForExit(Convert.ToInt32(Configuration["TimeOutCallOryx"]));

                if (!process.HasExited)
                {
                    if (process.Responding)
                        process.CloseMainWindow();
                    else
                        process.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        private void GenerateChequeReportInPdf(CV8 cv8, string pathFile, PD6 pd6, PD7 pd7, string cidade)
        {
            Rectangle a = new Rectangle(0, 0, 842, -595);
            Document doc = new Document(a);
            doc.AddCreationDate();
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pathFile, FileMode.Create));
            doc.Open();
            PdfContentByte cb = writer.DirectContent;
            cb.Rectangle(a);
            cb.BeginText();
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb.SetColorFill(BaseColor.BLACK);
            cb.SetFontAndSize(bf, 10);

            Converter extenso = new Converter();
            string linha1 = "";
            string linha2 = "*******************************************************************************************";
            
            decimal decimalVal = Convert.ToDecimal(cv8.Cv8valor.ToString().Replace(".", ",").Trim());

            string porextenso = extenso.ToExtenso(decimalVal);

            if (porextenso.Length >= 56)
            {
                linha1 = porextenso.Substring(0, 55);
                linha2 = porextenso.Substring(56, porextenso.Length - 56).PadRight(75, '*');
            }
            else
            {
                linha1 = porextenso.PadRight(65, '*');

            }
            int mes = cv8.Cv8vencim.Month;
            string mesExtenso = DateTimeFormatInfo.CurrentInfo.GetMonthName(mes).ToLower().ToUpper();

            cb.ShowTextAligned(Element.ALIGN_LEFT, string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", decimalVal), Int32.Parse(pd7.Pd7valor.Substring(0, 3)), -Int32.Parse(pd7.Pd7valor.Substring(4, 3)), 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, linha1, Int32.Parse(pd7.Pd7linha1.Substring(0, 3)), -Int32.Parse(pd7.Pd7linha1.Substring(4, 3)), 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, linha2, Int32.Parse(pd7.Pd7linha2.Substring(0, 3)), -Int32.Parse(pd7.Pd7linha2.Substring(4, 3)), 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cidade, Int32.Parse(pd7.Pd7cidade.Substring(0, 3)), -Int32.Parse(pd7.Pd7cidade.Substring(4, 3)), 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cv8.Cv8vencim.ToString("dd"), Int32.Parse(pd7.Pd7dia.Substring(0, 3)), -Int32.Parse(pd7.Pd7dia.Substring(4, 3)), 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, mesExtenso, Int32.Parse(pd7.Pd7mes.Substring(0, 3)), -Int32.Parse(pd7.Pd7mes.Substring(4, 3)), 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cv8.Cv8vencim.ToString("yyyy"), Int32.Parse(pd7.Pd7ano.Substring(0, 3)), -Int32.Parse(pd7.Pd7ano.Substring(4, 3)), 0);
            if (!string.IsNullOrWhiteSpace(pd6.Pd6nominal))
            {
                cb.ShowTextAligned(Element.ALIGN_LEFT, pd6.Pd6nominal, Int32.Parse(pd7.Pd7nominal.Substring(0, 3)), -Int32.Parse(pd7.Pd7nominal.Substring(4, 3)), 0);
            }
            cb.EndText();
            // mudando a letra do bom para
            cb.BeginText();
            BaseFont bf2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb.SetColorFill(BaseColor.BLACK);
            cb.SetFontAndSize(bf2, 18);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cv8.Cv8vencim.ToString("dd") + "/" + cv8.Cv8vencim.ToString("MM"), Int32.Parse(pd7.Pd7bompara.Substring(0, 3)), -Int32.Parse(pd7.Pd7bompara.Substring(4, 3)), 0);
            cb.EndText();
            if (pd6.Pd6cruzar == true)
            {
                CMYKColor black = new CMYKColor(1f, 1f, 1f, 1f);
                cb.SetColorStroke(black);
                cb.MoveTo(339, -268);
                cb.LineTo(423, -183);
                cb.ClosePathStroke();
                cb.MoveTo(340, -283);
                cb.LineTo(438, -182);
                cb.ClosePathStroke();
            }
            doc.Close();
        }

        private static void ValidateInstallment(CV8 cv8)
        {
            if (string.IsNullOrWhiteSpace(cv8.Cv8agente))
            {
                throw new Exception("Código do banco não informado.");
            }
            if (cv8.Cv8vencim == null)
            {
                throw new Exception("Data de vecimento não informada.");
            }
            if (cv8.Cv8valor == 0)
            {
                throw new Exception("Valor da parcela não pode ser zero.");
            }
        }

        private void RotatePdfDocument(ref string pathFile, int pageRotation)
        {
            if (pageRotation == 0)
                return;

            PdfReader reader = new PdfReader(pathFile);
            PdfDictionary page = reader.GetPageN(1);
            page.Put(PdfName.ROTATE, new PdfNumber(pageRotation));
            
            pathFile = Configuration["OryxPath"] + @"cheque2.pdf";
            
            FileStream fs = new FileStream(pathFile, FileMode.Create,
            FileAccess.Write, FileShare.None);
            PdfStamper stamper = new PdfStamper(reader, fs);
            
            stamper.Close();
            reader.Close();
        }
    }
}
