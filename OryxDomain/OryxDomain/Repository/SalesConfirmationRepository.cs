using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class SalesConfirmationRepository : Repository
    {
        public SalesConfirmationRepository(string path) : base(path)
        {
        }

        #region Oryx Gestão
        public async Task<int> Insert(VDE vde, IDbTransaction transaction = null)
        {
            string command = @"INSERT INTO VDE (VDEDOC, VDECLIENTE, VDEFECHA, VDEOBSERVA, VDEID, VDEOPERCOM, VDEDESCFAT)
                               VALUES (@vdedoc,@vdecliente,@vdefecha,@vdeobserva,@vdeid,@vdeopercom,@vdedescfat)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        vde.Vdedoc,
                        vde.Vdecliente,
                        vde.Vdefecha,
                        vde.Vdeobserva,
                        vde.Vdeid,
                        vde.Vdeopercom,
                        vde.Vdedescfat,
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

        public async Task<VDE> Find(string vdedoc)
        {
            string command = @"SELECT VDE.*, CF1NOME FROM VDE INNER JOIN CF1 ON CF1CLIENTE = VDECLIENTE WHERE VDEDOC = @vdedoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VDE vde = await connection.QueryFirstOrDefaultAsync<VDE>(command, new { vdedoc });

                return vde;
            }
        }

        public async Task<int> Update(VDE vde)
        {
            string command = @"UPDATE VDE
                                  SET VDECLIENTE = @vdecliente,
                                      VDEFECHA = @vdefecha,
                                      VDEOBSERVA = @vdeobserva,
                                      VDEID = @vdeid,
                                      VDEOPERCOM = @vdeopercom,
                                      VDEDESCFAT = @vdedescfat
                                WHERE VDEDOC = @vdedoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    vde.Vdedoc,
                    vde.Vdecliente,
                    vde.Vdefecha,
                    vde.Vdeobserva,
                    vde.Vdeid,
                    vde.Vdeopercom,
                    vde.Vdedescfat,
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string vdedoc)
        {
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = 0;
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VDE WHERE VDEDOC = @vdedoc", new { vdedoc }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VDF WHERE VDFDOC = @vdedoc", new { vdedoc }, transaction: transaction);
                        affectedRows = await connection.ExecuteAsync("DELETE FROM VDZ WHERE VDZDOC = @vdedoc", new { vdedoc }, transaction: transaction);

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

        public async Task<IList<VDE>> Search(string search, int limit, int offset, string orderBy)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT VDE.*
                                    , CF1NOME
                                    , (CASE WHEN (SELECT COUNT(*) FROM CV5 WHERE CV5DOCCONF = VDEDOC AND CV5ENTSAI = 1) > 0 THEN 1 ELSE 0 END) AS Invoiced
                                    , (CASE WHEN (SELECT COUNT(*) FROM CV5 WHERE CV5DOCCONF = VDEDOC AND CV5ENTSAI = 1) > 0 THEN (SELECT CV5PEDIDO FROM CV5 WHERE CV5DOCCONF = VDEDOC AND CV5ENTSAI = 1 LIMIT 1) ELSE '' END) AS OrderInvoiced
                                 FROM VDE
                                INNER JOIN CF1 ON CF1CLIENTE = VDECLIENTE
                                WHERE (VDEDOC LIKE @search
                                   OR VDECLIENTE LIKE @search
                                   OR UPPER(CF1NOME) LIKE UPPER(@search))
                                ORDER BY VDEDOC " + orderBy + " LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDE> salesConfirmation = await connection.QueryAsync<VDE>(command, new { search, limit, offset });

                return salesConfirmation.ToList();
            }
        }

        public async Task<VD6> FindIsInvoice(string vdedoc)
        {
            string command = @"SELECT * FROM VD6 WHERE VD6DOCCONF = @vdedoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD6 vd6 = await connection.QueryFirstOrDefaultAsync<VD6>(command, new { vdedoc });

                return vd6;
            }
        }
        #endregion

        #region oryx esquadrias
        public async Task<IList<VDE>> FindVde(string vd6pedido, string vd6embarq)
        {
            string command = @"SELECT *
                                 FROM VDE, 
                                WHERE VDEPEDIDO = @vd6pedido AND VDEEMBARQ = @vd6embarq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDE> lstVd5 = await connection.QueryAsync<VDE>(command, new { vd6pedido, vd6embarq });

                return lstVd5.ToList();
            }
        }
        #endregion

    }
}
