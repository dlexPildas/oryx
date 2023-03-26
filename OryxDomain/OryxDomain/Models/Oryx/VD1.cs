using OryxDomain.Models.Enums;
using System;
using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class VD1
    {
        public string Vd1pedido { get; set; }
        public string Vd1cliente { get; set; }
        public string Cf1nome { get; set; }
        public string Vd1repres { get; set; }
        public string Vd1transp { get; set; }
        public string Vd1pedrep { get; set; }
        public DateTime Vd1entrada { get; set; }
        public DateTime Vd1entrega { get; set; }
        public string Vd1conpgto { get; set; }
        public string Vd1lista { get; set; }
        public string Vd1opercom { get; set; }
        public decimal Vd1comis { get; set; }
        public decimal Vd1descon { get; set; }
        public decimal Vd1despon { get; set; }
        public int Vd1desdias { get; set; }
        public FreightType Vd1frete { get; set; }
        public bool Vd1pronta { get; set; }
        public string Vd1redesp { get; set; }
        public string Vd1usuario { get; set; }
        public string Vd1docimp { get; set; }
        public DateTime Vd1abert { get; set; }
        public string OrderDate
        { 
            get 
            {
                return this.Vd1abert.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public string Vd1observa { get; set; }
        public string Vd1encerra { get; set; }
        public bool Vd1excluir { get; set; }
        public string Vd1emissor { get; set; }
        public decimal Vd1vlfrete { get; set; }
        public string Vd1local { get; set; }
        public string Vd1diasemb { get; set; }
        public int Vd1matcli { get; set; }
        public string Vd1ecomped { get; set; }
        public string Vd1status { get; set; }
        public string Vd1statdes { get; set; }
        public decimal Vd1total { get; set; }
        public string Vd1vend { get; set; }
        public decimal Vd1comven { get; set; }
        public int QtdeItems { get; set; }
        public StepCreateOrderType Step { get; set; }
        public IList<OrderItem> Items { get; set; }
        public IList<VD2> LstVd2 { get; set; }
        public IList<VD3> LstVd3 { get; set; }
        public IList<VD5> LstVd5 { get; set; }
        public IList<VD6> LstVd6 { get; set; }
        public bool Vd1consig { get; set; }
        public decimal Vd1vltroca { get; set; }
        public string CodAuthSalesMall { get; set; }
        public string Cf3nome { get; set; }
        public string Cf3estado { get; set; }
        public string Cf1fone { get; set; }
        public string Cf1email { get; set; }
        public string Cf6nome { get; set; }
        public string Cv6nome { get; set; }
        public decimal Cv6limpraz { get; set; }
        public string Cv3nome { get; set; }
        public string Vd4nome { get; set; }
        public string Cf7nome { get; set; }
        public string Vdedoc { get; set; }
        public string Ve0nome { get; set; }
        public ShippingInfoOfOrderModel ShippingInfoOfOrderModel { get; set; }
        public OtherExpensesModel OtherExpenses { get; set; }

        public bool HasRomaneio { get; set; }
        public bool HasNF { get; set; }
        public bool HasPendingRomaneio { get; set; }
        public bool HasPendingNF { get; set; }
        public bool ConsignmentClosed { get; set; }
    }
}
