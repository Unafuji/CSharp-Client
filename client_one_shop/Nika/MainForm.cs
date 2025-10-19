
using System.Data;
using System.Globalization;
using client_one_shop.Nika.Controllers;
using client_one_shop.Nika.Models;

namespace client_one_shop.Nika
{
    public partial class MainForm : Form
    {
        private readonly BookShopController _controller;
        private List<BookItem> _books = new();
        private DataTable _cartTable = default!;

        public MainForm()
        {
            InitializeComponent();
            _controller = new BookShopController();
            this.Load += async (_, __) =>
            {
                InitializeCart();
                await LoadBooksAsync();
                RenderBooks();
                UpdateCartSummary();
            };
        }

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

        private void InitializeCart()
        {
            _cartTable = new DataTable();
            _cartTable.Columns.Add("BookId", typeof(int));
            _cartTable.Columns.Add("Title", typeof(string));
            _cartTable.Columns.Add("Author", typeof(string));
            _cartTable.Columns.Add("Qty", typeof(int));
            _cartTable.Columns.Add("UnitPrice", typeof(decimal));
            _cartTable.Columns.Add("LineTotal", typeof(decimal), "Qty * UnitPrice");

            dataGridViewCart.DataSource = _cartTable;
            dataGridViewCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Recompute totals when user edits Qty
            dataGridViewCart.CellValueChanged += (_, e) =>
            {
                if (e.RowIndex >= 0 && dataGridViewCart.Columns[e.ColumnIndex].Name == "Qty")
                    UpdateCartSummary();
            };
            dataGridViewCart.RowsRemoved += (_, __) => UpdateCartSummary();
        }

        private void AddToCart(BookItem book)
        {
            foreach (DataRow row in _cartTable.Rows)
            {
                if ((int)row["BookId"] == book.BookId)
                {
                    row["Qty"] = Convert.ToInt32(row["Qty"], CultureInfo.InvariantCulture) + 1;
                    UpdateCartSummary();
                    return;
                }
            }
            _cartTable.Rows.Add(book.BookId, book.Name, book.Author, 1, book.Price, book.Price);
            UpdateCartSummary();
        }

        private void UpdateCartSummary()
        {
            decimal total = 0m;
            foreach (DataRow row in _cartTable.Rows)
            {
                var qty = Convert.ToInt32(row["Qty"], CultureInfo.InvariantCulture);
                var price = Convert.ToDecimal(row["UnitPrice"], CultureInfo.InvariantCulture);
                row["LineTotal"] = qty * price;
                total += qty * price;
            }
            labelTotal.Text = $"Total: {total:C2}";
            labelItemCount.Text = $"Items: {_cartTable.Rows.Count}";
        }

        private void RenderBooks()
        {
            flowLayoutPanelBooks.Controls.Clear();

            foreach (var book in _books)
            {
                var panel = new Panel
                {
                    Size = new Size(200, 270),
                    Margin = new Padding(10),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var pictureBox = new PictureBox
                {
                    Size = new Size(180, 150),
                    Location = new Point(10, 10),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.WhiteSmoke
                };
                using (var bmp = new Bitmap(1, 1)) { }
                pictureBox.Paint += (_, e) =>
                {
                    var rect = new Rectangle(0, 0, pictureBox.Width - 1, pictureBox.Height - 1);
                    e.Graphics.DrawRectangle(Pens.LightGray, rect);
                    TextRenderer.DrawText(e.Graphics, "No Image", pictureBox.Font,
                        new Rectangle(0, 0, pictureBox.Width, pictureBox.Height),
                        SystemColors.GrayText, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                };

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
                    Size = new Size(180, 28),
                    Tag = book
                };
                addButton.Click += (_, __) => AddToCart(book);

                panel.Controls.AddRange(new Control[] { pictureBox, titleLabel, authorLabel, priceLabel, addButton });
                flowLayoutPanelBooks.Controls.Add(panel);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var adminFrm = new AdminFrm();
            adminFrm.Show();
            this.Hide();
        }
    }
}
