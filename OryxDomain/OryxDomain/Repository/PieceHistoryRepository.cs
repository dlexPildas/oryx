using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PieceHistoryRepository : Repository
    {
        public PieceHistoryRepository(string path) : base(path)
        {
        }

        public async Task<PieceData> FindPieceData(string of3peca)
        {
            string command = @"SELECT PR0DESC,
                               PR0REFER,
                               PR2COR,
                               CR1NOME 
                               FROM OF3
                               INNER JOIN PR0 ON OF3PRODUTO = PR0PRODUTO
                               LEFT JOIN PR2 ON PR2PRODUTO = OF3PRODUTO 
                               AND OF3OPCAO=PR2OPCAO
                               LEFT JOIN CR1 ON CR1COR = PR2COR
                               WHERE OF3PECA = @of3peca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PieceData piece = await connection.QueryFirstOrDefaultAsync<PieceData>(command, new { of3peca });

                return piece;
            }
        }

        public async Task<OF1> FindFabricationOrder(string of3peca)
        {
            string command = @"SELECT * FROM 
                               OF1,
                               OF3 
                               WHERE OF3ORDEM = OF1ORDEM 
                               AND OF3PECA = @of3peca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                OF1 fabricationorder = await connection.QueryFirstOrDefaultAsync<OF1>(command, new { of3peca });

                return fabricationorder;
            }
        }

        public async Task<List<ShiftReportsModel>> FindShiftReports(string of3peca)
        {
            string command = @"SELECT MV4.* ,
                                MV5.* ,
                                CE3NOME,
                                PR4NOME 
                                FROM 
                                PR4,
                                MV4,
                                MV5,
                                CE3 
                                WHERE MV5OPER=PR4OPER 
                                AND MV4LIDER=CE3LIDER 
                                AND MV5RELAT=MV4RELAT 
                                AND MV5PECA = @of3peca 
                                ORDER BY MV4INICIO";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ShiftReportsModel> ofalst = await connection.QueryAsync<ShiftReportsModel>(command, new { of3peca });

                return ofalst.ToList();
            }
        }

        public async Task<List<GuideModel>> FindGuide(string of3peca)
        {
            string command = @"SELECT PR4NOME,
                               CE0NOME,
                               MV3ENTRADA,
                               MV3DOC,
                               MV3SAIDA 
                               FROM 
                               MV3,
                               PR4,
                               OF3,
                               MV2,
                               MV1,
                               CE0 
                               WHERE MV3PECA=OF3PECA 
                               AND MV2OPER=PR4OPER
                               AND MV2DOC=MV1DOC 
                               AND MV1CELULA = CE0CELULA 
                               AND MV3DOC=MV1DOC 
                               AND MV3PECA=@of3peca 
                               AND YEAR(MV3ENTRADA) > 1900
                               ORDER BY MV3ENTRADA";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<GuideModel> guidelst = await connection.QueryAsync<GuideModel>(command, new { of3peca });

                return guidelst.ToList();
            }
        }

        public async Task<List<DevolutionModel>> FindDevolution(string of3peca)
        {
            string command = @"SELECT VDLDOC,
                                VDLPEDIDO,
                                VD1CLIENTE,
                                CF1NOME,
                                VDLPECA,
                                VDLLEITURA 
                                FROM VDK,
                                VDL,
                                VD1,
                                CF1 
                                WHERE VDKDOC = VDLDOC 
                                AND VDLPEDIDO = VD1PEDIDO 
                                AND VD1CLIENTE = CF1CLIENTE 
                                AND VDLPECA = @of3peca 
                                ORDER BY VDLLEITURA";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<DevolutionModel> devolution = await connection.QueryAsync<DevolutionModel>(command, new { of3peca });

                return devolution.ToList();
            }
        }

        public async Task<LastExpeditionModel> FindLastExpedition(string of3peca)
        {
            string command = @"SELECT CF1NOME
                             ,CF3NOME
                             ,CF3ESTADO
                             ,VD1PEDIDO
	                         ,VD1CONSIG
                             ,VD8VOLUME
                             ,VD8LEITURA
                             ,VDEDOC
                             ,VDEID
                             FROM VD8
                             INNER JOIN VD7 ON VD8VOLUME = VD7VOLUME 
                             INNER JOIN VD6 ON VD7PEDIDO = VD6PEDIDO 
                             AND VD7EMBARQ = VD6EMBARQ
                             INNER JOIN VD1 ON VD7PEDIDO = VD1PEDIDO
                             INNER JOIN CF1 ON VD1CLIENTE = CF1CLIENTE
                             INNER JOIN CF2 ON CF1CEP = CF2CEP
                             INNER JOIN CF3 ON CF2LOCAL = CF3LOCAL
                             LEFT JOIN VDF ON VDFPECA = VD8PECA 
                             AND VDFVOLUME = VD8VOLUME
                             LEFT JOIN VDE ON VDEDOC = VDFDOC
                             WHERE VD8PECA = @of3peca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                LastExpeditionModel lastexpedition = await connection.QueryFirstOrDefaultAsync<LastExpeditionModel>(command, new { of3peca });

                return lastexpedition;
            }
        }
    }
}
