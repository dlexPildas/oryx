using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class MallParametersRepository : Repository
    {
        public MallParametersRepository(string path) : base(path)
        {
        }

        public async Task<int> Update(PD5 pd5)
        {
            string command = @"UPDATE PD5
                                  SET PD5TIPINSH = @Pd5tipinsh,PD5URL = @Pd5url,PD5USUARIO = @Pd5usuario,PD5SENHA = @Pd5senha
                                where PD5CODIGO = @Pd5codigo";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        pd5.Pd5codigo,
                        pd5.Pd5tipinsh,
                        pd5.Pd5url,
                        pd5.Pd5usuario,
                        pd5.Pd5senha
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Delete(int pd5codigo)
        {
            string command = @"DELETE FROM PD5 where PD5CODIGO = @pd5codigo";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    return await connection.ExecuteAsync(command, new { pd5codigo });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Insert(PD5 pd5)
        {
            string command = @"INSERT INTO PD5 (PD5TIPINSH,PD5URL,PD5USUARIO,PD5SENHA)
                               VALUES (@Pd5tipinsh,@Pd5url,@Pd5usuario,@Pd5senha)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        pd5.Pd5codigo,
                        pd5.Pd5tipinsh,
                        pd5.Pd5url,
                        pd5.Pd5usuario,
                        pd5.Pd5senha
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PD5> Find(MallIntegrationType pd5tipinsh)
        {
            string command = @"SELECT PD5.*
                                 FROM PD5
                                WHERE PD5TIPINSH = @pd5tipinsh";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PD5 pd5 = await connection.QueryFirstOrDefaultAsync<PD5>(command, new { pd5tipinsh });

                return pd5;
            }
        }
        public async Task<PD5> Find(int pd5codigo)
        {
            string command = @"SELECT PD5.*
                                 FROM PD5
                                WHERE PD5CODIGO = @pd5codigo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PD5 pd5 = await connection.QueryFirstOrDefaultAsync<PD5>(command, new { pd5codigo });

                return pd5;
            }
        }

        public async Task<IList<PD5>> FindAll()
        {
            string command = @"SELECT PD5.*
                                 FROM PD5";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PD5> lstPd5 = await connection.QueryAsync<PD5>(command);

                return lstPd5.ToList();
            }
        }
    }
}
