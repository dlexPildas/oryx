using Dapper;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ImageRepository : Repository
    {
        public ImageRepository(string path) : base(path)
        {
        }

        public async Task<int> Insert(B2I b2i)
        {
            string command = @"INSERT INTO 
                               B2I (B2IPRODUTO, B2ICAMINHO, B2IPRINCIP)
                               values (@b2iproduto, @b2icaminho, @b2iprincip)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        b2iproduto = b2i.B2iproduto,
                        b2icaminho = b2i.B2icaminho,
                        b2iprincip = b2i.B2iprincip
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(int b2icodigo)
        {
            string command = @"DELETE FROM B2I WHERE B2ICODIGO = @b2icodigo";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    await connection.ExecuteAsync(command, new { b2icodigo });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> SetMain(B2I b2i)
        {
            string command = @"UPDATE B2I SET B2IPRINCIP = @b2iprincip WHERE B2ICODIGO = @b2icodigo";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        b2iprincip = b2i.B2iprincip,
                        b2icodigo = b2i.B2icodigo
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<B2I> FindMain(string b2iproduto)
        {
            string command = @"SELECT B2I.*
                        FROM B2I
                        WHERE B2IPRODUTO = @b2iproduto AND B2IPRINCIP = 1";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    B2I b2i = await connection.QueryFirstOrDefaultAsync<B2I>(command, new { b2iproduto });

                    return b2i;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<B2I>> FindList(string b2iproduto)
        {
            string command = @"SELECT B2I.*
                        FROM B2I
                        WHERE B2IPRODUTO = @b2iproduto
                        ORDER BY B2IPRINCIP DESC, B2ICODIGO";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<B2I> b2i = await connection.QueryAsync<B2I>(command, new { b2iproduto });

                    return b2i.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
