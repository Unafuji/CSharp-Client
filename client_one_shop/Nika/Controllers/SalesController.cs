

using client_one_shop.Connections;
using Microsoft.Data.SqlClient;
using System.Data;

namespace client_one_shop.Nika.Controllers
{
    internal class SalesController
    {
         
        public int CreateSale(DateTime saleDateUtc, decimal totalAmount)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("CreateSale", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SaleDateUtc", saleDateUtc);
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);

                SqlParameter outputId = new SqlParameter("@NewSaleId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputId);

                conn.Open();
                cmd.ExecuteNonQuery();

                return (int)outputId.Value;
            }
        }

        // Create Sale Item
        public int CreateSaleItem(int saleId, int bookId, int quantity, decimal unitPrice)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("CreateSaleItem", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SaleId", saleId);
                cmd.Parameters.AddWithValue("@BookId", bookId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);

                SqlParameter outputId = new SqlParameter("@NewSaleItemId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputId);

                conn.Open();
                cmd.ExecuteNonQuery();

                return (int)outputId.Value;
            }
        }

        // Get Sale by ID
        public DataTable GetSaleById(int saleId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("GetSaleById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SaleId", saleId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }

        // Get Sale Item by ID
        public DataTable GetSaleItemById(int saleItemId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("GetSaleItemById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SaleItemId", saleItemId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }

        // Update Sale
        public void UpdateSale(int saleId, DateTime saleDateUtc, decimal totalAmount)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("UpdateSale", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SaleId", saleId);
                cmd.Parameters.AddWithValue("@SaleDateUtc", saleDateUtc);
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Update Sale Item
        public void UpdateSaleItem(int saleItemId, int bookId, int quantity, decimal unitPrice)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("UpdateSaleItem", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SaleItemId", saleItemId);
                cmd.Parameters.AddWithValue("@BookId", bookId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete Sale
        public void DeleteSale(int saleId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("DeleteSale", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SaleId", saleId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete Sale Item
        public void DeleteSaleItem(int saleItemId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("DeleteSaleItem", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SaleItemId", saleItemId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetAllSalesWithItems()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("GetAllSalesWithItems", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }
    }
}
