using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ContactRepository : Repository
    {
        public ContactRepository(string path) : base(path)
        {
        }

        public async Task<CFE> Find(string cfecliente, string cfenome)
        {
            string statement = "SELECT * FROM CFE WHERE CFECLIENTE = @cfecliente AND CFENOME = @cfenome";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                CFE cfe = await connection.QueryFirstOrDefaultAsync<CFE>(statement, new { cfecliente, cfenome });
                return cfe;
            }
        }
        public async Task<int> Insert(CFE cfe)
        {
            string command = @"INSERT INTO CFE(CFECLIENTE,CFENOME,CFEFONEFIX,CFERAMAL,CFEFONECEL,CFEEMAIL,CFEOBSERV,CFEDOCFIN,CFEDOCFIS,CFEMARKET,CFEDEPART,CFECPF,CFENASC,CFEANIVERS)
                                VALUES(@cfecliente,@cfenome,@cfefonefix,@cferamal,@cfefonecel,@cfeemail,@cfeobserv,@cfedocfin,@cfedocfis,@cfemarket,@cfedepart,@cfecpf,@cfenasc,@cfeanivers)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cfe.Cfecliente,
                    cfe.Cfenome,
                    cfe.Cfefonefix,
                    cfe.Cferamal,
                    cfe.Cfefonecel,
                    cfe.Cfeemail,
                    cfe.Cfeobserv,
                    cfe.Cfedocfin,
                    cfe.Cfedocfis,
                    cfe.Cfemarket,
                    cfe.Cfedepart,
                    cfe.Cfecpf,
                    cfe.Cfenasc,
                    cfe.Cfeanivers
                });

                return affectedRows;
            }
        }

        public async Task<int> Update(CFE cfe)
        {
            string command = @"UPDATE CFE
                                  SET CFEFONEFIX = @cfefonefix, CFERAMAL = @cferamal, CFEFONECEL = @cfefonecel, CFEEMAIL = @cfeemail, CFEOBSERV = @cfeobserv, CFEDOCFIN = @cfedocfin, CFEDOCFIS = @cfedocfis, CFEMARKET = @cfemarket, CFEDEPART = @cfedepart, CFECPF = @cfecpf, CFENASC = @cfenasc, CFEANIVERS = @cfeanivers
                                WHERE CFECLIENTE = @cfecliente, CFENOME = @cfenome";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cfe.Cfecliente,
                    cfe.Cfenome,
                    cfe.Cfefonefix,
                    cfe.Cferamal,
                    cfe.Cfefonecel,
                    cfe.Cfeemail,
                    cfe.Cfeobserv,
                    cfe.Cfedocfin,
                    cfe.Cfedocfis,
                    cfe.Cfemarket,
                    cfe.Cfedepart,
                    cfe.Cfecpf,
                    cfe.Cfenasc,
                    cfe.Cfeanivers,
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string cfecliente, string cfenome)
        {
            string statement = "DELETE FROM CFE WHERE CFECLIENTE = @cfecliente AND CFENOME = @cfenome";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var effectedRows = await connection.ExecuteAsync(statement, new { cfecliente, cfenome });
                return effectedRows;
            }
        }

        public async Task<int> DeleteAll(string cfecliente)
        {
            string statement = "DELETE FROM CFE WHERE CFECLIENTE = @cfecliente";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var effectedRows = await connection.ExecuteAsync(statement, new { cfecliente });
                return effectedRows;
            }
        }

        public async Task<IList<CFE>> FindAll(string cfecliente)
        {
            string statement = "SELECT * FROM CFE WHERE CFECLIENTE = @cfecliente";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<CFE> lstCfe = await connection.QueryAsync<CFE>(statement, new { cfecliente });
                return lstCfe.ToList();
            }
        }

        public async Task<IList<string>> FindByCfemarket(string cfecliente)
        {
            string statement = "SELECT CFEEMAIL FROM CFE WHERE CFECLIENTE = @cfecliente AND CFEMARKET = 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<string> lstCfe = await connection.QueryAsync<string>(statement, new { cfecliente });
                return lstCfe.ToList();
            }
        }

        public async Task<IList<string>> FindByCfedocfis(string cfecliente)
        {
            string statement = "SELECT CFEEMAIL FROM CFE WHERE CFECLIENTE = @cfecliente AND CFEDOCFIS = 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<string> lstCfe = await connection.QueryAsync<string>(statement, new { cfecliente });
                return lstCfe.ToList();
            }
        }

        public async Task<IList<string>> FindByCfedocfin(string cfecliente)
        {
            string statement = "SELECT CFEEMAIL FROM CFE WHERE CFECLIENTE = @cfecliente AND CFEDOCFIN = 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<string> lstCfe = await connection.QueryAsync<string>(statement, new { cfecliente });
                return lstCfe.ToList();
            }
        }
    }
}
