using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class OrderRepository : Repository
    {
        public OrderRepository(string path) : base(path)
        {
        }

        #region B2B

        public async Task<IList<VD1>> FindOrders(string cf1cliente)
        {
            string command = @"SELECT Vd1pedido, Vd1abert, vd1status, B2enome AS Vd1statdes, SUM(VD3QTDE * VD5PRECO) AS Vd1total, Vd1entrega, sum(vd3qtde) AS QtdeItems
                                 FROM VD1
                                INNER JOIN VD2 ON VD2PEDIDO = VD1PEDIDO
                                INNER JOIN VD3 ON VD3PEDIDO = VD1PEDIDO AND VD3PRODUTO = VD2PRODUTO
                                INNER JOIN VD5 ON VD5PEDIDO = VD1PEDIDO AND VD5PRODUTO = VD3PRODUTO AND VD5TAMANHO = VD3TAMANHO
                                INNER JOIN B2E ON B2ESTATUS = VD1STATUS
                                WHERE VD1CLIENTE = @cf1cliente
                                GROUP BY VD1PEDIDO
                                ORDER BY VD1ABERT DESC";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD1> lstVd1 = await connection.QueryAsync<VD1>(command, new { cf1cliente });

                return lstVd1.ToList();
            }
        }

        public async Task<VD1> FindB2b(string vd1pedido)
        {
            string command = @"SELECT Vd1pedido, Vd1abert, vd1status, B2enome AS Vd1statdes, SUM(VD3QTDE * VD5PRECO) AS Vd1total, Vd1entrega, sum(vd3qtde) AS QtdeItems
                                 FROM VD1
                                INNER JOIN VD2 ON VD2PEDIDO = VD1PEDIDO
                                INNER JOIN VD3 ON VD3PEDIDO = VD1PEDIDO AND VD3PRODUTO = VD2PRODUTO
                                INNER JOIN VD5 ON VD5PEDIDO = VD1PEDIDO AND VD5PRODUTO = VD3PRODUTO AND VD5TAMANHO = VD3TAMANHO
                                INNER JOIN B2E ON B2ESTATUS = VD1STATUS
                                WHERE VD1PEDIDO = @vd1pedido
                                GROUP BY VD1PEDIDO
                                ORDER BY VD1ABERT DESC";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD1 vd1 = await connection.QueryFirstOrDefaultAsync<VD1>(command, new { vd1pedido });

                return vd1;
            }
        }

        public async Task<IList<OrderItem>> FindItems(string vd1pedido)
        {
            string command = @"SELECT 
                                      Vd2produto
                                    , Vd3opcao
                                    , Vd3tamanho
                                    , Vd3qtde
                                    , Pr0desc
                                    , (SELECT B2ICAMINHO FROM B2I WHERE B2IPRODUTO = PR0PRODUTO AND B2IPRINCIP = 1 LIMIT 1) AS Pr0imagem
                                    , Pr0colecao
                                    , Pr0grupo
                                    , Pr0etiq
                                    , Cr1nome
                                    , Gr1desc
                                    , Vd5preco
                                FROM VD2
                               INNER JOIN VD3 ON VD3PEDIDO = VD2PEDIDO AND VD3PRODUTO = VD2PRODUTO
                               INNER JOIN VD5 ON VD5PEDIDO = VD2PEDIDO AND VD5PRODUTO = VD3PRODUTO AND VD5TAMANHO = VD3TAMANHO
                               INNER JOIN PR0 ON PR0PRODUTO = VD2PRODUTO
                               INNER JOIN PR2 ON PR2PRODUTO = PR0PRODUTO AND PR2OPCAO = VD3OPCAO
                               INNER JOIN CR1 ON CR1COR = PR2COR
                               INNER JOIN GR1 ON GR1TAMANHO = VD3TAMANHO
                              WHERE VD2PEDIDO = @vd1pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<OrderItem> lstItems = await connection.QueryAsync<OrderItem>(command, new { vd1pedido });

                return lstItems.ToList();
            }
        }

        public async Task<int> SaveOrderB2b(VD1 order)
        {
            string command = @"INSERT INTO 
                               VD1 (Vd1cliente, Vd1abert, Vd1entrada, Vd1pedido, Vd1lista, Vd1usuario, Vd1status, Vd1emissor, Vd1observa)
                               VALUES (@Vd1cliente, @Vd1abert, @Vd1entrada, @Vd1pedido, @Vd1lista, @Vd1usuario, @Vd1status, @Vd1emissor, @Vd1observa)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        order.Vd1cliente,
                        order.Vd1abert,
                        order.Vd1entrada,
                        order.Vd1pedido,
                        order.Vd1lista,
                        order.Vd1usuario,
                        order.Vd1status,
                        order.Vd1emissor,
                        order.Vd1observa,
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region global methods

        public async Task<int> DeleteCompleteOrder(string vd1pedido)
        {
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = 0;
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        IEnumerable<string> volumes = await connection.QueryAsync<string>("SELECT VD7VOLUME FROM VD7 WHERE VD7PEDIDO = @vd1pedido", new { vd1pedido }, transaction: transaction);
                        if (volumes.Any())
                        {
                            affectedRows = await connection.ExecuteAsync("DELETE FROM VD8 WHERE VD8VOLUME IN @volumes", new { volumes }, transaction: transaction);
                            affectedRows = await connection.ExecuteAsync("DELETE FROM VDV WHERE VDVVOLUME IN @volumes", new { volumes }, transaction: transaction);
                        }
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD7 WHERE VD7PEDIDO = @vd1pedido", new { vd1pedido }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD6 WHERE VD6PEDIDO = @vd1pedido", new { vd1pedido }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD5 WHERE VD5PEDIDO = @vd1pedido", new { vd1pedido }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD3 WHERE VD3PEDIDO = @vd1pedido", new { vd1pedido }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD2 WHERE VD2PEDIDO = @vd1pedido", new { vd1pedido }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD1 WHERE VD1PEDIDO = @vd1pedido", new { vd1pedido }, transaction: transaction);

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

        public async Task<int> DeleteCompleteOrderConsigned(string vd1pedido, string shipSequence, IList<ShipItem> lstShipItems)
        {
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = 0;
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        IEnumerable<string> volumes = await connection.QueryAsync<string>("SELECT VD7VOLUME FROM VD7 WHERE VD7PEDIDO = @vd1pedido AND VD7EMBARQ = @shipSequence", new { vd1pedido, shipSequence }, transaction: transaction);
                        if (volumes.Any())
                        {
                            affectedRows = await connection.ExecuteAsync("DELETE FROM VD8 WHERE VD8VOLUME = @volumes", new { volumes }, transaction: transaction);
                            affectedRows = await connection.ExecuteAsync("DELETE FROM VDV WHERE VDVVOLUME = @volumes", new { volumes }, transaction: transaction);
                        }
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD7 WHERE VD7PEDIDO = @vd1pedido AND VD7EMBARQ = @shipSequence", new { vd1pedido, shipSequence }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD6 WHERE VD6PEDIDO = @vd1pedido AND VD6EMBARQ = @shipSequence", new { vd1pedido, shipSequence }, transaction: transaction);
                        foreach (ShipItem shipItem in lstShipItems)
                        {
                            await RemoveVD3(
                                  shipItem.Pedido
                                , shipItem.Produto
                                , shipItem.Opcao
                                , shipItem.Tamanho
                                , (int)shipItem.Qtde);
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

        public async Task<int> ClearZeroLines(string vd1pedido)
        {
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = 0;
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VDV WHERE VDVQTDE = 0 AND VDVVOLUME IN (SELECT VD7VOLUME FROM VD7 WHERE VD7PEDIDO = @vd1pedido)", new { vd1pedido }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD2 WHERE (VD2PEDIDO, VD2PRODUTO) NOT IN (SELECT VD3PEDIDO, VD3PRODUTO FROM VD3 WHERE VD3QTDE > 0 AND VD3PEDIDO = @vd1pedido) AND VD2PEDIDO = @vd1pedido", new { vd1pedido }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD5 WHERE (VD5PEDIDO, VD5PRODUTO, VD5OPCAO, VD5TAMANHO) IN (SELECT VD3PEDIDO, VD3PRODUTO, VD3OPCAO, VD3TAMANHO FROM VD3 WHERE VD3QTDE = 0 AND VD3PEDIDO = @vd1pedido)", new { vd1pedido }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VD3 WHERE VD3QTDE = 0 AND VD3PEDIDO = @vd1pedido", new { vd1pedido }, transaction: transaction);
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

        #region VD1
        public async Task<int> Insert(VD1 vd1, IDbTransaction transaction = null)
        {
            string command = @"INSERT INTO VD1 (VD1PEDIDO,VD1CLIENTE,VD1REPRES,VD1TRANSP,VD1PEDREP,VD1ENTRADA,VD1ENTREGA,VD1CONPGTO,VD1LISTA,VD1OPERCOM,VD1COMIS,VD1DESCON,VD1DESPON,VD1DESDIAS,VD1FRETE,VD1PRONTA,VD1CONSIG,VD1REDESP,VD1USUARIO,VD1DOCIMP,VD1ABERT,VD1OBSERVA,VD1ENCERRA,VD1EXCLUIR,VD1EMISSOR,VD1VLFRETE,VD1LOCAL,VD1DIASEMB,VD1MATCLI,VD1ECOMPED,VD1STATUS,VD1VEND,VD1COMVEN)
                               VALUES (@Vd1pedido,@Vd1cliente,@Vd1repres,@Vd1transp,@Vd1pedrep,@Vd1entrada,@Vd1entrega,@Vd1conpgto,@Vd1lista,@Vd1opercom,@Vd1comis,@Vd1descon,@Vd1despon,@Vd1desdias,@Vd1frete,@Vd1pronta,@Vd1consig,@Vd1redesp,@Vd1usuario,@Vd1docimp,@Vd1abert,@Vd1observa,@Vd1encerra,@Vd1excluir,@Vd1emissor,@Vd1vlfrete,@Vd1local,@Vd1diasemb,@Vd1matcli,@Vd1ecomped,@Vd1status,@vd1vend,@vd1comven)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    vd1.Vd1pedido,
                    vd1.Vd1cliente,
                    vd1.Vd1repres,
                    vd1.Vd1transp,
                    vd1.Vd1pedrep,
                    vd1.Vd1entrada,
                    vd1.Vd1entrega,
                    vd1.Vd1conpgto,
                    vd1.Vd1lista,
                    vd1.Vd1opercom,
                    vd1.Vd1comis,
                    vd1.Vd1descon,
                    vd1.Vd1despon,
                    vd1.Vd1desdias,
                    vd1.Vd1frete,
                    vd1.Vd1pronta,
                    vd1.Vd1consig,
                    vd1.Vd1redesp,
                    vd1.Vd1usuario,
                    vd1.Vd1docimp,
                    vd1.Vd1abert,
                    vd1.Vd1observa,
                    vd1.Vd1encerra,
                    vd1.Vd1excluir,
                    vd1.Vd1emissor,
                    vd1.Vd1vlfrete,
                    vd1.Vd1local,
                    vd1.Vd1diasemb,
                    vd1.Vd1matcli,
                    vd1.Vd1ecomped,
                    vd1.Vd1status,
                    vd1.Vd1vend,
                    vd1.Vd1comven
                },
                transaction);

                return affectedRows;
            }
        }

        public async Task<VD1> Find(string vd1pedido)
        {
            string command = @"SELECT VD1.*
                                    , CF1.CF1NOME
                                    , CF1.CF1FONE
                                    , CF1.CF1EMAIL
                                    , CF3.CF3NOME
                                    , CF3.CF3ESTADO
                                    , CF6.CF6NOME
                                    , CV6.CV6NOME
                                    , CV6.CV6LIMPRAZ
                                    , CV3.CV3NOME
                                    , VD4.VD4NOME
                                    , CF7.CF7NOME
                                    , VE0.VE0VEND
                                    , CF1VEND.CF1NOME AS Ve0nome
                                 FROM VD1
                                 LEFT JOIN CF1 ON CF1.CF1CLIENTE = VD1CLIENTE
                                 LEFT JOIN CF2 ON CF2CEP = CF1CEP
                                 LEFT JOIN CF3 ON CF3LOCAL = CF2LOCAL
                                 LEFT JOIN CF6 ON CF6REPRES = VD1REPRES
                                 LEFT JOIN CV6 ON CV6LISTA = VD1LISTA
                                 LEFT JOIN CV3 ON CV3OPERCOM = VD1OPERCOM
                                 LEFT JOIN VD4 ON VD4CONPGTO = VD1CONPGTO
                                 LEFT JOIN CF7 ON CF7TRANSP = VD1TRANSP
                                 LEFT JOIN VE0 ON VE0VEND = VD1VEND
                                 LEFT JOIN CF1 AS CF1VEND ON CF1VEND.CF1CLIENTE = VE0CLIENTE
                                WHERE VD1PEDIDO = @vd1pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD1 vd1 = await connection.QueryFirstOrDefaultAsync<VD1>(command, new { vd1pedido });

                return vd1;
            }
        }

        public async Task<int> Update(VD1 vd1)
        {
            string command = @"UPDATE VD1 
                                  SET VD1CLIENTE = @Vd1cliente,VD1REPRES = @Vd1repres,VD1TRANSP = @Vd1transp,VD1PEDREP = @Vd1pedrep,VD1ENTRADA = @Vd1entrada,VD1ENTREGA = @Vd1entrega,VD1CONPGTO = @Vd1conpgto,VD1LISTA = @Vd1lista,VD1OPERCOM = @Vd1opercom,VD1COMIS = @Vd1comis,VD1DESCON = @Vd1descon,VD1DESPON = @Vd1despon,VD1DESDIAS = @Vd1desdias,VD1FRETE = @Vd1frete,VD1PRONTA = @Vd1pronta,VD1CONSIG = @Vd1consig,VD1REDESP = @Vd1redesp,VD1USUARIO = @Vd1usuario,VD1DOCIMP = @Vd1docimp,VD1ABERT = @Vd1abert,VD1OBSERVA = @Vd1observa,VD1ENCERRA = @Vd1encerra,VD1EXCLUIR = @Vd1excluir,VD1EMISSOR = @Vd1emissor,VD1VLFRETE = @Vd1vlfrete,VD1LOCAL = @Vd1local,VD1DIASEMB = @Vd1diasemb,VD1MATCLI = @Vd1matcli,VD1ECOMPED = @Vd1ecomped,VD1STATUS = @Vd1status, VD1VEND = @vd1vend, VD1COMVEN = @vd1comven
                                WHERE VD1PEDIDO = @Vd1pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    vd1.Vd1pedido,
                    vd1.Vd1cliente,
                    vd1.Vd1repres,
                    vd1.Vd1transp,
                    vd1.Vd1pedrep,
                    vd1.Vd1entrada,
                    vd1.Vd1entrega,
                    vd1.Vd1conpgto,
                    vd1.Vd1lista,
                    vd1.Vd1opercom,
                    vd1.Vd1comis,
                    vd1.Vd1descon,
                    vd1.Vd1despon,
                    vd1.Vd1desdias,
                    vd1.Vd1frete,
                    vd1.Vd1pronta,
                    vd1.Vd1consig,
                    vd1.Vd1redesp,
                    vd1.Vd1usuario,
                    vd1.Vd1docimp,
                    vd1.Vd1abert,
                    vd1.Vd1observa,
                    vd1.Vd1encerra,
                    vd1.Vd1excluir,
                    vd1.Vd1emissor,
                    vd1.Vd1vlfrete,
                    vd1.Vd1local,
                    vd1.Vd1diasemb,
                    vd1.Vd1matcli,
                    vd1.Vd1ecomped,
                    vd1.Vd1status,
                    vd1.Vd1vend,
                    vd1.Vd1comven
                });

                return affectedRows;
            }
        }

        public async Task<VD1> FindOpenTransferOrder(string lx2cliest)
        {
            string command = @"SELECT * FROM VD1 WHERE VD1CLIENTE = @lx2cliest AND VD1PRONTA = 1 AND VD1CONSIG = 1 ORDER BY VD1PEDIDO";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD1 vd1 = await connection.QueryFirstOrDefaultAsync<VD1>(command, new { lx2cliest });

                return vd1;
            }
        }
        #endregion
        #region VD2
        public async Task<int> SaveVd2(VD2 vd2)
        {
            string command = @"INSERT INTO 
                               VD2 (VD2PEDIDO, VD2PRODUTO, VD2ETIQ, VD2ENTREGA)
                               VALUES (@Vd2pedido, @Vd2produto, @Vd2etiq, @Vd2entrega)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        vd2.Vd2pedido,
                        vd2.Vd2produto,
                        vd2.Vd2etiq,
                        vd2.Vd2entrega
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IList<VD2>> FindVd2(string vd2pedido)
        {
            string command = @"SELECT *
                                 FROM VD2
                                WHERE VD2PEDIDO = @vd2pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD2> lstVd2 = await connection.QueryAsync<VD2>(command, new { vd2pedido });

                return lstVd2.ToList();
            }
        }
        public async Task<int> DeleteVd2(string vd1pedido)
        {
            string statement = "DELETE FROM VD2 WHERE VD2PEDIDO = @vd1pedido";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    var affectedRows = await connection.ExecuteAsync(statement, new { vd1pedido });
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region VD3
        public async Task<int> SaveVd3(VD3 vd3)
        {
            string command = @"INSERT INTO 
                               VD3 (VD3PEDIDO, VD3PRODUTO, VD3OPCAO, VD3TAMANHO, VD3QTDE)
                               VALUES (@Vd3pedido, @Vd3produto, @Vd3opcao, @Vd3tamanho, @Vd3qtde)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        vd3.Vd3pedido,
                        vd3.Vd3produto,
                        vd3.Vd3opcao,
                        vd3.Vd3tamanho,
                        vd3.Vd3qtde,
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<VD3>> FindVd3(string vd3pedido)
        {
            string command = @"SELECT *
                                 FROM VD3
                                WHERE VD3PEDIDO = @vd3pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD3> lstVd3 = await connection.QueryAsync<VD3>(command, new { vd3pedido });

                return lstVd3.ToList();
            }
        }
        public async Task<int> DeleteVd3(string vd1pedido)
        {
            string statement = "DELETE FROM VD3 WHERE VD3PEDIDO = @vd1pedido";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    var affectedRows = await connection.ExecuteAsync(statement, new { vd1pedido });
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<VD3> FindVd3(string vd3pedido, string vd3produto, string vd3opcao, string vd3tamanho)
        {
            string command = @"SELECT *
                                 FROM VD3
                                WHERE VD3PEDIDO = @vd3pedido
                                  AND VD3PRODUTO = @vd3produto
                                  AND VD3OPCAO = @vd3opcao
                                  AND VD3TAMANHO = @vd3tamanho";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD3 vd3 = await connection.QueryFirstOrDefaultAsync<VD3>(command, new { vd3pedido, vd3produto, vd3opcao, vd3tamanho });

                return vd3;
            }
        }

        public async Task<int> RemoveVD3(string vd3pedido, string vd3produto, string vd3opcao, string vd3tamanho, decimal vd3qtde, IDbTransaction transaction = null)
        {
            string command = @"UPDATE VD3 
                                  SET VD3QTDE = VD3QTDE - @vd3qtde
                                WHERE VD3PEDIDO = @vd3pedido 
                                  AND VD3PRODUTO = @vd3produto
                                  AND VD3OPCAO = @vd3opcao
                                  AND VD3TAMANHO = @vd3tamanho
                                  AND VD3QTDE > 0";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, new { vd3pedido, vd3produto, vd3opcao, vd3tamanho, vd3qtde }, transaction);

                return affectedRows;
            }
        }
        #endregion
        #region VD5
        public async Task<int> SaveVd5(VD5 vd5)
        {
            string command = @"INSERT INTO 
                               VD5 (VD5PEDIDO, VD5PRODUTO, VD5TAMANHO, VD5PRECO, VD5OPCAO)
                               VALUES (@Vd5pedido, @Vd5produto, @Vd5tamanho, @Vd5preco, @Vd5opcao)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        vd5.Vd5pedido,
                        vd5.Vd5produto,
                        vd5.Vd5tamanho,
                        vd5.Vd5preco,
                        vd5.Vd5opcao
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IList<VD5>> FindVd5(string vd5pedido)
        {
            string command = @"SELECT *
                                 FROM VD5
                                WHERE VD5PEDIDO = @vd5pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD5> lstVd5 = await connection.QueryAsync<VD5>(command, new { vd5pedido });

                return lstVd5.ToList();
            }
        }
        public async Task<IList<VD5>> FindVd5(string vd6pedido, string vd6embarq)
        {
            string command = @"SELECT *
                                 FROM VD5, 
                                WHERE VD5PEDIDO = @vd6pedido AND VD5EMBARQ = @vd6embarq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD5> lstVd5 = await connection.QueryAsync<VD5>(command, new { vd6pedido, vd6embarq });

                return lstVd5.ToList();
            }
        }
        public async Task<int> DeleteVd5(string vd1pedido)
        {
            string statement = "DELETE FROM VD5 WHERE VD5PEDIDO = @vd1pedido";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    var affectedRows = await connection.ExecuteAsync(statement, new { vd1pedido });
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<VD5> FindVd5(string vd5pedido, string vd5produto, string vd5opcao, string vd5tamanho)
        {
            string command = @"SELECT *
                                 FROM VD5
                                WHERE VD5PEDIDO = @vd5pedido
                                  AND VD5PRODUTO = @vd5produto
                                  AND VD5OPCAO = @vd5opcao
                                  AND VD5TAMANHO = @vd5tamanho";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD5 vd5 = await connection.QueryFirstOrDefaultAsync<VD5>(command, new { vd5pedido, vd5produto, vd5opcao, vd5tamanho });

                return vd5;
            }
        }
        #endregion
        #region VD6
        public async Task<int> SaveVd6(VD6 vd6, IDbTransaction transaction = null)
        {
            string command = @"INSERT INTO VD6 (VD6PEDIDO, VD6EMBARQ, VD6USUARIO, VD6ABERT, VD6FECHA, VD6DOCCONF)
                               VALUES (@Vd6pedido, @Vd6embarq, @Vd6usuario, @Vd6abert, @Vd6fecha, @Vd6docconf)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    vd6.Vd6pedido,
                    vd6.Vd6embarq,
                    vd6.Vd6usuario,
                    vd6.Vd6abert,
                    vd6.Vd6fecha,
                    vd6.Vd6docconf
                },
                transaction);

                return affectedRows;
            }
        }
        public async Task<IList<VD6>> FindVd6(string vd6pedido)
        {
            string command = @"SELECT *
                                 FROM VD6
                                WHERE VD6PEDIDO = @vd6pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD6> lstVd6 = await connection.QueryAsync<VD6>(command, new { vd6pedido });

                return lstVd6.ToList();
            }
        }

        public async Task<VD6> FindVd6(string vd6pedido, string vd6embarq)
        {
            string command = @"SELECT *
                                 FROM VD6
                                WHERE VD6PEDIDO = @vd6pedido
                                  AND VD6EMBARQ = @vd6embarq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD6 vd6 = await connection.QueryFirstOrDefaultAsync<VD6>(command, new { vd6pedido, vd6embarq });

                return vd6;
            }
        }

        public async Task<int> CloseShipment(string vd6pedido, string vd6embarq, DateTime vd6fecha)
        {
            string command = @"UPDATE VD6 SET VD6FECHA = @vd6fecha WHERE VD6PEDIDO = @vd6pedido AND VD6EMBARQ = @vd6embarq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, new { vd6pedido, vd6embarq, vd6fecha });

                return affectedRows;
            }
        }

        public async Task<IList<ShipSaleConfirmationModel>> FindShipForSaleConfirmation(string vd1pedido)
        {
            string command = @"SELECT VD6EMBARQ
                                    , VD6ABERT
                                 FROM VD6
                                WHERE VD6PEDIDO = @vd1pedido
                                  AND VD6DOCCONF = ''";

            string command1 = @"SELECT COALESCE(SUM(VD8PRECO),0)
                                  FROM VD8
                                 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME AND VD7PEDIDO = @vd1pedido AND VD7EMBARQ = @vd6embarq";
            string command2 = @"SELECT COALESCE(SUM(VDVQTDE * VDVPRECO),0)
                                  FROM VDV
                                 INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME AND VD7PEDIDO = @vd1pedido AND VD7EMBARQ = @vd6embarq";
            string command3 = @"SELECT COALESCE(COUNT(VD8PECA),0)
                                  FROM VD8
                                 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME AND VD7PEDIDO = @vd1pedido AND VD7EMBARQ = @vd6embarq";
            string command4 = @"SELECT COALESCE(SUM(VDVQTDE),0)
                                  FROM VDV
                                 INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME AND VD7PEDIDO = @vd1pedido AND VD7EMBARQ = @vd6embarq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IList<ShipSaleConfirmationModel> lstVd6;
                using (var transaction = connection.BeginTransaction())
                {
                    IEnumerable<ShipSaleConfirmationModel>  tempLstVd6 = await connection.QueryAsync<ShipSaleConfirmationModel>(command, new { vd1pedido }, transaction);
                    lstVd6 = tempLstVd6.ToList();
                    foreach (ShipSaleConfirmationModel vd6 in lstVd6)
                    {
                        vd6.Amount = await connection.QueryFirstOrDefaultAsync<decimal>(command1, new { vd1pedido, vd6.Vd6embarq }, transaction);
                        vd6.Amount += await connection.QueryFirstOrDefaultAsync<decimal>(command2, new { vd1pedido, vd6.Vd6embarq }, transaction);
                        vd6.QtyItems = await connection.QueryFirstOrDefaultAsync<int>(command3, new { vd1pedido, vd6.Vd6embarq }, transaction);
                        vd6.QtyItems += await connection.QueryFirstOrDefaultAsync<int>(command4, new { vd1pedido, vd6.Vd6embarq }, transaction);
                    }
                }

                return lstVd6;
            }
        }

        public async Task<int> Update(VD6 vd6, IDbTransaction transaction = null)
        {
            string command = @"UPDATE VD6
                                  SET VD6USUARIO = @Vd6usuario,VD6ABERT = @Vd6abert,VD6FECHA = @Vd6fecha,VD6DOCCONF = @Vd6docconf
                                WHERE VD6PEDIDO = @Vd6pedido AND VD6EMBARQ = @Vd6embarq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    vd6.Vd6pedido,
                    vd6.Vd6embarq,
                    vd6.Vd6usuario,
                    vd6.Vd6abert,
                    vd6.Vd6fecha,
                    vd6.Vd6docconf
                }, transaction);

                return affectedRows;
            }
        }
        #endregion
        #region VD7
        public async Task<int> SaveVd7(VD7 vd7, IDbTransaction transaction = null)
        {
            string command = @"INSERT INTO 
                               VD7 (VD7PEDIDO, VD7EMBARQ, VD7VOLUME)
                               VALUES (@Vd7pedido, @Vd7embarq, @Vd7volume)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        vd7.Vd7pedido,
                        vd7.Vd7embarq,
                        vd7.Vd7volume,
                    },
                    transaction);

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IList<VD7>> FindVd7(string vd7pedido, string vd7embarq)
        {
            string command = @"SELECT *
                                 FROM VD7
                                WHERE VD7PEDIDO = @vd7pedido AND VD7EMBARQ = @vd7embarq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD7> lstVd7 = await connection.QueryAsync<VD7>(command, new { vd7pedido, vd7embarq });

                return lstVd7.ToList();
            }
        }

        public async Task<VD7> FindVd7(string vd7volume)
        {
            string command = @"SELECT *
                                 FROM VD7
                                WHERE VD7VOLUME = @vd7volume";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD7 vd7 = await connection.QueryFirstOrDefaultAsync<VD7>(command, new { vd7volume });

                return vd7;
            }
        }

        public async Task<int> DeleteVd7(string vd7volume)
        {
            string statement = "DELETE FROM VD7 WHERE VD7VOLUME = @vd7volume";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = await connection.ExecuteAsync(statement, new { vd7volume });
                return affectedRows;
            }
        }
        
        public async Task<VD7> FindVd7ByPiece(string vd8peca, string vd1pedido)
        {
            string command = @"SELECT VD7.*
                                 FROM VD7
                                INNER JOIN VD8 ON VD8VOLUME = VD7VOLUME AND VD8PECA = @vd8peca
                                WHERE VD7PEDIDO = @vd1pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD7 vd7 = await connection.QueryFirstOrDefaultAsync<VD7>(command, new { vd1pedido, vd8peca });

                return vd7;
            }
        }
        #endregion
        #region VD8
        public async Task<int> SaveVd8(VD8 vd8, IDbTransaction transaction = null)
        {
            string command = @"INSERT INTO 
                               VD8 (VD8VOLUME, VD8PECA, VD8PRECO, VD8LEITURA)
                               VALUES (@Vd8volume, @Vd8peca, @Vd8preco, @Vd8leitura)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        vd8.Vd8volume,
                        vd8.Vd8peca,
                        vd8.Vd8preco,
                        vd8.Vd8leitura
                    },
                    transaction);

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IList<VD8>> FindVd8(string vd8volume)
        {
            string command = @"SELECT VD8.*, OF3PRODUTO, OF3OPCAO, OF3TAMANHO, PR0DESC, CR1NOME
                                 FROM VD8
                                INNER JOIN OF3 ON OF3PECA = VD8PECA
                                INNER JOIN PR0 ON PR0PRODUTO = OF3PRODUTO
                                INNER JOIN PR2 ON PR2PRODUTO = PR0PRODUTO AND PR2OPCAO = OF3OPCAO
                                INNER JOIN CR1 ON CR1COR = PR2COR
                                WHERE VD8VOLUME = @vd8volume";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD8> lstVd8 = await connection.QueryAsync<VD8>(command, new { vd8volume });

                return lstVd8.ToList();
            }
        }
        public async Task<int> DeleteVd8ByVolume(string vd8volume, IDbTransaction transaction = null)
        {
            string statement = "DELETE FROM VD8 WHERE VD8VOLUME = @vd8volume";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = await connection.ExecuteAsync(statement, new { vd8volume }, transaction);
                return affectedRows;
            }
        }
        public async Task<VD8> FindVd8InSales(string vd8peca, string vd8volume)
        {
            string command = @"SELECT * FROM VD8,VD7,VD1 WHERE VD8VOLUME=VD7VOLUME AND VD7PEDIDO = VD1PEDIDO AND VD8PECA = @vd8peca AND VD8VOLUME <> @vd8volume";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD8 vd8 = await connection.QueryFirstOrDefaultAsync<VD8>(command, new { vd8peca, vd8volume });

                return vd8;
            }
        }

        public async Task<VD8> FindVd8ByPiece(string of3peca)
        {
            string command = @"SELECT VD8.*
                                 FROM VD8
                                INNER JOIN VD7 ON VD8VOLUME=VD7VOLUME
                                INNER JOIN VD1 ON VD7PEDIDO = VD1PEDIDO
                                 LEFT JOIN CF1 ON VD1CLIENTE = CF1CLIENTE
                                WHERE VD8PECA = @of3peca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD8 vd8 = await connection.QueryFirstOrDefaultAsync<VD8>(command, new { of3peca });

                return vd8;
            }
        }

        public async Task<int> RemoveVD8(string vd8peca)
        {
            string statement = "DELETE FROM VD8 WHERE VD8PECA = @vd8peca";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    var affectedRows = await connection.ExecuteAsync(statement, new { vd8peca });
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region VDV
        public async Task<int> SaveVdv(VDV vdv)
        {
            string command = @"INSERT INTO 
                               VDV (VDVVOLUME, VDVPECA, VDVPRECO, VDVLEITURA, VDVQTDE, VDVPRODUTO, VDVOPCAO, VDVTAMANHO)
                               VALUES (@Vdvvolume, @Vdvpeca, @Vdvpreco, @Vdvleitura, @Vdvqtde,@Vdvproduto,@Vdvopcao,@Vdvtamanho)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        vdv.Vdvvolume,
                        vdv.Vdvpeca,
                        vdv.Vdvpreco,
                        vdv.Vdvleitura,
                        vdv.Vdvqtde,
                        vdv.Vdvproduto,
                        vdv.Vdvopcao,
                        vdv.Vdvtamanho
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IList<VDV>> FindVdV(string vdvvolume)
        {
            string command = @"SELECT VDV.*, PR0DESC, CR1NOME
                                 FROM VDV
                                 LEFT JOIN PR0 ON PR0PRODUTO = VDVPRODUTO
                                 LEFT JOIN PR2 ON PR2PRODUTO = PR0PRODUTO AND PR2OPCAO = VDVOPCAO
                                 LEFT JOIN CR1 ON CR1COR = PR2COR
                                WHERE VDVVOLUME = @vdvvolume";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDV> lstVdv = await connection.QueryAsync<VDV>(command, new { vdvvolume });

                return lstVdv.ToList();
            }
        }
        public async Task<int> DeleteVdvByVolume(string vdvvolume, IDbTransaction transaction = null)
        {
            string statement = "DELETE FROM VDV WHERE VDVVOLUME = @vdvvolume";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    var affectedRows = await connection.ExecuteAsync(statement, new { vdvvolume }, transaction);
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<VDV>> FindVdV(string eancodigo, string cf1cliente, bool consigned)
        {
            string command = @"SELECT VDV.*
                                 FROM VDV
                                INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME
                                INNER JOIN VD1 ON VD1PEDIDO = VD7PEDIDO
                                WHERE VDVPECA = @eancodigo
                                  AND VD1CLIENTE = @cf1cliente
                                  AND VD1CONSIG = @consigned
                                ORDER BY VD1ABERT, VD7VOLUME";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDV> lstVdv = await connection.QueryAsync<VDV>(command, new { eancodigo, cf1cliente, consigned });

                return lstVdv.ToList();
            }
        }

        public async Task<int> RemoveVDV(string vdvpeca, string vdvvolume, string vdvproduto, string vdvopcao, string vdvtamanho, decimal vdvqtde, decimal vdvpreco)
        {
            string command = @"UPDATE VDV 
                                  SET VDVQTDE = VDVQTDE - @vdvqtde
                                WHERE VDVVOLUME = @vdvvolume
                                  AND VDVPECA = @vdvpeca
                                  AND VDVPRODUTO = @vdvproduto
                                  AND VDVOPCAO = @vdvopcao
                                  AND VDVTAMANHO = @vdvtamanho
                                  AND VDVPRECO = @vdvpreco
                                  AND VDVQTDE > 0";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, new { vdvpeca, vdvvolume, vdvproduto, vdvopcao, vdvtamanho, vdvqtde, vdvpreco });

                return affectedRows;
            }
        }

        public async Task<IList<VDV>> FindVdV(string pr0produto, string pr2opcao, string pr3tamanho, string cf1cliente, bool consigned)
        {
            string command = @"SELECT VDV.*
                                 FROM VDV
                                INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME
                                INNER JOIN VD1 ON VD1PEDIDO = VD7PEDIDO
                                WHERE VDVPRODUTO = @pr0produto
                                  AND VDVOPCAO = @pr2opcao
                                  AND VDVTAMANHO = @pr3tamanho
                                  AND VD1CLIENTE = @cf1cliente
                                  AND VD1CONSIG = @consigned
                                  AND EXISTS (SELECT 1 FROM CV5 WHERE CV5PEDIDO = VD7PEDIDO)
                                ORDER BY VD1ABERT, VD7VOLUME";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDV> lstVdv = await connection.QueryAsync<VDV>(command, new { pr0produto, pr2opcao, pr3tamanho, cf1cliente, consigned });

                return lstVdv.ToList();
            }
        }
        public async Task<IList<VDV>> FindVdV(string pr0produto, string pr2opcao, string pr3tamanho, string cf1cliente, bool consigned, decimal vdvpreco)
        {
            string command = @"SELECT VDV.*
                                 FROM VDV
                                INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME
                                INNER JOIN VD1 ON VD1PEDIDO = VD7PEDIDO
                                WHERE VDVPRODUTO = @pr0produto
                                  AND VDVOPCAO = @pr2opcao
                                  AND VDVTAMANHO = @pr3tamanho
                                  AND VDVPRECO = @vdvpreco
                                  AND VD1CLIENTE = @cf1cliente
                                  AND VD1CONSIG = @consigned
                                  AND EXISTS (SELECT 1 FROM CV5 WHERE CV5PEDIDO = VD7PEDIDO)
                                ORDER BY VD1ABERT";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDV> lstVdv = await connection.QueryAsync<VDV>(command, new { pr0produto, pr2opcao, pr3tamanho, cf1cliente, consigned, vdvpreco });

                return lstVdv.ToList();
            }
        }
        #endregion
        #region SELECTS
        public async Task<IList<VD1>> SearchPending(string search, int limit, int offset, DateTime since, DateTime until, string orderBy)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT VD1.*,
                                      CF1NOME,
                                      (CASE
                                           WHEN (SELECT SUM(CV5TOTALNF) FROM CV5, VD6 WHERE CV5PEDIDO = VD1PEDIDO AND VD6PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND ((CV5MODELO <> '55' AND CV5MODELO <> '65') OR ((CV5MODELO = '55' OR CV5MODELO = '65') AND NOT EXISTS (SELECT CV5DOC FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND CV5MODELO = '99')))) > 0 THEN (SELECT SUM(CV5TOTALNF) FROM CV5, VD6 WHERE CV5PEDIDO = VD1PEDIDO AND VD6PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND ((CV5MODELO <> '55' AND CV5MODELO <> '65') OR ((CV5MODELO = '55' OR CV5MODELO = '65') AND NOT EXISTS (SELECT CV5DOC FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND CV5MODELO = '99'))))
                                           ELSE COALESCE((SELECT SUM(VD8PRECO) FROM VD8 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD1PEDIDO),0) + COALESCE((SELECT SUM(VDVQTDE * VDVPRECO) FROM VDV INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME WHERE VD7PEDIDO = VD1PEDIDO),0)
                                      END) AS Vd1total
                                 FROM VD1
                                 LEFT JOIN CF1 ON CF1CLIENTE = VD1CLIENTE
                                 LEFT JOIN CF2 ON CF2CEP = CF1CEP
                                 LEFT JOIN CF3 ON CF3LOCAL = CF2LOCAL
                                INNER JOIN VD6 ON VD6PEDIDO = VD1PEDIDO AND VD6FECHA = '1899-12-30 00:00:00'
                                WHERE (VD1PEDIDO LIKE @search
                                   OR VD1CLIENTE LIKE @search
                                   OR UPPER(CF1NOME) LIKE UPPER(@search)
                                   OR UPPER(CF3NOME) LIKE UPPER(@search))
                                  AND DATE(VD1ABERT) BETWEEN DATE(@since) AND DATE(@until)
                                ORDER BY VD1PEDIDO " +  orderBy + " LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD1> orders = await connection.QueryAsync<VD1>(command, new { search, limit, offset, since, until });

                return orders.ToList();
            }
        }
        public async Task<IList<VD1>> Search(string search, int limit, int offset, DateTime since, DateTime until, string orderBy)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT VD1.*,
                                      CF1NOME,
                                      (CASE
                                           WHEN (SELECT SUM(CV5TOTALNF) FROM CV5, VD6 WHERE CV5PEDIDO = VD1PEDIDO AND VD6PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND ((CV5MODELO <> '55' AND CV5MODELO <> '65') OR ((CV5MODELO = '55' OR CV5MODELO = '65') AND NOT EXISTS (SELECT CV5DOC FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND CV5MODELO = '99')))) > 0 THEN (SELECT SUM(CV5TOTALNF) FROM CV5, VD6 WHERE CV5PEDIDO = VD1PEDIDO AND VD6PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND ((CV5MODELO <> '55' AND CV5MODELO <> '65') OR ((CV5MODELO = '55' OR CV5MODELO = '65') AND NOT EXISTS (SELECT CV5DOC FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND CV5MODELO = '99'))))
                                           ELSE COALESCE((SELECT SUM(VD8PRECO) FROM VD8 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD1PEDIDO),0) + COALESCE((SELECT SUM(VDVQTDE * VDVPRECO) FROM VDV INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME WHERE VD7PEDIDO = VD1PEDIDO),0)
                                      END) AS Vd1total,
                                      (CASE
                                          WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5MODELO = '99') > 0
                                          THEN true
                                          ELSE false
                                       END
                                       ) AS HasRomaneio,
                                       (CASE
                                          WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND (CV5MODELO = '55' OR CV5MODELO = '65')) > 0
                                          THEN true
                                          ELSE false
                                       END
                                       ) AS HasNF,
                                       (CASE
                                          WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5MODELO = '99' AND CV5EMITIDO = 0 AND TRIM(CV5SITUA) = '') > 0
                                          THEN true
                                          ELSE false
                                       END
                                       ) AS HasPendingRomaneio,
                                       (CASE
                                          WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND (CV5MODELO = '55' OR CV5MODELO = '65') AND CV5EMITIDO = 0 AND TRIM(CV5SITUA) = '') > 0
                                          THEN true
                                          ELSE false
                                       END
                                       ) AS HasPendingNF,
                                       (SELECT (COUNT(*) = SUM(IF(VD6DOCCONF = '', 0, 1))) FROM VD6 WHERE VD6PEDIDO = VD1PEDIDO) AS ConsignmentClosed
                                 FROM VD1
                                LEFT JOIN CF1 ON CF1CLIENTE = VD1CLIENTE
                                LEFT JOIN CF2 ON CF2CEP = CF1CEP
                                LEFT JOIN CF3 ON CF3LOCAL = CF2LOCAL
                                WHERE (VD1PEDIDO LIKE @search
                                   OR VD1CLIENTE LIKE @search
                                   OR UPPER(CF1NOME) LIKE UPPER(@search)
                                   OR UPPER(CF3NOME) LIKE UPPER(@search))
                                  AND VD1ABERT BETWEEN @since AND @until
                                ORDER BY VD1ABERT " + @orderBy +" LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD1> orders = await connection.QueryAsync<VD1>(command, new { search, limit, offset, since, until, orderBy });

                return orders.ToList();
            }
        }

        public async Task<string> FindLastVd1pedido()
        {
            string command = @"SELECT MAX(VD1PEDIDO) AS ULTIMO FROM VD1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string last = await connection.QueryFirstOrDefaultAsync<string>(command);

                return last;
            }
        }

        public async Task<IList<SearchConsigmentOrder>> SearchConsignment(string search, int limit, int offset, DateTime since, DateTime until, IList<string> lstCustomerException)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT V1.*,
                                      CF1NOME,
                                      (SELECT COUNT(*) FROM VD6 WHERE VD6PEDIDO = VD1PEDIDO) AS QtyShips,
                                      COALESCE((SELECT COUNT(VD8PECA) FROM VD8 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD1PEDIDO AND NOT EXISTS (SELECT 1 FROM VDF WHERE VD8PECA = VDFPECA AND VDFVOLUME = VD8VOLUME)),0) + COALESCE((SELECT SUM(VDVQTDE) FROM VDV INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME WHERE VD7PEDIDO = VD1PEDIDO),0) AS QtyItems,
                                      (SELECT SUM(CV5TOTALPR) FROM CV5, VD6 WHERE CV5PEDIDO = VD1PEDIDO AND VD6PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND ((CV5MODELO <> '55' AND CV5MODELO <> '65') OR ((CV5MODELO = '55' OR CV5MODELO = '65') AND NOT EXISTS (SELECT CV5DOC FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND CV5MODELO = '99')))) AS Cv5total,
                                      COALESCE((SELECT SUM(VD8PRECO) FROM VD8 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD1PEDIDO AND NOT EXISTS (SELECT 1 FROM VDF WHERE VD8PECA = VDFPECA AND VDFVOLUME = VD8VOLUME)),0) + COALESCE((SELECT SUM(VDVQTDE * VDVPRECO) FROM VDV INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME WHERE VD7PEDIDO = VD1PEDIDO),0) AS Vd1total
                                 FROM VD1 AS V1
                                LEFT JOIN CF1 ON CF1CLIENTE = VD1CLIENTE
                                LEFT JOIN CF6 ON CF6REPRES = VD1REPRES
                                WHERE (VD1PEDIDO LIKE @search
                                   OR VD1CLIENTE LIKE @search
                                   OR UPPER(CF1NOME) LIKE UPPER(@search)
                                   OR VD1REPRES LIKE @search
                                   OR UPPER(CF6NOME) LIKE UPPER(@search))
                                  AND VD1ABERT BETWEEN @since AND @until
                                  AND VD1CONSIG = 1
                                  AND ((SELECT COUNT(VD8PECA) FROM VD8 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD1PEDIDO AND NOT EXISTS (SELECT 1 FROM VDF WHERE VD8PECA = VDFPECA AND VDFVOLUME = VD8VOLUME)) > 0
                                        OR
                                       (SELECT SUM(VDVQTDE) FROM VDV INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME WHERE VD7PEDIDO = VD1PEDIDO) > 0)
                                  AND NOT EXISTS (SELECT 1 FROM VD1 AS V2 INNER JOIN LX2 ON V2.VD1CLIENTE = LX2CLIEST AND LX2DEBEST = 1 WHERE V2.VD1PRONTA = 1 AND V2.VD1CONSIG = 1 AND V2.VD1PEDIDO = V1.VD1PEDIDO)
                                  {0}
                                LIMIT @limit OFFSET @offset";

            command = string.Format(command, lstCustomerException == null || lstCustomerException.Count == 0 ? "" : "                                  AND NOT EXISTS (SELECT 1 FROM VD1 AS V2 WHERE V2.VD1PRONTA = 1 AND V2.VD1CONSIG = 1 AND V2.VD1PEDIDO = V1.VD1PEDIDO AND V2.VD1CLIENTE IN @lstCustomerException)");


            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<SearchConsigmentOrder> orders = await connection.QueryAsync<SearchConsigmentOrder>(command, new { search, limit, offset, since, until, lstCustomerException });

                return orders.ToList();
            }
        }

        public async Task<VD1> FindOpenConsignedByCf1cliente(string cf1cliente)
        {
            string command = @"SELECT VD1.*
                                 FROM VD1
                                INNER JOIN VD6 ON VD6PEDIDO = VD1PEDIDO AND VD6DOCCONF = ''
                                WHERE VD1CLIENTE = @cf1cliente
                                  AND VD1CONSIG = 1
                                ORDER BY VD1PEDIDO DESC LIMIT 1";

            string command2= @"SELECT COALESCE(SUM(VD8PRECO),0) AS Amount
                                 FROM VD8
                                INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME
                                INNER JOIN VD6 ON VD6PEDIDO = VD7PEDIDO AND VD7EMBARQ = VD6.VD6EMBARQ
                                WHERE VD6DOCCONF = ''
                                  AND VD6PEDIDO = @vd1pedido";
            string command3 = @"SELECT COALESCE(SUM(VDVQTDE* VDVPRECO),0)
                                  FROM VDV
                                 INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME
                                 INNER JOIN VD6 ON VD7PEDIDO = VD6.VD6PEDIDO AND VD7EMBARQ = VD6.VD6EMBARQ
                                 WHERE VD6DOCCONF = ''
                                   AND VD6PEDIDO = @vd1pedido;";


            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                VD1 vd1;
                using (var transaciton = connection.BeginTransaction())
                {
                    vd1 = await connection.QueryFirstOrDefaultAsync<VD1>(command, new { cf1cliente }, transaciton);
                    if (vd1 != null)
                    {
                        vd1.Vd1total = await connection.QueryFirstOrDefaultAsync<decimal>(command2, new { vd1.Vd1pedido }, transaciton);
                        vd1.Vd1total += await connection.QueryFirstOrDefaultAsync<decimal>(command3, new { vd1.Vd1pedido }, transaciton);
                    }
                }

                return vd1;
            }
        }

        public async Task<IList<string>> FindInvoincedOrder(string vd1pedido, string docfis)
        {
            string command = @"SELECT CV5DOC
                                 FROM CV5
                                WHERE CV5PEDIDO = @vd1pedido AND CV5TIPO = @docfis";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<string> lstCv5 = await connection.QueryAsync<string>(command, new { vd1pedido, docfis });

                return lstCv5.ToList();
            }
        }
        public async Task<VD6> FindLastVd6Embarq(string vd1pedido)
        {
            string command = @"SELECT *
                                 FROM VD6
                                WHERE VD6PEDIDO = @vd1pedido
                                ORDER BY VD6EMBARQ DESC LIMIT 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD6 vd6 = await connection.QueryFirstOrDefaultAsync<VD6>(command, new { vd1pedido });

                return vd6;
            }
        }
        public async Task<IList<VD8>> FindVd8ByOrder(string vd1pedido)
        {
            string command = @"SELECT *
                                 FROM VD8
                                INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME
                                WHERE VD7PEDIDO = @vd1pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD8> lstVd8 = await connection.QueryAsync<VD8>(command, new { vd1pedido });

                return lstVd8.ToList();
            }
        }

        public async Task<IList<VDV>> FindVdvByOrder(string vd1pedido)
        {
            string command = @"SELECT *
                                 FROM VDV
                                INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME
                                WHERE VD7PEDIDO = @vd1pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDV> lstVdv = await connection.QueryAsync<VDV>(command, new { vd1pedido });

                return lstVdv.ToList();
            }
        }

        public async Task<IList<ShipItem>> FindAllShipItemsByVd8(string cv5pedido, string cv5embarq)
        {
            string command = @"SELECT OF3PRODUTO AS Produto
                                    , OF3OPCAO AS Opcao
                                    , OF3TAMANHO AS Tamanho
                                    , VD8PRECO AS Preco
                                    , OF3ORDEM AS Ordem
                                    , OF3LOTE AS Lote
                                    , COUNT(*) AS Qtde
                                 FROM VD8
                                INNER JOIN VD7 ON VD8VOLUME = VD7VOLUME
                                INNER JOIN OF3 ON VD8PECA = OF3PECA
                                WHERE VD7PEDIDO = @cv5pedido
                                  AND VD7EMBARQ = @cv5embarq
                                GROUP BY 1,2,3,4,5,6
                                ORDER BY 1,2,3,4,5,6";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ShipItem> lstShipItem = await connection.QueryAsync<ShipItem>(command, new { cv5pedido, cv5embarq });

                return lstShipItem.ToList();
            }
        }

        public async Task<IList<ShipItem>> FindAllShipItemsByVdv(string cv5pedido, string cv5embarq)
        {
            string command = @"SELECT VDVPRODUTO AS Produto
                                    , VDVOPCAO AS Opcao
                                    , VDVTAMANHO AS Tamanho
                                    , VDVPRECO AS Preco
                                    , '' AS Ordem
                                    , '' AS Lote
                                    , VDVQTDE AS Qtde
                                 FROM VDV
                                INNER JOIN VD7 ON VDVVOLUME = VD7VOLUME
                                WHERE VD7PEDIDO = @cv5pedido
                                  AND VD7EMBARQ = @cv5embarq
                                ORDER BY 1,2,3,4";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ShipItem> lstShipItem = await connection.QueryAsync<ShipItem>(command, new { cv5pedido, cv5embarq });

                return lstShipItem.ToList();
            }
        }

        public async Task<IList<ShipItem>> FindAllShipItemsForMemories(string cv5pedido)
        {
            string command = @"SELECT VD2PRODUTO AS Produto
                                    , '' AS Opcao
                                    , VD2TAMANHO AS Tamanho
                                    , VD2PRECO AS Preco
                                    , '' AS Ordem
                                    , '' AS Lote
                                    , VD2QTDE AS Qtde
                                 FROM VD2
                                WHERE VD2PEDIDO = @cv5pedido
                                ORDER BY 1,3";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ShipItem> lstShipItem = await connection.QueryAsync<ShipItem>(command, new { cv5pedido });

                return lstShipItem.ToList();
            }
        }

        public async Task<IList<ShipItem>> FindAllShipItemsForEsquadrias(string cv5pedido, string cv5embarq)
        {
            string command = @"SELECT VD2PRODUTO AS Produto
                                    , VD2OPCAO AS Opcao
                                    , ' ' AS Tamanho
                                    , VD2QTDE AS Qtde
                                    , VD2PRECO AS Preco
                                    , '' AS Ordem
                                    , '' AS Lote
                                    , VD2PEDIDO AS Pedido
                                    , VD2ITEM AS Item
                                    , VD2DETALHE AS Detalhe
                                    , PR0DESC AS Desc
                                 FROM VD2
                                INNER JOIN VD7 ON VD2ITEM = VD7ITEM AND VD2PEDIDO = VD7PEDIDO
                                INNER JOIN VD6 ON VD7PEDIDO = VD6PEDIDO AND VD6PEDIDO = VD7PEDIDO AND VD6EMBARQ = VD7EMBARQ
                                INNER JOIN PR0 ON VD2PRODUTO = PR0PRODUTO
                                WHERE VD6PEDIDO = @cv5pedido
                                  AND VD6EMBARQ = @vd6embarq
                                ORDER BY 1,9";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ShipItem> lstShipItem = await connection.QueryAsync<ShipItem>(command, new { cv5pedido });

                return lstShipItem.ToList();
            }
        }

        public async Task<IList<string>> FindAllShipItems(string of3produto, string of3opcao, string of3tamanho, string volume, string pedido)
        {
            string command = @"SELECT VD8PECA 
                                 FROM VD8, OF3, VD7
                                WHERE VD8PECA = OF3PECA 
                                  AND VD8VOLUME = VD7VOLUME
                                  AND OF3PRODUTO = @of3produto
                                  AND OF3OPCAO = @of3opcao
                                  AND OF3TAMANHO = @of3tamanho
                                  AND VD8VOLUME <> @volume
                                  AND VD7PEDIDO = @pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<string> lstShipItem = await connection.QueryAsync<string>(command, new { of3produto, of3opcao, of3tamanho, volume, pedido });

                return lstShipItem.ToList();
            }
        }

        public async Task<OF3> FindUsedVd8(string of3produto, string of3opcao, string of3tamanho, string volume, DateTime horaDoCalculo)
        {
            string command = @"SELECT OF3PRODUTO, OF3OPCAO, OF3TAMANHO, COUNT(*) AS PLEDISPO
                                 FROM VD8, OF3
                                WHERE VD8PECA = OF3PECA
                                  AND OF3PRODUTO = @of3produto
                                  AND OF3OPCAO = @of3opcao
                                  AND OF3TAMANHO = @of3tamanho
                                  AND VD8VOLUME <> @volume
                                  AND VD8LEITURA > @horaDoCalculo
                                GROUP BY OF3PRODUTO,OF3OPCAO,OF3TAMANHO";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                OF3 of3 = await connection.QueryFirstOrDefaultAsync<OF3>(command, new { of3produto, of3opcao, of3tamanho, volume, horaDoCalculo });

                return of3;
            }
        }

        public async Task<int> FindQtyInNewOrders(string vd3produto, string vd3opcao, string vd3tamanho, DateTime horaDoCalculo)
        {
            string command = @"SELECT SUM(VD3QTDE) AS PLEDISPO
                                 FROM VD3, VD1
                                WHERE VD3PEDIDO = VD1PEDIDO
                                  AND VD3PRODUTO = @vd3produto
                                  AND VD3OPCAO = @vd3opcao
                                  AND VD3TAMANHO = @vd3tamanho
                                  AND VD1ABERT > @horaDoCalculo
                                GROUP BY VD3PRODUTO,VD3OPCAO,VD3TAMANHO";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                int pledispo = await connection.QueryFirstOrDefaultAsync<int>(command, new { vd3produto, vd3opcao, vd3tamanho, horaDoCalculo });

                return pledispo;
            }
        }

        public async Task<IList<CustomerHistoryModel>> FindCustomerHistory(string cf1cliente, DateTime since, DateTime until, IList<string> lstCv5tipo)
        {
            string command = @"SELECT   VD1.VD1PEDIDO AS Number
                                      , VD1.VD1ABERT AS Dtabert  
                                      , 1 AS Entsai
                                      , (CASE
                                            WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5MODELO = '99') > 0
                                            THEN true
                                            ELSE false
                                         END
                                        ) AS HasRomaneio
                                      , (CASE
                                            WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND (CV5MODELO = '55' OR CV5MODELO = '65')) > 0
                                                THEN true
                                                ELSE false
                                            END
                                        ) AS HasNF
                                      , VD1.VD1CONSIG AS Consignment
                                      , (SELECT (COUNT(*) = SUM(IF(VD6DOCCONF = '', 0, 1))) FROM VD6 WHERE VD6PEDIDO = VD1PEDIDO) AS ConsignmentClosed
                                      , (CASE
                                           WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5MODELO = '99' AND CV5EMITIDO = 0 AND TRIM(CV5SITUA) = '') > 0
                                           THEN true
                                           ELSE false
                                         END
                                         ) AS HasPendingRomaneio
                                       , (CASE
                                            WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND (CV5MODELO = '55' OR CV5MODELO = '65') AND CV5EMITIDO = 0 AND TRIM(CV5SITUA) = '') > 0
                                            THEN true
                                            ELSE false
                                          END
                                         ) AS HasPendingNF
                                       , (CASE
                                            WHEN (SELECT COUNT(CV5DOC) FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND TRIM(CV5SITUA) = '' AND CV5DOCCONF <> '') > 0
                                            THEN true
                                            ELSE false
                                          END
                                         ) AS IsSalesConfirmation
                                       , (CASE
                                               WHEN (SELECT SUM(CV5TOTALNF) FROM CV5, VD6 WHERE CV5PEDIDO = VD1PEDIDO AND VD6PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND ((CV5MODELO <> '55' AND CV5MODELO <> '65') OR ((CV5MODELO = '55' OR CV5MODELO = '65') AND NOT EXISTS (SELECT CV5DOC FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND CV5MODELO = '99')))) > 0 THEN (SELECT SUM(CV5TOTALNF) FROM CV5, VD6 WHERE CV5PEDIDO = VD1PEDIDO AND VD6PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND ((CV5MODELO <> '55' AND CV5MODELO <> '65') OR ((CV5MODELO = '55' OR CV5MODELO = '65') AND NOT EXISTS (SELECT CV5DOC FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND CV5MODELO = '99'))))
                                               ELSE COALESCE((SELECT SUM(VD8PRECO) FROM VD8 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD1PEDIDO),0) + COALESCE((SELECT SUM(VDVQTDE * VDVPRECO) FROM VDV INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME WHERE VD7PEDIDO = VD1PEDIDO),0)
                                          END
                                         ) AS Total
                                FROM VD1
                                LEFT JOIN CF1 ON CF1CLIENTE = VD1CLIENTE
                                LEFT JOIN CF2 ON CF2CEP = CF1CEP
                                LEFT JOIN CF3 ON CF3LOCAL = CF2LOCAL
                                WHERE VD1CLIENTE = @cf1cliente
                                  AND VD1ABERT BETWEEN @since AND @until
                                  AND EXISTS (SELECT 1 FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5TIPO IN @lstCv5tipo)
                                ORDER BY VD1ABERT DESC";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CustomerHistoryModel> orders = await connection.QueryAsync<CustomerHistoryModel>(command, new { cf1cliente, since, until, lstCv5tipo });

                return orders.ToList();
            }
        }

        public async Task<IList<CustomerHistoryDocsModel>> FindCustomerHistoryDocs(string vd1pedido)
        {
            string command = @"SELECT CV5DOC, CV5TIPO, CV5EMISSOR
                                 FROM CV5
                                WHERE CV5PEDIDO = @vd1pedido";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CustomerHistoryDocsModel> LstFiscalDocs = await connection.QueryAsync<CustomerHistoryDocsModel>(command, new { vd1pedido });

                return LstFiscalDocs.ToList();
            }
        }

        public async Task<SearchConsigmentOrder> FindOpenConsignment(string cf1cliente, IList<string> lstCustomerException)
        {
            string command = @"SELECT V1.*,
                                      CF1NOME,
                                      (SELECT COUNT(*) FROM VD6 WHERE VD6PEDIDO = VD1PEDIDO) AS QtyShips,
                                      COALESCE((SELECT COUNT(VD8PECA) FROM VD8 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD1PEDIDO AND NOT EXISTS (SELECT 1 FROM VDF WHERE VD8PECA = VDFPECA AND VDFVOLUME = VD8VOLUME)),0) + COALESCE((SELECT SUM(VDVQTDE) FROM VDV INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME WHERE VD7PEDIDO = VD1PEDIDO),0) AS QtyItems,
                                      (SELECT SUM(CV5TOTALPR) FROM CV5, VD6 WHERE CV5PEDIDO = VD1PEDIDO AND VD6PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND ((CV5MODELO <> '55' AND CV5MODELO <> '65') OR ((CV5MODELO = '55' OR CV5MODELO = '65') AND NOT EXISTS (SELECT CV5DOC FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND CV5MODELO = '99')))) AS Cv5total,
                                      COALESCE((SELECT SUM(VD8PRECO) FROM VD8 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD1PEDIDO AND NOT EXISTS (SELECT 1 FROM VDF WHERE VD8PECA = VDFPECA AND VDFVOLUME = VD8VOLUME)),0) + COALESCE((SELECT SUM(VDVQTDE * VDVPRECO) FROM VDV INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME WHERE VD7PEDIDO = VD1PEDIDO),0) AS Vd1total
                                 FROM VD1 AS V1
                                INNER JOIN CF1 ON CF1CLIENTE = VD1CLIENTE
                                WHERE VD1CLIENTE = @cf1cliente
                                  AND VD1CONSIG = 1
                                  AND ((SELECT COUNT(VD8PECA) FROM VD8 INNER JOIN VD7 ON VD7VOLUME = VD8VOLUME WHERE VD7PEDIDO = VD1PEDIDO AND NOT EXISTS (SELECT 1 FROM VDF WHERE VD8PECA = VDFPECA AND VDFVOLUME = VD8VOLUME)) > 0
                                        OR
                                       (SELECT SUM(VDVQTDE) FROM VDV INNER JOIN VD7 ON VD7VOLUME = VDVVOLUME WHERE VD7PEDIDO = VD1PEDIDO) > 0)
                                  AND NOT EXISTS (SELECT 1 FROM VD1 AS V2 INNER JOIN LX2 ON V2.VD1CLIENTE = LX2CLIEST AND LX2DEBEST = 1 WHERE V2.VD1PRONTA = 1 AND V2.VD1CONSIG = 1 AND V2.VD1PEDIDO = V1.VD1PEDIDO)";

            command += lstCustomerException == null || lstCustomerException.Count == 0 ? "" : "                                  AND NOT EXISTS (SELECT 1 FROM VD1 AS V2 WHERE V2.VD1PRONTA = 1 AND V2.VD1CONSIG = 1 AND V2.VD1PEDIDO = V1.VD1PEDIDO AND V2.VD1CLIENTE IN @lstCustomerException)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.QueryFirstOrDefaultAsync<SearchConsigmentOrder>(command, new { cf1cliente, lstCustomerException });
            }
        }
        #endregion
        #endregion
    }
}
