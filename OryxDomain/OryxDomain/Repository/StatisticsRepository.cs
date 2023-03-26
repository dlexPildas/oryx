using Dapper;
using OryxDomain.Models;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class StatisticsRepository : Repository
    {
        public StatisticsRepository(string path) : base(path)
        {
        }

        public async Task<CashierStatisticsModel> FindCountSales()
        {
            string command = @"SELECT * FROM PDV_TOT_VENDAS";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CashierStatisticsModel model = await connection.QueryFirstOrDefaultAsync<CashierStatisticsModel>(command);

                return model;
            }
        }

        public async Task<CashierStatisticsModel> FindCountSalesItems()
        {
            string command = @"SELECT * FROM PDV_TOT_QTDE_VENDAS";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CashierStatisticsModel model = await connection.QueryFirstOrDefaultAsync<CashierStatisticsModel>(command);

                return model;
            }
        }

        public async Task<CashierStatisticsModel> FindCountSalesForNewCustomers()
        {
            string command = @"SELECT * FROM PDV_QTDE_VENDAS_NOVOS_CLIENTES";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CashierStatisticsModel model = await connection.QueryFirstOrDefaultAsync<CashierStatisticsModel>(command);

                return model;
            }
        }

        public async Task<CashierStatisticsModel> FindSalesAmount()
        {
            string command = @"SELECT * FROM PDV_VALOR_VENDAS";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CashierStatisticsModel model = await connection.QueryFirstOrDefaultAsync<CashierStatisticsModel>(command);

                return model;
            }
        }
    }
}
