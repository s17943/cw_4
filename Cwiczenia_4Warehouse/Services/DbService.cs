using System.Data.SqlClient;
using Cwiczenia_4Warehouse.Models;

namespace Cwiczenia_4Warehouse.Services;

     public class DbService : IDbService
    {
        private readonly string connectionString = "Data Source=db-mssql;Initial Catalog=s17943;Integrated Security=True";
        
        public async Task<int> AddProduct(Warehouse warehouse)
        {
            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand();

            cmd.Connection = con;
            await con.OpenAsync();

            cmd.CommandText = "SELECT TOP 1 [Order].IdOrder FROM [Order] " +
                                "LEFT JOIN Product_Warehouse ON [Order].IdOrder = Product_Warehouse.IdOrder " +
                                "WHERE [Order].IdProduct = @IdProduct " +
                                "AND [Order].Amount = @Amount " +
                                "AND Product_Warehouse.IdProductWarehouse IS NULL " +
                                "AND [Order].CreatedAt < @CreatedAt";

            cmd.Parameters.AddWithValue("IdProduct", warehouse.IdProduct);
            cmd.Parameters.AddWithValue("Amount", warehouse.Amount);
            cmd.Parameters.AddWithValue("CreatedAt", warehouse.CreatedAt);
            
            var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) throw new Exception("Order is empty.");

            await reader.ReadAsync();
            
            int idOrder = int.Parse(reader["IdOrder"].ToString());
            await reader.CloseAsync();

            cmd.Parameters.Clear();

            cmd.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
            cmd.Parameters.AddWithValue("IdProduct", warehouse.IdProduct);

            reader = await cmd.ExecuteReaderAsync();
            
            if (!reader.HasRows) throw new Exception("Please state an existing product id.");
            
            await reader.ReadAsync();
            double price = double.Parse(reader["Price"].ToString());
            await reader.CloseAsync();

            cmd.Parameters.Clear();

            cmd.CommandText = "SELECT IdWarehouse FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
            cmd.Parameters.AddWithValue("IdWarehouse", warehouse.IdWarehouse);

            reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) throw new Exception("Please state a valid warehouse Id");

            await reader.CloseAsync();
            cmd.Parameters.Clear();
            

            var transaction = (SqlTransaction) await con.BeginTransactionAsync();
            cmd.Transaction = transaction;

            try
            {
                cmd.CommandText = "UPDATE [Order] SET FulfilledAt = @CreatedAt WHERE IdOrder = @IdOrder";
                cmd.Parameters.AddWithValue("CreatedAt", warehouse.CreatedAt);
                cmd.Parameters.AddWithValue("IdOrder", idOrder);

                int rowsUpdated = await cmd.ExecuteNonQueryAsync();

                if (rowsUpdated < 1) throw new Exception();

                cmd.Parameters.Clear();

                cmd.CommandText = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                    $"VALUES(@param1, @param2, @param3, @param4, @param4*{price}, @param5)";

                cmd.Parameters.AddWithValue("param1", warehouse.IdWarehouse);
                cmd.Parameters.AddWithValue("param2", warehouse.IdProduct);
                cmd.Parameters.AddWithValue("param3", idOrder);
                cmd.Parameters.AddWithValue("param4", warehouse.Amount);
                cmd.Parameters.AddWithValue("param5", warehouse.CreatedAt);

                int noOfRows = await cmd.ExecuteNonQueryAsync();
                if (noOfRows <= 0) throw new Exception("No rows have been added by the operation.");
                await transaction.CommitAsync();
                
            } catch (Exception) { 
                await transaction.RollbackAsync();
                throw new Exception("Database error has occurred.");
            }

            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT TOP 1 IdProductWarehouse FROM Product_Warehouse ORDER BY IdProductWarehouse DESC";

            reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            int idProductWarehouse = int.Parse(reader["IdProductWarehouse"].ToString());
            await reader.CloseAsync();
            await con.CloseAsync();
           
            return idProductWarehouse;
        }
    }
