using client_one_shop.Connections;
using client_one_shop.Models;
using Microsoft.Data.SqlClient;

namespace client_one_shop.Controllers
{
    internal class RetrievalData
    {
        //public static string _connectionString =
        //    "Server=Admin\\SQLEXPRESS;Database=TB_Poduct;Trusted_Connection=True;TrustServerCertificate=True;";

        //public static string _connectionString =
        //    "Server=Admin\\SQLEXPRESS;Database=TB_Poduct;User Id=sa;Password=123;Trusted_Connection=True;";
    //    public static string _connectionString =
    //"Server=Admin\\SQLEXPRESS;Database=TB_Poduct;User Id=sa;Password=123;Encrypt=False;";

        public static List<Product> GetProducts()
        {
            var products = new List<Product>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Product", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Product
                            {
                                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                QuantityInStock = reader.GetInt32(reader.GetOrdinal("QuantityInStock")),
                                SKU = reader.IsDBNull(reader.GetOrdinal("SKU")) ? "" : reader.GetString(reader.GetOrdinal("SKU")),
                                CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                ImgBytes = reader.IsDBNull(reader.GetOrdinal("img"))
                                           ? null
                                           : (byte[])reader["img"]
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return products;
        }
    }
}
