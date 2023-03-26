using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OryxDomain.Http
{
    public static class HttpUtilities
    {
        #region public methods
        public static async Task<string> GetAsync(string baseurl, string router, string parameters = "", string authorization = "", Dictionary<string, string> queries = null, string customAuthorization = "")
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(baseurl) })
            {
                if (!string.IsNullOrEmpty(authorization))
                    httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authorization);

                if (!string.IsNullOrWhiteSpace(customAuthorization))
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", customAuthorization);

                string uriReq = router;
                if (!string.IsNullOrWhiteSpace(parameters))
                {
                    uriReq = string.Format("{0}/{1}", router, parameters);
                }

                if (queries != null)
                {
                    uriReq = string.Concat(uriReq, "?");
                    foreach (KeyValuePair<string, string> ele1 in queries)
                    {
                        uriReq = string.Concat(uriReq, ele1.Key + "=" + ele1.Value +"&");
                    }
                }
                using (var response = await httpClient.GetAsync(uriReq))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        string message = SetMessageError(
                            response,
                            response.RequestMessage.RequestUri.AbsoluteUri);
                        throw new Exception(message);
                    }
                }
            }
        }

        public static async Task<string> CallPostAsync(string baseurl, string router, string content = "", string authorization = "", Dictionary<string, string> queries = null, string customAuthorization = "")
        {
            StringContent stringContent;
            if (string.IsNullOrWhiteSpace(content))
            {
                stringContent = new StringContent("{}", Encoding.UTF8, "application/json");
            }
            else
            {
                stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            }
            if (queries != null)
            {
                router = string.Concat(router, "?");
                foreach (KeyValuePair<string, string> ele1 in queries)
                {
                    router = string.Concat(router, ele1.Key + "=" + ele1.Value + "&");
                }
            }
            return await PostAsync(baseurl, router, stringContent, authorization, customAuthorization);
        }

        public static async Task<string> CallPutAsync(string baseurl, string router, string content = "", string authorization = "", Dictionary<string, string> queries = null)
        {
            StringContent stringContent;
            if (string.IsNullOrWhiteSpace(content))
            {
                stringContent = new StringContent("{}", Encoding.UTF8, "application/json");
            }
            else
            {
                stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            }
            if (queries != null)
            {
                router = string.Concat(router, "?");
                foreach (KeyValuePair<string, string> ele1 in queries)
                {
                    router = string.Concat(router, ele1.Key + "=" + ele1.Value + "&");
                }
            }
            return await PutAsync(baseurl, router, stringContent, authorization);
        }
        #endregion

        #region private methods


        private static async Task<string> PostAsync(string baseurl, string router, StringContent content, string authorization, string customAuthorization)
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(baseurl) })
            {
                if (!string.IsNullOrEmpty(authorization))
                    httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authorization);

                if (!string.IsNullOrWhiteSpace(customAuthorization))
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", customAuthorization);

                using (var response = await httpClient.PostAsync(router, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        string message = SetMessageError(
                            response,
                            response.RequestMessage.RequestUri.AbsoluteUri);

                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                            throw new UnauthorizedAccessException(message);
                        else
                            throw new Exception(message);
                    }
                }
            }
        }

        private static async Task<string> PutAsync(string baseurl, string router, StringContent content = null, string authorization = "")
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(baseurl) })
            {
                if (content == null)
                    content = new StringContent("{}", Encoding.UTF8, "application/json");

                if (!string.IsNullOrEmpty(authorization))
                    httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authorization);
                
                using (var response = await httpClient.PutAsync(router, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        string message = SetMessageError(
                            response,
                            response.RequestMessage.RequestUri.AbsoluteUri);
                        throw new Exception(message);
                    }
                }
            }
        }
        
        private static string SetMessageError(HttpResponseMessage response, string request)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return string.Format(
                            "Não foi possível concluir a requisição para {0}. Response status: {1}. Error: {2}",
                            request,
                            response.StatusCode,
                            "Não encontrado(a)");
            }
            return string.Format(
                            "Não foi possível concluir a requisição para {0}. Response status: {1}. Error: {2}",
                            request,
                            response.StatusCode,
                            response.Content.ReadAsStringAsync());
        }
        #endregion
    }
}
