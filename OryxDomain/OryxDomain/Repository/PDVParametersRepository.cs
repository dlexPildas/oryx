using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PDVParametersRepository : Repository
    {
        public PDVParametersRepository(string path) : base(path)
        {
        }

        public async Task<int> Update(LXD lxd)
        {
            string command = @"UPDATE LXD
                                  SET LXDLISTA = @lxdlista
                                    , LXDEAN = @lxdean
                                    , LXDINTESHO = @lxdintesho
                                    , LXDCLIENTE = @lxdcliente
                                    , LXDCONPGTO = @lxdconpgto
                                    , LXDTITULO = @lxdtitulo
                                    , LXDROMANF = @lxdromanf
                                    , LXDMANTER = @lxdmanter
                                    , LXDDEVANO = @lxddevano
                                    , LXDDEVDIAS = @lxddevdias
                                    , LXDOBRCLI = @lxdobrcli
                                    , LXDRFID = @lxdrfid
                                    , LXDNFCE = @lxdnfce
                                    , LXDCONSIG = @lxdconsig
                                    , LXDNF = @lxdnf
                                    , LXDROMANEI = @lxdromanei
                                    , LXDQTDE = @lxdqtde
                                    , LXDMULTROM = @lxdmultrom
                                    , LXDTIPINSH = @lxdtipinsh
                                    , LXDTITULO1 = @lxdtitulo1
                                    , LXDTITULO2 = @lxdtitulo2
                                    , LXDTITULO3 = @lxdtitulo3
                                    , LXDTITULOD = @lxdtitulod
                                    , LXDDOCVEN1 = @lxddocven1
                                    , LXDDOCVEN2 = @lxddocven2
                                    , LXDDOCVEN3 = @lxddocven3
                                    , LXDDOCVEN4 = @lxddocven4
                                    , LXDDOCVEN5 = @lxddocven5
                                    , LXDDOCDEV1 = @lxddocdev1
                                    , LXDDOCDEV2 = @lxddocdev2
                                    , LXDDOCDEV3 = @lxddocdev3
                                    , LXDDOCDEV4 = @lxddocdev4
                                    , LXDDEVVEN = @lxddevven
                                    , LXDCONSNF = @lxdconsnf
                                    , LXDULTGUIA = @lxdultguia
                                    , LXDMOTIDEV = @lxdmotidev
                                    , LXDENTRDEV = @lxdentrdev
                                    , LXDDOCENT1 = @lxddocent1
                                    , LXDDOCENT2 = @lxddocent2
                                    , LXDDESCDEV = @lxddescdev
                                    , LXDMERCADO = @lxdmercado
                                    , LXDCAIXA = @lxdcaixa
                                    , LXDETIQVOL = @lxdetiqvol
                                    , LXDTIPETIQ = @lxdtipetiq
                                    , LXDNFBOL = @lxdnfbol
                                    , LXDABAOUTR = @lxdabaoutr
                                    , LXDDOCCRE = @lxddoccre
                                    , LXDDESCBRU = @lxddescbru
                                    , LXDORYXBI = @lxdoryxbi
                                    , LXDCHEQUE = @lxdcheque
                                    , LXDSALVAR = @lxdsalvar
                                    , LXDPRINREL = @lxdprinrel
                                    , LXDRELAT = @lxdrelat
                                    , LXDORYXWHA = @lxdoryxwha
                                    , LXDPRECMOD = @lxdprecmod
                                    , LXDPRECEST = @lxdprecest
                                    , LXDPRECLIS = @lxdpreclis
                                    , LXDESTVEN = @lxdestven
                                    , LXDESTNEG = @lxdestneg
                                    , LXDESTBLOQ = @lxdestbloq
                                    , LXDCOMADEV = @lxdcomadev
                                WHERE LXDPADRAO = @lxdpadrao";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    lxdpadrao = lxd.Lxdpadrao,
                    lxdlista = lxd.Lxdlista,
                    lxdean = lxd.Lxdean,
                    lxdintesho = lxd.Lxdintesho,
                    lxdcliente = lxd.Lxdcliente,
                    lxdconpgto = lxd.Lxdconpgto,
                    lxdtitulo = lxd.Lxdtitulo,
                    lxdromanf = lxd.Lxdromanf,
                    lxdmanter = lxd.Lxdmanter,
                    lxddevano = lxd.Lxddevano,
                    lxddevdias = lxd.Lxddevdias,
                    lxdobrcli = lxd.Lxdobrcli,
                    lxdrfid = lxd.Lxdrfid,
                    lxdnfce = lxd.Lxdnfce,
                    lxdconsig = lxd.Lxdconsig,
                    lxdnf = lxd.Lxdnf,
                    lxdromanei = lxd.Lxdromanei,
                    lxdqtde = lxd.Lxdqtde,
                    lxdmultrom = lxd.Lxdmultrom,
                    lxdtipinsh = lxd.Lxdtipinsh,
                    lxdtitulo1 = lxd.Lxdtitulo1,
                    lxdtitulo2 = lxd.Lxdtitulo2,
                    lxdtitulo3 = lxd.Lxdtitulo3,
                    lxdtitulod = lxd.Lxdtitulod,
                    lxddocven1 = lxd.Lxddocven1,
                    lxddocven2 = lxd.Lxddocven2,
                    lxddocven3 = lxd.Lxddocven3,
                    lxddocven4 = lxd.Lxddocven4,
                    lxddocven5 = lxd.Lxddocven5,
                    lxddocdev1 = lxd.Lxddocdev1,
                    lxddocdev2 = lxd.Lxddocdev2,
                    lxddocdev3 = lxd.Lxddocdev3,
                    lxddocdev4 = lxd.Lxddocdev4,
                    lxddevven = lxd.Lxddevven,
                    lxdconsnf = lxd.Lxdconsnf,
                    lxdultguia = lxd.Lxdultguia,
                    lxdmotidev = lxd.Lxdmotidev,
                    lxdentrdev = lxd.Lxdentrdev,
                    lxddocent1 = lxd.Lxddocent1,
                    lxddocent2 = lxd.Lxddocent2,
                    lxddescdev = lxd.Lxddescdev,
                    lxdmercado = lxd.Lxdmercado,
                    lxdcaixa = lxd.Lxdcaixa,
                    lxdetiqvol = lxd.Lxdetiqvol,
                    lxdtipetiq = lxd.Lxdtipetiq,
                    lxdnfbol = lxd.Lxdnfbol,
                    lxdabaoutr = lxd.Lxdabaoutr,
                    lxddoccre = lxd.Lxddoccre,
                    lxddescbru = lxd.Lxddescbru,
                    lxdoryxbi = lxd.Lxdoryxbi,
                    lxdcheque = lxd.Lxdcheque,
                    lxdsalvar = lxd.Lxdsalvar,
                    lxdprinrel = lxd.Lxdprinrel,
                    lxdrelat = lxd.Lxdrelat,
                    lxdoryxwha = lxd.Lxdoryxwha,
                    lxdprecmod = lxd.Lxdprecmod,
                    lxdprecest = lxd.Lxdprecest,
                    lxdpreclis = lxd.Lxdpreclis,
                    lxdestven = lxd.Lxdestven,
                    lxdestneg = lxd.Lxdestneg,
                    lxdestbloq = lxd.Lxdestbloq,
                    lxdcomadev = lxd.Lxdcomadev
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete()
        {
            string command = @"DELETE FROM LXD";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command);
            }
        }

        public async Task<int> Insert(LXD lxd)
        {
            string command = @"INSERT INTO LXD (LXDPADRAO, LXDLISTA, LXDEAN, LXDINTESHO, LXDCLIENTE, LXDCONPGTO, LXDTITULO, LXDROMANF, LXDMANTER, LXDDEVANO, LXDDEVDIAS, LXDOBRCLI,LXDRFID,LXDNFCE,LXDCONSIG,LXDNF,LXDROMANEI,LXDQTDE,LXDMULTROM,LXDTIPINSH,LXDTITULO1,LXDTITULO2,LXDTITULO3,LXDTITULOD,LXDDOCVEN1,LXDDOCVEN2,LXDDOCVEN3,LXDDOCVEN4,LXDDOCVEN5,LXDDOCDEV1,LXDDOCDEV2,LXDDOCDEV3,LXDDOCDEV4,LXDDEVVEN,LXDCONSNF,LXDULTGUIA,LXDMOTIDEV,LXDENTRDEV,LXDDOCENT1,LXDDESCDEV,LXDMERCADO,LXDCAIXA,LXDETIQVOL,LXDTIPETIQ,LXDNFBOL,LXDABAOUTR,LXDDOCCRE,LXDDESCBRU,LXDORYXBI,LXDCHEQUE,LXDSALVAR,LXDPRINREL,LXDRELAT,LXDORYXWHA,LXDPRECMOD,LXDPRECEST,LXDPRECLIS,LXDESTVEN,LXDESTNEG,LXDESTBLOQ,LXDCOMADEV)
                               VALUES (@lxdpadrao,@lxdlista,@lxdean,@lxdintesho,@lxdcliente,@lxdconpgto,@lxdtitulo,@lxdromanf,@lxdmanter,@lxddevano,@lxddevdias,@lxdobrcli,@lxdrfid,@lxdnfce,@lxdconsig,@lxdnf,@lxdromanei,@lxdqtde,@lxdmultrom,@lxdtipinsh,@lxdtitulo1,@lxdtitulo2,@lxdtitulo3,@lxdtitulod,@lxddocven1,@lxddocven2,@lxddocven3,@lxddocven4,@lxddocven5,@lxddocdev1,@lxddocdev2,@lxddocdev3,@lxddocdev4,@lxddevven,@lxdconsnf,@lxdultguia,@lxdmotidev,@lxdentrdev,@lxddocent1,@lxddescdev,@lxdmercado,@lxdcaixa,@lxdetiqvol,@lxdtipetiq,@lxdnfbol,@lxdabaoutr,@lxddoccre,@lxddescbru,@lxdoryxbi,@lxdcheque,@lxdsalvar,@lxdprinrel,@lxdrelat,@lxdoryxwha,@lxdprecmod,@lxdprecest,@lxdpreclis,@lxdestven,@lxdestneg,@lxdestbloq,@lxdcomadev)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    lxdpadrao = lxd.Lxdpadrao,
                    lxdlista = lxd.Lxdlista,
                    lxdean = lxd.Lxdean,
                    lxdintesho = lxd.Lxdintesho,
                    lxdcliente = lxd.Lxdcliente,
                    lxdconpgto = lxd.Lxdconpgto,
                    lxdtitulo = lxd.Lxdtitulo,
                    lxdromanf = lxd.Lxdromanf,
                    lxdmanter = lxd.Lxdmanter,
                    lxddevano = lxd.Lxddevano,
                    lxddevdias = lxd.Lxddevdias,
                    lxdobrcli = lxd.Lxdobrcli,
                    lxdrfid = lxd.Lxdrfid,
                    lxdnfce = lxd.Lxdnfce,
                    lxdconsig = lxd.Lxdconsig,
                    lxdnf = lxd.Lxdnf,
                    lxdromanei = lxd.Lxdromanei,
                    lxdqtde = lxd.Lxdqtde,
                    lxdmultrom = lxd.Lxdmultrom,
                    lxdtipinsh = lxd.Lxdtipinsh,
                    lxdtitulo1 = lxd.Lxdtitulo1,
                    lxdtitulo2 = lxd.Lxdtitulo2,
                    lxdtitulo3 = lxd.Lxdtitulo3,
                    lxdtitulod = lxd.Lxdtitulod,
                    lxddocven1 = lxd.Lxddocven1,
                    lxddocven2 = lxd.Lxddocven2,
                    lxddocven3 = lxd.Lxddocven3,
                    lxddocven4 = lxd.Lxddocven4,
                    lxddocven5 = lxd.Lxddocven5,
                    lxddocdev1 = lxd.Lxddocdev1,
                    lxddocdev2 = lxd.Lxddocdev2,
                    lxddocdev3 = lxd.Lxddocdev3,
                    lxddocdev4 = lxd.Lxddocdev4,
                    lxddevven = lxd.Lxddevven,
                    lxdconsnf = lxd.Lxdconsnf,
                    lxdultguia = lxd.Lxdultguia,
                    lxdmotidev = lxd.Lxdmotidev,
                    lxdentrdev = lxd.Lxdentrdev,
                    lxddocent1 = lxd.Lxddocent1,
                    lxddocent2 = lxd.Lxddocent2,
                    lxddescdev = lxd.Lxddescdev,
                    lxdmercado = lxd.Lxdmercado,
                    lxdcaixa = lxd.Lxdcaixa,
                    lxdetiqvol = lxd.Lxdetiqvol,
                    lxdtipetiq = lxd.Lxdtipetiq,
                    lxdnfbol = lxd.Lxdnfbol,
                    lxdabaoutr = lxd.Lxdabaoutr,
                    lxddoccre = lxd.Lxddoccre,
                    lxddescbru = lxd.Lxddescbru,
                    lxdoryxbi = lxd.Lxdoryxbi,
                    lxdcheque = lxd.Lxdcheque,
                    lxdsalvar = lxd.Lxdsalvar,
                    lxdprinrel = lxd.Lxdprinrel,
                    lxdrelat = lxd.Lxdrelat,
                    lxdoryxwha = lxd.Lxdoryxwha,
                    lxdprecmod = lxd.Lxdprecmod,
                    lxdprecest = lxd.Lxdprecest,
                    lxdpreclis = lxd.Lxdpreclis,
                    lxdestven = lxd.Lxdestven,
                    lxdestneg = lxd.Lxdestneg,
                    lxdestbloq = lxd.Lxdestbloq,
                    lxdcomadev = lxd.Lxdcomadev
                });

                return affectedRows;
            }
        }

        public async Task<LXD> Find()
        {
            string command = @"SELECT  LXD.*
                                     , CV2T1.CV2NOME AS Lxdtitulo1Desc
                                     , CV2T2.CV2NOME AS Lxdtitulo2Desc
                                     , CV2T3.CV2NOME AS Lxdtitulo3Desc
                                     , CV2TD.CV2NOME AS LxdtitulodDesc
                                FROM LXD
                                LEFT JOIN CV2 AS CV2T1 ON CV2T1.CV2TITULO = LXD.LXDTITULO1
                                LEFT JOIN CV2 AS CV2T2 ON CV2T2.CV2TITULO = LXD.LXDTITULO2
                                LEFT JOIN CV2 AS CV2T3 ON CV2T3.CV2TITULO = LXD.LXDTITULO3
                                LEFT JOIN CV2 AS CV2TD ON CV2TD.CV2TITULO = LXD.LXDTITULOD";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                LXD lxd = await connection.QueryFirstOrDefaultAsync<LXD>(command);

                return lxd;
            }
        }
    }
}
