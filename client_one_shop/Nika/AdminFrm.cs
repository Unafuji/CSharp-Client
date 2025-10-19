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
        // Staged file selections (set by Browse buttons, used on Create)
        private string? _pendingImagePath;
        private string? _pendingImageContentType;
        private string? _pendingPdfPath;

        public AdminFrm()
        {
            InitializeComponent();
            _controller = new BookShopController();
             
            button4.Click += BtnBrowseImage;
        }

        private async void AdminFrm_Load(object sender, EventArgs e)
        {
            AutoFillDates();
            await LoadBooksAsync();
        }

        private void AutoFillDates()
        {
            // CreatedAtUtc (textBox1) -> now UTC with seconds
            textBox1.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            // PublishedDate (textBoxStock) -> today (date only)
            textBoxStock.Text = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
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
                    dt.Rows.Add(
                        b.BookId,
                        b.Name,
                        b.Author,
                        b.Price,
                        b.ISBN,
                        b.PublishedDate,
                        b.CreatedAtUtc
                    );

                dataGridViewBooks.AutoGenerateColumns = true;
                dataGridViewBooks.DataSource = dt;
                dataGridViewBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                var author = textBoxAuthorId.Text?.Trim();
                if (!TryParseDecimal(textBoxListPrice.Text, out var price) || price <= 0)
                {
                    MessageBox.Show("Price must be a positive number.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var isbn = textBoxCostPrice.Text?.Trim();
                DateTime? publishedDate = TryParseDate(textBoxStock.Text);

                // 1) Create the book
                var newId = await _controller.CreateBookAsync(
                    name: name,
                    price: price,
                    author: string.IsNullOrWhiteSpace(author) ? null : author,
                    isbn: string.IsNullOrWhiteSpace(isbn) ? null : isbn,
                    publishedDate: publishedDate
                );

                textBoxBookId.Text = newId.ToString();

                // 2) Upload whatever was staged
                await UploadPendingAsync(newId);

                MessageBox.Show("Book created.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 3) refresh and clear staging
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

                var author = textBoxAuthorId.Text?.Trim();
                if (!TryParseDecimal(textBoxListPrice.Text, out var price) || price <= 0)
                {
                    MessageBox.Show("Price must be a positive number.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var isbn = textBoxCostPrice.Text?.Trim();
                DateTime? publishedDate = TryParseDate(textBoxStock.Text);

                await _controller.UpdateBookAsync(
                    id, name, price,
                    string.IsNullOrWhiteSpace(author) ? null : author,
                    string.IsNullOrWhiteSpace(isbn) ? null : isbn,
                    publishedDate
                );

                // New: attach any staged files to this existing book
                await UploadPendingAsync(id);

                MessageBox.Show("Book updated.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadBooksAsync();

                // optional: clear staged after successful update
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

        private void dataGridViewBooks_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewBooks.SelectedRows.Count == 0) return;

            var row = dataGridViewBooks.SelectedRows[0];
            textBoxBookId.Text = row.Cells["BookId"].Value?.ToString() ?? "";
            textBoxListPrice.Text = row.Cells["Price"].Value?.ToString() ?? "";
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

            // Stage only; upload happens after Create/Update
            _pendingPdfPath = dlg.FileName;

            // Designer says label6 is for Pdf Path
            label6.Text = dlg.FileName;
        }



        private void BtnBrowseImage(object? sender, EventArgs e)
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

        private void BtnAdd(object sender, EventArgs e)
        {
        }

        private void BtnMultiInsert(object sender, EventArgs e)
        {
        }

        private void BtnClearTextBox(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            textBoxBookId.Clear();
            textBoxISBN13.Clear();
            textBoxAuthorId.Clear();
            textBoxListPrice.Clear();
            textBoxCostPrice.Clear();
            textBoxStock.Clear();
            textBox1.Clear();
            label2.Text = "..."; // pdf path label
            label6.Text = "..."; // image path label
            dataGridViewBooks.ClearSelection();
            AutoFillDates();

            // also clear staged paths
            _pendingPdfPath = null;
            _pendingImagePath = null;
            _pendingImageContentType = null;
        }


        private void lbPdfPath(object sender, EventArgs e) { }

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

        private void buttonPOS_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SalesForm sales = new SalesForm();
            sales.Show();
            this.Hide();
        }
        private void dataGridViewBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore header clicks or empty rows
            if (e.RowIndex < 0 || dataGridViewBooks.CurrentRow == null) return;

            var row = dataGridViewBooks.Rows[e.RowIndex];

            // Safely assign values from the grid to the form fields
            textBoxBookId.Text = row.Cells["BookId"].Value?.ToString() ?? string.Empty;
            textBoxISBN13.Text = row.Cells["Name"].Value?.ToString() ?? string.Empty;
            textBoxAuthorId.Text = row.Cells["Author"].Value?.ToString() ?? string.Empty;
            textBoxListPrice.Text = row.Cells["Price"].Value?.ToString() ?? string.Empty;
            textBoxCostPrice.Text = row.Cells["ISBN"].Value?.ToString() ?? string.Empty;
            textBoxStock.Text = (row.Cells["PublishedDate"].Value is DateTime pd)
                                        ? pd.ToString("yyyy-MM-dd")
                                        : string.Empty;
            textBox1.Text = (row.Cells["CreatedAtUtc"].Value is DateTime cd)
                                        ? cd.ToString("yyyy-MM-dd HH:mm:ss")
                                        : string.Empty;
        }
        private async Task UploadPendingAsync(int bookId)
        {
            // upload PDF if staged
            if (!string.IsNullOrWhiteSpace(_pendingPdfPath) && File.Exists(_pendingPdfPath))
            {
                using var pdf = File.OpenRead(_pendingPdfPath);
                await _controller.AddBookPdfAsync(bookId, Path.GetFileName(_pendingPdfPath), "application/pdf", pdf);
            }

            // upload Image if staged
            if (!string.IsNullOrWhiteSpace(_pendingImagePath) && File.Exists(_pendingImagePath))
            {
                var contentType = _pendingImageContentType ?? "application/octet-stream";
                using var img = File.OpenRead(_pendingImagePath);
                await _controller.AddBookImageAsync(bookId, Path.GetFileName(_pendingImagePath), contentType, img);
            }
        }

    }
}
