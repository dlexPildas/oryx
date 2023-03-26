using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class UserRepository : Repository
    {
        public UserRepository(string path) : base(path)
        {
        }

        public async Task<IList<DC4>> FindAll()
        {
            string command = @"SELECT DC4USUARIO, DC4NOME FROM DC4";
            
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<DC4> lstDc4 = await connection.QueryAsync<DC4>(command);

                return lstDc4.ToList();
            }
        }

        public async Task<DC4> Find(string dc4usuario)
        {
            string command = @"SELECT * FROM DC4 WHERE DC4USUARIO = @dc4usuario ";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                DC4 dc4 = await connection.QueryFirstOrDefaultAsync<DC4>(command, new { dc4usuario });

                return dc4;
            }
        }

        public async Task<IList<DC5>> FindAllDc5(string dc5usuario)
        {
            string command = @"SELECT * FROM DC5 WHERE DC5USUARIO = @dc5usuario ";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<DC5> lstDc5 = await connection.QueryAsync<DC5>(command, new { dc5usuario });

                return lstDc5.ToList();
            }
        }

        public async Task<int> ChangePass(string dc4usuario, string newPass)
        {
            string command = @"UPDATE DC4 SET DC4SENHA = @newPass WHERE DC4USUARIO = @dc4usuario";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, new { dc4usuario, newPass });

                return affectedRows;
            }
        }

        public async Task<DC5> GetDeleteArchivePermission(string dc5usuario, string dc5arquivo)
        {
            string command = @"SELECT * FROM DC5 
                               WHERE DC5USUARIO = @dc5usuario 
                               AND DC5ARQUIVO = @dc5arquivo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                DC5 lstDc5 = await connection.QueryFirstOrDefaultAsync<DC5>(command, new { dc5usuario,dc5arquivo });

                return lstDc5;
            }
        }
    }
}
