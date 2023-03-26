using Microsoft.Extensions.Configuration;
using OryxDomain.Models;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Exceptions;
using OryxDomain.Models.Oryx;
using OryxDomain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Services
{
    public class ProductService
    {
        private readonly IConfiguration Configuration;
        private readonly PiecesRecordRepository PiecesRecordRepository;
        private readonly OrderParametersRepository OrderParametersRepository;
        private readonly ReleasedPiecesRepository ReleasedPiecesRepository;
        private readonly NonConformingPiecesRepository NonconformingPiecesRepository;
        private readonly OrderRepository OrderRepository;
        private readonly PromptDeliveryItemsRepository PromptDeliveryItemsRepository;
        private readonly RequirementsCalculationRepository RequirementsCalculationRepository;
        private readonly PDVParametersRepository PDVParametersRepository;
        private readonly ParametersRepository ParametersRepository;
        private readonly FiscalDocumentRepository FiscalDocumentRepository;
        private readonly LXD lxd;

        public ProductService(IConfiguration configuration)
        {
            Configuration = configuration;
            PiecesRecordRepository = new PiecesRecordRepository(Configuration["OryxPath"] + "oryx.ini");
            OrderParametersRepository = new OrderParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            ReleasedPiecesRepository = new ReleasedPiecesRepository(Configuration["OryxPath"] + "oryx.ini");
            NonconformingPiecesRepository = new NonConformingPiecesRepository(Configuration["OryxPath"] + "oryx.ini");
            OrderRepository = new OrderRepository(Configuration["OryxPath"] + "oryx.ini");
            PromptDeliveryItemsRepository = new PromptDeliveryItemsRepository(Configuration["OryxPath"] + "oryx.ini");
            RequirementsCalculationRepository = new RequirementsCalculationRepository(Configuration["OryxPath"] + "oryx.ini");
            PDVParametersRepository = new PDVParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            ParametersRepository = new ParametersRepository(Configuration["OryxPath"] + "oryx.ini");
            FiscalDocumentRepository = new FiscalDocumentRepository(Configuration["OryxPath"] + "oryx.ini");
            lxd = PDVParametersRepository.Find().Result;
        }

        public void ValidateOf3peca(string of3peca, string volume, string pedido, IList<SalesItemModel> lstVd8, ref bool naoConforme, ref bool confIndis, bool prontaEntrega = false)
        {
            //Não deveríamos ter esses parâmetros no PDV? ou usamos o LX2? vai ficar validações em duas tabelas diferentes que tem quase a mesma coisa
            LX2 lx2 = OrderParametersRepository.Find().Result;
            if (lx2 == null)
                throw new Exception("Parâmetros para pedidos não cadastrados");
            
            OF3 of3 = PiecesRecordRepository.Find(of3peca).Result;
            if (of3 == null)
            {
                throw new Exception(string.Format("Peça {0} inexistente.", of3peca));
            }

            if (!string.IsNullOrWhiteSpace(of3.Of3ordem.Trim()))
            {
                // verificando se aceita-se peça de produção
                if (lx2.Lx2implant == ImplantedPiecesType.ONLY_IMPLANTED)
                    throw new Exception("Expedição impedida para peças programadas.");

                // verificando se a peca foi liberada
                OF0 of0 = ReleasedPiecesRepository.Find(of3peca).Result;
                if (of0 == null)
                    throw new Exception("Peça não liberada.");
            }
            else
            {
                //verificando se aceita-se peça implantada
                if (lx2.Lx2implant == ImplantedPiecesType.ONLY_NOT_IMPLANTED)
                    throw new Exception("Peça implantada. Não admitida.");
            }

            // verificando se a peça é nao conforme
            OFH ofh = NonconformingPiecesRepository.Find(of3peca).Result;
            if(ofh != null)
            {
                naoConforme = true;
            }

            //permitir a venda caso seja um volume de um pedido consignado
            VD8 vd8 = OrderRepository.FindVd8InSales(of3peca, volume).Result;
            if (vd8 != null)
            {
                VD1 vd1 = OrderRepository.Find(OrderRepository.FindVd7(vd8.Vd8volume).Result.Vd7pedido).Result;
                if (!lx2.Lx2debest || (lx2.Lx2debest && (!vd1.Vd1cliente.Equals(lx2.Lx2cliest) || (vd1.Vd1cliente.Equals(lx2.Lx2cliest) && !vd1.Vd1pronta && !vd1.Vd1consig))))
                    throw new Exception(string.Format("Peça {0} ja foi expedida no volume {1}.", of3peca, vd8.Vd8volume));
            }
            if (vd8 == null && lx2.Lx2debest)
                throw new StockValidationException(string.Format("Peça {0} não consta no estoque {1}", of3peca, lx2.StockName));

            // verificando se a peca não foi produzida para outro pedido
            if (of3.Of3cativa &&
                !string.IsNullOrWhiteSpace(of3.Of3ordem.Trim()) &&
                !of3.Of3pedido.Equals(pedido) &&
                !lx2.Lx2outros)
            {
                throw new Exception("Peça produzida para o pedido " + of3.Of3pedido + ".");
            }

            // verificando se o item consta no pedido
            if (!prontaEntrega)
            {
                VD3 vd3 = OrderRepository.FindVd3(pedido, of3.Of3produto, of3.Of3opcao, of3.Of3tamanho).Result;
                if (vd3 == null)
                    throw new Exception("Item não consta do Pedido.");

                // verificando se a quantidade não excede o pedido

                // 1.verificando outros volumes
                IList<string> lstShipItems = OrderRepository.FindAllShipItems(of3.Of3produto, of3.Of3opcao, of3.Of3tamanho, volume, pedido).Result;
                int qtyShipped = lstShipItems.Count;

                // verificando este volume
                qtyShipped += lstVd8.Where(p => p.Vd2produto.Equals(vd3.Vd3produto) && p.Vd3opcao.Equals(vd3.Vd3opcao) && p.Vd3tamanho.Equals(vd3.Vd3tamanho)).Count();

                if (qtyShipped >= vd3.Vd3qtde)
                {
                    throw new Exception("Quantidade expedida maior que o pedido (" + vd3.Vd3qtde + ").");
                }
            }
            else
            {
                // verificando disponibilidade para pronta entrega
                if (lx2.Lx2pronta == UnavaiablePieceType.CONFIRM_SHIP &&
                    lx2.Lx2pronta == UnavaiablePieceType.REJECT_SHIP)
                {
                    PLE ple = PromptDeliveryItemsRepository.Find(of3.Of3produto, of3.Of3opcao, of3.Of3tamanho).Result;
                    DateTime horaDoCalculo = RequirementsCalculationRepository.FindCalcDate().Result;
                    OF3 usedPieces = OrderRepository.FindUsedVd8(of3.Of3produto, of3.Of3opcao, of3.Of3tamanho, volume, horaDoCalculo).Result;
                    int qtyInNewOrders = OrderRepository.FindQtyInNewOrders(of3.Of3produto, of3.Of3opcao, of3.Of3tamanho, horaDoCalculo).Result;

                    int qtyInCart = lstVd8.Where(p => p.Vd3leitura > horaDoCalculo && p.Vd2produto.Equals(of3.Of3produto) && p.Vd3opcao.Equals(of3.Of3opcao) && p.Vd3tamanho.Equals(of3.Of3tamanho)).Count();

                    if ((ple.Pledispo - of3.Pledispo - qtyInCart - qtyInNewOrders) < 1)
                    {
                        if (lx2.Lx2pronta == UnavaiablePieceType.CONFIRM_SHIP)
                            confIndis = true;
                        else
                            throw new Exception("Peça indisponível, utilizavel em pedidos pendentes.");
                    }
                }
            }
        }

        public async Task<InfosForReturnModel> ValidateVdlpeca(ReturnItemModel returnItemModel, string cf1cliente, bool consigned)
        {
            LX0 lx0 = await ParametersRepository.GetLx0();

            LX2 lx2 = await OrderParametersRepository.Find();
            if (lx2 == null)
                throw new Exception("Parâmetros comerciais de Pedidos não cadastrados");

            //verificando se a peca existe
            OF3 of3 = await PiecesRecordRepository.Find(returnItemModel.Of3peca);
            if (of3 == null)
            {
                throw new Exception(string.Format("Peça {0} inexistente.", returnItemModel.Of3peca));
            }

            //verificando se a peca ja foi expedida
            VD8 vd8 = await OrderRepository.FindVd8ByPiece(returnItemModel.Of3peca);
            if (vd8 == null)
            {
                throw new Exception("Peça não foi expedida.");
            }

            VD7 vd7 = await OrderRepository.FindVd7(vd8.Vd8volume);
            VD1 vd1 = await OrderRepository.Find(vd7.Vd7pedido);

            //if (vd1.Vd1consig != consigned)
            //{
            //    throw new Exception(string.Format("Peça pertence ao pedido {0} {1}", vd1.Vd1pedido, vd1.Vd1consig ? "consignado" : "de venda"));
            //}

            if (lxd.Lxddevano &&
                vd8.Vd8leitura.Year != DateTime.Now.Year)
            {
                throw new Exception(string.Format("Devolução/troca só pode ser realizada para peças expedidas dentro do ano.<br/>Peça expedida em {0} volume {1}.", vd8.Vd8leitura.ToString("dd/MM/yyyy"), vd8.Vd8volume));
            }
            if (lxd.Lxddevdias > 0 &&
                !vd1.Vd1cliente.Equals(lx0.Lx0cliente) &&
                vd8.Vd8leitura.AddDays(lxd.Lxddevdias) < DateTime.Now)
            {
                throw new Exception(string.Format("Peça expedida em {0} volume {1}.<br/>Prazo de {2} dias esgotado.", vd8.Vd8leitura.ToString("dd/MM/yyyy"), vd8.Vd8volume, lxd.Lxddevdias));
            }
            
            if (!string.IsNullOrWhiteSpace(cf1cliente) &&
                !vd1.Vd1cliente.Equals(cf1cliente) &&
                !lxd.Lxdmercado)
            {
                if (!lx2.Lx2debest || (lx2.Lx2debest && (!vd1.Vd1cliente.Equals(lx2.Lx2cliest) || (vd1.Vd1cliente.Equals(lx2.Lx2cliest) && !vd1.Vd1pronta && !vd1.Vd1consig))))
                    throw new Exception(string.Format("Peça pertence ao pedido {0}.<br/>{1}", vd1.Vd1pedido, vd1.Cf1nome));
                else
                    throw new Exception("Peça não foi expedida.");
            }

            //verificando se a peça não faz parte de acerto de consignação - não foi implementado pois será permitido devolver peças acertadas
            //** o Oryx valida se a peça está acetada a partir do VDF
            //VD8 vd8acertconsig = await OrderRepository.FindVd8ByPiece(returnItemModel.Of3peca, !consigned);
            //if (vd8acertconsig != null)
            //{
            //    VD7 vd7acertconsig = await OrderRepository.FindVd7(vd8acertconsig.Vd8volume);
            //    throw new Exception(string.Format("Venda em consignação confirmada no pedido {0}.", vd7acertconsig.Vd7pedido));
            //}
            return new InfosForReturnModel()
            {
                Vd7embarq = vd7.Vd7embarq,
                Vd7pedido = vd7.Vd7pedido,
                Vd7volume = vd7.Vd7volume,
                Price = vd8.Vd8preco
            };
        }
        
        public async Task FindInfosForReturn(ReturnItemModel returnItemModel, InfosForReturnModel infosForReturnModel, string cf1cliente)
        {
            VD1 vd1 = await OrderRepository.Find(infosForReturnModel.Vd7pedido);
            if (vd1 == null)
                throw new Exception(string.Format("Pedido {0} não encontrado", infosForReturnModel.Vd7pedido));
            
            CV7 cv7 = await FiscalDocumentRepository.FindCv7(returnItemModel.Pr0produto, returnItemModel.Pr2opcao, returnItemModel.Pr3tamanho, infosForReturnModel.Vd7pedido, infosForReturnModel.Vd7embarq, new List<string> { lxd.Lxddocven1, lxd.Lxddocven2 }, infosForReturnModel.Price);

            if (cv7 == null)
                cv7 = await FiscalDocumentRepository.FindCv7(returnItemModel.Pr0produto, returnItemModel.Pr2opcao, returnItemModel.Pr3tamanho, infosForReturnModel.Vd7pedido, infosForReturnModel.Vd7embarq, new List<string> { lxd.Lxddocven3, lxd.Lxddocven4, lxd.Lxddocven5 }, infosForReturnModel.Price);

            if (cv7 == null)
                throw new Exception(string.Format("Não foi possível encontrar faturamento para o pedido {1}-{0} e produto {2}-{3}-{4}", infosForReturnModel.Vd7embarq, infosForReturnModel.Vd7pedido, returnItemModel.Pr0produto, returnItemModel.Pr2opcao, returnItemModel.Pr3tamanho));

            CV5 cv5 = await FiscalDocumentRepository.Find(cv7.Cv7doc, cv7.Cv7tipo, cv7.Cv7emissor);

            if (cv5 == null)
                throw new Exception(string.Format("Documento não encontrado para {0}-{1}", cv7.Cv7doc, cv7.Cv7tipo));
            
            returnItemModel.Doc = cv5.Cv5doc + "-"+ cv5.Cv5tipo;
            returnItemModel.Volume = infosForReturnModel.Vd7volume;
            if (!lxd.Lxdean)
            {
                if(string.IsNullOrWhiteSpace(cf1cliente))
                    returnItemModel.Cf1cliente = vd1.Vd1cliente;
                returnItemModel.Consigned = vd1.Vd1consig;
            }

            decimal cv7descon = lxd.Lxddescdev ? cv7.Cv7descon : 0;
            if (cv7descon > 0)
            {
                cv7descon = await CalculateCv7DesconByOrderItem(cv5, cv7, cv7descon);
            }

            //atribuindo impostos e demais informações no item
            returnItemModel.Preco = cv7.Cv7vlunit;
            returnItemModel.Descon = cv7descon;
            returnItemModel.Precodesc = cv7.Cv7vlunit - cv7descon;
            returnItemModel.Vdedoc = cv5.Cv5docconf;
            returnItemModel.Taxes = new ReturnTaxesModel()
            {
                Baseicm = Math.Round(cv7.Cv7baseicm / cv7.Cv7qtde, 2),
                Valicm = Math.Round(cv7.Cv7valicm / cv7.Cv7qtde, 2),
                Pdif = cv5.Cv5pdif,
                Baseicp = cv7.Cv7baseicp,
                Aliqicm = cv7.Cv7aliqicm,
                Ipi = cv7.Cv7ipi,
                Cfop = cv7.Cv7cfop
            };

            if (lxd.Lxddescdev && cv7.Cv7descon == 0 && cv5.Cv5descon > 0)
            {
                decimal cv5descon = cv5.Cv5descon;
                //cuidar na recuperação do desconto do item para não considerar o desconto do descfat quando tiver romaneio
                //mas no caso de CF ou NF a vulso, não precisa descontar esse valor
                if (cv5.Cv5tipo.Equals(lxd.Lxddocven1))
                {
                    IList<CV7> items = await FiscalDocumentRepository.FindAllCv7(cv5.Cv5doc, cv5.Cv5emissor, cv5.Cv5tipo);
                    decimal grossAmount = items.Sum(cv7 => cv7.Cv7vltotal);
                    decimal percentChange = (cv5.Cv5descfat / grossAmount) * 100;
                    cv5descon -= percentChange;
                }

                cv7descon = Math.Round((cv7.Cv7vlunit * cv5descon) / 100, 2);
                returnItemModel.Preco = cv7.Cv7vlunit;
                returnItemModel.Descon = cv7descon;
                returnItemModel.Precodesc = cv7.Cv7vlunit - cv7descon;
            }
        }

        public async Task<InfosForReturnModel> ValidateVdxpeca(ReturnItemModel returnItemModel, string cf1cliente, bool consigned, IList<ReturnItemModel> lstItems, decimal qty)
        {
            VD7 vd7 = null;
            InfosForReturnModel infosForReturnModel = null;
            VDV vdv = null;
            IList<VDV> lstVdv = null;

            if (returnItemModel.Vdxqtdeent == 0)
            {
                if (!string.IsNullOrWhiteSpace(returnItemModel.Eancodigo))
                {
                    lstVdv = await OrderRepository.FindVdV(returnItemModel.Eancodigo, cf1cliente, consigned);
                }
                if (lstVdv == null || lstVdv.Count == 0)
                {
                    lstVdv = await OrderRepository.FindVdV(returnItemModel.Pr0produto, returnItemModel.Pr2opcao, returnItemModel.Pr3tamanho, cf1cliente, consigned);
                }

                //remover as quantidades na lista de vdv, conforme o preço, para ajudar 

                IList<ReturnItemModel> lstByPiece = lstItems
                    .Where(i => i.Eancodigo.Equals(returnItemModel.Eancodigo) &&
                                i.Pr0produto.Equals(returnItemModel.Pr0produto) &&
                                i.Pr2opcao.Equals(returnItemModel.Pr2opcao) &&
                                i.Pr3tamanho.Equals(returnItemModel.Pr3tamanho) &&
                                i.Vdxqtdeent == 0)?
                    .ToList();

                foreach (ReturnItemModel piecesInCart in lstByPiece)
                {
                    VDV vdv2 = lstVdv.FirstOrDefault(vdv => vdv.Vdvpreco == piecesInCart.Preco);
                    if (vdv2 != null)
                        vdv2.Vdvqtde -= piecesInCart.Qtde;
                }

                decimal qtytoadd = qty;
                foreach (var item in lstVdv)
                {
                    if ((item.Vdvqtde - qtytoadd) >= 0)
                    {
                        vdv = item;
                        vd7 = await OrderRepository.FindVd7(item.Vdvvolume);
                        break;
                    }
                    else
                    {
                        qtytoadd -= item.Vdvqtde;
                        item.Vdvqtde = 0;
                    }
                }
            }

            if (vd7 == null)
            {
                string message = string.Format(
                      "Cliente não possui peças de venda para devolução <br/>Referência {0}{1}{2}"
                    , returnItemModel.Pr0produto
                    , !string.IsNullOrWhiteSpace(returnItemModel.Pr2opcao) ? "-" + returnItemModel.Pr2opcao : string.Empty
                    , !string.IsNullOrWhiteSpace(returnItemModel.Pr3tamanho) ? "-" + returnItemModel.Pr3tamanho : string.Empty);

                if (lxd.Lxdentrdev && !consigned)
                    message += "<br/><span class=\"caption\">Caso a peça tenha sido vendida em outra loja, escolha dar entrada na peça/produto.</span>";

                if ((returnItemModel.Vdxqtdeent == 0 && lxd.Lxdentrdev) || !lxd.Lxdentrdev)
                    throw new Exception(message);

                return infosForReturnModel;
            }

            if (lxd.Lxddevano &&
                vdv.Vdvleitura.Year != DateTime.Now.Year)
            {
                throw new Exception(string.Format("Devolução/troca só pode ser realizada para peças expedidas dentro do ano.<br/>Peça expedida em {0} volume {1}.", vdv.Vdvleitura.ToString("dd/MM/yyyy"), vdv.Vdvvolume));
            }
            if (lxd.Lxddevdias > 0 &&
                vdv.Vdvleitura.AddDays(lxd.Lxddevdias) < DateTime.Now)
            {
                throw new Exception(string.Format("Peça expedida em {0} volume {1}.<br/>Prazo de {2} dias esgotado.", vdv.Vdvleitura.ToString("dd/MM/yyyy"), vdv.Vdvvolume, lxd.Lxddevdias));
            }

            infosForReturnModel = new InfosForReturnModel()
            {
                Price = vdv.Vdvpreco,
                Vd7volume = vd7.Vd7volume,
                Vd7embarq = vd7.Vd7embarq,
                Vd7pedido = vd7.Vd7pedido
            };
            return infosForReturnModel;
        }

        private async Task<decimal> CalculateCv7DesconByOrderItem(CV5 cv5, CV7 cv7, decimal cv7descon)
        {
            //cuidar na recuperação do desconto do item para não considerar o desconto do descfat quando tiver romaneio
            if (cv5.Cv5tipo.Equals(lxd.Lxddocven1))
            {
                IList<CV7> items = await FiscalDocumentRepository.FindAllCv7(cv5.Cv5doc, cv5.Cv5emissor, cv5.Cv5tipo);
                decimal grossAmount = items.Sum(cv7 => cv7.Cv7vltotal);
                decimal percentChange = Math.Round((cv5.Cv5descfat / grossAmount) * 100, 2, MidpointRounding.AwayFromZero);
                cv7descon = ((cv7.Cv7desconp - percentChange) * cv7.Cv7vlunit) / 100;
            }
            else
            {
                //no caso de CF ou NF a vulso, não precisa descontar esse valor - necessário dividir pela quantidade pois grava a soma
                cv7descon = cv7descon / cv7.Cv7qtde;
            }

            return Math.Round(cv7descon,2);
        }
    }
}
