using Dapper;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class SecurityRepository : Repository
    {
        public SecurityRepository(string path) : base(path)
        {
        }

        public async Task<int> Insert(TOK tok)
        {
            string command = @"insert into 
                               tok (toktoken, toklogin, tokcriado, tokexpira)
                               values (@toktoken, @toklogin, @tokcriado, @tokexpira)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    toktoken = tok.Toktoken,
                    toklogin = tok.Toklogin,
                    tokcriado = tok.Tokcriado,
                    tokexpira = tok.Tokexpira,
                });

                return affectedRows;
            }
        }

        public async Task<TOK> FindByLogin(string toklogin)
        {
            string command = @"SELECT tokcodigo, toktoken, toklogin, tokcriado, tokexpira
                               FROM tok
                               WHERE toklogin =  @toklogin";
            
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                TOK tok = await connection.QueryFirstOrDefaultAsync<TOK>(command, new { toklogin });

                return tok;
            }
        }

        public async Task<TOK> FindValidateToken(string toklogin, string toktoken)
        {
            string command = @"SELECT tokcodigo, toktoken, toklogin, tokcriado, tokexpira
                               FROM tok
                               WHERE toklogin =  @toklogin
                                 AND toktoken =  @toktoken";
            
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                TOK tok = await connection.QueryFirstOrDefaultAsync<TOK>(command, new { toklogin, toktoken });

                return tok;
            }
        }

        public async Task Delete(string token)
        {
            string command = @"DELETE FROM TOK WHERE TOKTOKEN = @token";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                await connection.ExecuteAsync(command, new { token });
            }
        }

        public async Task<TOK> FindWhiteList(string toktoken)
        {
            string command = @"SELECT tokcodigo, toktoken, toklogin, tokcriado, tokexpira
                               FROM tok
                               WHERE toktoken =  @toktoken";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                TOK tok = await connection.QueryFirstOrDefaultAsync<TOK>(command, new { toktoken });

                return tok;
            }
        }

        public async Task<int> Logoff(string token, DateTime expiration)
        {
            string command = @"UPDATE TOK SET TOKEXPIRA = @expiration WHERE TOKTOKEN = @token";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { token, expiration });
            }
        }

        public async Task<string> FindLx9Valid(string lx9acesso)
        {
            string command = @"SELECT LX9USUARIO FROM LX9 WHERE LX9ACESSO =  @lx9acesso AND LX9SAIDA = '1899-12-30 00:00:00' OR LX9SAIDA > NOW()";
            
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string Lx9usuario = await connection.QueryFirstOrDefaultAsync<string>(command, new { lx9acesso });

                return Lx9usuario;
            }
        }

        public async Task<int> LogoffInternal(string lx9acesso, DateTime lx9saida)
        {
            string command = @"UPDATE LX9 SET LX9SAIDA = @lx9saida WHERE LX9ACESSO = @lx9acesso";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, new { lx9acesso, lx9saida });

                return affectedRows;
            }
        }

        public async Task<int> LogAuthInternal(string lx9acesso, string lx9usuario, string lx9comput, DateTime lx9entrada)
        {
            string command = @"INSERT INTO 
                               LX9 (LX9ACESSO,LX9USUARIO,LX9COMPUT,LX9ENTRADA)
                               values (@lx9acesso, @lx9usuario, @lx9comput, @lx9entrada)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    lx9acesso,
                    lx9usuario,
                    lx9comput,
                    lx9entrada
                });

                return affectedRows;
            }
        }

        public async Task<string> FindLx9(string lx9acesso)
        {
            string command = @"SELECT LX9USUARIO FROM LX9 WHERE LX9ACESSO = @lx9acesso";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string Lx9usuario = await connection.QueryFirstOrDefaultAsync<string>(command, new { lx9acesso });

                return Lx9usuario;
            }
        }
    }
}
