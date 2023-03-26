using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OryxDomain.Repository
{
    public class RepresentativeRepository : Repository
    {
        public RepresentativeRepository(string path) : base(path)
        {
        }

        public async Task<IList<CF6>> FindAll()
        {
            string command = @"SELECT * FROM CF6";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<CF6> lstCf6 = await connection.QueryAsync<CF6>(command);

                    return lstCf6.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CF6> Find(string cf6repres)
        {
            string command = @"SELECT CF6.*, CF1AVISO
                                 FROM CF6
                                 LEFT JOIN CF1 ON CF6CLIENTE = CF1CLIENTE
                                WHERE CF6REPRES = @cf6repres";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    CF6 cf6 = await connection.QueryFirstOrDefaultAsync<CF6>(command, new { cf6repres });

                    return cf6;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CF6>> FindByCustomer(string cf6cliente)
        {
            string command = @"SELECT *
                                 FROM CF6
                                WHERE CF6CLIENTE = @cf6cliente";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    IEnumerable<CF6> result =  await connection.QueryAsync<CF6>(command, new { cf6cliente });
                    return result?.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IList<string>> FindAllWithLastRepres(string cf1cliente)
        {
           string command = "SELECT DISTINCT VD1REPRES FROM VD1 WHERE VD1CLIENTE = @cf1cliente AND VD1REPRES <> '' ORDER BY VD1ABERT DESC LIMIT 5";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<string> last = await connection.QueryAsync<string>(command, new { cf1cliente });

                return last.ToList();
            }
        }

        public async Task<int> Insert(CF6 cf6)
        {
            string command = @"INSERT INTO cf6(CF6REPRES, CF6NOME, CF6COMIS, CF6PERCEMI, CF6PERCLIQ, CF6CLIENTE, CF6PEDIDOI, CF6PEDIDOF, CF6CTRL1, cf6retorno, CF6ULTPED, CF6PLACA, CF6PLACAUF, CF6DOCFIS, CF6FORMS, CF6NAOALT)
                            VALUES (@cf6repres,@cf6nome,@cf6comis,@cf6percemi,@cf6percliq,@cf6cliente,@cf6pedidoi,@cf6pedidof,@cf6ctrl1,@cf6retorno,@cf6ultped,@cf6placa,@cf6placauf,@cf6docfis,@cf6forms,@cf6naoalt)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        Cf6repres = cf6.Cf6repres,
                        Cf6nome = cf6.Cf6nome,
                        Cf6comis = cf6.Cf6comis,
                        Cf6percemi = cf6.Cf6percemi, 
                        Cf6percliq = cf6.Cf6percliq, 
                        Cf6cliente = cf6.Cf6cliente, 
                        Cf6pedidoi = cf6.Cf6pedidoi,
                        Cf6pedidof = cf6.Cf6pedidof,
                        Cf6ctrl1 = cf6.Cf6ctrl1,
                        cf6retorno = cf6.cf6retorno,
                        Cf6ultped = cf6.Cf6ultped,
                        Cf6placa = cf6.Cf6placa,
                        Cf6placauf = cf6.Cf6placauf,
                        Cf6docfis = cf6.Cf6docfis,
                        Cf6forms = cf6.Cf6forms,
                        Cf6naoalt = cf6.Cf6naoalt
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CF6> FindLastByCustomer(string cf6cliente)
        {
            string command = @"SELECT cf6.*
                                 FROM CF6
                                INNER JOIN VD1 ON VD1REPRES = CF6REPRES
                                WHERE VD1CLIENTE = @cf6cliente
                                ORDER BY VD1ENTRADA DESC LIMIT 1";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                CF6 result = await connection.QueryFirstOrDefaultAsync<CF6>(command, new { cf6cliente });
                return result;
            }
        }
    }
}
