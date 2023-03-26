using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using OryxDomain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OryxDomain.Middleware
{
    public static class ReturnModelExceptionExtensions
    {
        public static IApplicationBuilder UseReturnModelException(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ReturnModelException>();
        }
    }

    class ReturnModelException
    {
        private readonly RequestDelegate _next;

        public ReturnModelException(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                ReturnModel<object> returnModel = new ReturnModel<object>();
                returnModel.SetError(ex);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(
                          returnModel
                        , new JsonSerializerSettings() 
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        }));
            }
        }
    }
}
