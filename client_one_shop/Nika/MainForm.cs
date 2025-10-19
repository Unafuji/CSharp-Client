using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using client_one_shop.Nika.Controllers;

namespace client_one_shop.Nika
{
    public partial class MainForm : Form
    {
        private readonly BookShopController _controller;
        private List<BookItem> _books = new();
        private DataTable _cartTable = default!;
        private readonly SalesController _salesController = new SalesController();

        // thumbnail cache + placeholders
        private readonly Dictionary<int, Image> _thumbCache = new();
        private Image? _placeholderImage;
        private Image? _pdfBadgeImage;

        public MainForm()
        {
            InitializeComponent();
            _controller = new BookShopController();

            // Build cart once with correct schema
            InitializeCart();

            // Load async after UI is ready
            this.Load += async (_, __) =>
            {
                await LoadBooksAsync();
                RenderBooks();
                UpdateCartSummary();
            };
        }

        /* =========================
           Data loading from controller
           ========================= */
        private async Task LoadBooksAsync()
        {
            var books = await _controller.GetBooksAsync();
            _books = books.Select(b => new BookItem
            {
                BookId = b.BookId,
                Name = b.Name,
                Author = b.Author ?? "",
                Price = b.Price
            }).ToList();
        }

        /* =========================
           Cart table + grid
           ========================= */
        private void InitializeCart()
        {
            _cartTable = new DataTable();

            // Image column first
            _cartTable.Columns.Add("Thumb", typeof(Image));
            _cartTable.Columns.Add("BookId", typeof(int));
            _cartTable.Columns.Add("Title", typeof(string));
            _cartTable.Columns.Add("Author", typeof(string));
            _cartTable.Columns.Add("Qty", typeof(int));
            _cartTable.Columns.Add("UnitPrice", typeof(decimal));
            _cartTable.Columns.Add("LineTotal", typeof(decimal), "Qty * UnitPrice");

            dataGridViewCart.DataSource = _cartTable;
            dataGridViewCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Ensure image column renders properly
            if (dataGridViewCart.Columns["Thumb"] is DataGridViewImageColumn imgCol)
            {
                imgCol.HeaderText = "";
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                //imgCol.Width = 56;
            }
            else
            {
                var newImgCol = new DataGridViewImageColumn
                {
                    DataPropertyName = "Thumb",
                    HeaderText = "",
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = 56
                };
                dataGridViewCart.Columns.Insert(0, newImgCol);
            }
            dataGridViewCart.RowTemplate.Height = 56;

            // Recalc totals on qty edit/remove
            dataGridViewCart.CellEndEdit += (_, e) =>
            {
                if (e.RowIndex >= 0 && dataGridViewCart.Columns[e.ColumnIndex].DataPropertyName == "Qty")
                    UpdateCartSummary();
            };
            dataGridViewCart.RowsRemoved += (_, __) => UpdateCartSummary();

            // init placeholders once
            _placeholderImage = CreatePlaceholder("IMG");
            _pdfBadgeImage = CreatePlaceholder("PDF", back: Color.Firebrick);
        }

        private void AddToCart(BookItem book)
        {
            // If exists, bump qty
            foreach (DataRow row in _cartTable.Rows)
            {
                if ((int)row["BookId"] == book.BookId)
                {
                    row["Qty"] = Convert.ToInt32(row["Qty"], CultureInfo.InvariantCulture) + 1;
                    UpdateCartSummary();
                    return;
                }
            }

            // New row with placeholder; LineTotal is computed, do not assign
            var r = _cartTable.NewRow();
            r["Thumb"] = _placeholderImage;
            r["BookId"] = book.BookId;
            r["Title"] = book.Name;
            r["Author"] = book.Author;
            r["Qty"] = 1;
            r["UnitPrice"] = book.Price;
            _cartTable.Rows.Add(r);

            UpdateCartSummary();

            // Fire-and-forget thumbnail load
            _ = LoadThumbForRowAsync(r, book.BookId);
        }

        private async Task LoadThumbForRowAsync(DataRow row, int bookId)
        {
            try
            {
                if (_thumbCache.TryGetValue(bookId, out var cached))
                {
                    SetRowThumb(row, cached);
                    return;
                }

                // Try cover image
                var bytes = await _controller.GetBookCoverBytesAsync(bookId);
                Image? img = null;
                if (bytes != null && bytes.Length > 0)
                {
                    using var ms = new MemoryStream(bytes);
                    img = Image.FromStream(ms);
                }

                // Fallback: PDF badge if any PDF exists, else generic IMG
                if (img == null)
                {
                    var hasPdf = await _controller.BookHasPdfAsync(bookId);
                    img = hasPdf ? _pdfBadgeImage : _placeholderImage;
                }

                if (img != null)
                    _thumbCache[bookId] = img;

                SetRowThumb(row, img);
            }
            catch
            {
                SetRowThumb(row, _placeholderImage);
            }
        }

