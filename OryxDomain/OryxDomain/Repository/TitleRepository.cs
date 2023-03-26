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
    public class TitleRepository : Repository
    {
        public TitleRepository(string path) : base(path)
        {
        }

        public async Task<IList<CV2>> FindAll()
        {
            string command = @"SELECT * FROM CV2";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<CV2> lstCv2 = await connection.QueryAsync<CV2>(command);

                    return lstCv2.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CV2> Find(string cv2titulo)
        {
            string command = @"SELECT *
                                 FROM CV2
                                WHERE CV2TITULO = @cv2titulo";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    CV2 cv2 = await connection.QueryFirstOrDefaultAsync<CV2>(command, new { cv2titulo });

                    return cv2;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
