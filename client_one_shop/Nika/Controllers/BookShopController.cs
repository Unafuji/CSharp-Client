using client_one_shop.Connections;
using client_one_shop.Nika.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace client_one_shop.Nika.Controllers
{
    internal sealed class BookShopController
    {
        private static SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString.connectionString);
        }

        public async Task<int> CreateBookAsync(
            string name,
            decimal price,
            string? author = null,
            string? isbn = null,
            DateTime? publishedDate = null,
            CancellationToken ct = default)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.spBooks_Create", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@Author", (object?)author ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ISBN", (object?)isbn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PublishedDate", (object?)publishedDate ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync(ct);
            return Convert.ToInt32(result);
        }

        public async Task<Book?> GetBookAsync(int bookId, CancellationToken ct = default)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.spBooks_Read", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@BookId", bookId);

            using var rdr = await cmd.ExecuteReaderAsync(ct);
            if (!await rdr.ReadAsync(ct)) return null;
            return MapBook(rdr);
        }

        public async Task<IReadOnlyList<Book>> GetBooksAsync(CancellationToken ct = default)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.spBooks_Read", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@BookId", DBNull.Value);

            using var rdr = await cmd.ExecuteReaderAsync(ct);
            var list = new List<Book>();
            while (await rdr.ReadAsync(ct))
                list.Add(MapBook(rdr));

            return list;
        }

        public async Task<int> UpdateBookAsync(
            int bookId,
            string name,
            decimal price,
            string? author = null,
            string? isbn = null,
            DateTime? publishedDate = null,
            CancellationToken ct = default)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.spBooks_Update", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@BookId", bookId);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@Author", (object?)author ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ISBN", (object?)isbn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PublishedDate", (object?)publishedDate ?? DBNull.Value);

            var rows = await cmd.ExecuteNonQueryAsync(ct);
            return rows;
        }

        public async Task<int> DeleteBookAsync(int bookId, CancellationToken ct = default)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.spBooks_Delete", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@BookId", bookId);

            var rows = await cmd.ExecuteNonQueryAsync(ct);
            return rows;
        }


        public async Task<int> AddBookImageAsync(
            int bookId,
            string? fileName,
            string? contentType,
            Stream imageStream,
            CancellationToken ct = default)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.spBookImages_Create", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@BookId", bookId);
            cmd.Parameters.AddWithValue("@FileName", (object?)fileName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContentType", (object?)contentType ?? DBNull.Value);

            using var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms, ct);
            cmd.Parameters.Add(new SqlParameter("@ImageData", SqlDbType.VarBinary, -1) { Value = ms.ToArray() });

            var result = await cmd.ExecuteScalarAsync(ct);
            return Convert.ToInt32(result);
        }

        public async Task<int> AddBookPdfAsync(
            int bookId,
            string? fileName,
            string? contentType,
            Stream pdfStream,
            CancellationToken ct = default)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("dbo.spBookFiles_Create", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@BookId", bookId);
            cmd.Parameters.AddWithValue("@FileName", (object?)fileName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContentType", (object?)contentType ?? "application/pdf");

            using var ms = new MemoryStream();
            await pdfStream.CopyToAsync(ms, ct);
            cmd.Parameters.Add(new SqlParameter("@PdfData", SqlDbType.VarBinary, -1) { Value = ms.ToArray() });

            var result = await cmd.ExecuteScalarAsync(ct);
            return Convert.ToInt32(result);
        }
        private static Book MapBook(SqlDataReader r) => new Book
        {
            BookId = r.GetInt32(r.GetOrdinal("BookId")),
            Name = r.GetString(r.GetOrdinal("Name")),
            Author = r.IsDBNull(r.GetOrdinal("Author")) ? null : r.GetString(r.GetOrdinal("Author")),
            Price = r.GetDecimal(r.GetOrdinal("Price")),
            ISBN = r.IsDBNull(r.GetOrdinal("ISBN")) ? null : r.GetString(r.GetOrdinal("ISBN")),
            PublishedDate = r.IsDBNull(r.GetOrdinal("PublishedDate")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("PublishedDate")),
            CreatedAtUtc = r.GetDateTime(r.GetOrdinal("CreatedAtUtc"))
        };

        public async Task<bool> BookHasPdfAsync(int bookId, CancellationToken ct = default)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand(@"
        IF EXISTS (SELECT 1 FROM dbo.BookFiles WHERE BookId = @id)
            SELECT 1
        ELSE
            SELECT 0;", conn);
            cmd.Parameters.AddWithValue("@id", bookId);

            var val = await cmd.ExecuteScalarAsync(ct);
            return Convert.ToInt32(val) == 1;
        }
        public async Task<byte[]?> GetBookCoverBytesAsync(int bookId, CancellationToken ct = default)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand(@"
        SELECT TOP (1) ImageData 
        FROM dbo.BookImages
        WHERE BookId = @id
        ORDER BY CreatedAtUtc DESC;", conn);
            cmd.Parameters.AddWithValue("@id", bookId);

            var result = await cmd.ExecuteScalarAsync(ct);
            return result == DBNull.Value || result is null ? null : (byte[])result;
        }




    }
}
