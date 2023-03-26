using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class StockRepository : Repository
    {
        public StockRepository(string path) : base(path)
        {
        }

        public async Task<int> DeleteFromTransferOrder(string vd8peca, string vd1cliente)
        {
            string command = @"DELETE FROM VD8 WHERE VD8PECA = @vd8peca AND VD8VOLUME IN (SELECT VD7VOLUME FROM VD7 INNER JOIN VD1 ON VD7PEDIDO = VD1PEDIDO AND VD1CLIENTE = @vd1cliente AND VD1CONSIG = 1 AND VD1PRONTA = 1)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { vd8peca, vd1cliente });
            }
        }

        #region EAN
        public async Task<StockModel> FindStockByEAN(string pr0produto, string pr2opcao, string pr3tamanho)
        {
            string command = @"#cursor excluir
                                CREATE TEMPORARY TABLE EXCLUIR (CV7EMISSOR CHAR(14), CV7TIPO CHAR(3), CV7DOC CHAR(6), CV7CODIGO CHAR(254), CV7COR CHAR(3), CV7TAMANHO CHAR(3), CV7QTDE DECIMAL(10), CV7EAN CHAR(12), PR0REFER CHAR(18), PR0DESC CHAR(254), CV5ENTSAI DECIMAL(1));

                                INSERT INTO EXCLUIR 
                                  SELECT  CV7EMISSOR
                                        , CV7TIPO
                                        , CV7DOC
                                        , CV7CODIGO
                                        , CV7COR
                                        , CV7TAMANHO
                                        , CV7QTDE
                                        , CV7EAN
                                        , PR0REFER
                                        , PR0DESC
                                        , CV5ENTSAI
                                    FROM CV7,CV5,PR0,VD1,VD6
                                    WHERE PR0PRODUTO=CV7CODIGO 
                                    AND CV5EMISSOR=CV7EMISSOR 
                                    AND CV5TIPO=CV7TIPO 
                                    AND CV5DOC=CV7DOC 
                                    AND CV5PEDIDO=VD1PEDIDO 
                                    AND VD6PEDIDO=VD1PEDIDO 
                                    AND CV5EMBARQ=VD6EMBARQ  
                                    AND CV5EMITIDO=1 
                                    AND CV5SITUA=''
                                    AND CV5ITEMFAB>=1 
                                    AND ((CV5.CV5MODELO = '55' OR CV5.CV5MODELO = '65') AND EXISTS(SELECT CV5.CV5DOC FROM CV5 WHERE  CV5.CV5MODELO = '99' AND CV5PEDIDO=VD1PEDIDO AND CV5EMBARQ=VD6EMBARQ AND CV5.CV5SITUA = '' AND CV5.CV5EMITIDO = 1 AND CV5.CV5ENTSAI = 1));
        
                                SELECT  CV7CODIGO AS PR0PRODUTO
                                      , CV7COR AS PR2OPCAO
                                      , CV7TAMANHO AS PR3TAMANHO
                                      , SUM(IF(cv5entsai = 1, CV7QTDE * -1, CV7QTDE)) AS STOCK
                                      , PR0DESC
                                      , PR0.PR0FAMILIA
                                      , PR0.PR0GRUPO
									  , PR0.PR0CATEG
									  , PR0.PR0SEGMENT
									  , PR0.PR0IMAGEM
									  , PR0.PR0PESOLIQ
                                 FROM CV7,CV5,PR0
                                WHERE PR0PRODUTO=CV7CODIGO
                                  AND CV5EMISSOR=CV7EMISSOR
                                  AND CV5TIPO=CV7TIPO
                                  AND CV5DOC=CV7DOC
                                  AND CV5EMITIDO=1
                                  AND CV5SITUA=''
                                  AND CV5ITEMFAB>=1
                                  AND NOT EXISTS (SELECT 1 FROM EXCLUIR AS E WHERE E.CV7EMISSOR = CV7.CV7EMISSOR AND E.CV7TIPO = CV7.CV7TIPO AND E.CV7DOC = CV7.CV7DOC)
                                  AND CV7CODIGO = @pr0produto
                                  AND CV7COR = @pr2opcao
                                  AND CV7TAMANHO = @pr3tamanho
                                GROUP BY CV7CODIGO,CV7COR,CV7TAMANHO
                                ORDER BY CV7CODIGO,CV7COR,CV7TAMANHO,CV5ENTSAI,CV5EMISSAO;";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                StockModel stock = await connection.QueryFirstOrDefaultAsync<StockModel>(command, new { pr0produto, pr2opcao, pr3tamanho });

                return stock;
            }
        }

        public async Task<StockModel> FindStockOrderByEAN(string vd2produto, string vd3opcao, string vd3tamanho, string vd1pedido)
        {
            string command = @"#cursor excluir
                                CREATE TEMPORARY TABLE EXCLUIR (CV7EMISSOR CHAR(14), CV7TIPO CHAR(3), CV7DOC CHAR(6), CV7CODIGO CHAR(254), CV7COR CHAR(3), CV7TAMANHO CHAR(3), CV7QTDE DECIMAL(10), CV7EAN CHAR(12), PR0REFER CHAR(18), PR0DESC CHAR(254), CV5ENTSAI DECIMAL(1));

                                INSERT INTO EXCLUIR 
                                  SELECT  CV7EMISSOR
                                        , CV7TIPO
                                        , CV7DOC
                                        , CV7CODIGO
                                        , CV7COR
                                        , CV7TAMANHO
                                        , CV7QTDE
                                        , CV7EAN
                                        , PR0REFER
                                        , PR0DESC
                                        , CV5ENTSAI
                                    FROM CV7,CV5,PR0,VD1,VD6
                                    WHERE PR0PRODUTO=CV7CODIGO 
                                    AND CV5EMISSOR=CV7EMISSOR 
                                    AND CV5TIPO=CV7TIPO 
                                    AND CV5DOC=CV7DOC 
                                    AND CV5PEDIDO=VD1PEDIDO 
                                    AND CV5PEDIDO = @vd1pedido
                                    AND VD6PEDIDO=VD1PEDIDO 
                                    AND CV5EMBARQ=VD6EMBARQ  
                                    AND CV5EMITIDO=1 
                                    AND CV5SITUA=''
                                    AND CV5ITEMFAB>=1 
                                    AND ((CV5.CV5MODELO = '55' OR CV5.CV5MODELO = '65') AND EXISTS(SELECT CV5.CV5DOC FROM CV5 WHERE  CV5.CV5MODELO = '99' AND CV5PEDIDO=VD1PEDIDO AND CV5EMBARQ=VD6EMBARQ AND CV5.CV5SITUA = '' AND CV5.CV5EMITIDO = 1 AND CV5.CV5ENTSAI = 1));
        
                                SELECT  CV7CODIGO AS PR0PRODUTO
                                      , CV7COR AS PR2OPCAO
                                      , CV7TAMANHO AS PR3TAMANHO
                                      , SUM(IF(cv5entsai = 1, CV7QTDE * -1, CV7QTDE)) AS STOCK
                                      , PR0DESC
                                      , PR0.PR0FAMILIA
                                      , PR0.PR0GRUPO
									  , PR0.PR0CATEG
									  , PR0.PR0SEGMENT
									  , PR0.PR0IMAGEM
									  , PR0.PR0PESOLIQ
                                 FROM CV7,CV5,PR0
                                WHERE PR0PRODUTO=CV7CODIGO
                                  AND CV5EMISSOR=CV7EMISSOR
                                  AND CV5TIPO=CV7TIPO
                                  AND CV5DOC=CV7DOC
                                  AND CV5EMITIDO=1
                                  AND CV5SITUA=''
                                  AND CV5ITEMFAB>=1
                                  AND NOT EXISTS (SELECT 1 FROM EXCLUIR AS E WHERE E.CV7EMISSOR = CV7.CV7EMISSOR AND E.CV7TIPO = CV7.CV7TIPO AND E.CV7DOC = CV7.CV7DOC)
                                  AND CV7CODIGO = @vd2produto
                                  AND CV7COR = @vd3opcao
                                  AND CV7TAMANHO = @vd3tamanho
                                  AND CV5PEDIDO = @vd1pedido
                                GROUP BY CV7CODIGO,CV7COR,CV7TAMANHO
                                ORDER BY CV7CODIGO,CV7COR,CV7TAMANHO,CV5ENTSAI,CV5EMISSAO;";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                StockModel stock = await connection.QueryFirstOrDefaultAsync<StockModel>(command, new { vd2produto, vd3opcao, vd3tamanho, vd1pedido });

                return stock;
            }
        }
        #endregion

        #region peça única
        public async Task<StockModel> FindStockByPieces(string pr0produto, string pr2opcao, string pr3tamanho)
        {
            string command = @"CREATE TEMPORARY TABLE saldotemp (PRODUTO CHAR(10), OPCAO CHAR(3), TAMANHO CHAR(3), QUANTIDADE DECIMAL(10));
                               INSERT INTO saldotemp 
                                    SELECT of3.OF3PRODUTO AS PRODUTO,
                                           of3.OF3OPCAO AS OPCAO,
                                           of3.OF3TAMANHO AS TAMANHO,
                                            COUNT(0) AS QUANTIDADE
                                        FROM of3
                                        INNER JOIN PR0 ON PR0.PR0PRODUTO =OF3.OF3PRODUTO
                                        LEFT JOIN OF0 ON OF3.OF3PECA=OF0.OF0PECA
                                        WHERE (OF3.OF3ORDEM = '' OR OF0.OF0PECA IS NOT NULL)
                                          AND OF3PRODUTO = @pr0produto
                                          AND OF3OPCAO = @pr2opcao
                                          AND OF3TAMANHO = @pr3tamanho
                              
                                    UNION ALL

                                    SELECT of3.OF3PRODUTO AS PRODUTO,
                                           of3.OF3OPCAO AS OPCAO,
                                           of3.OF3TAMANHO AS TAMANHO,
                                           (COUNT(0)*-1) AS QUANTIDADE
                                      FROM vd8
                                     INNER JOIN OF3 ON OF3.OF3PECA =VD8.VD8PECA
                                     INNER JOIN PR0 ON PR0.PR0PRODUTO =OF3.OF3PRODUTO
                                      LEFT JOIN OF0 ON OF3.OF3PECA=OF0.OF0PECA
                                     WHERE (OF3.OF3ORDEM = '' OR OF0.OF0PECA IS NOT NULL)
                                       AND OF3PRODUTO = @pr0produto
                                       AND OF3OPCAO = @pr2opcao
                                       AND OF3TAMANHO = @pr3tamanho

                                    UNION ALL

                                    SELECT VD3.VD3PRODUTO AS PRODUTO
                                         , VD3.VD3OPCAO AS OPCAO
                                         , VD3.VD3TAMANHO AS TAMANHO
                                         , SUM(VD3.VD3QTDE*-1 + (SELECT COUNT(*) AS EXPEDIDO FROM VD8 INNER JOIN OF3 ON OF3PECA = VD8PECA INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD3PEDIDO AND OF3PRODUTO = VD3PRODUTO AND OF3OPCAO = VD3OPCAO AND OF3TAMANHO = VD3TAMANHO )) AS QUANTIDADE
                                     FROM VD3
                                     INNER JOIN VD1 ON VD1PEDIDO = VD3PEDIDO
                                     WHERE (VD1PEDREP <> '' OR VD1ECOMPED <> '')
                                       AND VD3PRODUTO = @pr0produto
                                       AND VD3OPCAO = @pr2opcao
                                       AND VD3TAMANHO = @pr3tamanho
                                       AND VD1PRONTA = 1
                                       AND (VD3.VD3QTDE- (SELECT COUNT(*) AS EXPEDIDO FROM VD8 INNER JOIN OF3 ON OF3PECA = VD8PECA INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD3PEDIDO AND OF3PRODUTO = VD3PRODUTO AND OF3OPCAO = VD3OPCAO AND OF3TAMANHO = VD3TAMANHO )) > 0
                                     GROUP BY  VD3.VD3PRODUTO,
                                               VD3.VD3OPCAO,
                                               VD3.VD3TAMANHO;
                                               
                                               
									SELECT ESTOQUE.PRODUTO AS PR0PRODUTO,
									   ESTOQUE.OPCAO AS PR2OPCAO,
									   ESTOQUE.TAMANHO AS PR3TAMANHO,
									   SUM(ESTOQUE.QUANTIDADE) AS STOCK,
									   PR0.PR0DESC,
									   PR0.PR0FAMILIA,
									   PR0.PR0GRUPO,
									   PR0.PR0CATEG,
									   PR0.PR0SEGMENT,
									   PR0.PR0IMAGEM,
									   PR0.PR0PESOLIQ,
									   PRB.PRBNOME,
									   PRS.PRSNOME,
									   CR1.CR1NOME,
									   GR1.GR1POSICAO,
									   0 AS VD8PRECO
								  FROM SALDOTEMP AS ESTOQUE
								 INNER JOIN PR0 ON PR0.PR0PRODUTO = ESTOQUE.PRODUTO
								  LEFT JOIN PRB ON PRB.PRBFAMILIA = PR0.PR0FAMILIA
								  LEFT JOIN PRS ON PRS.PRSGRUPO = PR0.PR0GRUPO
								  JOIN GR1 ON GR1.GR1TAMANHO = ESTOQUE.TAMANHO
								  JOIN CR1 ON CR1.CR1COR = ESTOQUE.OPCAO
								   AND PRODUTO = @pr0produto
                                   AND OPCAO = @pr2opcao
                                   AND TAMANHO = @pr3tamanho;";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                StockModel stock = await connection.QueryFirstOrDefaultAsync<StockModel>(command, new { pr0produto, pr2opcao, pr3tamanho });

                return stock;
            }
        }

        public async Task<StockModel> FindStockByPieces(string pr0produto)
        {
            string command = @"CREATE TEMPORARY TABLE saldotemp (PRODUTO CHAR(254), OPCAO CHAR(3), TAMANHO CHAR(3), QUANTIDADE DECIMAL(10));
                               INSERT INTO saldotemp 
                                    SELECT of3.OF3PRODUTO AS PRODUTO,
                                           of3.OF3OPCAO AS OPCAO,
                                           of3.OF3TAMANHO AS TAMANHO,
                                            COUNT(0) AS QUANTIDADE
                                        FROM of3
                                        INNER JOIN PR0 ON PR0.PR0PRODUTO =OF3.OF3PRODUTO
                                        LEFT JOIN OF0 ON OF3.OF3PECA=OF0.OF0PECA
                                        WHERE (OF3.OF3ORDEM = '' OR OF0.OF0PECA IS NOT NULL)
                                          AND OF3PRODUTO = @pr0produto
                              
                                    UNION ALL

                                    SELECT of3.OF3PRODUTO AS PRODUTO,
                                           of3.OF3OPCAO AS OPCAO,
                                           of3.OF3TAMANHO AS TAMANHO,
                                           (COUNT(0)*-1) AS QUANTIDADE
                                      FROM vd8
                                     INNER JOIN OF3 ON OF3.OF3PECA =VD8.VD8PECA
                                     INNER JOIN PR0 ON PR0.PR0PRODUTO =OF3.OF3PRODUTO
                                      LEFT JOIN OF0 ON OF3.OF3PECA=OF0.OF0PECA
                                     WHERE (OF3.OF3ORDEM = '' OR OF0.OF0PECA IS NOT NULL)
                                       AND OF3PRODUTO = @pr0produto
                                     GROUP BY OF3PRODUTO

                                    UNION ALL

                                    SELECT VD3.VD3PRODUTO AS PRODUTO
                                         , VD3.VD3OPCAO AS OPCAO
                                         , VD3.VD3TAMANHO AS TAMANHO
                                         , SUM(VD3.VD3QTDE*-1 + (SELECT COUNT(*) AS EXPEDIDO FROM VD8 INNER JOIN OF3 ON OF3PECA = VD8PECA INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD3PEDIDO AND OF3PRODUTO = VD3PRODUTO AND OF3OPCAO = VD3OPCAO AND OF3TAMANHO = VD3TAMANHO )) AS QUANTIDADE
                                     FROM VD3
                                     INNER JOIN VD1 ON VD1PEDIDO = VD3PEDIDO
                                     WHERE (VD1PEDREP <> '' OR VD1ECOMPED <> '')
                                       AND VD3PRODUTO = @pr0produto
                                       AND VD1PRONTA = 1
                                       AND (VD3.VD3QTDE- (SELECT COUNT(*) AS EXPEDIDO FROM VD8 INNER JOIN OF3 ON OF3PECA = VD8PECA INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD3PEDIDO AND OF3PRODUTO = VD3PRODUTO AND OF3OPCAO = VD3OPCAO AND OF3TAMANHO = VD3TAMANHO )) > 0
                                     GROUP BY  VD3.VD3PRODUTO,
                                               VD3.VD3OPCAO,
                                               VD3.VD3TAMANHO;
                                               
									SELECT ESTOQUE.PRODUTO AS PR0PRODUTO,
									   ESTOQUE.OPCAO AS PR2OPCAO,
									   ESTOQUE.TAMANHO AS PR3TAMANHO,
									   SUM(ESTOQUE.QUANTIDADE) AS STOCK,
									   PR0.PR0DESC,
									   PR0.PR0FAMILIA,
									   PR0.PR0GRUPO,
									   PR0.PR0CATEG,
									   PR0.PR0SEGMENT,
									   PR0.PR0IMAGEM,
									   PR0.PR0PESOLIQ,
									   PRB.PRBNOME,
									   PRS.PRSNOME,
									   CR1.CR1NOME,
									   GR1.GR1POSICAO,
									   0 AS VD8PRECO
								  FROM SALDOTEMP AS ESTOQUE
								 INNER JOIN PR0 ON PR0.PR0PRODUTO = ESTOQUE.PRODUTO
								  LEFT JOIN PRB ON PRB.PRBFAMILIA = PR0.PR0FAMILIA
								  LEFT JOIN PRS ON PRS.PRSGRUPO = PR0.PR0GRUPO
								  JOIN GR1 ON GR1.GR1TAMANHO = ESTOQUE.TAMANHO
								  JOIN CR1 ON CR1.CR1COR = ESTOQUE.OPCAO
								 WHERE PRODUTO = @pr0produto;";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                StockModel stock = await connection.QueryFirstOrDefaultAsync<StockModel>(command, new { pr0produto });

                return stock;
            }
        }

        public async Task<StockModel> FindStockOrderByPieces(string pr0produto, string pr2opcao, string pr3tamanho, string vd1pedido)
        {
            string command = @"CREATE TEMPORARY TABLE saldotemp (PRODUTO CHAR(10), OPCAO CHAR(3), TAMANHO CHAR(3), QUANTIDADE DECIMAL(10));
                               INSERT INTO saldotemp 
                                    SELECT of3.OF3PRODUTO AS PRODUTO,
                                           of3.OF3OPCAO AS OPCAO,
                                           of3.OF3TAMANHO AS TAMANHO,
                                           (COUNT(0)*-1) AS QUANTIDADE
                                      FROM VD8
                                     INNER JOIN VD7 ON VD7.VD7VOLUME = VD8.VD8VOLUME
                                     INNER JOIN OF3 ON OF3.OF3PECA = VD8.VD8PECA
                                     INNER JOIN PR0 ON PR0.PR0PRODUTO = OF3.OF3PRODUTO
                                      LEFT JOIN OF0 ON OF3.OF3PECA = OF0.OF0PECA
                                     WHERE (OF3.OF3ORDEM = '' OR OF0.OF0PECA IS NOT NULL)
                                       AND OF3PRODUTO = @pr0produto
                                       AND OF3OPCAO = @pr2opcao
                                       AND OF3TAMANHO = @pr3tamanho
                                       AND VD7PEDIDO = @vd1pedido

                                    UNION ALL

                                    SELECT VD3.VD3PRODUTO AS PRODUTO
                                         , VD3.VD3OPCAO AS OPCAO
                                         , VD3.VD3TAMANHO AS TAMANHO
                                         , SUM(VD3.VD3QTDE*-1 + (SELECT COUNT(*) AS EXPEDIDO FROM VD8 INNER JOIN OF3 ON OF3PECA = VD8PECA INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD3PEDIDO AND OF3PRODUTO = VD3PRODUTO AND OF3OPCAO = VD3OPCAO AND OF3TAMANHO = VD3TAMANHO )) AS QUANTIDADE
                                     FROM VD3
                                     INNER JOIN VD1 ON VD1PEDIDO = VD3PEDIDO
                                     WHERE (VD1PEDREP <> '' OR VD1ECOMPED <> '')
                                       AND VD3PRODUTO = @pr0produto
                                       AND VD3OPCAO = @pr2opcao
                                       AND VD3TAMANHO = @pr3tamanho
                                       AND VD1PRONTA = 1
                                       AND VD1PEDIDO = @vd1pedido
                                       AND (VD3.VD3QTDE- (SELECT COUNT(*) AS EXPEDIDO FROM VD8 INNER JOIN OF3 ON OF3PECA = VD8PECA INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD3PEDIDO AND OF3PRODUTO = VD3PRODUTO AND OF3OPCAO = VD3OPCAO AND OF3TAMANHO = VD3TAMANHO )) > 0
                                     GROUP BY  VD3.VD3PRODUTO,
                                               VD3.VD3OPCAO,
                                               VD3.VD3TAMANHO;
                                               
                                               
								SELECT ESTOQUE.PRODUTO AS PR0PRODUTO,
									   ESTOQUE.OPCAO AS PR2OPCAO,
									   ESTOQUE.TAMANHO AS PR3TAMANHO,
									   SUM(ESTOQUE.QUANTIDADE) AS STOCK,
									   PR0.PR0DESC,
									   PR0.PR0FAMILIA,
									   PR0.PR0GRUPO,
									   PR0.PR0CATEG,
									   PR0.PR0SEGMENT,
									   PR0.PR0IMAGEM,
									   PR0.PR0PESOLIQ,
									   PRB.PRBNOME,
									   PRS.PRSNOME,
									   CR1.CR1NOME,
									   GR1.GR1POSICAO,
									   0 AS VD8PRECO
								  FROM SALDOTEMP AS ESTOQUE
								 INNER JOIN PR0 ON PR0.PR0PRODUTO = ESTOQUE.PRODUTO
								  LEFT JOIN PRB ON PRB.PRBFAMILIA = PR0.PR0FAMILIA
								  LEFT JOIN PRS ON PRS.PRSGRUPO = PR0.PR0GRUPO
								  JOIN GR1 ON GR1.GR1TAMANHO = ESTOQUE.TAMANHO
								  JOIN CR1 ON CR1.CR1COR = ESTOQUE.OPCAO
								   AND PRODUTO = @pr0produto
                                   AND OPCAO = @pr2opcao
                                   AND TAMANHO = @pr3tamanho;";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                StockModel stock = await connection.QueryFirstOrDefaultAsync<StockModel>(command, new { pr0produto, pr2opcao, pr3tamanho, vd1pedido });

                return stock;
            }
        }
        #endregion
    }
}
