using OryxDomain.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OryxDomain.Authentication
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly SecurityRepository SecurityRepository;

        public CustomCookieAuthenticationEvents(IConfiguration configuration)
        {
            SecurityRepository = new SecurityRepository(configuration["OryxPath"] + "oryx.ini");
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            ClaimsPrincipal claimPrincipal = context.HttpContext.User;

            var lx9acesso = (from c in claimPrincipal.Identities.First().Claims
                             where c.Type == "lx9acesso"
                             select c.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(lx9acesso) ||
                string.IsNullOrWhiteSpace(await SecurityRepository.FindLx9Valid(lx9acesso)))
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}
