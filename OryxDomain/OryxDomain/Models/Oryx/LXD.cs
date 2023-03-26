using OryxDomain.Models.Enums;
using System.Collections.Generic;

namespace OryxDomain.Models.Oryx
{
    public class LXD
    {
        public string Lxdpadrao { get; set; }
        public string Lxdlista { get; set; }
        public bool Lxdean { get; set; }
        public bool Lxdintesho { get; set; }
        public string Lxdcliente { get; set; }
        public string Lxdconpgto { get; set; }
        public string Lxdtitulo { get; set; }
        public bool Lxdromanf { get; set; }
        public bool Lxdmanter { get; set; }
        public bool Lxddevano { get; set; }
        public int Lxddevdias { get; set; }
        public bool Lxdobrcli { get; set; }
        public bool Lxdrfid { get; set; }
        public bool Lxdnfce { get; set; }
        public bool Lxdconsig { get; set; }
        public bool Lxdnf { get; set; }
        public bool Lxdromanei { get; set; }
        public bool Lxdqtde { get; set; }
        public bool Lxdmultrom { get; set; }
        public MallIntegrationType Lxdtipinsh { get; set; }
        public string Lxdtitulo1 { get; set; }
        public string Lxdtitulo2 { get; set; }
        public string Lxdtitulo3 { get; set; }
        public string Lxdtitulod { get; set; }
        public string Lxdtitulo1Desc { get; set; }
        public string Lxdtitulo2Desc { get; set; }
        public string Lxdtitulo3Desc { get; set; }
        public string LxdtitulodDesc { get; set; }
        public string Lxddocven1 { get; set; }
        public string Lxddocven2 { get; set; }
        public string Lxddocven3 { get; set; }
        public string Lxddocven4 { get; set; }
        public string Lxddocdev1 { get; set; }
        public string Lxddocdev2 { get; set; }
        public string Lxddocdev3 { get; set; }
        public string Lxddocven5 { get; set; }
        public string Lxddocdev4 { get; set; }
        public bool Lxddevven { get; set; }
        public bool Lxdconsnf { get; set; }
        public bool Lxdultguia { get; set; }
        public string Lxdmotidev { get; set; }
        public bool Lxdentrdev { get; set; }
        public string Lxddocent1 { get; set; }
        public string Lxddocent2 { get; set; }
        public bool Lxddescdev { get; set; }
        public bool Lxdmercado { get; set; }
        public bool Lxdcaixa { get; set; }
        public bool Lxdetiqvol { get; set; }
        public VolumeLabelType Lxdtipetiq { get; set; }
        public bool Lxdnfbol { get; set; }
        public bool Lxdabaoutr { get; set; }
        public string Lxddoccre { get; set; }
        public bool Lxddescbru { get; set; }
        public string Lxdoryxbi { get; set; }
        public bool Lxdcheque { get; set; }
        public bool Lxdsalvar { get; set; }
        public bool Lxdprinrel { get; set; }
        public string Lxdrelat { get; set; }
        public bool Lxdoryxwha { get; set; }
        public bool Lxdprecmod { get; set; }
        public bool Lxdprecest { get; set; }
        public string Lxdpreclis { get; set; }
        public bool Lxdestven { get; set; }
        public bool Lxdestneg { get; set; }
        public bool Lxdestbloq { get; set; }
        public decimal Lxdcomadev { get; set; }

        public Dictionary<string, string> LstCardBrand 
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "01", "Visa" },
                    { "02", "MasterCard" },
                    { "03", "American" },
                    { "04", "Sorocred" },
                    { "05", "Diners Club" },
                    { "06", "Elo" },
                    { "07", "HyperCard" },
                    { "08", "Aura" },
                    { "09", "Cabal" },
                    { "99", "Outros" }
                };
            }
            set { }
        }
    }
}