using Dapper;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class CartRepository : Repository
    {
        public CartRepository(string path) : base(path)
        {
        }
        public async Task<CAR> Find(string cf1cliente)
        {
            string command = @"SELECT Carlogin, Carcart, Carabert
                        FROM CAR
                        WHERE CARLOGIN = @cf1cliente";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    CAR car = await connection.QueryFirstOrDefaultAsync<CAR>(command, new { cf1cliente });

                    return car;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(string cf1cliente)
        {
            string command = @"DELETE FROM CAR WHERE CARLOGIN = @cf1cliente";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    await connection.ExecuteAsync(command, new { cf1cliente });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Save(CAR cart)
        {
            string command = @"INSERT INTO 
                               CAR (CARLOGIN, CARCART, CARABERT)
                               values (@carlogin, @carcart, @carabert)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        carlogin = cart.Carlogin,
                        carcart = cart.Carcart,
                        carabert = cart.Carabert
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Update(CAR cart)
        {
            string command = @"UPDATE CAR SET CARCART = @carcart, CARABERT = @carabert WHERE CARLOGIN = @carlogin";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        carlogin = cart.Carlogin,
                        carcart = cart.Carcart,
                        carabert = cart.Carabert
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
