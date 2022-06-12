using System.Data;
using System.Data.SqlClient;
using Cwiczenia_4Warehouse.Models;

namespace Cwiczenia_4Warehouse.Services;

public class DbService2
{
    private readonly string connectionString = "Data Source=db-mssql;Initial Catalog=s17943;Integrated Security=True";

    public async Task<int> AddProduct(Warehouse warehouse)
    {
        int id = 0;

        using var con = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("AddProduct", con);

        var transaction = (SqlTransaction) await con.BeginTransactionAsync();
        cmd.Transaction = transaction;

        try
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("IdProduct", warehouse.IdProduct);
            cmd.Parameters.AddWithValue("IdWarehouse", warehouse.IdWarehouse);
            cmd.Parameters.AddWithValue("Amount", warehouse.Amount);
            cmd.Parameters.AddWithValue("CreatedAt", warehouse.CreatedAt);

            await con.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();

            if (rows < 1) throw new Exception("Error: no rows have been affected");

            await transaction.CommitAsync();
        } catch (Exception)
        {
            await transaction.RollbackAsync();
            throw new Exception();
        }

        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "SELECT TOP 1 IdProductWarehouse FROM Product_Warehouse ORDER BY IdProductWarehouse DESC";

        using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();
        if (await reader.ReadAsync())
            id = int.Parse(reader["IdProductWarehouse"].ToString());
        await reader.CloseAsync();

        await con.CloseAsync();

        return id;
    }
}