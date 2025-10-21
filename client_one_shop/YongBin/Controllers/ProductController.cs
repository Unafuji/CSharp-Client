
using client_one_shop.Connections;
using Microsoft.Data.SqlClient;
using System.Data;

namespace client_one_shop.YongBin.Controllers
{
    internal class ProductController
    {


        public bool AddProduct(string productName, string description, decimal price, int quantity)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_CreateProduct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddProduct Error: " + ex.Message);
                return false;
            }
        }

        public DataTable GetProduct(int? productId = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_GetProduct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (productId.HasValue)
                        cmd.Parameters.AddWithValue("@ProductID", productId.Value);
                    else
                        cmd.Parameters.AddWithValue("@ProductID", DBNull.Value);

                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetProduct Error: " + ex.Message);
            }

            return dt;
        }

        public bool UpdateProduct(int productId, string productName, string description, decimal price, int quantity)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_UpdateProduct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateProduct Error: " + ex.Message);
                return false;
            }
        }

        public bool DeleteProduct(int productId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_DeleteProduct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteProduct Error: " + ex.Message);
                return false;
            }
        }

        // -----------------------------
        // BULK INSERT PRODUCTS
        // -----------------------------

        public bool BulkInsertProducts(List<(string ProductName, string Description, decimal Price, int Quantity)> productList)
        {
            try
            {
                DataTable productTable = new DataTable();
                productTable.Columns.Add("ProductName", typeof(string));
                productTable.Columns.Add("Description", typeof(string));
                productTable.Columns.Add("Price", typeof(decimal));
                productTable.Columns.Add("Quantity", typeof(int));

                foreach (var p in productList)
                {
                    productTable.Rows.Add(p.ProductName, p.Description, p.Price, p.Quantity);
                }

                using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_BulkInsertProducts", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@Products", productTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "ProductType"; // Must match type name in SQL

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("BulkInsertProducts Error: " + ex.Message);
                return false;
            }
        }

        // -----------------------------
        // PRODUCT IMAGES CRUD
        // -----------------------------

        public bool AddProductImage(int productId, byte[] imageData, string caption)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_AddProductImage", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@ImageData", imageData ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Caption", caption ?? (object)DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddProductImage Error: " + ex.Message);
                return false;
            }
        }

        public DataTable GetProductImages(int? productId = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_GetProductImages", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (productId.HasValue)
                        cmd.Parameters.AddWithValue("@ProductID", productId.Value);
                    else
                        cmd.Parameters.AddWithValue("@ProductID", DBNull.Value);

                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetProductImages Error: " + ex.Message);
            }

            return dt;
        }

        public bool UpdateProductImage(int imageId, byte[] imageData, string caption)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_UpdateProductImage", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ImageID", imageId);
                    cmd.Parameters.AddWithValue("@ImageData", imageData ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Caption", caption ?? (object)DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateProductImage Error: " + ex.Message);
                return false;
            }
        }


        public bool DeleteProductImage(int imageId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_DeleteProductImage", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ImageID", imageId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteProductImage Error: " + ex.Message);
                return false;
            }
        }
        public DataTable GetProductsWithImages(int? productId = null)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_GetProductsWithImages", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (productId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ProductID", DBNull.Value);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                try
                {
                    conn.Open();
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving products with images: " + ex.Message, ex);
                }
            }

            return dt;
        }
        public DataTable GetProductsWithImages()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConnectionString.connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetProductsWithImages", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }

            return dt;
        }

    }
}
