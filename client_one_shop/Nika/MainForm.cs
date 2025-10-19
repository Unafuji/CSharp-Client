using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data; 

namespace client_one_shop.Nika
{
    public partial class MainForm : Form
    {
        private List<Book> books;
        private DataTable cartTable;

        public MainForm()
        {
            InitializeComponent();
            InitializeBooks();
            InitializeCart();
            DisplayBooks();
            UpdateCartSummary();
        }

        private void InitializeBooks()
        {
            books = new List<Book>
            {
                new Book("The Great Gatsby", "F. Scott Fitzgerald", 9.99m),
                new Book("1984", "George Orwell", 12.99m),
                new Book("To Kill a Mockingbird", "Harper Lee", 11.99m),
                new Book("Pride and Prejudice", "Jane Austen", 8.99m),
                new Book("The Catcher in the Rye", "J.D. Salinger", 10.99m),
                new Book("Lord of the Rings", "J.R.R. Tolkien", 15.99m)
            };
        }

        private void InitializeCart()
        {
            cartTable = new DataTable();
            cartTable.Columns.Add("Title", typeof(string));
            cartTable.Columns.Add("Author", typeof(string));
            cartTable.Columns.Add("Price", typeof(decimal));
            dataGridViewCart.DataSource = cartTable;
        }

        private void DisplayBooks()
        {
            flowLayoutPanelBooks.Controls.Clear();
            foreach (var book in books)
            {
                var panel = new Panel
                {
                    Size = new Size(200, 250),
                    Margin = new Padding(10),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var pictureBox = new PictureBox
                {
                    Size = new Size(180, 150),
                    Location = new Point(10, 10),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };

                // Load image from project resources
                try
                {
                    //pictureBox.Image = Properties.Resources.book_placeholder; // Ensure book_placeholder is added to resources
                }
                catch
                {
                    // Fallback if resource is missing
                    pictureBox.BackColor = Color.LightGray;
                    pictureBox.Text = "No Image";
                }

                var titleLabel = new Label
                {
                    Text = book.Title,
                    Location = new Point(10, 170),
                    Size = new Size(180, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var authorLabel = new Label
                {
                    Text = book.Author,
                    Location = new Point(10, 190),
                    Size = new Size(180, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var priceLabel = new Label
                {
                    Text = $"${book.Price:F2}",
                    Location = new Point(10, 210),
                    Size = new Size(180, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var addButton = new Button
                {
                    Text = "Add to Cart",
                    Location = new Point(10, 230),
                    Size = new Size(180, 20),
                    Tag = book
                };
                addButton.Click += AddToCartButton_Click;

                panel.Controls.AddRange(new Control[] { pictureBox, titleLabel, authorLabel, priceLabel, addButton });
                flowLayoutPanelBooks.Controls.Add(panel);
            }
        }

        private void AddToCartButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            var book = button.Tag as Book;
            cartTable.Rows.Add(book.Title, book.Author, book.Price);
            UpdateCartSummary();
        }

        private void UpdateCartSummary()
        {
            decimal total = cartTable.AsEnumerable().Sum(row => row.Field<decimal>("Price"));
            labelTotal.Text = $"Total: ${total:F2}";
            labelItemCount.Text = $"Items: {cartTable.Rows.Count}";
        }

        private class Book
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public decimal Price { get; set; }

            public Book(string title, string author, decimal price)
            {
                Title = title;
                Author = author;
                Price = price;
            }
        }

        private void labelItemCount_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminFrm adminFrm = new AdminFrm();
            adminFrm.Show();
            this.Hide();    
        }
    }
}