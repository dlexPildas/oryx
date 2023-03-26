using OryxDomain.Http;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class EmailService
    {
        readonly ParametersRepository ParametersRepository;
        readonly IConfiguration Configuration;
        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<bool> SendMail(PostEmailModel postEmailModel, AuthModel authModel)
        {
            B2B b2b = await ParametersRepository.GetB2b();
            string content = JsonConvert.SerializeObject(postEmailModel);
            ReturnModel<bool> returnModel = JsonConvert.DeserializeObject<ReturnModel<bool>>(await HttpUtilities.CallPostAsync(
                  b2b.B2bbaseurl
                , "/Messaging/Messaging/SendEmail"
                , content
                , await Authenticate(b2b, await ParametersRepository.GetLx0(), authModel)));
            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }

            return returnModel.ObjectModel;
        }

        private async Task<string> Authenticate(B2B b2b, LX0 lX0, AuthModel authModel)
        {
            ReturnModel<AuthenticatedModel> returnAuth = JsonConvert.DeserializeObject<ReturnModel<AuthenticatedModel>>(
                await HttpUtilities.CallPostAsync(
                      b2b.B2bbaseurl
                    , "/Authentication/Authentication/AuthInternal"
                    , JsonConvert.SerializeObject(authModel)));

            if (returnAuth.IsError)
            {
                throw new Exception(returnAuth.MessageError);
            }

            string authorization = returnAuth.ObjectModel.Token;
            return authorization;
        }
    }
}
