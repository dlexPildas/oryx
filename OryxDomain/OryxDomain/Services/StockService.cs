using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class StockService
    {
        private readonly OrderParametersRepository OrderParametersRepository;
        private readonly StockRepository StockRepository;
        private readonly OrderRepository OrderRepository;
        private readonly OrderService OrderService;
        private readonly DictionaryService DictionaryService;
        private readonly DictionaryRepository DictionaryRepository;
        private readonly ParametersRepository ParametersRepository;
        private readonly PDVParametersRepository PDVParametersRepository;
        private IConfiguration Configuration;

        public StockService(IConfiguration configuration)
        {
            Configuration = configuration;
            OrderParametersRepository = new OrderParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            StockRepository = new StockRepository(Configuration["OryxPath"] + "oryx.ini");
            OrderRepository = new OrderRepository(Configuration["OryxPath"] + "oryx.ini");
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            PDVParametersRepository = new PDVParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            OrderService = new OrderService(Configuration);
            DictionaryService = new DictionaryService(Configuration);
        }

        public string Sku(string codigo, SalesItemType itemfab = SalesItemType.MATERIALS, string cor = "", string tamanho = "")
        {
            string cSku = ((int)itemfab).ToString() + codigo;
            cor = cor.Trim();
            tamanho = tamanho.Trim();
            cSku += string.IsNullOrWhiteSpace(cor) ? "" : "."+cor;
            cSku += string.IsNullOrWhiteSpace(tamanho) ? "" : "-"+tamanho;
            return cSku;
        }

        public async Task RemovePiecesFromTransferOrder(IList<Vd7CartModel> lstVd7)
        {
            LX2 lx2 = await OrderParametersRepository.Find();
            foreach (Vd7CartModel vd7 in lstVd7)
            {
                foreach (SalesItemModel product in vd7.Items)
                {
                    await StockRepository.DeleteFromTransferOrder(product.Vd8peca, lx2.Lx2cliest);
                }
            }
        }

        public async Task AddPiecesToTransferOrder(string authorization, string lx9usuario, IList<VDL> lstVdl = null, IList<VD8> lstVd8 = null)
        {
            VD1 vd1;
            VD6 vd6;
            string vd1pedido = string.Empty;
            string vd6embarq = string.Empty;
            string vd7volume = string.Empty;

            LX2 lx2 = await OrderParametersRepository.Find();
            LX0 lx0 = await ParametersRepository.GetLx0();

            vd1 = await OrderRepository.FindOpenTransferOrder(lx2.Lx2cliest);

            //se não existir pedido, criar
            if (vd1 == null)
            {
                vd1pedido = await DictionaryService.GetNextNumber(nameof(vd1.Vd1pedido), authorization, false);
                vd1 = new VD1()
                {
                    Vd1abert = DateTime.Now,
                    Vd1cliente = lx2.Lx2cliest,
                    Vd1consig = true,
                    Vd1emissor = lx0.Lx0cliente,
                    Vd1entrada = DateTime.Now,
                    Vd1pedido = vd1pedido,
                    Vd1pronta = true,
                    Vd1usuario = lx9usuario,
                };
            }
            else
            {
                //caso haja pedido, complementar o seu embarque aberto ou criar um novo
                vd1 = await OrderService.FindOrder(vd1.Vd1pedido, authorization);
            }
            if (vd1.LstVd6 == null || vd1.LstVd6.Count == 0 || vd1.LstVd6[vd1.LstVd6.Count - 1].Vd6fecha > Constants.MinDateOryx)
            {
                vd6embarq = await OrderService.FindNextShipSequence(vd1.Vd1pedido);
                vd6 = new VD6()
                {
                    Vd6pedido = vd1.Vd1pedido,
                    Vd6abert = DateTime.Now,
                    Vd6embarq = vd6embarq,
                    Vd6usuario = lx9usuario,
                    LstVd7 = new List<VD7>()
                };
            }
            else
            {
                vd6 = vd1.LstVd6[0];
                if (vd6.LstVd7 == null)
                    vd6.LstVd7 = new List<VD7>();
            }
            if (vd6.LstVd7.Count == 0)
            {
                vd7volume = await DictionaryService.GetNextNumber("VD7VOLUME", authorization, false);
                vd6.LstVd7.Add(new VD7()
                {
                    Vd7embarq = vd6.Vd6embarq,
                    Vd7pedido = vd1.Vd1pedido,
                    Vd7volume = vd7volume
                });
            }

            VD7 vd7 = vd6.LstVd7[0];

            vd7.LstVd8 = new List<VD8>();

            if (lstVdl != null)
            {
                foreach (VDL vdl in lstVdl)
                {
                    vd7.LstVd8.Add(new VD8()
                    {
                        Vd8leitura = vdl.Vdlleitura,
                        Vd8peca = vdl.Vdlpeca,
                        Vd8preco = vdl.Vdlpreco,
                        Vd8volume = vd7.Vd7volume
                    });
                }
            }

            if (lstVd8 != null)
            {
                foreach (VD8 vd8 in lstVd8)
                {
                    vd7.LstVd8.Add(new VD8()
                    {
                        Vd8leitura = vd8.Vd8leitura,
                        Vd8peca = vd8.Vd8peca,
                        Vd8preco = vd8.Vd8preco,
                        Vd8volume = vd7.Vd7volume
                    });
                }
            }

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    if (!string.IsNullOrWhiteSpace(vd1pedido))
                    {
                        //insert vd1 - caso não exista
                        await OrderRepository.Insert(vd1, transaction);

                        //salvar o número do próximo pedido
                        await DictionaryRepository.SaveNextNumber("VD1PEDIDO", vd1pedido, transaction);
                    }
                    //insert vd6 - caso não exista ou esteja fechado o ultimo
                    if (!string.IsNullOrWhiteSpace(vd6embarq))
                        await OrderRepository.SaveVd6(vd6, transaction);
                    if (!string.IsNullOrWhiteSpace(vd7volume))
                    {
                        //insert vd7 - caso não exista
                        await OrderRepository.SaveVd7(vd7, transaction);

                        //salvar o número do próximo volume
                        await DictionaryRepository.SaveNextNumber("VD7VOLUME", vd7volume, transaction);
                    }

                    //insert vd8 - no novo volume ou no antigo
                    foreach (VD8 vd8 in vd7.LstVd8)
                    {
                        await OrderRepository.SaveVd8(vd8, transaction);
                    }

                    transaction.Commit();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    connection.Close();
                    throw ex;
                }
            }
        }

        public async Task<decimal> FindStock(string pr0produto, string pr2opcao, string pr3tamanho)
        {
            LXD lxd = await PDVParametersRepository.Find();

            if (lxd.Lxdean)
            {
                StockModel stock = await StockRepository.FindStockByEAN(pr0produto, pr2opcao, pr3tamanho);
                return stock == null ? 0 : stock.Stock;
            }
            else
            {
                StockModel stock;
                //caso não tenha opção de cor ou tamanho, buscar por referência apenas
                if (string.IsNullOrWhiteSpace(pr2opcao) || string.IsNullOrWhiteSpace(pr3tamanho))
                    stock = await StockRepository.FindStockByPieces(pr0produto);
                else
                    stock = await StockRepository.FindStockByPieces(pr0produto, pr2opcao, pr3tamanho);
                
                return stock == null ? 0 : stock.Stock;
            }
        }

        public async Task<decimal> FindStockByOrder(string vd2produto, string vd3opcao, string vd3tamanho, string vd1pedido)
        {
            LXD lxd = await PDVParametersRepository.Find();

            if (lxd.Lxdean)
            {
                StockModel stock = await StockRepository.FindStockOrderByEAN(vd2produto, vd3opcao, vd3tamanho, vd1pedido);
                return stock == null ? 0 : stock.Stock;
            }
            else
            {
                StockModel stock = await StockRepository.FindStockOrderByPieces(vd2produto, vd3opcao, vd3tamanho, vd1pedido);
                return stock == null ? 0 : stock.Stock;
            }
        }
    }
}
