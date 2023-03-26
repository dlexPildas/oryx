using Dapper;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class FormRepository : Repository
    {
        public FormRepository(string path) : base(path)
        {
        }

        public async Task<IList<dynamic>> Search(string dc1arqpai, string dc1campai, IList<string> moreFilters, string text, string field, int limit, IList<string> conditions, string moreJoins)
        {
            text = text.Replace(" ", "%");
            string moreFields = "";
            string moreConditions = "";
            string moreEspecificCondition = "";
            text = string.Format("%{0}%", text).ToUpper();
            foreach (var moreFilter in moreFilters)
            {
                moreFields += ", " + moreFilter;
                moreConditions += " OR " + moreFilter + " LIKE '"+ text + "'";
            }
            foreach (string con in conditions)
            {
                moreEspecificCondition += " AND " + con;
            }
            string command = string.Format("SELECT {0} AS {1} {4} FROM {2} {8} WHERE (UPPER({0}) like '{3}' {5}) {7} limit {6}", dc1campai, field, dc1arqpai, text, moreFields, moreConditions, limit, moreEspecificCondition, moreJoins);

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<dynamic> lstDc1 = await connection.QueryAsync<dynamic>(command, new { });

                return lstDc1.ToList();
            }
        }

        public async Task<IList<dynamic>> FindTable(string dc1arquivo, IList<string> moreFilters, string text, string field, string orderBy)
        {
            text = text.Replace(" ", "%");
            string moreFields = "";
            string moreConditions = "";
            text = string.Format("%{0}%", text).ToUpper();
            foreach (var moreFilter in moreFilters)
            {
                moreFields += ", " + moreFilter;
                moreConditions += " OR " + moreFilter + " LIKE '" + text + "'";
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
                orderBy = " order by " + orderBy;

            string command = string.Format("SELECT {0} {3} FROM {1} WHERE UPPER({0}) like '{2}' {4} {5} limit 200", field, dc1arquivo, text, moreFields, moreConditions, orderBy);

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<dynamic> lstDc1 = await connection.QueryAsync<dynamic>(command, new { });

                return lstDc1.ToList();
            }
        }
    }
}
