using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxRfidStandard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Services
{
    public class RfidService
    {
        private readonly IConfiguration Configuration;
        private readonly ProductionOrderParametersRepository ProductionOrderParametersRepository;
        private readonly TerminalRepository TerminalRepository;
        private readonly DictionaryRepository DictionaryRepository;

        public RfidService(IConfiguration configuration)
        {
            Configuration = configuration;
            ProductionOrderParametersRepository = new ProductionOrderParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            TerminalRepository = new TerminalRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<IList<string>> FindAsync(string terminal)
        {
            PD0 pd0 = await TerminalRepository.Find(terminal);

            LX1 lx1 = await ProductionOrderParametersRepository.Find();

            if (string.IsNullOrWhiteSpace(lx1.Lx1rfidurl))
            {
                DC1 dc1 = await DictionaryRepository.FindDC1ByDc1campo(nameof(lx1.Lx1rfidurl));
                throw new Exception(string.Format("Campo {0} não cadastrado", dc1.Dc1nome));
            }

            if (string.IsNullOrWhiteSpace(pd0.Pd0antena))
                return null;

            string time = Configuration["RfidDuration"];

            RfidServices rfidServices = new RfidServices();

            return await rfidServices.FindRfid(lx1.Lx1rfidurl, pd0.Pd0antena, time);
        }

        public async Task<IList<string>> FindBySales(string terminal, IList<SalesItemModel> lstItems)
        {
            IList<string> payloadRfids = new List<string>();

            IList<string> lstRfid = await FindAsync(terminal);

            foreach (var rfid in lstRfid)
            {
                SalesItemModel saleItem = lstItems.FirstOrDefault(sl => sl.Of3rfid == rfid);
                if (saleItem == null)
                    payloadRfids.Add(rfid);
            }

            return payloadRfids;
        }

        public async Task<IList<string>> FindByReturn(string terminal, IList<ReturnItemModel> lstItems)
        {
            IList<string> payloadRfids = new List<string>();

            IList<string> lstRfid = await FindAsync(terminal);

            foreach (var rfid in lstRfid)
            {
                ReturnItemModel returnItem = lstItems.FirstOrDefault(sl => sl.Of3rfid == rfid);
                if (returnItem == null)
                    payloadRfids.Add(rfid);
            }

            return payloadRfids;
        }
    }
}
