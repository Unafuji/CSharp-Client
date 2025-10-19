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

        public AdminFrm()
        {
            InitializeComponent();
            _controller = new BookShopController();

            // Your Designer didn’t wire this. Do it here so the image upload actually fires.
            // Designer already wires button2 -> BtnBrowsePDF, keep that.
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
                dt.Columns.Add("Price", typeof(decimal));

                foreach (var b in books)
                    dt.Rows.Add(b.BookId, b.Name, b.Price);

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
                // Your labels are “Name/Title/Author/Price/ISBN/PublishedDate”
                var name = textBoxISBN13.Text?.Trim();      // labeled “Name”
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Name is required.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var author = textBoxAuthorId.Text?.Trim();  // labeled “Author”
                if (!TryParseDecimal(textBoxListPrice.Text, out var price) || price <= 0)
                {
                    MessageBox.Show("Price must be a positive number.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var isbn = textBoxCostPrice.Text?.Trim();   // labeled “ISBN”
                DateTime? publishedDate = TryParseDate(textBoxStock.Text); // labeled “PublishedDate”

                var newId = await _controller.CreateBookAsync(
                    name: name,
                    price: price,
                    author: string.IsNullOrWhiteSpace(author) ? null : author,
                    isbn: string.IsNullOrWhiteSpace(isbn) ? null : isbn,
                    publishedDate: publishedDate
                );

                textBoxBookId.Text = newId.ToString();
                MessageBox.Show("Book created.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadBooksAsync();
                AutoFillDates(); // refresh defaults for next create
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

                MessageBox.Show("Book updated.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadBooksAsync();
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
            catch (SqlException ex) when (ex.Number == 547) // FK violation
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
            textBoxTitle.Text = row.Cells["Name"].Value?.ToString() ?? "";
            textBoxListPrice.Text = row.Cells["Price"].Value?.ToString() ?? "";
            // Keep date boxes as user-entered or auto-filled; don’t overwrite with partial grid metadata.
        }

        private async void BtnBrowsePDF(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxBookId.Text, out var bookId))
            {
                MessageBox.Show("Select or create a book first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new OpenFileDialog
            {
                Filter = "PDF Files|*.pdf",
                Title = "Select a PDF file",
                Multiselect = false
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var fs = File.OpenRead(dlg.FileName);
                await _controller.AddBookPdfAsync(
                    bookId,
                    Path.GetFileName(dlg.FileName),
                    "application/pdf",
                    fs
                );

                // show selected PDF path in the lower label (label2 per Designer)
                label2.Text = dlg.FileName;
                MessageBox.Show("PDF uploaded.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show("That BookId does not exist. Create the book first.",
                    "Foreign Key blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PDF upload failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnBrowseImage(object? sender, EventArgs e)
        {
            if (!int.TryParse(textBoxBookId.Text, out var bookId))
            {
                MessageBox.Show("Select or create a book first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp",
                Title = "Select an image file",
                Multiselect = false
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var fs = File.OpenRead(dlg.FileName);

                var contentType = GetImageContentTypeFromExtension(Path.GetExtension(dlg.FileName));
                await _controller.AddBookImageAsync(
                    bookId,
                    Path.GetFileName(dlg.FileName),
                    contentType,
                    fs
                );
                 
                label6.Text = dlg.FileName;
                MessageBox.Show("Image uploaded.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show("That BookId does not exist. Create the book first.",
                    "Foreign Key blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Image upload failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            textBoxTitle.Clear();      
            textBoxAuthorId.Clear();   
            textBoxListPrice.Clear();  
            textBoxCostPrice.Clear();  
            textBoxStock.Clear();      
            textBox1.Clear();          
                                       
            label2.Text = "...";       
            label6.Text = "...";       

            dataGridViewBooks.ClearSelection();
            AutoFillDates();
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
    }
}
