using client_one_shop.Models;

namespace client_one_shop.Controls
{
    public sealed class ProductCard : UserControl
    {
        private readonly PictureBox _pic;
        private readonly Label _name;
        private readonly Label _price;

        public Product Product { get; }
        public event EventHandler<Product>? AddRequested;
 
     
        public ProductCard(Product product, Image? placeholder = null)
        {
            Product = product ?? throw new ArgumentNullException(nameof(product));
            this.Width = 160;
            this.Height = 170;
            this.Margin = new Padding(10);
            this.BackColor = SystemColors.ControlLightLight;

            _pic = new PictureBox
            {
                Dock = DockStyle.Top,
                Height = 110,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = placeholder
            };

            _name = new Label
            {
                Dock = DockStyle.Top,
                Height = 32,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = product.Name
            };

            _price = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Text = product.Price.ToString("N2")
            };

            var bottom = new Panel { Dock = DockStyle.Fill, Padding = new Padding(2, 2, 2, 2) };
            bottom.Controls.Add(_price);
            bottom.Controls.Add(_name);

            this.Controls.Add(bottom);
            this.Controls.Add(_pic);

            // clicking anywhere triggers AddRequested
            this.Click += (_, __) => AddRequested?.Invoke(this, Product);
            foreach (Control c in this.Controls)
                c.Click += (_, __) => AddRequested?.Invoke(this, Product);
        }
        public void SetImage(Image img)
        {
            if (img == null) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => _pic.Image = img));
            }
            else
            {
                _pic.Image = img;
            }
        }

        public async Task LoadImageAsync()
        {
            if (Product.ImgBytes == null || Product.ImgBytes.Length == 0)
                return;

            try
            {
                using var ms = new MemoryStream(Product.ImgBytes);
                var img = Image.FromStream(ms);
                if (this.IsHandleCreated && !this.IsDisposed)
                    this.BeginInvoke(new Action(() => _pic.Image = (Image)img.Clone()));
            }
            catch
            {
                // fallback silently
            }
        }


        public static async Task<Image?> DefaultLoader(string pathOrUrl)
        {
            if (Uri.TryCreate(pathOrUrl, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                var bytes = await http.GetByteArrayAsync(uri);
                using var ms = new MemoryStream(bytes);
                return Image.FromStream(ms);
            } 
            if (File.Exists(pathOrUrl))
            { 
                using var fs = new FileStream(pathOrUrl, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var img = Image.FromStream(fs);
                return (Image)img.Clone();
            }
            return null;
        }
    }
}
