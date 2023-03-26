using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OryxDomain.Http;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class OrderService
    {
        private IConfiguration Configuration;
        private readonly OrderRepository OrderRepository;
        private readonly FormatterService FormatterService;
        private readonly APIParametersRepository APIParametersRepository;
        public OrderService(IConfiguration configuration)
        {
            Configuration = configuration;
            OrderRepository = new OrderRepository(Configuration["OryxPath"] + "oryx.ini");
            APIParametersRepository = new APIParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            FormatterService = new FormatterService(Configuration);
        }

        public async Task<string> FindNextShipSequence(string vd1pedido)
        {
            int lastVd6embarq = 1;
            VD6 lastVd6 = await OrderRepository.FindLastVd6Embarq(vd1pedido);
            if (lastVd6 != null && lastVd6.Vd6fecha > Constants.MinDateOryx)
            {
                lastVd6embarq = Convert.ToInt32(lastVd6.Vd6embarq) + 1;
            }

            int lenghtVd6embarq = await FormatterService.FindFieldLength("VD6EMBARQ");
            string shipSequence = Formatters.FormatId(lastVd6embarq.ToString(), lenghtVd6embarq);
            return shipSequence;
        }

        public async Task<VD1> FindOrder(string vd1pedido, string authorization)
        {
            LXE lxe = await APIParametersRepository.Find();
            if (lxe == null)
                throw new Exception("Parâmetros de API não cadastrados");

            string jsonReponse = await HttpUtilities.GetAsync(
                  lxe.Lxebaseurl
                , "/Order/Order/Find"
                , vd1pedido
                , authorization);

            ReturnModel<VD1> returnModel = JsonConvert.DeserializeObject<ReturnModel<VD1>>(jsonReponse);

            if (returnModel.IsError)
            {
                throw new Exception(returnModel.MessageError);
            }

            return returnModel.ObjectModel;
        }
    }
}
