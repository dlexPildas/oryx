using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class FiscalDocumentRepository : Repository
    {
        public FiscalDocumentRepository(string path) : base(path)
        {
        }

        public async Task<int> SetEmit(string cv5emissor, string cv5tipo, string cv5doc, bool cv5emitido)
        {
            string command = @"UPDATE CV5 SET CV5EMITIDO = @cv5emitido WHERE CV5EMISSOR = @cv5emissor AND CV5TIPO = @cv5tipo AND CV5DOC = @cv5doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, new { cv5emissor, cv5tipo, cv5doc, cv5emitido });

                return affectedRows;
            }
        }

        public async Task<int> SetPrint(string cv5emissor, string cv5tipo, string cv5doc, bool cv5impres)
        {
            string command = @"UPDATE CV5 SET CV5IMPRES= @cv5impres WHERE CV5EMISSOR = @cv5emissor AND CV5TIPO = @cv5tipo AND CV5DOC = @cv5doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, new { cv5emissor, cv5tipo, cv5doc, cv5impres });

                return affectedRows;
            }
        }

        public async Task<CV5> Find(string cv5doc, string cv5tipo, string cv5emissor)
        {
            string command = @"SELECT * FROM CV5 WHERE CV5DOC = @cv5doc AND CV5TIPO = @cv5tipo AND CV5EMISSOR = @cv5emissor";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { cv5doc, cv5tipo, cv5emissor });

                return cv5;
            }
        }

        public async Task<CV5> FindByShip(string cv5pedido, string cv5embarq, string cv5tipo)
        {
            string command = @"SELECT * FROM CV5 WHERE CV5PEDIDO = @cv5pedido AND CV5EMBARQ = @cv5embarq AND CV5TIPO = @cv5tipo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { cv5pedido, cv5embarq, cv5tipo });

                return cv5;
            }
        }

        public async Task<IList<CV5>> FindByOrder(string cv5pedido)
        {
            string command = @"SELECT * FROM CV5 WHERE CV5PEDIDO = @cv5pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CV5> lstCv5 = await connection.QueryAsync<CV5>(command, new { cv5pedido });

                return lstCv5.ToList();
            }
        }

        public async Task<IList<CV5>> FindByOrderWithAuthentication(string cv5pedido)
        {
            string command = @"SELECT *, AUTENTICACAO FROM CV5 LEFT JOIN TECDATASOFT ON CV5DOC = CV7DOC AND CV5EMISSOR = CV7EMISSOR AND CV5TIPO = CV7TIPO WHERE CV5PEDIDO = @cv5pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CV5> lstCv5 = await connection.QueryAsync<CV5>(command, new { cv5pedido });

                return lstCv5.ToList();
            }
        }

        public async Task<int> SaveFiscalDocument(CV5 cv5, IList<CV7> lstCv7, IList<CV8> lstCv8, CVT cvt, IList<CVQ> lstCvq, IList<CVJ> lstCvj)
        {
            string commandcv5 = @"INSERT INTO CV5 (CV5EMISSOR,CV5TIPO,CV5DOC,CV5EMISPRO,CV5TAMANHO,CV5EAN,CV5CUPOM,CV5ONLINE,CV5EMISNOM,CV5EMISEST,CV5ECF,CV5ATENDE,CV5TOTALIZ,CV5USUARIO,CV5EMISSAO,CV5DHSAIDA,CV5CLIENTE,CV5REPRES,CV5REPNOM,CV5PEDIDO,CV5EMBARQ,CV5PEDCOM,CV5DOCREC,CV5DOCMOV,CV5TRANSP,CV5REDESP,CV5PEDREP,CV5CONPGTO,CV5CENTRO,CV5EDITAR,CV5CONPNOM,CV5LISTA,CV5LIMDESC,CV5LIMDESP,CV5LIMDIAS,CV5LIMPRAZ,CV5NOMLIS,CV5OPERCOM,CV5OPERDES,CV5ENTSAI,CV5CONSIG,CV5HISEMI,CV5HISLIQ,CV5CST,CV5TITULO,CV5TITNOM,CV5BCOPOR,CV5NOMPOR,CV5COMIS,CV5DESCON,CV5DESCONV,CV5DESPON,CV5DESDIAS,CV5FRETE,CV5PRONTA,CV5QTDEVOL,CV5ESPEVOL,CV5MARCVOL,CV5NUMEVOL,cv5pesobru,cv5pesoliq,CV5TRPLACA,CV5TRPLAUF,CV5ALIQICM,CV5BASEICM,CV5VALICM,CV5BASEISE,CV5BASEOUT,CV5BASEICP,CV5BASEISP,CV5BASEOUP,CV5BASEIPI,cv5observ,CV5BASESUP,CV5ALIQSUB,CV5BASESUB,CV5VALSUB,CV5TOTALPR,CV5VLFRETE,CV5SEGURO,CV5OUTRAS,CV5IPI,CV5TOTALNF,CV5MODELO,CV5SERIE,CV5SUBSER,CV5CFOP,CV5CFOPNOM,CV5CLINOME,CV5ENDCLI,CV5CEP,CV5INSEST,CV5FONE,CV5NOMELOC,CV5ESTADO,CV5TRNOME,CV5TRCNPJ,CV5TRENDER,CV5TRESTAD,CV5TRLOCAL,CV5TRINSCR,CV5ITEMFAB,CV5CNPJ,CV5CIC,CV5EMITIDO,CV5IMPRES,CV5FLAG1,CV5CONTAD,CV5CONTAC,CV5SITUA,CV5CREDSIM,CV5NFECHAV,CV5CSTIPI,CV5CSTPIS,CV5BASEPIS,CV5ALIQPIS,CV5CSTCOF,CV5BASECOF,CV5ALIQCOF,CV5BASEISS,CV5ALIQISS,CV5CODISS,CV5NDI,CV5DDI,CV5LOCDES,CV5UFDES,CV5DDES,CV5CODEXP,CV5NADICAO,CV5SEQADIC,CV5FABRIC,CV5DESCAD,CV5BASEII,CV5IMPIMP,CV5DESPAD,CV5IOFIMP,CV5UFEMB,CV5LOCEMB,CV5ENDENTR,CV5UFENTR,CV5DESENTR,CV5CEPENTR,CV5DIGVAL,CV5NPROT,CV5DHREC,CV5XML,CV5TITULO1,CV5TITULO2,CV5TITULO3,CV5BAICLI,CV5NUMCLI,CV5LOCAL,CV5COMPL,CV5RECLOTE,cv5pais,CV5EMAIL,CV5ORIGEM,CV5NAOCONT,CV5ENQIPI,cv5aliqipi,CV5PISIMP,CV5COFIMP,CV5NUMDRAW,CV5TIPODA,CV5UFDA,CV5NUMDA,CV5AUTDA,CV5VALORDA,CV5VCTODA,CV5PGTODA,CV5INDPROC,CV5NUMPROC,CV5AJUSTE,CV5CREDPIS,CV5CREDCOF,CV5CREDICM,CV5NUMORIG,CV5VLRSERV,CV5TIPOLIG,CV5TENSAO,CV5CODCONS,cv5docdev,CV5CONTIGE,CV5REGDPEC,CV5ESTADOC,CV5DESTINC,CV5VNFC,CV5VICMSC,CV5TOTTRIB,CV5TOTTRI1,CV5TPVIATR,CV5VAFRMM,CV5TPINTER,CV5CNPJTER,CV5PDIF,CV5FCPDEST,CV5ICMDEST,CV5ICMREME,CV5DESCFAT,CV5NPROCAN,CV5NPROINU,CV5DOCCONF,CV5CAMBIO,CV5CBENEF,CV5CAIXA, CV5VFCPST)
                                  VALUES (@Cv5emissor, @Cv5tipo, @Cv5doc, @Cv5emispro, @Cv5tamanho, @Cv5ean, @Cv5cupom, @Cv5online, @Cv5emisnom, @Cv5emisest, @Cv5ecf, @Cv5atende, @Cv5totaliz, @Cv5usuario, @Cv5emissao, @Cv5dhsaida, @Cv5cliente, @Cv5repres, @Cv5repnom, @Cv5pedido, @Cv5embarq, @Cv5pedcom, @Cv5docrec, @Cv5docmov, @Cv5transp, @Cv5redesp, @Cv5pedrep, @Cv5conpgto, @Cv5centro, @Cv5editar, @Cv5conpnom, @Cv5lista, @Cv5limdesc, @Cv5limdesp, @Cv5limdias, @Cv5limpraz, @Cv5nomlis, @Cv5opercom, @Cv5operdes, @Cv5entsai, @Cv5consig, @Cv5hisemi, @Cv5hisliq, @Cv5cst, @Cv5titulo, @Cv5titnom, @Cv5bcopor, @Cv5nompor, @Cv5comis, @Cv5descon, @Cv5desconv, @Cv5despon, @Cv5desdias, @Cv5frete, @Cv5pronta, @Cv5qtdevol, @Cv5espevol, @Cv5marcvol, @Cv5numevol, @Cv5pesobru, @Cv5pesoliq, @Cv5trplaca, @Cv5trplauf, @Cv5aliqicm, @Cv5baseicm, @Cv5valicm, @Cv5baseise, @Cv5baseout, @Cv5baseicp, @Cv5baseisp, @Cv5baseoup, @Cv5baseipi, @Cv5observ, @Cv5basesup, @Cv5aliqsub, @Cv5basesub, @Cv5valsub, @Cv5totalpr, @Cv5vlfrete, @Cv5seguro, @Cv5outras, @Cv5ipi, @Cv5totalnf, @Cv5modelo, @Cv5serie, @Cv5subser, @Cv5cfop, @Cv5cfopnom, @Cv5clinome, @Cv5endcli, @Cv5cep, @Cv5insest, @Cv5fone, @Cv5nomeloc, @Cv5estado, @Cv5trnome, @Cv5trcnpj, @Cv5trender, @Cv5trestad, @Cv5trlocal, @Cv5trinscr, @Cv5itemfab, @Cv5cnpj, @Cv5cic, @Cv5emitido, @Cv5impres, @Cv5flag1, @Cv5contad, @Cv5contac, @Cv5situa, @Cv5credsim, @Cv5nfechav, @Cv5cstipi, @Cv5cstpis, @Cv5basepis, @Cv5aliqpis, @Cv5cstcof, @Cv5basecof, @Cv5aliqcof, @Cv5baseiss, @Cv5aliqiss, @Cv5codiss, @Cv5ndi, @Cv5ddi, @Cv5locdes, @Cv5ufdes, @Cv5ddes, @Cv5codexp, @Cv5nadicao, @Cv5seqadic, @Cv5fabric, @Cv5descad, @Cv5baseii, @Cv5impimp, @Cv5despad, @Cv5iofimp, @Cv5ufemb, @Cv5locemb, @Cv5endentr, @Cv5ufentr, @Cv5desentr, @Cv5cepentr, @Cv5digval, @Cv5nprot, @Cv5dhrec, @Cv5xml, @Cv5titulo1, @Cv5titulo2, @Cv5titulo3, @Cv5baicli, @Cv5numcli, @Cv5local, @Cv5compl, @Cv5reclote, @Cv5pais, @Cv5email, @Cv5origem, @Cv5naocont, @Cv5enqipi, @Cv5aliqipi, @Cv5pisimp, @Cv5cofimp, @Cv5numdraw, @Cv5tipoda, @Cv5ufda, @Cv5numda, @Cv5autda, @Cv5valorda, @Cv5vctoda, @Cv5pgtoda, @Cv5indproc, @Cv5numproc, @Cv5ajuste, @Cv5credpis, @Cv5credcof, @Cv5credicm, @Cv5numorig, @Cv5vlrserv, @Cv5tipolig, @Cv5tensao, @Cv5codcons, @Cv5docdev, @Cv5contige, @Cv5regdpec, @Cv5estadoc, @Cv5destinc, @Cv5vnfc, @Cv5vicmsc, @Cv5tottrib, @Cv5tottri1, @Cv5tpviatr, @Cv5vafrmm, @Cv5tpinter, @Cv5cnpjter, @Cv5pdif, @Cv5fcpdest, @Cv5icmdest, @Cv5icmreme, @Cv5descfat, @Cv5nprocan, @Cv5nproinu, @Cv5docconf, @Cv5cambio, @Cv5cbenef,@cv5caixa,@cv5vfcpst)";
            string commandcv7 = @"INSERT INTO CV7 (CV7EMISSOR,CV7TIPO,CV7DOC,CV7ITEM,CV7CODIGO,CV7COR,CV7TAMANHO,CV7DESC,CV7REFER,cv7qtde,CV7UNMED,cv7vlunit,CV7VLTOTAL,CV7DESCON,CV7PESO,CV7CFOP,CV7ALIQICM,CV7ALIQSUB,CV7BASEICM,CV7BASESUB,CV7BASEISE,CV7BASEOUT,cv7codclas,CV7FLAG1,CV7CLASSIF,CV7EAN,CV7IPI,CV7SUBTOT,CV7CST,CV7VALICM,CV7VALSUB,CV7BASESUP,CV7ORIGEM,CV7ALIQIPI,CV7CSTIPI,CV7BASEIPI,CV7CODISS,cv7contac,cv7contad,CV7TOTTRIB,CV7BASEICP,CV7PEDEDI,CV7ITEMEDI,CV7INFADIC,CV7SERVICO,CV7CSTPIS,CV7CSTCOF,CV7BASEPIS,CV7BASECOF,CV7ALIQPIS,CV7ALIQCOF,CV7DESCONP,CV7CPROD,CV7UCOM,CV7QCOM,CV7CBENEF,CV7PDIF,CV7PFCPST,CV7VFCPST)
                                  VALUES (@Cv7emissor,@Cv7tipo,@Cv7doc,@Cv7item,@Cv7codigo,@Cv7cor,@Cv7tamanho,@Cv7desc,@Cv7refer,@Cv7qtde,@Cv7unmed,@Cv7vlunit,@Cv7vltotal,@Cv7descon,@Cv7peso,@Cv7cfop,@Cv7aliqicm,@Cv7aliqsub,@Cv7baseicm,@Cv7basesub,@Cv7baseise,@Cv7baseout,@Cv7codclas,@Cv7flag1,@Cv7classif,@Cv7ean,@Cv7ipi,@Cv7subtot,@Cv7cst,@Cv7valicm,@Cv7valsub,@Cv7basesup,@Cv7origem,@Cv7aliqipi,@Cv7cstipi,@Cv7baseipi,@Cv7codiss,@Cv7contac,@Cv7contad,@Cv7tottrib,@Cv7baseicp,@Cv7pededi,@Cv7itemedi,@Cv7infadic,@Cv7servico,@Cv7cstpis,@Cv7cstcof,@Cv7basepis,@Cv7basecof,@Cv7aliqpis,@Cv7aliqcof,@Cv7desconp,@Cv7cprod,@Cv7ucom,@Cv7qcom,@Cv7cbenef,@cv7pdif,@cv7pfcpst,@cv7vfcpst)";
            string commandcv8 = @"INSERT INTO CV8 (CV8EMISSOR,CV8TIPO,CV8DOC,CV8PARCELA,CV8TIPOTIT,CV8CMC7,CV8VALOR,CV8BCOPOR,CV8FLAG1,CV8IMPRES,CV8VENCIM,CV8BARRAS,CV8AGENTE,CV8CONTA,CV8NSU,CV8BAND,CV8BOLETO,CV8EXTRATO)
                                  VALUES (@Cv8emissor,@Cv8tipo,@Cv8doc,@Cv8parcela,@Cv8tipotit,@Cv8cmc7,@Cv8valor,@Cv8bcopor,@Cv8flag1,@Cv8impres,@Cv8vencim,@Cv8barras,@Cv8agente,@Cv8conta,@Cv8nsu,@Cv8band,@Cv8boleto,@Cv8extrato)";
            string commandcvt = @"INSERT INTO CVT (CVTEMISSOR,CVTTIPO,CVTDOC,CVTCLIENT0,CVTNOME0,CVTLOGRAD0,CVTNUMERO0,CVTCOMPL0,CVTBAIRRO0,CVTCEP0,CVTLOCAL0,CVTXMUN0,CVTESTADO0,CVTFONE0,CVTINSEST0,CVTCLIENT1,CVTNOME1,CVTLOGRAD1,CVTNUMERO1,CVTCOMPL1,CVTBAIRRO1,CVTCEP1,CVTLOCAL1,CVTXMUN1,CVTESTADO1,CVTFONE1,CVTINSEST1,CVTCLIENT2,CVTNOME2,CVTLOGRAD2,CVTNUMERO2,CVTCOMPL2,CVTBAIRRO2,CVTCEP2,CVTLOCAL2,CVTXMUN2,CVTESTADO2,CVTFONE2,CVTINSEST2,CVTCLIENT3,CVTNOME3,CVTLOGRAD3,CVTNUMERO3,CVTCOMPL3,CVTBAIRRO3,CVTCEP3,CVTLOCAL3,CVTXMUN3,CVTESTADO3,CVTFONE3,CVTINSEST3)
                                  VALUES (@Cvtemissor, @Cvttipo, @Cvtdoc, @Cvtclient0, @Cvtnome0, @Cvtlograd0, @Cvtnumero0, @Cvtcompl0, @Cvtbairro0, @Cvtcep0, @Cvtlocal0, @Cvtxmun0, @Cvtestado0, @Cvtfone0, @Cvtinsest0, @Cvtclient1, @Cvtnome1, @Cvtlograd1, @Cvtnumero1, @Cvtcompl1, @Cvtbairro1, @Cvtcep1, @Cvtlocal1, @Cvtxmun1, @Cvtestado1, @Cvtfone1, @Cvtinsest1, @Cvtclient2, @Cvtnome2, @Cvtlograd2, @Cvtnumero2, @Cvtcompl2, @Cvtbairro2, @Cvtcep2, @Cvtlocal2, @Cvtxmun2, @Cvtestado2, @Cvtfone2, @Cvtinsest2, @Cvtclient3, @Cvtnome3, @Cvtlograd3, @Cvtnumero3, @Cvtcompl3, @Cvtbairro3, @Cvtcep3, @Cvtlocal3, @Cvtxmun3, @Cvtestado3, @Cvtfone3, @Cvtinsest3)";
            string commandcvq = @"INSERT INTO CVQ (CVQEMISSOR,CVQTIPO,CVQDOC,CVQEMIREFE,CVQTIPREFE,CVQDOCREFE)
                                 VALUES (@Cvqemissor,@Cvqtipo,@Cvqdoc,@Cvqemirefe,@Cvqtiprefe,@Cvqdocrefe)";
            string commandcvj = @"INSERT INTO CVJ (CVJEMISSOR,CVJTIPO,CVJDOC,CVJLINHA,CVJOBSERVA)
                                 VALUES (@Cvjemissor,@Cvjtipo,@Cvjdoc,@Cvjlinha,@Cvjobserva)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = 0;
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        affectedRows = await connection.ExecuteAsync(commandcv5, new { cv5.Cv5emissor, cv5.Cv5tipo, cv5.Cv5doc, cv5.Cv5emispro, cv5.Cv5tamanho, cv5.Cv5ean, cv5.Cv5cupom, cv5.Cv5online, cv5.Cv5emisnom, cv5.Cv5emisest, cv5.Cv5ecf, cv5.Cv5atende, cv5.Cv5totaliz, cv5.Cv5usuario, cv5.Cv5emissao, cv5.Cv5dhsaida, cv5.Cv5cliente, cv5.Cv5repres, cv5.Cv5repnom, cv5.Cv5pedido, cv5.Cv5embarq, cv5.Cv5pedcom, cv5.Cv5docrec, cv5.Cv5docmov, cv5.Cv5transp, cv5.Cv5redesp, cv5.Cv5pedrep, cv5.Cv5conpgto, cv5.Cv5centro, cv5.Cv5editar, cv5.Cv5conpnom, cv5.Cv5lista, cv5.Cv5limdesc, cv5.Cv5limdesp, cv5.Cv5limdias, cv5.Cv5limpraz, cv5.Cv5nomlis, cv5.Cv5opercom, cv5.Cv5operdes, cv5.Cv5entsai, cv5.Cv5consig, cv5.Cv5hisemi, cv5.Cv5hisliq, cv5.Cv5cst, cv5.Cv5titulo, cv5.Cv5titnom, cv5.Cv5bcopor, cv5.Cv5nompor, cv5.Cv5comis, cv5.Cv5descon, cv5.Cv5desconv, cv5.Cv5despon, cv5.Cv5desdias, cv5.Cv5frete, cv5.Cv5pronta, cv5.Cv5qtdevol, cv5.Cv5espevol, cv5.Cv5marcvol, cv5.Cv5numevol, cv5.Cv5pesobru, cv5.Cv5pesoliq, cv5.Cv5trplaca, cv5.Cv5trplauf, cv5.Cv5aliqicm, cv5.Cv5baseicm, cv5.Cv5valicm, cv5.Cv5baseise, cv5.Cv5baseout, cv5.Cv5baseicp, cv5.Cv5baseisp, cv5.Cv5baseoup, cv5.Cv5baseipi, cv5.Cv5observ, cv5.Cv5basesup, cv5.Cv5aliqsub, cv5.Cv5basesub, cv5.Cv5valsub, cv5.Cv5totalpr, cv5.Cv5vlfrete, cv5.Cv5seguro, cv5.Cv5outras, cv5.Cv5ipi, cv5.Cv5totalnf, cv5.Cv5modelo, cv5.Cv5serie, cv5.Cv5subser, cv5.Cv5cfop, cv5.Cv5cfopnom, cv5.Cv5clinome, cv5.Cv5endcli, cv5.Cv5cep, cv5.Cv5insest, cv5.Cv5fone, cv5.Cv5nomeloc, cv5.Cv5estado, cv5.Cv5trnome, cv5.Cv5trcnpj, cv5.Cv5trender, cv5.Cv5trestad, cv5.Cv5trlocal, cv5.Cv5trinscr, cv5.Cv5itemfab, cv5.Cv5cnpj, cv5.Cv5cic, cv5.Cv5emitido, cv5.Cv5impres, cv5.Cv5flag1, cv5.Cv5contad, cv5.Cv5contac, cv5.Cv5situa, cv5.Cv5credsim, cv5.Cv5nfechav, cv5.Cv5cstipi, cv5.Cv5cstpis, cv5.Cv5basepis, cv5.Cv5aliqpis, cv5.Cv5cstcof, cv5.Cv5basecof, cv5.Cv5aliqcof, cv5.Cv5baseiss, cv5.Cv5aliqiss, cv5.Cv5codiss, cv5.Cv5ndi, cv5.Cv5ddi, cv5.Cv5locdes, cv5.Cv5ufdes, cv5.Cv5ddes, cv5.Cv5codexp, cv5.Cv5nadicao, cv5.Cv5seqadic, cv5.Cv5fabric, cv5.Cv5descad, cv5.Cv5baseii, cv5.Cv5impimp, cv5.Cv5despad, cv5.Cv5iofimp, cv5.Cv5ufemb, cv5.Cv5locemb, cv5.Cv5endentr, cv5.Cv5ufentr, cv5.Cv5desentr, cv5.Cv5cepentr, cv5.Cv5digval, cv5.Cv5nprot, cv5.Cv5dhrec, cv5.Cv5xml, cv5.Cv5titulo1, cv5.Cv5titulo2, cv5.Cv5titulo3, cv5.Cv5baicli, cv5.Cv5numcli, cv5.Cv5local, cv5.Cv5compl, cv5.Cv5reclote, cv5.Cv5pais, cv5.Cv5email, cv5.Cv5origem, cv5.Cv5naocont, cv5.Cv5enqipi, cv5.Cv5aliqipi, cv5.Cv5pisimp, cv5.Cv5cofimp, cv5.Cv5numdraw, cv5.Cv5tipoda, cv5.Cv5ufda, cv5.Cv5numda, cv5.Cv5autda, cv5.Cv5valorda, cv5.Cv5vctoda, cv5.Cv5pgtoda, cv5.Cv5indproc, cv5.Cv5numproc, cv5.Cv5ajuste, cv5.Cv5credpis, cv5.Cv5credcof, cv5.Cv5credicm, cv5.Cv5numorig, cv5.Cv5vlrserv, cv5.Cv5tipolig, cv5.Cv5tensao, cv5.Cv5codcons, cv5.Cv5docdev, cv5.Cv5contige, cv5.Cv5regdpec, cv5.Cv5estadoc, cv5.Cv5destinc, cv5.Cv5vnfc, cv5.Cv5vicmsc, cv5.Cv5tottrib, cv5.Cv5tottri1, cv5.Cv5tpviatr, cv5.Cv5vafrmm, cv5.Cv5tpinter, cv5.Cv5cnpjter, cv5.Cv5pdif, cv5.Cv5fcpdest, cv5.Cv5icmdest, cv5.Cv5icmreme, cv5.Cv5descfat, cv5.Cv5nprocan, cv5.Cv5nproinu, cv5.Cv5docconf, cv5.Cv5cambio, cv5.Cv5cbenef, cv5.Cv5caixa, cv5.Cv5vfcpst }, transaction: transaction);
                        foreach (CV7 cv7 in lstCv7)
                        {
                            affectedRows = await connection.ExecuteAsync(commandcv7, new { cv7.Cv7emissor, cv7.Cv7tipo, cv7.Cv7doc, cv7.Cv7item, cv7.Cv7codigo, cv7.Cv7cor, cv7.Cv7tamanho, cv7.Cv7desc, cv7.Cv7refer, cv7.Cv7qtde, cv7.Cv7unmed, cv7.Cv7vlunit, cv7.Cv7vltotal, cv7.Cv7descon, cv7.Cv7peso, cv7.Cv7cfop, cv7.Cv7aliqicm, cv7.Cv7aliqsub, cv7.Cv7baseicm, cv7.Cv7basesub, cv7.Cv7baseise, cv7.Cv7baseout, cv7.Cv7codclas, cv7.Cv7flag1, cv7.Cv7classif, cv7.Cv7ean, cv7.Cv7ipi, cv7.Cv7subtot, cv7.Cv7cst, cv7.Cv7valicm, cv7.Cv7valsub, cv7.Cv7basesup, cv7.Cv7origem, cv7.Cv7aliqipi, cv7.Cv7cstipi, cv7.Cv7baseipi, cv7.Cv7codiss, cv7.Cv7contac, cv7.Cv7contad, cv7.Cv7tottrib, cv7.Cv7baseicp, cv7.Cv7pededi, cv7.Cv7itemedi, cv7.Cv7infadic, cv7.Cv7servico, cv7.Cv7cstpis, cv7.Cv7cstcof, cv7.Cv7basepis, cv7.Cv7basecof, cv7.Cv7aliqpis, cv7.Cv7aliqcof, cv7.Cv7desconp, cv7.Cv7cprod, cv7.Cv7ucom, cv7.Cv7qcom, cv7.Cv7cbenef, cv7.Cv7pdif, cv7.Cv7pfcpst, cv7.Cv7vfcpst }, transaction: transaction);
                        }
                        if (lstCv8 != null && lstCv8.Count > 0)
                        {
                            foreach (CV8 cv8 in lstCv8)
                            {
                                affectedRows = await connection.ExecuteAsync(commandcv8, new { cv8.Cv8emissor, cv8.Cv8tipo, cv8.Cv8doc, cv8.Cv8parcela, cv8.Cv8tipotit, cv8.Cv8cmc7, cv8.Cv8valor, cv8.Cv8bcopor, cv8.Cv8flag1, cv8.Cv8impres, cv8.Cv8vencim, cv8.Cv8barras, cv8.Cv8agente, cv8.Cv8conta, cv8.Cv8nsu, cv8.Cv8band, cv8.Cv8boleto, cv8.Cv8extrato }, transaction: transaction);
                            }
                        }
                        if (cvt != null)
                        {
                            affectedRows = await connection.ExecuteAsync(commandcvt, new { cvt.Cvtemissor, cvt.Cvttipo, cvt.Cvtdoc, cvt.Cvtclient0, cvt.Cvtnome0, cvt.Cvtlograd0, cvt.Cvtnumero0, cvt.Cvtcompl0, cvt.Cvtbairro0, cvt.Cvtcep0, cvt.Cvtlocal0, cvt.Cvtxmun0, cvt.Cvtestado0, cvt.Cvtfone0, cvt.Cvtinsest0, cvt.Cvtclient1, cvt.Cvtnome1, cvt.Cvtlograd1, cvt.Cvtnumero1, cvt.Cvtcompl1, cvt.Cvtbairro1, cvt.Cvtcep1, cvt.Cvtlocal1, cvt.Cvtxmun1, cvt.Cvtestado1, cvt.Cvtfone1, cvt.Cvtinsest1, cvt.Cvtclient2, cvt.Cvtnome2, cvt.Cvtlograd2, cvt.Cvtnumero2, cvt.Cvtcompl2, cvt.Cvtbairro2, cvt.Cvtcep2, cvt.Cvtlocal2, cvt.Cvtxmun2, cvt.Cvtestado2, cvt.Cvtfone2, cvt.Cvtinsest2, cvt.Cvtclient3, cvt.Cvtnome3, cvt.Cvtlograd3, cvt.Cvtnumero3, cvt.Cvtcompl3, cvt.Cvtbairro3, cvt.Cvtcep3, cvt.Cvtlocal3, cvt.Cvtxmun3, cvt.Cvtestado3, cvt.Cvtfone3, cvt.Cvtinsest3 }, transaction: transaction);
                        }
                        if (lstCvq != null && lstCvq.Count > 0)
                        {
                            foreach (CVQ cvq in lstCvq)
                            {
                                affectedRows = await connection.ExecuteAsync(commandcvq, new { cvq.Cvqemissor,cvq.Cvqtipo,cvq.Cvqdoc,cvq.Cvqemirefe,cvq.Cvqtiprefe,cvq.Cvqdocrefe }, transaction: transaction);
                            }
                        }
                        if (lstCvj != null && lstCvj.Count >0)
                        {
                            foreach (CVJ cvj in lstCvj)
                            {
                                affectedRows = await connection.ExecuteAsync(commandcvj, new { cvj.Cvjemissor, cvj.Cvjtipo, cvj.Cvjdoc, cvj.Cvjlinha, cvj.Cvjobserva }, transaction: transaction);
                            }
                        }

                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }

                }
                return affectedRows;
            }
        }

        public async Task<int> Update(CV5 cv5)
        {
            string command = @"UPDATE CV5 
                                  SET CV5EMISPRO = @Cv5emispro, CV5TAMANHO = @Cv5tamanho, CV5EAN = @Cv5ean, CV5CUPOM = @Cv5cupom, CV5ONLINE = @Cv5online, CV5EMISNOM = @Cv5emisnom, CV5EMISEST = @Cv5emisest, CV5ECF = @Cv5ecf, CV5ATENDE = @Cv5atende, CV5TOTALIZ = @Cv5totaliz, CV5USUARIO = @Cv5usuario, CV5EMISSAO = @Cv5emissao, CV5DHSAIDA = @Cv5dhsaida, CV5CLIENTE = @Cv5cliente, CV5REPRES = @Cv5repres, CV5REPNOM = @Cv5repnom, CV5PEDIDO = @Cv5pedido, CV5EMBARQ = @Cv5embarq, CV5PEDCOM = @Cv5pedcom, CV5DOCREC = @Cv5docrec, CV5DOCMOV = @Cv5docmov, CV5TRANSP = @Cv5transp, CV5REDESP = @Cv5redesp, CV5PEDREP = @Cv5pedrep, CV5CONPGTO = @Cv5conpgto, CV5CENTRO = @Cv5centro, CV5EDITAR = @Cv5editar, CV5CONPNOM = @Cv5conpnom, CV5LISTA = @Cv5lista, CV5LIMDESC = @Cv5limdesc, CV5LIMDESP = @Cv5limdesp, CV5LIMDIAS = @Cv5limdias, CV5LIMPRAZ = @Cv5limpraz, CV5NOMLIS = @Cv5nomlis, CV5OPERCOM = @Cv5opercom, CV5OPERDES = @Cv5operdes, CV5ENTSAI = @Cv5entsai, CV5CONSIG = @Cv5consig, CV5HISEMI = @Cv5hisemi, CV5HISLIQ = @Cv5hisliq, CV5CST = @Cv5cst, CV5TITULO = @Cv5titulo, CV5TITNOM = @Cv5titnom, CV5BCOPOR = @Cv5bcopor, CV5NOMPOR = @Cv5nompor, CV5COMIS = @Cv5comis, CV5DESCON = @Cv5descon, CV5DESCONV = @Cv5desconv, CV5DESPON = @Cv5despon, CV5DESDIAS = @Cv5desdias, CV5FRETE = @Cv5frete, CV5PRONTA = @Cv5pronta, CV5QTDEVOL = @Cv5qtdevol, CV5ESPEVOL = @Cv5espevol, CV5MARCVOL = @Cv5marcvol, CV5NUMEVOL = @Cv5numevol, cv5pesobru = @Cv5pesobru, cv5pesoliq = @Cv5pesoliq, CV5TRPLACA = @Cv5trplaca, CV5TRPLAUF = @Cv5trplauf, CV5ALIQICM = @Cv5aliqicm, CV5BASEICM = @Cv5baseicm, CV5VALICM = @Cv5valicm, CV5BASEISE = @Cv5baseise, CV5BASEOUT = @Cv5baseout, CV5BASEICP = @Cv5baseicp, CV5BASEISP = @Cv5baseisp, CV5BASEOUP = @Cv5baseoup, CV5BASEIPI = @Cv5baseipi, cv5observ = @Cv5observ, CV5BASESUP = @Cv5basesup, CV5ALIQSUB = @Cv5aliqsub, CV5BASESUB = @Cv5basesub, CV5VALSUB = @Cv5valsub, CV5TOTALPR = @Cv5totalpr, CV5VLFRETE = @Cv5vlfrete, CV5SEGURO = @Cv5seguro, CV5OUTRAS = @Cv5outras, CV5IPI = @Cv5ipi, CV5TOTALNF = @Cv5totalnf, CV5MODELO = @Cv5modelo, CV5SERIE = @Cv5serie, CV5SUBSER = @Cv5subser, CV5CFOP = @Cv5cfop, CV5CFOPNOM = @Cv5cfopnom, CV5CLINOME = @Cv5clinome, CV5ENDCLI = @Cv5endcli, CV5CEP = @Cv5cep, CV5INSEST = @Cv5insest, CV5FONE = @Cv5fone, CV5NOMELOC = @Cv5nomeloc, CV5ESTADO = @Cv5estado, CV5TRNOME = @Cv5trnome, CV5TRCNPJ = @Cv5trcnpj, CV5TRENDER = @Cv5trender, CV5TRESTAD = @Cv5trestad, CV5TRLOCAL = @Cv5trlocal, CV5TRINSCR = @Cv5trinscr, CV5ITEMFAB = @Cv5itemfab, CV5CNPJ = @Cv5cnpj, CV5CIC = @Cv5cic, CV5EMITIDO = @Cv5emitido, CV5IMPRES = @Cv5impres, CV5FLAG1 = @Cv5flag1, CV5CONTAD = @Cv5contad, CV5CONTAC = @Cv5contac, CV5SITUA = @Cv5situa, CV5CREDSIM = @Cv5credsim, CV5NFECHAV = @Cv5nfechav, CV5CSTIPI = @Cv5cstipi, CV5CSTPIS = @Cv5cstpis, CV5BASEPIS = @Cv5basepis, CV5ALIQPIS = @Cv5aliqpis, CV5CSTCOF = @Cv5cstcof, CV5BASECOF = @Cv5basecof, CV5ALIQCOF = @Cv5aliqcof, CV5BASEISS = @Cv5baseiss, CV5ALIQISS = @Cv5aliqiss, CV5CODISS = @Cv5codiss, CV5NDI = @Cv5ndi, CV5DDI = @Cv5ddi, CV5LOCDES = @Cv5locdes, CV5UFDES = @Cv5ufdes, CV5DDES = @Cv5ddes, CV5CODEXP = @Cv5codexp, CV5NADICAO = @Cv5nadicao, CV5SEQADIC = @Cv5seqadic, CV5FABRIC = @Cv5fabric, CV5DESCAD = @Cv5descad, CV5BASEII = @Cv5baseii, CV5IMPIMP = @Cv5impimp, CV5DESPAD = @Cv5despad, CV5IOFIMP = @Cv5iofimp, CV5UFEMB = @Cv5ufemb, CV5LOCEMB = @Cv5locemb, CV5ENDENTR = @Cv5endentr, CV5UFENTR = @Cv5ufentr, CV5DESENTR = @Cv5desentr, CV5CEPENTR = @Cv5cepentr, CV5DIGVAL = @Cv5digval, CV5NPROT = @Cv5nprot, CV5DHREC = @Cv5dhrec, CV5XML = @Cv5xml, CV5TITULO1 = @Cv5titulo1, CV5TITULO2 = @Cv5titulo2, CV5TITULO3 = @Cv5titulo3, CV5BAICLI = @Cv5baicli, CV5NUMCLI = @Cv5numcli, CV5LOCAL = @Cv5local, CV5COMPL = @Cv5compl, CV5RECLOTE = @Cv5reclote, cv5pais = @Cv5pais, CV5EMAIL = @Cv5email, CV5ORIGEM = @Cv5origem, CV5NAOCONT = @Cv5naocont, CV5ENQIPI = @Cv5enqipi, cv5aliqipi = @Cv5aliqipi, CV5PISIMP = @Cv5pisimp, CV5COFIMP = @Cv5cofimp, CV5NUMDRAW = @Cv5numdraw, CV5TIPODA = @Cv5tipoda, CV5UFDA = @Cv5ufda, CV5NUMDA = @Cv5numda, CV5AUTDA = @Cv5autda, CV5VALORDA = @Cv5valorda, CV5VCTODA = @Cv5vctoda, CV5PGTODA = @Cv5pgtoda, CV5INDPROC = @Cv5indproc, CV5NUMPROC = @Cv5numproc, CV5AJUSTE = @Cv5ajuste, CV5CREDPIS = @Cv5credpis, CV5CREDCOF = @Cv5credcof, CV5CREDICM = @Cv5credicm, CV5NUMORIG = @Cv5numorig, CV5VLRSERV = @Cv5vlrserv, CV5TIPOLIG = @Cv5tipolig, CV5TENSAO = @Cv5tensao, CV5CODCONS = @Cv5codcons, cv5docdev = @Cv5docdev, CV5CONTIGE = @Cv5contige, CV5REGDPEC = @Cv5regdpec, CV5ESTADOC = @Cv5estadoc, CV5DESTINC = @Cv5destinc, CV5VNFC = @Cv5vnfc, CV5VICMSC = @Cv5vicmsc, CV5TOTTRIB = @Cv5tottrib, CV5TOTTRI1 = @Cv5tottri1, CV5TPVIATR = @Cv5tpviatr, CV5VAFRMM = @Cv5vafrmm, CV5TPINTER = @Cv5tpinter, CV5CNPJTER = @Cv5cnpjter, CV5PDIF = @Cv5pdif, CV5FCPDEST = @Cv5fcpdest, CV5ICMDEST = @Cv5icmdest, CV5ICMREME = @Cv5icmreme, CV5DESCFAT = @Cv5descfat, CV5NPROCAN = @Cv5nprocan, CV5NPROINU = @Cv5nproinu, CV5DOCCONF = @Cv5docconf, CV5CAMBIO = @Cv5cambio, CV5CBENEF = @Cv5cbenef, CV5CAIXA = @cv5caixa
                                WHERE CV5EMISSOR = @Cv5emissor AND CV5TIPO = @Cv5tipo AND CV5DOC = @Cv5doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, new { cv5.Cv5emissor, cv5.Cv5tipo, cv5.Cv5doc, cv5.Cv5emispro, cv5.Cv5tamanho, cv5.Cv5ean, cv5.Cv5cupom, cv5.Cv5online, cv5.Cv5emisnom, cv5.Cv5emisest, cv5.Cv5ecf, cv5.Cv5atende, cv5.Cv5totaliz, cv5.Cv5usuario, cv5.Cv5emissao, cv5.Cv5dhsaida, cv5.Cv5cliente, cv5.Cv5repres, cv5.Cv5repnom, cv5.Cv5pedido, cv5.Cv5embarq, cv5.Cv5pedcom, cv5.Cv5docrec, cv5.Cv5docmov, cv5.Cv5transp, cv5.Cv5redesp, cv5.Cv5pedrep, cv5.Cv5conpgto, cv5.Cv5centro, cv5.Cv5editar, cv5.Cv5conpnom, cv5.Cv5lista, cv5.Cv5limdesc, cv5.Cv5limdesp, cv5.Cv5limdias, cv5.Cv5limpraz, cv5.Cv5nomlis, cv5.Cv5opercom, cv5.Cv5operdes, cv5.Cv5entsai, cv5.Cv5consig, cv5.Cv5hisemi, cv5.Cv5hisliq, cv5.Cv5cst, cv5.Cv5titulo, cv5.Cv5titnom, cv5.Cv5bcopor, cv5.Cv5nompor, cv5.Cv5comis, cv5.Cv5descon, cv5.Cv5desconv, cv5.Cv5despon, cv5.Cv5desdias, cv5.Cv5frete, cv5.Cv5pronta, cv5.Cv5qtdevol, cv5.Cv5espevol, cv5.Cv5marcvol, cv5.Cv5numevol, cv5.Cv5pesobru, cv5.Cv5pesoliq, cv5.Cv5trplaca, cv5.Cv5trplauf, cv5.Cv5aliqicm, cv5.Cv5baseicm, cv5.Cv5valicm, cv5.Cv5baseise, cv5.Cv5baseout, cv5.Cv5baseicp, cv5.Cv5baseisp, cv5.Cv5baseoup, cv5.Cv5baseipi, cv5.Cv5observ, cv5.Cv5basesup, cv5.Cv5aliqsub, cv5.Cv5basesub, cv5.Cv5valsub, cv5.Cv5totalpr, cv5.Cv5vlfrete, cv5.Cv5seguro, cv5.Cv5outras, cv5.Cv5ipi, cv5.Cv5totalnf, cv5.Cv5modelo, cv5.Cv5serie, cv5.Cv5subser, cv5.Cv5cfop, cv5.Cv5cfopnom, cv5.Cv5clinome, cv5.Cv5endcli, cv5.Cv5cep, cv5.Cv5insest, cv5.Cv5fone, cv5.Cv5nomeloc, cv5.Cv5estado, cv5.Cv5trnome, cv5.Cv5trcnpj, cv5.Cv5trender, cv5.Cv5trestad, cv5.Cv5trlocal, cv5.Cv5trinscr, cv5.Cv5itemfab, cv5.Cv5cnpj, cv5.Cv5cic, cv5.Cv5emitido, cv5.Cv5impres, cv5.Cv5flag1, cv5.Cv5contad, cv5.Cv5contac, cv5.Cv5situa, cv5.Cv5credsim, cv5.Cv5nfechav, cv5.Cv5cstipi, cv5.Cv5cstpis, cv5.Cv5basepis, cv5.Cv5aliqpis, cv5.Cv5cstcof, cv5.Cv5basecof, cv5.Cv5aliqcof, cv5.Cv5baseiss, cv5.Cv5aliqiss, cv5.Cv5codiss, cv5.Cv5ndi, cv5.Cv5ddi, cv5.Cv5locdes, cv5.Cv5ufdes, cv5.Cv5ddes, cv5.Cv5codexp, cv5.Cv5nadicao, cv5.Cv5seqadic, cv5.Cv5fabric, cv5.Cv5descad, cv5.Cv5baseii, cv5.Cv5impimp, cv5.Cv5despad, cv5.Cv5iofimp, cv5.Cv5ufemb, cv5.Cv5locemb, cv5.Cv5endentr, cv5.Cv5ufentr, cv5.Cv5desentr, cv5.Cv5cepentr, cv5.Cv5digval, cv5.Cv5nprot, cv5.Cv5dhrec, cv5.Cv5xml, cv5.Cv5titulo1, cv5.Cv5titulo2, cv5.Cv5titulo3, cv5.Cv5baicli, cv5.Cv5numcli, cv5.Cv5local, cv5.Cv5compl, cv5.Cv5reclote, cv5.Cv5pais, cv5.Cv5email, cv5.Cv5origem, cv5.Cv5naocont, cv5.Cv5enqipi, cv5.Cv5aliqipi, cv5.Cv5pisimp, cv5.Cv5cofimp, cv5.Cv5numdraw, cv5.Cv5tipoda, cv5.Cv5ufda, cv5.Cv5numda, cv5.Cv5autda, cv5.Cv5valorda, cv5.Cv5vctoda, cv5.Cv5pgtoda, cv5.Cv5indproc, cv5.Cv5numproc, cv5.Cv5ajuste, cv5.Cv5credpis, cv5.Cv5credcof, cv5.Cv5credicm, cv5.Cv5numorig, cv5.Cv5vlrserv, cv5.Cv5tipolig, cv5.Cv5tensao, cv5.Cv5codcons, cv5.Cv5docdev, cv5.Cv5contige, cv5.Cv5regdpec, cv5.Cv5estadoc, cv5.Cv5destinc, cv5.Cv5vnfc, cv5.Cv5vicmsc, cv5.Cv5tottrib, cv5.Cv5tottri1, cv5.Cv5tpviatr, cv5.Cv5vafrmm, cv5.Cv5tpinter, cv5.Cv5cnpjter, cv5.Cv5pdif, cv5.Cv5fcpdest, cv5.Cv5icmdest, cv5.Cv5icmreme, cv5.Cv5descfat, cv5.Cv5nprocan, cv5.Cv5nproinu, cv5.Cv5docconf, cv5.Cv5cambio, cv5.Cv5cbenef, cv5.Cv5caixa });

                return affectedRows;
            }
        }        

        public async Task<int> DeleteForInsert(string cv5doc, string cv5emissor, string cv5tipo)
        {
            string commandcv5 = @"DELETE FROM CV5 WHERE CV5EMISSOR = @Cv5emissor AND CV5TIPO = @Cv5tipo AND CV5DOC = @Cv5doc";
            string commandcv7 = @"DELETE FROM CV7 WHERE CV7EMISSOR = @Cv5emissor AND CV7TIPO = @Cv5tipo AND CV7DOC = @Cv5doc";
            string commandcv8 = @"DELETE FROM CV8 WHERE CV8EMISSOR = @Cv5emissor AND CV8TIPO = @Cv5tipo AND CV8DOC = @Cv5doc";
            string commandcvt = @"DELETE FROM CVT WHERE CVTEMISSOR = @Cv5emissor AND CVTTIPO = @Cv5tipo AND CVTDOC = @Cv5doc";
            string commandcvq = @"DELETE FROM CVQ WHERE CVQEMISSOR = @Cv5emissor AND CVQTIPO = @Cv5tipo AND CVQDOC = @Cv5doc";
            string commandcvj = @"DELETE FROM CVJ WHERE CVJEMISSOR = @Cv5emissor AND CVJTIPO = @Cv5tipo AND CVJDOC = @Cv5doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = 0;
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        affectedRows = await connection.ExecuteAsync(commandcv7, new { cv5emissor, cv5tipo, cv5doc }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync(commandcv8, new { cv5emissor, cv5tipo, cv5doc }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync(commandcvt, new { cv5emissor, cv5tipo, cv5doc }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync(commandcvq, new { cv5emissor, cv5tipo, cv5doc }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync(commandcvj, new { cv5emissor, cv5tipo, cv5doc }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync(commandcv5, new { cv5emissor, cv5tipo, cv5doc }, transaction: transaction);

                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }

                }
                return affectedRows;
            }
        }

        public async Task<CV5> FindByReturn(string vdkdoc, string cv5opercom)
        {
            string command = @"SELECT *
                                 FROM CV5
                                WHERE CV5DOCDEV = @vdkdoc AND CV5OPERCOM = @cv5opercom";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { vdkdoc, cv5opercom });

                return cv5;
            }
        }

        public async Task<IList<CV5>> Search(string search, int limit, int offset, DateTime since, DateTime until)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                                 FROM CV5
                                WHERE 
                                    (CV5DOC LIKE @search
                                    OR CVCTIPO LIKE @search
                                    OR CV5EMISSOR LIKE @search
                                    OR CV5CLIENTE LIKE @search
                                    OR CV5PEDIDO LIKE @search
                                    OR CV5DOCDEV LIKE @search
                                    OR CV5OPERCOM LIKE @search)
                                  AND CV5EMISSAO BETWEEN @since AND @until
                                ORDER BY CV5EMISSAO DESC
                                LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CV5> orders = await connection.QueryAsync<CV5>(command, new { search, limit, offset, since, until });

                return orders.ToList();
            }
        }

        public async Task<CV5> FindByCv5docdev(string cv5docdev)
        {
            string command = @"SELECT *
                                 FROM CV5
                                WHERE CV5DOCDEV = @cv5docdev AND CV5ENTSAI = 2";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { cv5docdev });

                return cv5;
            }
        }
        #region Oryx Gestão
        public async Task<CV5> FindFirstByShip(string vd6pedido, string vd6embarq)
        {
            string command = @"SELECT *
                                 FROM CV5
                                WHERE CV5PEDIDO = @vd6pedido AND CV5EMBARQ = @vd6embarq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { vd6pedido, vd6embarq });

                return cv5;
            }
        }
        public async Task<IList<CV5>> FindLstByShip(string vd6pedido, string vd6embarq)
        {
            string command = @"SELECT *
                                 FROM CV5
                                WHERE CV5PEDIDO = @vd6pedido AND CV5EMBARQ = @vd6embarq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CV5> lstCv5 = await connection.QueryAsync<CV5>(command, new { vd6pedido, vd6embarq });

                return lstCv5.ToList();
            }
        }

        public async Task<CV5> FindForMemoriesByOrder(string vd1pedido)
        {
            string command = @"SELECT CV5TIPO,CV5DOC
                                 FROM CV5,CV7
                                WHERE CV5EMISSOR = CV7EMISSOR 
                                  AND CV5TIPO = CV7TIPO
                                  AND CV5DOC = CV7DOC
                                  AND CV7PEDIDO = @vd1pedido
                                  AND CV5EMITIDO = 1 LIMIT 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { vd1pedido });

                return cv5;
            }
        }

        public async Task<CV5> FindByPurchaseOrder(string pedidoDeCompra)
        {
            string command = @"SELECT CV5DOC 
                                 FROM CV5
                                WHERE CV5PEDCOM = @pedidoDeCompra
                                  AND TRIM(CV5DOCREC) = ''
                                  AND CV5ENTSAI = 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { pedidoDeCompra });

                return cv5;
            }
        }

        public async Task<CV5> FindByCv5docconf(string cv5docconf)
        {
            string command = @"SELECT * FROM CV5 WHERE CV5DOCCONF = @cv5docconf AND CV5ENTSAI = 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { cv5docconf });

                return cv5;
            }
        }

        public async Task<string> FindLastDocByCv5Tipo(string tipo)
        {
            string command = @"SELECT MAX(CV5DOC) AS ULTIMO FROM CV5,LX0 WHERE CV5EMISSOR=LX0CLIENTE AND CV5TIPO = @tipo and cv5doc REGEXP '^[0-9]+$'";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string last = await connection.QueryFirstOrDefaultAsync<string>(command, new { tipo });

                return last;
            }
        }

        public async Task<CV5> FindByReturn(string vdkdoc, IList<string> lstOpercom)
        {
            string command = @"SELECT *
                                 FROM CV5
                                WHERE CV5DOCDEV = @vdkdoc AND CV5OPERCOM IN @lstOpercom";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { vdkdoc, lstOpercom });

                return cv5;
            }
        }

        public async Task<IList<CV5>> FindListByReturn(string vdkdoc, IList<string> lstOpercom)
        {
            string command = @"SELECT *
                                 FROM CV5
                                WHERE CV5DOCDEV = @vdkdoc AND CV5OPERCOM IN @lstOpercom";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CV5> lstCv5 = await connection.QueryAsync<CV5>(command, new { vdkdoc, lstOpercom });

                return lstCv5.ToList();
            }
        }
        #endregion

        #region atenda-se
        public async Task<CV5> FindByCv5atende(string cv5atende)
        {
            string command = @"SELECT CV5DOC FROM CV5 WHERE CV5ATENDE = @cv5atende";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { cv5atende });

                return cv5;
            }
        }
        #endregion

        #region Oryx Place
        public async Task<CV5> FindByCv5viagem(string cv5viagem)
        {
            string command = @"SELECT CV5TIPO,CV5DOC FROM CV5 WHERE CV5VIAGEM = @cv5viagem AND CV5VISITA = '       ' AND CV5ENTSAI = 2 AND CV5SITUA= ' '";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { cv5viagem });

                return cv5;
            }
        }

        public async Task<CV5> FindByCv5visita(string cv5visita)
        {
            string command = @"SELECT CV5TIPO, CV5DOC FROM CV5, VDQ WHERE CV5VIAGEM = VDQVIAGEM AND VDQVISITA = @cv5visita AND CV5VISITA = '       ' AND CV5ENTSAI = 2  and CV5SITUA = ' '";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV5 cv5 = await connection.QueryFirstOrDefaultAsync<CV5>(command, new { cv5visita });

                return cv5;
            }
        }
        #endregion

        #region Oryx Esquadrias
        public async Task<IList<InvoiceShipItems>> FindAllShipItemsForEsquadrias(string cv5pedido, string cv5embarq)
        {
            string command = @"SELECT CV7.*
                                    , CV5EMISSAO
                                    , CV5EMISNOM
                                    , VD5QTDE
                                    , VD6OPERDEV
                                 FROM CV7,VD5,VD6,CV5 
                                WHERE VD5PEDIDO = @cv5pedido
                                  AND VD5EMBARQ = @cv5embarq
                                  AND CV7EMISSOR = VD5EMISSOR
                                  AND CV7TIPO = VD5TIPO
                                  AND CV7DOC = VD5DOC
                                  AND CV7ITEM = VD5ITEM
                                  AND VD5PEDIDO = VD6PEDIDO
                                  AND VD5EMBARQ = VD6EMBARQ
                                  AND CV7EMISSOR = CV5EMISSOR
                                  AND CV7TIPO = CV5TIPO
                                  AND CV7DOC = CV5DOC
                                ORDER BY CV7EMISSOR,CV7TIPO,CV7DOC,CV7ITEM";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<InvoiceShipItems> lstItems = await connection.QueryAsync<InvoiceShipItems>(command, new { cv5pedido, cv5embarq });

                return lstItems.ToList();
            }
        }

        #endregion

        #region CV7
        public async Task<IList<CV7>> FindAllCv7(string cv5doc, string cv5emissor, string cv5tipo)
        {
            string command = @"SELECT *
                                 FROM CV7
                                WHERE CV7EMISSOR = @cv5emissor
                                  AND CV7TIPO = @cv5tipo
                                  AND CV7DOC = @cv5doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CV7> lstCv7 = await connection.QueryAsync<CV7>(command, new { cv5tipo, cv5emissor, cv5doc });

                return lstCv7.ToList();
            }
        }

        public async Task<CV7> FindCv7(string pr0produto, string pr2opcao, string pr3tamanho, string cv5pedido, string cv5embarq, IList<string> listCv5tipo)
        {
            string command = @"SELECT CV7.*
                                 FROM CV7
                                INNER JOIN CV5 ON CV7EMISSOR = CV5EMISSOR AND CV7TIPO = CV5TIPO AND CV7DOC = CV5DOC AND CV7TIPO in @listCv5tipo
                                WHERE CV7CODIGO = @pr0produto
                                  AND CV7COR = @pr2opcao
                                  AND CV7TAMANHO = @pr3tamanho
                                  AND CV5PEDIDO = @cv5pedido
                                  AND CV5EMBARQ = @cv5embarq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV7 cv7 = await connection.QueryFirstOrDefaultAsync<CV7>(command, new { pr0produto, pr2opcao, pr3tamanho, cv5pedido, cv5embarq, listCv5tipo });

                return cv7;
            }
        }

        public async Task<CV7> FindCv7(string pr0produto, string pr2opcao, string pr3tamanho, string vd7volume)
        {
            string command = @"SELECT CV7.*
                                 FROM CV7
                                INNER JOIN CV5 ON CV7EMISSOR = CV5EMISSOR AND CV7TIPO = CV5TIPO AND CV7DOC = CV5DOC
                                INNER JOIN VD6 ON VD6PEDIDO = CV5PEDIDO AND VD6EMBARQ = CV5EMBARQ
                                INNER JOIN VD7 ON VD7PEDIDO = CV5PEDIDO AND VD7EMBARQ = VD6EMBARQ
                                WHERE CV7CODIGO = @pr0produto
                                  AND CV7COR = @pr2opcao
                                  AND CV7TAMANHO = @pr3tamanho
                                  AND VD7VOLUME = @vd7volume";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV7 cv7 = await connection.QueryFirstOrDefaultAsync<CV7>(command, new { pr0produto, pr2opcao, pr3tamanho, vd7volume });

                return cv7;
            }
        }

        public async Task<CV7> FindCv7(string pr0produto, string pr2opcao, string pr3tamanho, string cv5pedido, string cv5embarq, IList<string> listCv5tipo, decimal cv7vlunit)
        {
            string command = @"SELECT CV7.*
                                 FROM CV7
                                INNER JOIN CV5 ON CV7EMISSOR = CV5EMISSOR AND CV7TIPO = CV5TIPO AND CV7DOC = CV5DOC AND CV7TIPO in @listCv5tipo
                                WHERE CV7CODIGO = @pr0produto
                                  AND CV7COR = @pr2opcao
                                  AND CV7TAMANHO = @pr3tamanho
                                  AND CV5PEDIDO = @cv5pedido
                                  AND CV5EMBARQ = @cv5embarq
                                  AND CV7VLUNIT = @cv7vlunit";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV7 cv7 = await connection.QueryFirstOrDefaultAsync<CV7>(command, new { pr0produto, pr2opcao, pr3tamanho, cv5pedido, cv5embarq, listCv5tipo, cv7vlunit });

                return cv7;
            }
        }
        #endregion

        #region CV8
        public async Task<IList<CV8>> FindAllCv8(string cv5doc, string cv5emissor, string cv5tipo)
        {
            string command = @"SELECT CV8.*, CV2CODNFCE AS Cv8codnfce
                                 FROM CV8
                                 LEFT JOIN CV2 ON CV2TITULO = CV8TIPOTIT
                                WHERE CV8EMISSOR = @cv5emissor
                                  AND CV8TIPO = @cv5tipo
                                  AND CV8DOC = @cv5doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CV8> lstCv8 = await connection.QueryAsync<CV8>(command, new { cv5tipo, cv5emissor, cv5doc });

                return lstCv8.ToList();
            }
        }

        public async Task<IList<CustomerHistoryPayment>> FindCustomerHistoryPayments(string cv5doc, string cv5tipo, string cv5emissor)
        {
            string command = @"SELECT CV8.*, CV2NOME, CV2CODNFCE AS Cv8codnfce
                                 FROM CV8
                                 LEFT JOIN CV2 ON CV2TITULO = CV8TIPOTIT
                                WHERE CV8EMISSOR = @cv5emissor
                                  AND CV8TIPO = @cv5tipo
                                  AND CV8DOC = @cv5doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CustomerHistoryPayment> lstCv8 = await connection.QueryAsync<CustomerHistoryPayment>(command, new { cv5tipo, cv5emissor, cv5doc });

                return lstCv8.ToList();
            }
        }
        #endregion

        #region CVJ
        public async Task<IList<CVJ>> FindAllCvj(string cv5doc, string cv5emissor, string cv5tipo)
        {
            string command = @"SELECT *
                                 FROM CVJ
                                WHERE CVJEMISSOR = @cv5emissor
                                  AND CVJTIPO = @cv5tipo
                                  AND CVJDOC = @cv5doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CVJ> lstCvj = await connection.QueryAsync<CVJ>(command, new { cv5tipo, cv5emissor, cv5doc });

                return lstCvj.ToList();
            }
        }
        #endregion

        #region CVQ
        public async Task<IList<CVQ>> FindAllCvq(string cv5doc, string cv5emissor, string cv5tipo)
        {
            string command = @"SELECT *
                                 FROM CVQ
                                WHERE CVQEMISSOR = @cv5emissor
                                  AND CVQTIPO = @cv5tipo
                                  AND CVQDOC = @cv5doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CVQ> lstCvq = await connection.QueryAsync<CVQ>(command, new { cv5tipo, cv5emissor, cv5doc });

                return lstCvq.ToList();
            }
        }
        #endregion

        #region CVT
        public async Task<CVT> FindCVT(string cv5doc, string cv5emissor, string cv5tipo)
        {
            string command = @"SELECT * FROM CVT WHERE CVTEMISSOR = @cv5emissor AND CVTTIPO = @cv5tipo AND CVTDOC = @cv5doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CVT cvt = await connection.QueryFirstOrDefaultAsync<CVT>(command, new { cv5doc, cv5emissor, cv5tipo });

                return cvt;
            }
        }
        #endregion

        #region CVP
        public async Task<int> Insert(CVP cvp)
        {
            string command = @"INSERT INTO CVP
                                  (cvpemissor, cvptipo, cvpdoc, cvpevento, cvpusuario, cvpdatah, cvpxcor1, cvpxcor2)
                                VALUES 
                                  (@cvpemissor, @cvptipo, @cvpdoc, @cvpevento, @cvpusuario, @cvpdatah, @cvpxcor1, @cvpxcor2)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, 
                    new {
                        cvpemissor = cvp.Cvpemissor,
                        cvptipo = cvp.Cvptipo,
                        cvpdoc = cvp.Cvpdoc,
                        cvpevento = cvp.Cvpevento,
                        cvpusuario = cvp.Cvpusuario,
                        cvpdatah = cvp.Cvpdatah,
                        cvpxcor1 = cvp.Cvpxcor1,
                        cvpxcor2 = cvp.Cvpxcor2 });

                return affectedRows;
            }

        }

        public async Task<int> UpdateCartaCorrecao(CVP cvp)
        {
            string command = @"UPDATE cvp SET 
                                  cvpnprot = @cvpnProt, cvpdhreg = @cvpdhreg)
                                WHERE 
                                  cvpevento=@nSeqEvento and cvpemissor=@cvpemissor and cvptipo=@cvptipo and cvpdoc=@cvpdoc)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        cvpnProt = cvp.Cvpnprot,
                        cvpdhreg = cvp.Cvpdhreg,
                        nSeqEvento = cvp.Cvpevento,
                        cvpemissor = cvp.Cvpemissor,
                        cvptipo = cvp.Cvptipo,
                        cvpdoc = cvp.Cvpdoc
                    });
                return affectedRows;
            }
        }

        #endregion

    }
}
