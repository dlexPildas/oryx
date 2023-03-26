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
    public class ReturnRepository : Repository
    {
        public ReturnRepository(string path) : base(path)
        {
        }

        public async Task<VDK> Find(string vdkdoc)
        {
            string command = @"SELECT VDK.*, CF1.CF1NOME
                                 FROM VDK
                                INNER JOIN CF1 ON CF1CLIENTE = VDKCLIENTE
                                INNER JOIN DC4 ON DC4USUARIO = VDKUSUARIO
                                WHERE VDKDOC = @vdkdoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VDK vdk = await connection.QueryFirstOrDefaultAsync<VDK>(command, new { vdkdoc });

                return vdk;
            }
        }

        public async Task<IList<VDL>> FindAllVdl(string vdldoc)
        {
            string command = @"SELECT VDL.*, OF3.OF3PRODUTO, OF3.OF3OPCAO, OF3.OF3TAMANHO, OF3.OF3LOTE, OF3.OF3ORDEM, PR0.PR0DESC
                                 FROM VDL
                                INNER JOIN OF3 ON OF3PECA = VDLPECA
                                INNER JOIN PR0 ON PR0PRODUTO = OF3PRODUTO
                                WHERE VDLDOC = @vdldoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDL> lstVdl = await connection.QueryAsync<VDL>(command, new { vdldoc });

                return lstVdl.ToList();
            }
        }

        public async Task<IList<VDX>> FindAllVdx(string vdxdoc)
        {
            string command = @"SELECT VDX.*, PR0.PR0DESC
                                 FROM VDX
                                INNER JOIN PR0 ON PR0PRODUTO = VDXPRODUTO
                                WHERE VDXDOC = @vdxdoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDX> lstVdX = await connection.QueryAsync<VDX>(command, new { vdxdoc });

                return lstVdX.ToList();
            }
        }

        public async Task<IList<ReturnItemModel>> FindAllVdxForRecover(string vdxdoc)
        {
            string command = @"SELECT
                                      VDX.VDXITEM
                                    , PR0.PR0IMAGEM
                                    , VDX.VDXPRODUTO AS Pr0produto
                                    , PR0.PR0DESC
                                    , VDX.VDXOPCAO AS Pr2opcao
                                    , VDX.VDXTAMANHO AS Pr3tamanho
                                    , VDX.VDXQTDE AS Qtde
                                    , VDX.VDXPRECO AS Preco
                                    , VDX.VDXPRECO * VDX.VDXQTDE AS Total
                                    , VDX.VDXPECA AS Eancodigo
                                    , VDX.VDXLEITURA AS Leitura
                                    , VDX.VDXGRAVOU AS Gravou
                                    , VDX.VDXQTDEENT
                                    , CR1.CR1NOME
                                    , GR1.GR1DESC
                                    , VDX.VDXVOLUME AS Volume
                                 FROM VDX
                                INNER JOIN PR0 ON PR0PRODUTO = VDXPRODUTO
                                 LEFT JOIN CR1 ON CR1COR = VDX.VDXOPCAO
                                 LEFT JOIN GR1 ON GR1TAMANHO = VDX.VDXTAMANHO
                                WHERE VDXDOC = @vdxdoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ReturnItemModel> lstVdX = await connection.QueryAsync<ReturnItemModel>(command, new { vdxdoc });

                return lstVdX.ToList();
            }
        }

        public async Task<IList<ReturnItemModel>> FindAllVdlForRecover(string vdldoc)
        {
            string command = @"SELECT 
                                      PR0.PR0IMAGEM
                                    , OF3.OF3PRODUTO AS Pr0produto
                                    , PR0.PR0DESC
                                    , OF3.OF3OPCAO AS Pr2opcao
                                    , OF3.OF3TAMANHO AS Pr3tamanho
                                    , 1 AS Qtde
                                    , VDL.VDLPRECO AS Preco
                                    , VDL.VDLPRECO * 1 AS Total
                                    , VDL.VDLPECA AS Of3peca
                                    , VDL.VDLLEITURA AS Leitura
                                    , VDL.VDLGRAVOU AS Gravou
                                    , CR1.CR1NOME
                                    , GR1.GR1DESC
                                    , VDL.VDLVOLUME AS Volume
                                    , COALESCE(VD1CONSIG, 0) As Consigned
                                 FROM VDL
                                INNER JOIN OF3 ON OF3PECA = VDLPECA
                                INNER JOIN PR0 ON PR0PRODUTO = OF3.OF3PRODUTO
                                INNER JOIN CR1 ON CR1COR = OF3.OF3OPCAO
                                INNER JOIN GR1 ON GR1TAMANHO = OF3.OF3TAMANHO
                                 LEFT JOIN VD1 ON VD1PEDIDO = VDLPEDIDO
                                WHERE VDLDOC = @vdldoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ReturnItemModel> lstVdl = await connection.QueryAsync<ReturnItemModel>(command, new { vdldoc });

                return lstVdl.ToList();
            }
        }

        public async Task<IList<VDK>> Search(string search, int limit, int offset, DateTime since, DateTime until, string orderBy)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT VDK.*,
                                      CF1NOME,
                                      (SELECT SUM(CV5TOTALNF) FROM CV5 WHERE CV5ENTSAI = 2 AND CV5DOCDEV = VDKDOC) AS Vdktotal,
                                      (CASE
                                          WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5DOCDEV = VDKDOC AND CV5MODELO = '99' AND CV5ENTSAI = 2) > 0
                                          THEN true
                                          ELSE false
                                       END
                                       ) AS HasRomaneio,
                                       (CASE
                                          WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5DOCDEV = VDKDOC AND (CV5MODELO = '55' OR CV5MODELO = '65') AND CV5ENTSAI = 2) > 0
                                          THEN true
                                          ELSE false
                                       END
                                       ) AS HasNF,
                                       (CASE
                                          WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5DOCDEV = VDKDOC AND CV5MODELO = '99' AND CV5ENTSAI = 2 AND CV5EMITIDO = 0) > 0
                                          THEN true
                                          ELSE false
                                       END
                                       ) AS HasPendingRomaneio,
                                       (CASE
                                          WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5DOCDEV = VDKDOC AND (CV5MODELO = '55' OR CV5MODELO = '65') AND CV5ENTSAI = 2 AND CV5EMITIDO = 0) > 0
                                          THEN true
                                          ELSE false
                                       END
                                       ) AS HasPendingNF
                                 FROM VDK
                                INNER JOIN CF1 ON CF1CLIENTE = VDKCLIENTE
                                WHERE (VDKDOC LIKE @search
                                   OR VDKCLIENTE LIKE @search
                                   OR UPPER(CF1NOME) LIKE UPPER(@search))
                                  AND VDKABERT BETWEEN @since AND @until
                                ORDER BY VDKABERT " + orderBy + " LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDK> orders = await connection.QueryAsync<VDK>(command, new { search, limit, offset, since, until });

                return orders.ToList();
            }
        }

        public async Task<IList<dynamic>> FindItemsVDLByCustomer(string cf1cliente)
        {
            string command = @"SELECT VDL.*, OF3.OF3PRODUTO, OF3.OF3OPCAO, OF3.OF3TAMANHO, OF3.OF3LOTE, OF3.OF3ORDEM, PR0.PR0DESC
                                 FROM VDL
                                INNER JOIN VDK ON VDKDOC = VDLDOC
                                INNER JOIN CV5 ON CV5DOCDEV = VDKDOC
                                INNER JOIN FNL ON FNLDOC = CV5DOC AND FNLTIPO = CV5TIPO AND FNLEMISSOR = CV5EMISSOR
                                INNER JOIN OF3 ON OF3PECA = VDLPECA
                                INNER JOIN PR0 ON PR0PRODUTO = OF3PRODUTO
                                WHERE VDKCLIENTE = @cf1cliente";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<dynamic> lstVdl = await connection.QueryAsync<VDL>(command, new { cf1cliente });

                return lstVdl.ToList();
            }
        }

        public async Task<IList<dynamic>> FindItemsVDXByCustomer(string cf1cliente)
        {
            string command = @"SELECT VDX.*, PR0.PR0DESC
                                 FROM VDX
                                INNER JOIN VDK ON VDKDOC = VDXDOC
                                INNER JOIN CV5 ON CV5DOCDEV = VDKDOC
                                INNER JOIN FNL ON FNLDOC = CV5DOC AND FNLTIPO = CV5TIPO AND FNLEMISSOR = CV5EMISSOR
                                INNER JOIN PR0 ON PR0PRODUTO = VDXPRODUTO
                                WHERE VDKCLIENTE = @cf1cliente";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<dynamic> lstVdX = await connection.QueryAsync<VDX>(command, new { cf1cliente });

                return lstVdX.ToList();
            }
        }

        public async Task<int> SaveReturn(VDK vdk, IList<VDL> lstVdl, IList<VDX> lstVdx)
        {
            string commandvdk = @"INSERT INTO VDK (VDKDOC,VDKCLIENTE,VDKMOTIVO,VDKUSUARIO,VDKMANTER,VDKABERT,VDKOBSERVA)
                                 VALUES (@Vdkdoc,@Vdkcliente,@Vdkmotivo,@Vdkusuario,@Vdkmanter,@Vdkabert,@Vdkobserva)";
            string commandvdl = @"INSERT INTO VDL (VDLDOC,VDLPECA,VDLLEITURA,VDLVOLUME,VDLGRAVOU,VDLPRECO,VDLPEDIDO)
                                 VALUES (@Vdldoc,@Vdlpeca,@Vdlleitura,@Vdlvolume,@Vdlgravou,@Vdlpreco,@Vdlpedido)";
            string commandvdx = @"INSERT INTO VDX (VDXDOC,VDXPECA,VDXLEITURA,VDXVOLUME,VDXGRAVOU,VDXPRECO,VDXPEDIDO,VDXQTDE,VDXPRODUTO,VDXOPCAO,VDXTAMANHO,VDXITEM,VDXQTDEENT)
                                 VALUES (@Vdxdoc,@Vdxpeca,@Vdxleitura,@Vdxvolume,@Vdxgravou,@Vdxpreco,@Vdxpedido,@Vdxqtde,@vdxproduto,@vdxopcao,@vdxtamanho,@vdxitem,@vdxqtdeent)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = 0;
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VDL WHERE VDLDOC = @Vdkdoc", new { vdk.Vdkdoc }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VDX WHERE VDXDOC = @Vdkdoc", new { vdk.Vdkdoc }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VDK WHERE VDKDOC = @Vdkdoc", new { vdk.Vdkdoc }, transaction: transaction);

                        affectedRows = await connection.ExecuteAsync(commandvdk, new { vdk.Vdkdoc, vdk.Vdkcliente, vdk.Vdkmotivo, vdk.Vdkusuario, vdk.Vdkmanter, vdk.Vdkabert, vdk.Vdkobserva }, transaction: transaction);
                        foreach (VDL vdl in lstVdl)
                        {
                            affectedRows = await connection.ExecuteAsync(commandvdl, new { vdl.Vdldoc, vdl.Vdlpeca, vdl.Vdlleitura, vdl.Vdlvolume, vdl.Vdlgravou, vdl.Vdlpreco, vdl.Vdlpedido }, transaction: transaction);
                        }
                        foreach (VDX vdx in lstVdx)
                        {
                            affectedRows = await connection.ExecuteAsync(commandvdx, new { vdx.Vdxdoc, vdx.Vdxpeca, vdx.Vdxleitura, vdx.Vdxvolume, vdx.Vdxgravou, vdx.Vdxpreco, vdx.Vdxpedido, vdx.Vdxqtde, vdx.Vdxproduto, vdx.Vdxopcao, vdx.Vdxtamanho, vdx.Vdxitem, vdx.Vdxqtdeent }, transaction: transaction);
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

        public async Task<IList<CustomerHistoryModel>> FindCustomerHistory(string cf1cliente, DateTime since, DateTime until, IList<string> lstCv5tipo)
        {
            string command = @"SELECT   VDK.VDKDOC AS Number
                                      , VDK.VDKABERT AS Dtabert  
                                      , 2 AS Entsai
                                      , (CASE
                                            WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5DOCDEV = VDKDOC AND CV5MODELO = '99' AND CV5ENTSAI = 2) > 0
                                            THEN true
                                            ELSE false
                                         END
                                        ) AS HasRomaneio
                                      , (CASE
                                            WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5DOCDEV = VDKDOC AND (CV5MODELO = '55' OR CV5MODELO = '65') AND CV5ENTSAI = 2) > 0
                                            THEN true
                                            ELSE false
                                         END
                                        ) AS HasNF
                                      , (CASE
                                            WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5DOCDEV = VDKDOC AND CV5MODELO = '99' AND CV5ENTSAI = 2 AND CV5EMITIDO = 0) > 0
                                            THEN true
                                            ELSE false
                                         END
                                        ) AS HasPendingRomaneio
                                      , (CASE
                                            WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5DOCDEV = VDKDOC AND (CV5MODELO = '55' OR CV5MODELO = '65') AND CV5ENTSAI = 2 AND CV5EMITIDO = 0) > 0
                                            THEN true
                                            ELSE false
                                         END
                                        ) AS HasPendingNF
                                      , (SELECT SUM(CV5TOTALNF) FROM CV5 WHERE CV5ENTSAI = 2 AND CV5DOCDEV = VDKDOC) AS Total
                                 FROM VDK
                                INNER JOIN CF1 ON CF1CLIENTE = VDKCLIENTE
                                WHERE VDKCLIENTE = @cf1cliente
                                  AND VDKABERT BETWEEN @since AND @until
                                  AND EXISTS (SELECT 1 FROM CV5 WHERE CV5DOCDEV = VDKDOC AND CV5TIPO IN @lstCv5tipo AND CV5ENTSAI = 2)
                                ORDER BY VDKABERT DESC";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CustomerHistoryModel> orders = await connection.QueryAsync<CustomerHistoryModel>(command, new { cf1cliente, since, until, lstCv5tipo });

                return orders.ToList();
            }
        }

        public async Task<IList<CustomerHistoryDocsModel>> FindCustomerHistoryDocs(string vdkdoc, IList<string> lstCv5tipo)
        {
            string command = @"SELECT CV5DOC, CV5TIPO, CV5EMISSOR, CV5TOTALNF
                                 FROM CV5
                                WHERE CV5DOCDEV = @vdkdoc
                                  AND CV5ENTSAI = 2
                                  AND CV5TIPO IN @lstCv5tipo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CustomerHistoryDocsModel> LstFiscalDocs = await connection.QueryAsync<CustomerHistoryDocsModel>(command, new { vdkdoc, lstCv5tipo });

                return LstFiscalDocs.ToList();
            }
        }
    }
}
