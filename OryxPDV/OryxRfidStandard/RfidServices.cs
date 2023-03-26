using OryxRfidStandard.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net.Http;

namespace OryxRfidStandard
{

    public class RfidServices
    {

        public string FindRfidBreakLine(string url, string antenna, string delaySearch)
        {
            List<string> rfidList = FindRfid(url, antenna, delaySearch).Result;
            string strReturn = string.Join("\n", rfidList);

            return strReturn;
        }

        public async Task<List<string>> FindRfid(string url, string antenna, string delaySearch)
        {
            var uri = new Uri(url);
            IList<RfIdModel> rfids = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = uri;
                HttpResponseMessage response = await client.GetAsync(string.Format("{0}?cod_portal={1}&delay_seg={2}", uri.AbsoluteUri, antenna, delaySearch));
                
                if (response.IsSuccessStatusCode)
                {
                    DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(List<RfIdModel>));
                    Stream s = await response.Content.ReadAsStreamAsync();
                    s.Position = 0;
                    rfids = (IList<RfIdModel>)ds.ReadObject(s);
                }
                else
                {
                    throw new Exception("Erro ao consultar antena Rfid.");
                }
            }
            List<string> returnList = new List<string>();
            foreach (RfIdModel rfId in rfids)
            {
                returnList.Add(rfId.tag);
            }
            return returnList;
        }
    }
}

