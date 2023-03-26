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
    public class PriceListRepository : Repository
    {
        public PriceListRepository(string path) : base(path)
        {
        }

        public async Task<IList<CV6>> FindAll()
        {
            string command = @"SELECT * FROM CV6 WHERE NOW() BETWEEN CV6VIGINI AND CV6VIGFIM";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<CV6> lstCv6 = await connection.QueryAsync<CV6>(command);

                    return lstCv6.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CV6> Find(string cv6lista)
        {
            string command = @"SELECT *
                                 FROM CV6
                                WHERE CV6LISTA = @cv6lista";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    CV6 cv6 = await connection.QueryFirstOrDefaultAsync<CV6>(command, new { cv6lista });

                    return cv6;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
