using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class APIParametersRepository : Repository
    {
        public APIParametersRepository(string path) : base(path)
        {
        }

        public async Task<int> Update(LXE lxe)
        {
            string command = @"UPDATE LXE
                                  SET 
                                      LXEBASEURL = @lxebaseurl
                                    , LXEIMAGSER = @lxeimagser
                                    , LXEAPICEP = @lxeapicep
                                    , LXEATUALIZ = @lxeatualiz
                                WHERE LXEPADRAO = @lxepadrao";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        lxepadrao = lxe.Lxepadrao,
                        lxebaseurl = lxe.Lxebaseurl,
                        lxeimagser = lxe.Lxeimagser,
                        lxeapicep = lxe.Lxeapicep,
                        lxeatualiz = lxe.Lxeatualiz
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Delete()
        {
            string command = @"DELETE FROM LXE";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    return await connection.ExecuteAsync(command);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Insert(LXE lxe)
        {
            string command = @"INSERT INTO LXE (LXEPADRAO, LXEBASEURL,LXEIMAGSER,LXEAPICEP,LXEATUALIZ)
                               VALUES (@lxepadrao,@lxebaseurl,@lxeimagser,@lxeapicep,@lxeatualiz)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        lxepadrao = lxe.Lxepadrao,
                        lxebaseurl = lxe.Lxebaseurl,
                        lxeimagser = lxe.Lxeimagser,
                        lxeatualiz = lxe.Lxeatualiz,
                        lxeapicep = lxe.Lxeapicep,
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LXE> Find()
        {
            string command = @"SELECT * FROM LXE";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                LXE lxe = await connection.QueryFirstOrDefaultAsync<LXE>(command);

                return lxe;
            }
        }
    }
}
