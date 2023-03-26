using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Ocelot.Middleware;
using System;
using System.Threading.Tasks;

namespace OryxGateway.Services
{
    public class SecurityService
    {
        readonly SecurityRepository SecurityRepository;
        readonly IConfiguration Configuration;
        public SecurityService(IConfiguration configuration)
        {
            Configuration = configuration;
            SecurityRepository = new SecurityRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task ProcessMiddleware(HttpContext ctx, Func<Task> next)
        {
            string accessToken = ctx.Request?.Headers?["Authorization"];
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                accessToken = accessToken.Replace("Bearer ", "");
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    TOK tok = await SecurityRepository.FindWhiteList(accessToken);
                    if (tok == null || tok.Tokexpira < DateTime.UtcNow)
                    {
                        ctx.Items.SetError(new UnauthenticatedError("Sua sessão expirou"));
                    }
                }
            }
            await next.Invoke();
        }
    }
}
