using client_one_shop.Connections;
using client_one_shop.Controllers;
using client_one_shop.Controls;
using client_one_shop.Models;
using Microsoft.Data.SqlClient; 
using System.Data;
using System.Globalization; 

namespace client_one_shop
{
    public partial class Form1 : Form
    {
        private const int ProductCardHeight = 170;
        private readonly Dictionary<string, Image> _imageCache = new(StringComparer.OrdinalIgnoreCase);
        private Image? _placeholder;

        private const decimal TaxRate = 0.00m;
        private decimal OrderLevelDiscount = 0.00m;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.btnPay.Click += btnPay_Click;

            // keep named handlers; designer-friendly
            this.productsFlow.Resize += productsFlow_Resize;
            this.productsFlow.ControlAdded += productsFlow_ControlAdded;
            this.actionsPanel.Resize += actionsPanel_Resize;

            _placeholder = BuildPlaceholder();
        }
        private DataTable BuildItemsTvp()
        {
            var tvp = new DataTable();
            tvp.Columns.Add("ProductID", typeof(int));
            tvp.Columns.Add("Qty", typeof(int));
            tvp.Columns.Add("UnitPrice", typeof(decimal));

            foreach (ListViewItem row in cartList.Items)
            {
                if (row.Tag is int productId == false) continue;
                if (!int.TryParse(row.SubItems[1].Text, out var qty) || qty <= 0) continue;
                 
                if (!decimal.TryParse(row.SubItems[2].Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out var unitPrice))
                    continue;

                tvp.Rows.Add(productId, qty, unitPrice);
            }

            return tvp;
        }
        private async void Form1_Load(object? sender, EventArgs e)
        { 
            this.txtSubtotal.Text = "0.00";
            this.txtDiscount.Text = "0.00";
            this.txtTax.Text = "0.00";
            this.txtGrand.Text = "0.00";

            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var products = await Task.Run(() => RetrievalData.GetProducts());
                BindProductsGrid(products);
                await LoadAllTileImagesLazy(products);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load products: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindProductsGrid(List<Product> products)
        {
            productsFlow.SuspendLayout();
            productsFlow.Controls.Clear();

            foreach (var p in products)
            {
                var tile = new ProductCard(p, _placeholder);
                tile.AddRequested += (_, prod) => AddToCart(prod);
                productsFlow.Controls.Add(tile);
            }

            productsFlow.ResumeLayout();
            EnforceFourPerRow();
        }
        // Form1.cs
        private async Task LoadAllTileImagesLazy(List<Product> products)
        {
            var sem = new SemaphoreSlim(8);  // decode at most 8 images at a time
            var tasks = new List<Task>();

            foreach (Control c in productsFlow.Controls)
            {
                if (c is ProductCard tile && tile.Product.ImgBytes is { Length: > 0 })
                {
                    await sem.WaitAsync();
                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            using var ms = new MemoryStream(tile.Product.ImgBytes);
                            using var src = Image.FromStream(ms);
                            var clone = (Image)src.Clone();

                            if (tile.IsHandleCreated && !tile.IsDisposed)
                                tile.BeginInvoke(new Action(() => tile.SetImage(clone)));
                        }
                        finally { sem.Release(); }
                    }));
                }
            }

            await Task.WhenAll(tasks);
        }


        private static string FormatMoney(decimal amount)
            => amount.ToString("N2", CultureInfo.CurrentCulture);

        private static bool TryParseMoney(string s, out decimal value)
            => decimal.TryParse(s, NumberStyles.Currency, CultureInfo.CurrentCulture, out value);

        private void AddToCart(Product p)
        {
            var existing = cartList.Items
                .Cast<ListViewItem>()
                .FirstOrDefault(i => (int)i.Tag == p.ProductID);

            if (existing != null)
            {
                if (int.TryParse(existing.SubItems[1].Text, out var qty))
                {
                    qty++;
                    existing.SubItems[1].Text = qty.ToString();
                    existing.SubItems[3].Text = FormatMoney(qty * p.Price);
                }
            }
            else
            {
                var lvi = new ListViewItem(p.Name) { Tag = p.ProductID };
                lvi.SubItems.Add("1");
                lvi.SubItems.Add(FormatMoney(p.Price));
                lvi.SubItems.Add(FormatMoney(p.Price));
                cartList.Items.Add(lvi);
            }

            RecalculateTotals();
        }

        private void RecalculateTotals()
        {
            decimal subtotal = 0m;

            foreach (ListViewItem row in cartList.Items)
            {
                if (!int.TryParse(row.SubItems[1].Text, out var qty)) continue;
                if (!TryParseMoney(row.SubItems[2].Text, out var price)) continue;

                subtotal += price * qty;
                row.SubItems[3].Text = FormatMoney(price * qty);
            }

            var discount = OrderLevelDiscount;
            var taxable = Math.Max(0, subtotal - discount);
            var tax = taxable * TaxRate;
            var grand = taxable + tax;

            txtSubtotal.Text = FormatMoney(subtotal);
            txtDiscount.Text = FormatMoney(discount);
            txtTax.Text = FormatMoney(tax);
            txtGrand.Text = FormatMoney(grand);
        }
         
        private void productsFlow_Resize(object? sender, EventArgs e) => EnforceFourPerRow();

        private void productsFlow_ControlAdded(object? sender, ControlEventArgs e)
        {
            if (e.Control is ProductCard card)
            {
                card.Height = ProductCardHeight; 
            }
        }

        private void EnforceFourPerRow()
        {
            var available = productsFlow.ClientSize.Width - productsFlow.Padding.Horizontal;
            if (available <= 0) return;

            var totalMargins = 4 * 20; 
            var w = (available - totalMargins) / 4;
            w = Math.Max(130, Math.Min(w, 220));

            foreach (Control c in productsFlow.Controls)
                c.Width = w;
        }

        private void actionsPanel_Resize(object? sender, EventArgs e)
        {
            btnPay.Left = actionsPanel.Width - btnPay.Width - 10;
            btnPay.Top = (actionsPanel.Height - btnPay.Height) / 2;

            btnClear.Left = btnPay.Left - btnClear.Width - 10;
            btnClear.Top = btnPay.Top;
        }

        private static Image BuildPlaceholder()
        {
            var bmp = new Bitmap(120, 90);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Gainsboro);
            using var pen = new Pen(Color.Silver);
            g.DrawRectangle(pen, 0, 0, bmp.Width - 1, bmp.Height - 1);
            using var font = new Font("Segoe UI", 8, FontStyle.Regular);
            var text = "No Image";
            var size = g.MeasureString(text, font);
            g.DrawString(text, font, Brushes.DimGray,
                (bmp.Width - size.Width) / 2, (bmp.Height - size.Height) / 2);
            return bmp;
        }

        private async void btnPay_Click(object? sender, EventArgs e)
        {
            if (cartList.Items.Count == 0)
            {
                MessageBox.Show("Cart is empty.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var itemsTvp = BuildItemsTvp();
            if (itemsTvp.Rows.Count == 0)
            {
                MessageBox.Show("No valid items in cart.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtGrand.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out var saleAmount))
            {
                MessageBox.Show("Invalid total.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var saleId = await InsertSaleAsync(
                    orderId: null,
                    saleDate: DateTime.Now,
                    paymentMethod: "Cash",   // TODO: bind from UI
                    saleAmount: saleAmount,
                    saleStatus: "Paid",      // business rule choice
                    salesPerson: Environment.UserName,
                    itemsTvp: itemsTvp);

                MessageBox.Show($"Payment successful. SaleID = {saleId}", "POS",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                cartList.Items.Clear();
                txtSubtotal.Text = txtDiscount.Text = txtTax.Text = txtGrand.Text = "0.00";

                await LoadProductsAsync();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Payment failed: {ex.Message}", "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<int> InsertSaleAsync(
            int? orderId,
            DateTime saleDate,
            string? paymentMethod,
            decimal saleAmount,
            string saleStatus,
            string? salesPerson,
            DataTable itemsTvp)
        {
            using var conn = new SqlConnection(ConnectionString.connectionString);
            using var cmd = new SqlCommand("dbo.InsertSale", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int) { Value = (object?)orderId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@SaleDate", SqlDbType.DateTime) { Value = saleDate });
            cmd.Parameters.Add(new SqlParameter("@PaymentMethod", SqlDbType.NVarChar, 50) { Value = (object?)paymentMethod ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@SaleAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = saleAmount });
            cmd.Parameters.Add(new SqlParameter("@SaleStatus", SqlDbType.NVarChar, 50) { Value = saleStatus });
            cmd.Parameters.Add(new SqlParameter("@SalesPerson", SqlDbType.NVarChar, 100) { Value = (object?)salesPerson ?? DBNull.Value });

            var itemsParam = cmd.Parameters.Add(new SqlParameter("@Items", SqlDbType.Structured)
            {
                TypeName = "dbo.SaleItemType",
                Value = itemsTvp
            });

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

    }
}
