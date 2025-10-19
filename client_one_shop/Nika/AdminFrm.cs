using client_one_shop.Nika.Controllers;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_one_shop.Nika
{
    public partial class AdminFrm : Form
    {
        private readonly BookShopController _controller;

        private string? _pendingImagePath;
        private string? _pendingImageContentType;
        private string? _pendingPdfPath;

        private DataTable _multiTable = default!;

        public AdminFrm()
        {
            InitializeComponent();
            _controller = new BookShopController();

            button4.Click += BtnBrowseImage;
            button2.Click += BtnBrowsePDF;
            dataGridViewBooks.CellClick += dataGridViewBooks_CellClick;

            InitializeStagingGrid();
        }

        private async void AdminFrm_Load(object sender, EventArgs e)
        {
            AutoFillDates();
            await LoadBooksAsync();
        }

        private async Task LoadBooksAsync()
        {
            try
            {
                var books = await _controller.GetBooksAsync();

                var dt = new DataTable();
                dt.Columns.Add("BookId", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Author", typeof(string));
                dt.Columns.Add("Price", typeof(decimal));
                dt.Columns.Add("ISBN", typeof(string));
                dt.Columns.Add("PublishedDate", typeof(DateTime));
                dt.Columns.Add("CreatedAtUtc", typeof(DateTime));

                foreach (var b in books)
                {
                    dt.Rows.Add(
                        b.BookId,
                        b.Name,
                        b.Author,
                        b.Price,
                        b.ISBN,
                        b.PublishedDate,
                        b.CreatedAtUtc
                    );
                }

                dataGridViewBooks.AutoGenerateColumns = true;
                dataGridViewBooks.DataSource = dt;
                dataGridViewBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridViewBooks.Columns["PublishedDate"] != null)
                    dataGridViewBooks.Columns["PublishedDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
                if (dataGridViewBooks.Columns["CreatedAtUtc"] != null)
                    dataGridViewBooks.Columns["CreatedAtUtc"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void AutoFillDates()
        {
            textBox1.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            textBoxStock.Text = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static bool TryParseDecimal(string input, out decimal value)
        {
            var ns = NumberStyles.Number;
            return decimal.TryParse(input, ns, CultureInfo.CurrentCulture, out value)
                || decimal.TryParse(input, ns, CultureInfo.InvariantCulture, out value);
        }

        private static DateTime? TryParseDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            if (DateTime.TryParse(input, CultureInfo.CurrentCulture, DateTimeStyles.None, out var d)) return d.Date;
            if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) return d.Date;
            return null;
        }

        private static string GetImageContentTypeFromExtension(string? ext)
        {
            ext = ext?.ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        private void ClearForm()
        {
            textBoxBookId.Clear();
            textBoxISBN13.Clear();
            textBoxAuthorId.Clear();
            textBoxListPrice.Clear();
            textBoxCostPrice.Clear();
            textBoxStock.Clear();
            textBox1.Clear();
            label2.Text = "...";
            label6.Text = "...";

            _pendingImagePath = null;
            _pendingPdfPath = null;
            _pendingImageContentType = null;

            dataGridViewBooks.ClearSelection();
            AutoFillDates();
        }
        private void dataGridViewBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dataGridViewBooks.CurrentRow == null) return;
            var row = dataGridViewBooks.Rows[e.RowIndex];

            textBoxBookId.Text = row.Cells["BookId"].Value?.ToString() ?? "";
            textBoxISBN13.Text = row.Cells["Name"].Value?.ToString() ?? "";
            textBoxAuthorId.Text = row.Cells["Author"].Value?.ToString() ?? "";
            textBoxListPrice.Text = row.Cells["Price"].Value?.ToString() ?? "";
            textBoxCostPrice.Text = row.Cells["ISBN"].Value?.ToString() ?? "";

            if (row.Cells["PublishedDate"].Value is DateTime pd)
                textBoxStock.Text = pd.ToString("yyyy-MM-dd");
            else
                textBoxStock.Clear();

            if (row.Cells["CreatedAtUtc"].Value is DateTime cd)
                textBox1.Text = cd.ToString("yyyy-MM-dd HH:mm:ss");
            else
                textBox1.Clear();

        }
        private void BtnBrowseImage(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp",
                Title = "Select an image file",
                Multiselect = false
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            _pendingImagePath = dlg.FileName;
            _pendingImageContentType = GetImageContentTypeFromExtension(Path.GetExtension(dlg.FileName));
            label2.Text = dlg.FileName;
        }

        private void BtnBrowsePDF(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "PDF Files|*.pdf",
                Title = "Select a PDF file",
                Multiselect = false
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            _pendingPdfPath = dlg.FileName;
            label6.Text = dlg.FileName;
        }

        private async void buttonCreate_Click(object sender, EventArgs e)
        {
            try
            {
                var name = textBoxISBN13.Text?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Name is required.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!TryParseDecimal(textBoxListPrice.Text, out var price) || price <= 0)
                {
                    MessageBox.Show("Price must be a positive number.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var author = textBoxAuthorId.Text?.Trim();
                var isbn = textBoxCostPrice.Text?.Trim();
                DateTime? publishedDate = TryParseDate(textBoxStock.Text);

                var newId = await _controller.CreateBookAsync(
                    name: name,
                    price: price,
                    author: string.IsNullOrWhiteSpace(author) ? null : author,
                    isbn: string.IsNullOrWhiteSpace(isbn) ? null : isbn,
                    publishedDate: publishedDate
                );

                textBoxBookId.Text = newId.ToString();

                await UploadPendingAsync(newId);

                MessageBox.Show("Book created.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadBooksAsync();
                AutoFillDates();
                _pendingPdfPath = null;
                _pendingImagePath = null;
                _pendingImageContentType = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Create failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxBookId.Text, out var id))
            {
                MessageBox.Show("Select a book first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var name = textBoxISBN13.Text?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Name is required.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!TryParseDecimal(textBoxListPrice.Text, out var price) || price <= 0)
                {
                    MessageBox.Show("Price must be a positive number.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var author = textBoxAuthorId.Text?.Trim();
                var isbn = textBoxCostPrice.Text?.Trim();
                DateTime? publishedDate = TryParseDate(textBoxStock.Text);

                await _controller.UpdateBookAsync(
                    id, name, price,
                    string.IsNullOrWhiteSpace(author) ? null : author,
                    string.IsNullOrWhiteSpace(isbn) ? null : isbn,
                    publishedDate
                );

                await UploadPendingAsync(id);

                MessageBox.Show("Book updated.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadBooksAsync();
                _pendingPdfPath = null;
                _pendingImagePath = null;
                _pendingImageContentType = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxBookId.Text, out var id))
            {
                MessageBox.Show("Select a book to delete.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Delete this book?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                await _controller.DeleteBookAsync(id);
                MessageBox.Show("Book deleted.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadBooksAsync();
                ClearForm();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show("Cannot delete: book is referenced by other data.", "Blocked",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UploadPendingAsync(int bookId)
        {
            if (!string.IsNullOrWhiteSpace(_pendingPdfPath) && File.Exists(_pendingPdfPath))
            {
                using var pdf = File.OpenRead(_pendingPdfPath);
                await _controller.AddBookPdfAsync(bookId, Path.GetFileName(_pendingPdfPath), "application/pdf", pdf);
            }
            if (!string.IsNullOrWhiteSpace(_pendingImagePath) && File.Exists(_pendingImagePath))
            {
                var contentType = _pendingImageContentType ?? "application/octet-stream";
                using var img = File.OpenRead(_pendingImagePath);
                await _controller.AddBookImageAsync(bookId, Path.GetFileName(_pendingImagePath), contentType, img);
            }
        }
        private void InitializeStagingGrid()
        {
            _multiTable = new DataTable();
            _multiTable.Columns.Add("Name", typeof(string));
            _multiTable.Columns.Add("Author", typeof(string));
            _multiTable.Columns.Add("Price", typeof(decimal));
            _multiTable.Columns.Add("ISBN", typeof(string));
            _multiTable.Columns.Add("PublishedDate", typeof(DateTime));
            _multiTable.Columns.Add("ImagePath", typeof(string));
            _multiTable.Columns.Add("PdfPath", typeof(string));

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = _multiTable;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void BtnAdd(object sender, EventArgs e)
        {
            var name = textBoxISBN13.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name is required to stage.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!TryParseDecimal(textBoxListPrice.Text, out var price) || price <= 0)
            {
                MessageBox.Show("Price must be a positive number.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var author = textBoxAuthorId.Text?.Trim();
            var isbn = textBoxCostPrice.Text?.Trim();
            var pub = TryParseDate(textBoxStock.Text) ?? DateTime.Today;

            var imgPath = label2.Text != "..." ? label2.Text : _pendingImagePath;
            var pdfPath = label6.Text != "..." ? label6.Text : _pendingPdfPath;

            var row = _multiTable.NewRow();
            row["Name"] = name!;
            row["Author"] = string.IsNullOrWhiteSpace(author) ? DBNull.Value : author!;
            row["Price"] = price;
            row["ISBN"] = string.IsNullOrWhiteSpace(isbn) ? DBNull.Value : isbn!;
            row["PublishedDate"] = pub;
            row["ImagePath"] = string.IsNullOrWhiteSpace(imgPath) ? DBNull.Value : imgPath!;
            row["PdfPath"] = string.IsNullOrWhiteSpace(pdfPath) ? DBNull.Value : pdfPath!;
            _multiTable.Rows.Add(row);

            ClearForm();
        }

        private async void BtnMultiInsert(object sender, EventArgs e)
        {
            if (_multiTable.Rows.Count == 0)
            {
                MessageBox.Show("Nothing staged. Use Add first.", "Empty",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var oldCreate = buttonCreate.Enabled;
            var oldUpdate = buttonUpdate.Enabled;
            var oldDelete = buttonDelete.Enabled;
            var oldAdd = button3.Enabled;
            var oldMulti = button1.Enabled;
            buttonCreate.Enabled = buttonUpdate.Enabled = buttonDelete.Enabled = button3.Enabled = button1.Enabled = false;

            int ok = 0, fail = 0;

            try
            {
                foreach (DataRow r in _multiTable.Rows)
                {
                    try
                    {
                        var name = Convert.ToString(r["Name"]);
                        var author = r["Author"] == DBNull.Value ? null : Convert.ToString(r["Author"]);
                        var price = Convert.ToDecimal(r["Price"], CultureInfo.InvariantCulture);
                        var isbn = r["ISBN"] == DBNull.Value ? null : Convert.ToString(r["ISBN"]);
                        var pub = r["PublishedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["PublishedDate"]);

                        var newId = await _controller.CreateBookAsync(name!, price, author, isbn, pub);

                        var imgPath = r["ImagePath"] == DBNull.Value ? null : Convert.ToString(r["ImagePath"]);
                        if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                        {
                            var contentType = GetImageContentTypeFromExtension(Path.GetExtension(imgPath));
                            using var stream = File.OpenRead(imgPath);
                            await _controller.AddBookImageAsync(newId, Path.GetFileName(imgPath), contentType, stream);
                        }

                        var pdfPath = r["PdfPath"] == DBNull.Value ? null : Convert.ToString(r["PdfPath"]);
                        if (!string.IsNullOrWhiteSpace(pdfPath) && File.Exists(pdfPath))
                        {
                            using var stream = File.OpenRead(pdfPath);
                            await _controller.AddBookPdfAsync(newId, Path.GetFileName(pdfPath), "application/pdf", stream);
                        }

                        ok++;
                    }
                    catch
                    {
                        fail++;
                    }
                }

                MessageBox.Show($"Bulk insert complete. Success: {ok}, Failed: {fail}",
                    "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _multiTable.Clear();
                await LoadBooksAsync();
            }
            finally
            {
                buttonCreate.Enabled = oldCreate;
                buttonUpdate.Enabled = oldUpdate;
                buttonDelete.Enabled = oldDelete;
                button3.Enabled = oldAdd;
                button1.Enabled = oldMulti;
            }
        }

        private void BtnClearTextBox(object sender, EventArgs e) => ClearForm();

        private void lbPdfPath(object sender, EventArgs e) { }

        private void buttonPOS_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SalesForm salesForm = new SalesForm();
            salesForm.Show();
            this.Hide();
        }
    }
}
