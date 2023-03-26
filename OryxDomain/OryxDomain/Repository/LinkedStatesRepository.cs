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
    public class LinkedStatesRepository : Repository
    {
        public LinkedStatesRepository(string path) : base(path)
        {
        }

        public async Task<IList<CV4>> Find(string cv4opercom)
        {
            string command = @"SELECT CV4.*
                                 FROM CV4
                                WHERE CV4OPERCOM = @cv4opercom";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CV4> cv4 = await connection.QueryAsync<CV4>(command, new { cv4opercom });

                return cv4.ToList();
            }
        }

        public async Task<CV4> Find(string cv4opercom, string cv4estado)
        {
            string command = @"SELECT CV4.*
                                 FROM CV4
                                WHERE CV4OPERCOM = @cv4opercom AND CV4ESTADO = @cv4estado";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV4 cv4 = await connection.QueryFirstOrDefaultAsync<CV4>(command, new { cv4opercom, cv4estado });

                return cv4;
            }
        }
    }
}