        // Marshal DataTable mutation to UI thread
        private void SetRowThumb(DataRow row, Image? img)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => row["Thumb"] = img));
            }
            else
            {
                row["Thumb"] = img;
            }
        }

        private static Image CreatePlaceholder(string text, int w = 180, int h = 150, Color? back = null)
        {
            var bg = back ?? Color.LightGray;
            var bmp = new Bitmap(w, h);
            using var g = Graphics.FromImage(bmp);
            g.Clear(bg);
            var font = new Font("Segoe UI", 14, FontStyle.Bold);
            var rect = new Rectangle(0, 0, w, h);
            TextRenderer.DrawText(g, text, font, rect, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            return bmp;
        }


        private void UpdateCartSummary()
        {
            decimal total = 0m;
            foreach (DataRow row in _cartTable.Rows)
            {
                var qty = Convert.ToInt32(row["Qty"], CultureInfo.InvariantCulture);
                var price = Convert.ToDecimal(row["UnitPrice"], CultureInfo.InvariantCulture);
                total += qty * price;
            }
            labelTotal.Text = $"Total: {total:C2}";
            labelItemCount.Text = $"Items: {_cartTable.Rows.Count}";
        }

        /* =========================
           UI: book cards
           ========================= */
        private async void RenderBooks()
        {
            flowLayoutPanelBooks.Controls.Clear();

            foreach (var book in _books)
            {
                var panel = new Panel
                {
                    Size = new Size(200, 280),
                    Margin = new Padding(10),
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Try to load image from DB
                PictureBox pictureBox = new PictureBox
                {
                    Size = new Size(180, 150),
                    Location = new Point(10, 10),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.WhiteSmoke
                };

                // placeholder until async load finishes
                pictureBox.Image = CreatePlaceholder("IMG");

                // Async load image
                // Async load image for this tile
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var bytes = await _controller.GetBookCoverBytesAsync(book.BookId);
                        if (bytes != null && bytes.Length > 0)
                        {
                            using var ms = new MemoryStream(bytes);
                            var img = Image.FromStream(ms);
                            pictureBox.Invoke(new Action(() => pictureBox.Image = img));
                        }
                        else
                        {
                            // No cover image → check if it has any PDF
                            var hasPdf = await _controller.BookHasPdfAsync(book.BookId);
                            pictureBox.Invoke(new Action(() =>
                                pictureBox.Image = hasPdf
                                    ? CreatePlaceholder("PDF", back: Color.Firebrick)
                                    : CreatePlaceholder("IMG")));
                        }
                    }
                    catch
                    {
                        pictureBox.Invoke(new Action(() =>
                            pictureBox.Image = CreatePlaceholder("ERR", back: Color.DarkGray)));
                    }
                });


                // Text labels
                var titleLabel = new Label
                {
                    Text = book.Name,
                    Location = new Point(10, 165),
                    Size = new Size(180, 20),
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoEllipsis = true
                };

                var authorLabel = new Label
                {
                    Text = book.Author,
                    Location = new Point(10, 185),
                    Size = new Size(180, 20),
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoEllipsis = true
                };

                var priceLabel = new Label
                {
                    Text = $"{book.Price:C2}",
                    Location = new Point(10, 205),
                    Size = new Size(180, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var addButton = new Button
                {
                    Text = "Add to Cart",
                    Location = new Point(10, 230),
                    Size = new Size(180, 30)
                };
                addButton.Click += (_, __) => AddToCart(book);

                panel.Controls.AddRange(new Control[]
                {
            pictureBox, titleLabel, authorLabel, priceLabel, addButton
                });

                flowLayoutPanelBooks.Controls.Add(panel);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var adminFrm = new AdminFrm();
            adminFrm.Show();
            this.Hide();
        }

        private sealed class BookItem
        {
            public int BookId { get; set; }
            public string Name { get; set; } = "";
            public string Author { get; set; } = "";
            public decimal Price { get; set; }
        }

        private void BtnSale(object sender, EventArgs e)
        {
            if (_cartTable.Rows.Count == 0)
            {
                MessageBox.Show("Cart is empty. Add items to proceed with sale.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                decimal totalAmount = 0m;
                foreach (DataRow row in _cartTable.Rows)
                {
                    var qty = Convert.ToInt32(row["Qty"], CultureInfo.InvariantCulture);
                    var price = Convert.ToDecimal(row["UnitPrice"], CultureInfo.InvariantCulture);
                    totalAmount += qty * price;
                }

                int saleId = _salesController.CreateSale(DateTime.UtcNow, totalAmount);

                foreach (DataRow row in _cartTable.Rows)
                {
                    int bookId = (int)row["BookId"];
                    int qty = Convert.ToInt32(row["Qty"], CultureInfo.InvariantCulture);
                    decimal unitPrice = Convert.ToDecimal(row["UnitPrice"], CultureInfo.InvariantCulture);

                    _salesController.CreateSaleItem(saleId, bookId, qty, unitPrice);
                }

                _cartTable.Clear();
                UpdateCartSummary();

                MessageBox.Show($"Sale completed! Sale ID: {saleId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during sale:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
