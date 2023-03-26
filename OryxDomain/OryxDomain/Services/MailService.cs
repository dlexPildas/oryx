using Microsoft.Extensions.Configuration;
using OryxDomain.Http;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class MailService
    {
        private readonly IConfiguration Configuration;
        private readonly APIParametersRepository APIParametersRepository;
        private readonly ParametersRepository ParametersRepository;
        public MailService(IConfiguration configuration)
        {
            Configuration = configuration;
            APIParametersRepository = new APIParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(configuration["OryxIni"]);
        }

        public async Task<string> SendMail(string receiver, string subject, string body, string anexo1, string anexo2, string anexo3 = "")
        {
            LXE lxe = await APIParametersRepository.Find();
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");

            LX0 lx0 = await ParametersRepository.GetLx0();
            if (lx0 == null)
                throw new Exception("Parâmetros Gerais não cadastrados.");

            if (string.IsNullOrWhiteSpace(lx0.Lx0smtp) ||
                string.IsNullOrWhiteSpace(lx0.Lx0user) ||
                string.IsNullOrWhiteSpace(lx0.Lx0senha) ||
                string.IsNullOrWhiteSpace(lx0.Lx0remet))
            {
                throw new Exception("Configuração ausente ou incomplemara de Servidor de email.\nVerifique em Parametros Gerais, Servidores NFE.");
            }

            Dictionary<string, string> querie = new Dictionary<string, string>()
            {
                {"servidor", lx0.Lx0smtp},
                {"usuario", lx0.Lx0user},
                {"senha", lx0.Lx0senha},
                {"remetente", lx0.Lx0remet},
                {"destinatario", receiver},
                {"assunto", subject},
                {"corpo", body},
                {"anexo1", anexo1},
                {"anexo2", anexo2},
                {"anexo3", anexo3},
                {"porta", lx0.Lx0smtppor},
                {"smtpssl", Convert.ToInt32(lx0.Lx0smtpssl).ToString()},
                {"ccOculta", lx0.Lx0oculta},
            };

            string emailRet = await HttpUtilities.CallPostAsync(lxe.Lxebaseurl, "/OryxNet/Principal/EnviarEmail", queries: querie);
            if (!string.IsNullOrEmpty(emailRet))
                throw new Exception("<b>Erro ao enviar e-mail</b>:\n"+ emailRet);

            return "Email enviado";
        }
    }
}
