using Dapper;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ProductRepository : Repository
    {
        public ProductRepository(string path) : base(path)
        {
        }

        public async Task<VD2> FindByvD1(string vd1pedido)
        {
            string command = @"SELECT *
                                FROM VD2
                                WHERE VD2PEDIDO = @vd1
                                INNER JOIN PR0 ON VD2PRODUTO = PR0.PR0PRODUTO";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD2 order = await connection.QueryFirstOrDefaultAsync<VD2>(command, new { vd1pedido });

                return order;
            }
        }

        public async Task<IEnumerable<HomeProductModel>> FindProductsByCollection(string co0colecao)
        {
            string command = @"SELECT Pr0produto
                                    , Pr0desc
                                    , (SELECT B2ICAMINHO FROM B2I WHERE B2IPRODUTO = PR0PRODUTO AND B2IPRINCIP = 1 LIMIT 1) AS Pr0imagem
                                    , (CASE
                                          WHEN (SELECT CVGPRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR0PRODUTO LIMIT 1) IS NOT NULL
                                          THEN (SELECT CVGPRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR0PRODUTO LIMIT 1)
                                          ELSE (SELECT (PR3PRECO*(CV6PERCENT/100)) AS CVGPRECO FROM PR3 INNER JOIN B2B ON B2BPADRAO = 1 INNER JOIN CV6 ON B2BPRECOS=CV6LISTA WHERE PR3PRODUTO = PR0PRODUTO ORDER BY 1 LIMIT 1)
                                       END
                                       ) AS Cvgpreco
                                    FROM PR0
                                    INNER JOIN CO0 ON CO0COLECAO = PR0COLECAO AND CO0B2B = 1 AND CO0COLECAO = @co0colecao
                                    INNER JOIN PRS ON PRSGRUPO = PR0GRUPO AND PRSB2B = 1
                                    WHERE PR0B2B = 0
                                    ORDER BY RAND()
                                    LIMIT 30";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<HomeProductModel> products = await connection.QueryAsync<HomeProductModel>(command, new { co0colecao });

                return products;
            }
        }

        public async Task<ProductModel> FindB2b(string pr0produto)
        {
            string command = @"SELECT Pr0produto
                                    , Pr0desc
                                    , Pr0colecao
                                    , Pr0etiq
                                    , Pr0ficha
                                    , Pr0grupo
                                    , (CASE
                                          WHEN (SELECT CVGPRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR0PRODUTO LIMIT 1) IS NOT NULL
                                          THEN (SELECT CVGPRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR0PRODUTO LIMIT 1)
                                          ELSE (SELECT (PR3PRECO*(CV6PERCENT/100)) AS CVGPRECO FROM PR3 INNER JOIN B2B ON B2BPADRAO = 1 INNER JOIN CV6 ON B2BPRECOS=CV6LISTA WHERE PR3PRODUTO = PR0PRODUTO ORDER BY 1 LIMIT 1)
                                       END
                                       ) AS Pr0preco
                                FROM PR0
                                INNER JOIN CO0 ON CO0COLECAO = PR0COLECAO AND CO0B2B = 1
                                INNER JOIN PRS ON PRSGRUPO = PR0GRUPO AND PRSB2B = 1
                                WHERE PR0B2B = 0
                                  AND PR0PRODUTO = @pr0produto";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                ProductModel product = await connection.QueryFirstOrDefaultAsync<ProductModel>(command, new { pr0produto });

                return product;
            }
        }

        public async Task<T> Find<T>(string pr0produto)
        {
            string command = @"SELECT *
                               FROM PR0
                               WHERE PR0PRODUTO = @pr0produto";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                T product = await connection.QueryFirstOrDefaultAsync<T>(command, new { pr0produto });

                return product;
            }
        }

        public async Task<ProductCartModel> FindProductCart(string pr0produto)
        {
            string command = @"SELECT PR0PRODUTO,
	                                  PR0DESC,
                                      PR0IMAGEM,
                                      PR0PESOLIQ,
                                      PR0PESOBRU
                                FROM PR0
                                WHERE PR0PRODUTO = @pr0produto";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                ProductCartModel product = await connection.QueryFirstOrDefaultAsync<ProductCartModel>(command, new { pr0produto });
                return product;
            }
        }

        public async Task<int> Update(PR0 pr0)
        {
            string command = @"UPDATE PR0
                               SET PR0REFER = @pr0refer,
                               PR0DESC = @pr0desc,
                               PR0ETIQ = @pr0etiq,
                               PR0COLECAO = @pr0colecao,
                               PR0FAMILIA = @pr0familia,
                               PR0CLASSIF = @pr0classif,
                               PR0GRADE = @pr0grade,
                               PR0MOLDE = @pr0molde,
                               PR0PILOTO = @pr0piloto,
                               PR0INST = @pr0inst,
                               PR0COMPOS = @pr0compos,
                               PR0ESPECIF = @pr0especif,
                               PR0IMAGEM = @pr0imagem,
                               PR0THUMB = @pr0thumb,
                               PR0REFCLI = @pr0refcli,
                               PR0LOTEMAX = @pr0lotemax,
                               PR0PESOACA = @pr0pesoaca,
                               PR0PESOBRU = @pr0pesobru,
                               PR0PESOLIQ = @pr0pesoliq,
                               PR0DESCFIS = @pr0descfis,
                               PR0EAN = @pr0ean,
                               PR0REVENDA = @pr0revenda,
                               PR0ORIGEM = @pr0origem,
                               PR0FCI = @pr0fci,
                               PR0CONTIMP = @pr0contimp,
                               PR0GRUPO = @pr0grupo,
                               PR0TOTTRIB = @pr0tottrib,
                               PR0UNMED = @pr0unmed,
                               PR0CATEG = @pr0categ,
                               PR0SEGMENT = @pr0segment,
                               PR0B2B = @pr0b2b,
                               PR0FICHA = @pr0ficha,
                               PR0UNMEDTR = @pr0unmedtr,
                               PR0FATORTR = @pr0fatortr,
                               PR0PRECOCO = @pr0precoco,
                               PR0ALTURA = @pr0altura,
                               PR0LARGURA = @pr0largura,
                               PR0COMPRIM = @pr0comprim,
                               WHERE PR0PRODUTO = @pr0produto";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        pr0refer = pr0.Pr0refer,
                        pr0desc = pr0.Pr0desc,
                        pr0etiq = pr0.Pr0etiq,
                        pr0colecao = pr0.Pr0colecao,
                        pr0familia = pr0.Pr0familia,
                        pr0classif = pr0.Pr0classif,
                        pr0grade = pr0.Pr0grade,
                        pr0molde = pr0.Pr0grade,
                        pr0piloto = pr0.Pr0piloto,
                        pr0inst = pr0.Pr0inst,
                        pr0compos = pr0.Pr0compos,
                        pr0especif = pr0.Pr0especif,
                        pr0imagem = pr0.Pr0imagem,
                        pr0thumb = pr0.Pr0thumb,
                        pr0refcli = pr0.Pr0refcli,
                        pr0lotemax = pr0.Pr0lotemax,
                        pr0pesoaca = pr0.Pr0pesoaca,
                        pr0pesobru = pr0.Pr0pesobru,
                        pr0pesoliq = pr0.Pr0pesoliq,
                        pr0descfis = pr0.Pr0descfis,
                        pr0ean = pr0.Pr0ean,
                        pr0revenda = pr0.Pr0revenda,
                        pr0origem = pr0.Pr0origem,
                        pr0fci = pr0.Pr0fci,
                        pr0contimp = pr0.Pr0contimp,
                        pr0grupo = pr0.Pr0grupo,
                        pr0tottrib = pr0.Pr0tottrib,
                        pr0unmed = pr0.Pr0unmed,
                        pr0categ = pr0.Pr0categ,
                        pr0segment = pr0.Pr0segment,
                        pr0b2b = pr0.Pr0b2b,
                        pr0ficha = pr0.Pr0ficha,
                        pr0unmedtr = pr0.Pr0unmedtr,
                        pr0fatortr = pr0.Pr0fatortr,
                        pr0precoco = pr0.Pr0precoco,
                        pr0produto = pr0.Pr0produto,
                        pr0altura = pr0.Pr0altura,
                        pr0largura = pr0.Pr0largura,
                        pr0comprim = pr0.Pr0comprim
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Insert(PR0 pr0)
        {
            string command = @"INSERT INTO PR0 
                               (PR0PRODUTO, 
                               PR0REFER,
                               PR0DESC,
                               PR0ETIQ,
                               PR0COLECAO,
                               PR0FAMILIA,
                               PR0CLASSIF,
                               PR0GRADE
                               PR0MOLDE,
                               PR0PILOTO,
                               PR0INST,
                               PR0COMPOS,
                               PR0ESPECIF,
                               PR0IMAGEM,
                               PR0THUMB,
                               PR0REFCLI,
                               PR0LOTEMAX,
                               PR0PESOACA
                               PR0PESOBRU,
                               PR0PESOLIQ,
                               PR0DESCFIS,
                               PR0EAN,
                               PR0REVENDA,
                               PR0ORIGEM,
                               PR0FCI,
                               PR0CONTIMP,
                               PR0GRUPO,
                               PR0TOTTRIB,
                               PR0UNMED
                               PR0CATEG,
                               PR0SEGMENT,
                               PR0B2B,
                               PR0FICHA,
                               PR0UNMEDTR,
                               PR0FATORTR,
                               PR0PRECOCO,
                               PR0ALTURA,
                               PR0LARGURA,
                               PR0COMPRIM)
                               VALUES (@pr0produto, 
                                @pr0refer, 
                                @pr0desc,
                                @pr0etiq, 
                                @pr0colecao, 
                                @pr0familia, 
                                @pr0classif, 
                                @pr0grade, 
                                @pr0molde, 
                                @pr0piloto,
                                @pr0inst, 
                                @pr0compos, 
                                @pr0especif, 
                                @pr0imagem, 
                                @pr0thumb, 
                                @pr0refcli, 
                                @pr0lotemax,
                                @pr0pesoaca,
                                @pr0pesobru,
                                @pr0pesoliq,
                                @pr0descfis, 
                                @pr0ean, 
                                @pr0revenda,
                                @pr0origem,
                                @pr0fci,
                                @pr0contimp,
                                @pr0grupo,
                                @pr0tottrib,
                                @pr0unmed,
                                @pr0categ
                                @pr0segment, 
                                @pr0b2b, 
                                @pr0ficha,
                                @pr0unmedtr, 
                                @pr0fatortr,
                                @pr0precoco,
                                @pr0altura,
                                @pr0largura,
                                @pr0comprim)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pr0produto = pr0.Pr0produto,
                    pr0refer = pr0.Pr0refer,
                    pr0desc = pr0.Pr0desc,
                    pr0etiq = pr0.Pr0etiq,
                    pr0colecao = pr0.Pr0colecao,
                    pr0familia = pr0.Pr0familia,
                    pr0classif = pr0.Pr0classif,
                    pr0grade = pr0.Pr0grade,
                    pr0molde = pr0.Pr0molde,
                    pr0piloto = pr0.Pr0piloto,
                    pr0inst = pr0.Pr0inst,
                    pr0compos = pr0.Pr0compos,
                    pr0especif = pr0.Pr0especif,
                    pr0imagem = pr0.Pr0imagem,
                    pr0thumb = pr0.Pr0thumb,
                    pr0refcli = pr0.Pr0refcli,
                    pr0lotemax = pr0.Pr0lotemax,
                    pr0pesoaca = pr0.Pr0pesoaca,
                    pr0pesobru = pr0.Pr0pesobru,
                    pr0pesoliq = pr0.Pr0pesoliq,
                    pr0descfis = pr0.Pr0descfis,
                    pr0ean = pr0.Pr0ean,
                    pr0revenda = pr0.Pr0revenda,
                    pr0origem = pr0.Pr0origem,
                    pr0fci = pr0.Pr0descfis,
                    pr0contimp = pr0.Pr0contimp,
                    pr0grupo = pr0.Pr0grupo,
                    pr0tottrib = pr0.Pr0tottrib,
                    pr0unmed = pr0.Pr0unmed,
                    pr0categ = pr0.Pr0categ,
                    pr0segment = pr0.Pr0segment,
                    pr0b2b = pr0.Pr0b2b,
                    pr0ficha = pr0.Pr0ficha,
                    pr0unmedtr = pr0.Pr0unmedtr,
                    pr0fatortr = pr0.Pr0fatortr,
                    pr0precoco = pr0.Pr0precoco,
                    pr0altura = pr0.Pr0altura,
                    pr0largura = pr0.Pr0largura,
                    pr0comprim = pr0.Pr0comprim
                });

                return affectedRows;
            }
        }

        public async Task<PR0> FindByAlterCode(string pr0produto, string pr0refer)
        {
            string command = @"SELECT *
                                FROM PR0
                                WHERE PR0REFER = @pr0refer
                                AND PR0PRODUTO <> @pr0produto";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PR0 product = await connection.QueryFirstOrDefaultAsync<PR0>(command, new { pr0produto, pr0refer });

                return product;
            }
        }

        public async Task<PR0> FindManagmentProduct(string pr0produto)
        {
            string command = @"SELECT Pr0produto
                                    , Pr0desc
                                    , Pr0colecao
                                    , Pr0etiq
                                    , Pr0ficha
                                    , Pr0grupo
                                FROM PR0
                                INNER JOIN CO0 ON CO0COLECAO = PR0COLECAO AND CO0B2B = 1
                                INNER JOIN PRS ON PRSGRUPO = PR0GRUPO AND PRSB2B = 1
                                WHERE PR0B2B = 0
                                  AND PR0PRODUTO = @pr0produto";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PR0 product = await connection.QueryFirstOrDefaultAsync<PR0>(command, new { pr0produto });

                return product;
            }
        }

        public async Task<IList<PR0>> SearchManagmentProducts(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            string command = @"SELECT Pr0produto
                                    , Pr0desc
                                    , (SELECT B2ICAMINHO FROM B2I WHERE B2IPRODUTO = PR0PRODUTO AND B2IPRINCIP = 1 LIMIT 1) AS Pr0imagem
                                    , Co0nome
                               FROM PR0
                               INNER JOIN CO0 ON CO0COLECAO = PR0COLECAO AND CO0B2B = 1
                               INNER JOIN PRS ON PRSGRUPO = PR0GRUPO AND PRSB2B = 1
                               WHERE PR0B2B = 0
                                 AND (PR0PRODUTO LIKE @search
                                  OR PR0DESC LIKE @search)
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PR0> products = await connection.QueryAsync<PR0>(command, new { search, limit, offset });

                return products.ToList();
            }
        }

        public async Task<IList<HomeProductModel>> Search(string search, IList<string> groups, IList<string> collections, IList<string> colors, IList<string> sizes)
        {
            search = string.Format("%{0}%", search);
            string command = @"SELECT DISTINCT Pr0produto
                                    , Pr0desc
                                    , (SELECT B2ICAMINHO FROM B2I WHERE B2IPRODUTO = PR0PRODUTO AND B2IPRINCIP = 1 LIMIT 1) AS Pr0imagem
                                    , (CASE
                                            WHEN (SELECT CVGPRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR0PRODUTO LIMIT 1) IS NOT NULL
                                            THEN (SELECT CVGPRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR0PRODUTO LIMIT 1)
                                            ELSE (SELECT (PR3PRECO*(CV6PERCENT/100)) AS CVGPRECO FROM PR3 INNER JOIN B2B ON B2BPADRAO = 1 INNER JOIN CV6 ON B2BPRECOS=CV6LISTA WHERE PR3PRODUTO = PR0PRODUTO ORDER BY 1 LIMIT 1)
                                       END
                                      ) AS Cvgpreco
                               FROM PR0
                               INNER JOIN CO0 ON CO0COLECAO = PR0COLECAO AND CO0B2B = 1" + (collections.Any() ? " AND CO0COLECAO IN @collections" : string.Empty)
                               + " INNER JOIN PRS ON PRSGRUPO = PR0GRUPO AND PRSB2B = 1" + (groups.Any() ? " AND PRSGRUPO IN @groups" : string.Empty)
                               + (colors.Any() ? " INNER JOIN PR2 ON PR2PRODUTO = PR0PRODUTO AND PR2OPCAO IN @colors" : string.Empty)
                               + (sizes.Any() ? " INNER JOIN PR3 ON PR3PRODUTO = PR0PRODUTO AND PR3TAMANHO IN @sizes" : string.Empty)
                               + @" WHERE PR0B2B = 0
                                 AND (PR0PRODUTO LIKE @search
                                  OR PR0DESC LIKE @search)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<HomeProductModel> products = await connection.QueryAsync<HomeProductModel>(command, new { search, groups, collections, colors, sizes });

                return products.ToList();
            }
        }

        public async Task<IList<PR2>> FindPr2List(string pr0produto)
        {
            string command = @"SELECT Pr2opcao, Cr1nome, Pr2imagem, Cr1numero
                                FROM PR2
                                INNER JOIN CR1 ON CR1COR = PR2COR
                                WHERE PR2FORACAT = 0
                                  AND PR2PRODUTO = @pr0produto";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PR2> colors = await connection.QueryAsync<PR2>(command, new { pr0produto });

                return colors.ToList();
            }
        }

        public async Task<IList<PR3>> FindPr3List(string pr0produto)
        {
            string command = @"SELECT 
                                      Pr3tamanho
                                    , Gr1desc
                                    , (CASE
                                          WHEN (SELECT CVGPRECO AS PR3PRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR3PRODUTO AND CVGTAMANHO = PR3TAMANHO) IS NOT NULL
                                          THEN (SELECT CVGPRECO AS PR3PRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR3PRODUTO AND CVGTAMANHO = PR3TAMANHO)
                                          ELSE (SELECT (PR3PRECO*(CV6PERCENT/100)) AS PR3PRECO FROM CV6 INNER JOIN B2B ON B2BPADRAO=1 AND B2BPRECOS=CV6LISTA)
                                       END
                                      ) AS Pr3preco
                                    , Pr3pesoliq   
                                 FROM PR3
                                INNER JOIN GR1 ON GR1TAMANHO = PR3TAMANHO
                                WHERE PR3PRODUTO = @pr0produto";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PR3> sizes = await connection.QueryAsync<PR3>(command, new { pr0produto });

                return sizes.ToList();
            }
        }

        public async Task<IList<HomeProductModel>> FindRelatedProducts(string pr0produto, string pr0colecao, string pr0grupo)
        {
            string command = @"select Pr0produto
                                    , Pr0desc
                                    , (SELECT B2ICAMINHO FROM B2I WHERE B2IPRODUTO = PR0PRODUTO AND B2IPRINCIP = 1 LIMIT 1) AS Pr0imagem
                                    , (CASE
                                        WHEN (SELECT CVGPRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR0PRODUTO LIMIT 1) IS NOT NULL
                                        THEN (SELECT CVGPRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR0PRODUTO LIMIT 1)
                                        ELSE (SELECT (PR3PRECO*(CV6PERCENT/100)) AS CVGPRECO FROM PR3 INNER JOIN B2B ON B2BPADRAO = 1 INNER JOIN CV6 ON B2BPRECOS=CV6LISTA WHERE PR3PRODUTO = PR0PRODUTO ORDER BY 1 LIMIT 1)
                                        END
                                    ) AS Cvgpreco
                            FROM PR0
                            INNER JOIN CO0 ON CO0COLECAO = PR0COLECAO AND CO0B2B = 1
                            LEFT JOIN PRS ON PRSGRUPO = PR0GRUPO AND PRSB2B = 1
                            WHERE PR0COLECAO = @pr0colecao
                              AND PR0GRUPO = @pr0grupo
                              AND PR0PRODUTO <> @pr0produto
                              AND PR0B2B = 0
                            ORDER BY RAND()
                            LIMIT 30";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<HomeProductModel> relatedProducts = await connection.QueryAsync<HomeProductModel>(command, new { pr0produto, pr0colecao, pr0grupo });

                return relatedProducts.ToList();
            }
        }

        public async Task<IEnumerable<HomeProductModel>> FindProductsByGroup(string prsgrupo)
        {
            string command = @"select Pr0produto
                                    , Pr0desc
                                    , (SELECT B2ICAMINHO FROM B2I WHERE B2IPRODUTO = PR0PRODUTO AND B2IPRINCIP = 1 LIMIT 1) AS Pr0imagem
                                    , (CASE
                                          WHEN (SELECT CVGPRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR0PRODUTO LIMIT 1) IS NOT NULL
                                          THEN (SELECT CVGPRECO FROM CVG INNER JOIN CVF ON CVFPRODUTO = CVGPRODUTO AND CVFLISTA = CVGLISTA INNER JOIN B2B ON B2BPRECOS = CVGLISTA AND B2BPADRAO = 1 WHERE CVGPRODUTO = PR0PRODUTO LIMIT 1)
                                          ELSE (SELECT (PR3PRECO*(CV6PERCENT/100)) AS CVGPRECO FROM PR3 INNER JOIN B2B ON B2BPADRAO = 1 INNER JOIN CV6 ON B2BPRECOS=CV6LISTA WHERE PR3PRODUTO = PR0PRODUTO ORDER BY 1 LIMIT 1)
                                       END
                                       ) AS Cvgpreco
                                FROM PR0
                                INNER JOIN CO0 ON CO0COLECAO = PR0COLECAO AND CO0B2B = 1
                                LEFT JOIN PRS ON PRSGRUPO = PR0GRUPO AND PRSB2B = 1 AND PRSGRUPO = @prsgrupo
                                WHERE PR0B2B = 0
                                ORDER BY RAND()
                                LIMIT 30";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<HomeProductModel> products = await connection.QueryAsync<HomeProductModel>(command, new { prsgrupo });

                return products;
            }
        }

        public async Task<IList<PR3>> FindSizes(IList<string> products)
        {
            string command = @"select distinct Pr3tamanho
                                    , Gr1desc
                                FROM PR3
                               INNER JOIN GR1 ON GR1TAMANHO = PR3TAMANHO
                               INNER JOIN PR0 ON PR0PRODUTO = PR3PRODUTO AND PR0PRODUTO IN @products and PR0B2B = 0";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PR3> sizes = await connection.QueryAsync<PR3>(command, new { products });

                return sizes.ToList();
            }
        }

        public async Task<IList<PR2>> FindColors(IList<string> products)
        {
            string command = @"select distinct Pr2opcao
                                    , Cr1nome
                                FROM PR2
                               INNER JOIN CR1 ON CR1COR = PR2COR
                               INNER JOIN PR0 ON PR0PRODUTO = PR2PRODUTO AND PR0PRODUTO IN @products and PR0B2B = 0";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PR2> colors = await connection.QueryAsync<PR2>(command, new { products });

                return colors.ToList();
            }
        }

        public async Task<IList<PRS>> FindGroups(IList<string> products)
        {
            string command = @"select distinct Prsgrupo
                                    , Prsnome
                                FROM PRS
                                INNER JOIN PR0 ON PRSGRUPO = PR0GRUPO AND PR0PRODUTO IN @products and PR0B2B = 0
                                WHERE PRSB2B = 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PRS> groups = await connection.QueryAsync<PRS>(command, new { products });

                return groups.ToList();
            }
        }

        public async Task<IList<CO0>> FindCollections(IList<string> products)
        {
            string command = @"select distinct Co0colecao
                                    , Co0nome
                                FROM CO0
                                INNER JOIN PR0 ON CO0COLECAO = PR0COLECAO AND PR0PRODUTO IN @products and PR0B2B = 0
                                WHERE CO0B2B = 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CO0> collections = await connection.QueryAsync<CO0>(command, new { products });

                return collections.ToList();
            }
        }

        public async Task<T> FindByEanTable<T>(string ean)
        {
            string command = @"SELECT   PR0.PR0PRODUTO,
	                                	PR0.PR0DESC,
                                        PR0.PR0IMAGEM,
                                        PR0.PR0PESOBRU,
                                        PR0.PR0PESOLIQ,
                                        EAN.EANCODIGO,
                                        PR2.PR2OPCAO, 
                                        CR1.CR1NOME,
                                        EAN.EANPRODUTO,
                                        GR1.GR1Desc,
                                        EANTAMANHO AS Pr3tamanho
                                    FROM PR0
                                      INNER JOIN EAN ON EAN.EANPRODUTO = PR0.PR0PRODUTO
                                      INNER JOIN PR2 ON PR2.PR2OPCAO = EAN.EANOPCAO AND PR2.PR2PRODUTO = PR0.PR0PRODUTO
                                      INNER JOIN CR1 ON CR1.CR1COR = PR2.PR2OPCAO
                                      INNER JOIN GR1 ON EAN.EANTAMANHO = GR1.GR1TAMANHO
                                    WHERE EAN.EANCODIGO = @eancodigo OR EAN.EANCODEXT = @eancodigo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                T product = await connection.QueryFirstOrDefaultAsync<T>(command, new { eancodigo = ean });
                return product;
            }
        }

        public async Task<T> FindByEan<T>(string ean)
        {
            string command = @"SELECT PR0PRODUTO,
                                      PR0DESC,
                                      PR0IMAGEM,
                                      PR0EAN as Eancodigo,
                                      PR0.PR0PESOBRU,
                                      PR0.PR0PESOLIQ
                                 FROM PR0
                                WHERE PR0EAN = @eancodigo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                T product = await connection.QueryFirstOrDefaultAsync<T>(command, new { eancodigo = ean });
                return product;
            }
        }

        public async Task<T> FindUtilizatedEan<T>(string pr0produto, string pr0ean)
        {
            string command = @"SELECT PR0PRODUTO,
                                FROM PR0
                                WHERE PR0EAN = @pr0ean
                                AND PR0PRODUTO <> @pr0produto";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                T product = await connection.QueryFirstOrDefaultAsync<T>(command, new { pr0produto, pr0ean });
                return product;
            }
        }

        public async Task<T> FindByOF3Table<T>(string of3Peca)
        {
            string command = @"SELECT PR0PRODUTO,
		                              PR0DESC,
		                              PR0IMAGEM,
                                      PR0.PR0PESOBRU,
                                      PR0.PR0PESOLIQ,
		                              PR2OPCAO, 
		                              CR1NOME,
		                              OF3TAMANHO as Pr3tamanho,
		                              GR1DESC,
		                              OF3PECA,
                                      OF3RFID
                                 FROM PR0
                                INNER JOIN OF3 ON OF3.OF3PRODUTO = PR0.PR0PRODUTO
	                            INNER JOIN PR2 ON PR2.PR2OPCAO = OF3.OF3OPCAO AND PR2.PR2PRODUTO = PR0.PR0PRODUTO
	                            INNER JOIN CR1 ON CR1.CR1COR = PR2.PR2OPCAO
	                            INNER JOIN GR1 ON OF3TAMANHO = GR1.GR1TAMANHO
                                WHERE OF3PECA =  @of3Peca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                T product = await connection.QueryFirstOrDefaultAsync<T>(command, new { of3Peca = of3Peca });
                return product;
            }
        }


        public async Task<IList<T>> FindForPriceQuery<T>(string text, string orderBy)
        {
            string command = @"SELECT PR0PRODUTO,
		                              PR0DESC,
		                              PR0IMAGEM,
                                      PR0PESOBRU
                                 FROM PR0
                                WHERE UPPER(PR0PRODUTO) LIKE UPPER(CONCAT('%',@text)) OR UPPER(PR0DESC) LIKE UPPER(CONCAT('%',@text,'%'))";
            command += " ORDER BY " + orderBy + ";";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<T> products = await connection.QueryAsync<T>(command, new { text });
                return products.ToList();
            }
        }


        public async Task<IList<T>> FindVariants<T>(string pr0produto)
        {
            string command = @"SELECT PR0PRODUTO,
		                              PR0DESC,
		                              PR2OPCAO, 
		                              CR1NOME,
		                              PR3TAMANHO,
		                              GR1DESC
                                 FROM PR0
                                INNER JOIN PR3 ON PR3.PR3PRODUTO = PR0.PR0PRODUTO
	                            INNER JOIN PR2 ON PR2.PR2PRODUTO = PR0.PR0PRODUTO
	                            INNER JOIN CR1 ON CR1.CR1COR = PR2.PR2OPCAO
	                            INNER JOIN GR1 ON PR3TAMANHO = GR1.GR1TAMANHO
                                WHERE PR0PRODUTO = @pr0produto
                                ORDER BY PR0PRODUTO,PR2OPCAO,GR1POSICAO";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<T> products = await connection.QueryAsync<T>(command, new { pr0produto });
                return products.ToList();
            }
        }



        #region price
        public async Task<decimal> FindSpecialPrice(string pr0Produto, string lista)
        {
            string command = @"SELECT cvgpreco
                                     FROM cvg
                                   WHERE cvglista =  @cvglista
                                    AND cvgproduto = @cvgproduto
                                order by cvgpreco";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                decimal cvgpreco = await connection.QueryFirstOrDefaultAsync<decimal>(command, new { cvglista = lista, cvgproduto = pr0Produto });
                return cvgpreco;
            }
        }
        public async Task<decimal> FindSpecialPrice(string pr0Produto, string lista, string tam)
        {
            string command = @"SELECT cvgpreco
                                     FROM cvg
                                   WHERE cvglista =  @cvglista
                                    AND cvgproduto = @cvgproduto
                                    AND cvgtamanho = @tamanho
                                order by cvgpreco";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                decimal cvgpreco = await connection.QueryFirstOrDefaultAsync<decimal>(command, new { cvglista = lista, cvgproduto = pr0Produto, tamanho = tam });
                return cvgpreco;
            }
        }

        public async Task<decimal> FindSpecialPrice(string pr0Produto, string lista, string tam, string cor)
        {
            string command = @"SELECT cvwpreco
                                     FROM cvw
                                   WHERE cvwlista =  @cvglista
                                    AND cvwproduto = @cvgproduto
                                    AND cvwtamanho = @tamanho
                                    AND cvwopcao = @cor
                                order by cvwpreco";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                decimal cvgpreco = await connection.QueryFirstOrDefaultAsync<decimal>(command, new
                {
                    cvglista = lista,
                    cvgproduto = pr0Produto,
                    tamanho = tam,
                    cor = cor
                });
                return cvgpreco;
            }
        }

        public async Task<decimal> FindPrice(string pr0Produto)
        {
            string command = @"SELECT pr3preco
                                     FROM pr3
                                   WHERE pr3produto =  @pr0Produto
                                order by pr3preco";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                decimal cvgpreco = await connection.QueryFirstOrDefaultAsync<decimal>(command, new { pr0Produto = pr0Produto });
                return cvgpreco;
            }
        }

        public async Task<decimal> FindPrice(string pr0Produto, string tam)
        {
            string command = @"SELECT pr3preco
                                     FROM pr3
                                   WHERE pr3produto =  @pr0Produto
                                    AND pr3tamanho = @tamanho
                                order by pr3preco";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                decimal cvgpreco = await connection.QueryFirstOrDefaultAsync<decimal>(command, new { pr0Produto = pr0Produto, tamanho = tam });
                return cvgpreco;
            }
        }
        #endregion

        #region taxes

        public async Task<OriginModel> FindSpecificTaxes(string pr0produto)
        {
            string command = @"SELECT PR0ORIGEM AS ORIGIN, PR0TOTTRIB AS TOTTRIB FROM PR0 WHERE PR0PRODUTO = @pr0produto";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                OriginModel pr0 = await connection.QueryFirstOrDefaultAsync<OriginModel>(command, new { pr0produto });
                return pr0;
            }
        }
        #endregion

        #region RFID
        public async Task<string> FindPr0ByRfid(string rfidCode)
        {
            string command = @"SELECT PR0PRODUTO
                                FROM PR0
                                WHERE PR0RfidCode = @rfidCode";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PR0 product = await connection.QueryFirstOrDefaultAsync<PR0>(command, new { rfidCode });
                if (product != null && !string.IsNullOrEmpty(product.Pr0produto))
                {
                    return product.Pr0produto;
                }
                return null;
            }
        }
        #endregion RFID


    }
}
