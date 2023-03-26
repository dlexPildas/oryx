using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PostageTagRepository : Repository
    {
        public PostageTagRepository(string path) : base(path)
        {
        }

        public async Task<IList<ET3>> FindList()
        {
            string command = @"SELECT ET3.*, VD1CLIENTE, CF1NOME FROM ET3
                               LEFT JOIN VD1 ON ET3PEDIDO = VD1PEDIDO
                               LEFT JOIN CF1 ON VD1CLIENTE = CF1CLIENTE";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ET3> lstet3 = await connection.QueryAsync<ET3>(command);

                return lstet3.ToList();
            }
        }

        public async Task<ET3> Find(string et3etiquet)
        {
            string command = @"SELECT ET3.*, VD1CLIENTE, CF1NOME FROM ET3
                               LEFT JOIN VD1 ON ET3PEDIDO = VD1PEDIDO
                               LEFT JOIN CF1 ON VD1CLIENTE = CF1CLIENTE
                               WHERE ET3ETIQUET = @et3etiquet";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                ET3 et3 = await connection.QueryFirstOrDefaultAsync<ET3>(command, new { et3etiquet });

                return et3;
            }
        }

        public async Task<IList<ET3>> Search(string search, int limit, int offset, IList<PostageTagStatusType> statuslst)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT ET3ETIQUET, ET3PEDIDO, VD1ABERT, 
                               VD1CLIENTE, CF1NOME, ET3ID, 
                               ET3STATUS, ET3LINKRAS FROM ET3
                               LEFT JOIN VD1 ON ET3PEDIDO = VD1PEDIDO
                               LEFT JOIN CF1 ON VD1CLIENTE = CF1CLIENTE
                               WHERE ET3ETIQUET LIKE @search
                                  OR ET3ID LIKE @search
                                  OR ET3PEDIDO LIKE @search
                                  OR ET3RASTREI LIKE @search
                                  OR ET3STATUS IN @statuslst
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ET3> lxflst = await connection.QueryAsync<ET3>(command, new { search, limit, offset, statuslst });

                return lxflst.ToList();
            }
        }

        public async Task<int> Update(ET3 et3)
        {
            string command = @"UPDATE ET3
                               SET ET3ID = @et3id
                                   ET3LINKRAS = @Et3linkras,
                                   ET3PEDIDO = @Et3pedido,
                                   ET3RASTREI = @Et3rastrei,
                                   ET3STATUS = @Et3status,
                               WHERE ET3ETIQUET = @Et3etiquet";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                   et3.Et3id,
                   et3.Et3linkras,
                   et3.Et3pedido,
                   et3.Et3status,
                   et3.Et3rastrei,
                   et3.Et3etiquet
                });

                return affectedRows;
            }
        }

        public async Task<int> Insert(ET3 et3)
        {
            string command = @"INSERT INTO ET3 (ET3ID, ET3LINKRAS, ET3PEDIDO, ET3RASTREI, ET3STATUS, ET3ETIQUET)
                               VALUES (@Et3id, @Et3linkras, @Et3pedido, @Et3rastrei, @Et3status, @Et3etiquet)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    et3.Et3id,
                    et3.Et3linkras,
                    et3.Et3pedido,
                    et3.Et3status,
                    et3.Et3rastrei,
                    et3.Et3etiquet,
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string et3etiquet)
        {
            string command = @"DELETE FROM ET3 WHERE ET3ETIQUET = @Et3etiquet";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { et3etiquet });
            }
        }

    }
}
