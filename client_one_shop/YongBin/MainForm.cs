using client_one_shop.YongBin.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_one_shop.YongBin
{
    public partial class MainForm : Form
    {

        private ProductController _productController = new ProductController();
        private int? selectedProductId = null;
        public MainForm()
        {
            InitializeComponent();
            SetupDataGridView2();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            dataGridView1.CellClick += dataGridView1_CellClick;

            LoadProducts();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            try
            {
                txtProductName.Text = row.Cells["ProductName"].Value?.ToString() ?? "";
                txtDescription.Text = row.Cells["Description"].Value?.ToString() ?? "";
                txtPrice.Text = row.Cells["Price"].Value?.ToString() ?? "";
                txtQuantity.Text = row.Cells["Quantity"].Value?.ToString() ?? "";

                selectedProductId = Convert.ToInt32(row.Cells["ProductID"].Value);

                // Load image from database
                DataTable imageTable = _productController.GetProductImages(selectedProductId.Value);

                if (imageTable.Rows.Count > 0)
                {
                    byte[] imageData = imageTable.Rows[0]["ImageData"] as byte[];
                    if (imageData != null && imageData.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            pictureBox1.Image?.Dispose();
                            pictureBox1.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pictureBox1.Image = null;
                    }
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data from row: " + ex.Message);
            }
        }


        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lblImagePath.Text = openFileDialog.FileName;
                     
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }


        private void BtnUpdate(object sender, EventArgs e)
        {
            if (selectedProductId == null)
            {
                MessageBox.Show("Please select a product to update.");
                return;
            }

            try
            {
                string name = txtProductName.Text;
                string description = txtDescription.Text;
                decimal price = decimal.Parse(txtPrice.Text);
                int quantity = int.Parse(txtQuantity.Text);

                bool isSuccess = _productController.UpdateProduct(
                    selectedProductId.Value, name, description, price, quantity
                );

                if (isSuccess)
                {
                    MessageBox.Show("Product updated successfully.");
                    LoadProducts();
                    BtnClearAllTextBox(sender, e);
                }
                else
                {
                    MessageBox.Show("Failed to update product.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtProductName.Text;
                string description = txtDescription.Text;
                decimal price = decimal.Parse(txtPrice.Text);
                int quantity = int.Parse(txtQuantity.Text);

                dataGridView2.Rows.Add(name, description, price, quantity);

                BtnClearAllTextBox(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input: " + ex.Message);
            }
        }

        private void SetupDataGridView2()
        {
            dataGridView2.Columns.Clear();

            dataGridView2.Columns.Add("ProductName", "Product Name");
            dataGridView2.Columns.Add("Description", "Description");
            dataGridView2.Columns.Add("Price", "Price");
            dataGridView2.Columns.Add("Quantity", "Quantity");

        }


        private void BtnCreate(object sender, EventArgs e)
        {
            try
            { 
                if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                    string.IsNullOrWhiteSpace(txtDescription.Text) ||
                    string.IsNullOrWhiteSpace(txtPrice.Text) ||
                    string.IsNullOrWhiteSpace(txtQuantity.Text))
                {
                    MessageBox.Show("Please fill in all product fields.");
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    MessageBox.Show("Invalid price.");
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity))
                {
                    MessageBox.Show("Invalid quantity.");
                    return;
                }

                if (string.IsNullOrEmpty(lblImagePath.Text) || !File.Exists(lblImagePath.Text))
                {
                    MessageBox.Show("Please browse and select a valid image file before adding the product.");
                    return;
                }

                string name = txtProductName.Text.Trim();
                string description = txtDescription.Text.Trim();
                string imagePath = lblImagePath.Text;

                // 1. Create product
                bool isSuccess = _productController.AddProduct(name, description, price, quantity);

                if (!isSuccess)
                {
                    MessageBox.Show("Failed to add product.");
                    return;
                }
                 
                var dt = _productController.GetProduct();
                DataRow productRow = dt.AsEnumerable()
                                       .FirstOrDefault(r => r.Field<string>("ProductName") == name);

                if (productRow == null)
                {
                    MessageBox.Show("Product added but could not retrieve ID.");
                    return;
                }

                int productId = productRow.Field<int>("ProductID");
                 
                byte[] imageData = File.ReadAllBytes(imagePath);
                string caption = name + " image";
                 
                bool imageSuccess = _productController.AddProductImage(productId, imageData, caption);

                if (!imageSuccess)
                {
                    MessageBox.Show("Product added but failed to upload image to database.");
                }

                MessageBox.Show("Product and image added successfully.");
                LoadProducts();
                BtnClearAllTextBox(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void BtnClearAllTextBox(object sender, EventArgs e)
        {
            onClear();
        }
        public void onClear()
        {
            txtProductName.Clear();
            txtDescription.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
            lblImagePath.Text = string.Empty;
            pictureBox1.Image = null;
            selectedProductId = null;
        }


        private void BtnDelete(object sender, EventArgs e)
        {
            if (selectedProductId == null)
            {
                MessageBox.Show("Please select a product to delete.");
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this product?", "Confirm", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                bool isSuccess = _productController.DeleteProduct(selectedProductId.Value);

                if (isSuccess)
                {
                    MessageBox.Show("Product deleted successfully.");
                    LoadProducts();
                    BtnClearAllTextBox(sender, e);
                }
                else
                {
                    MessageBox.Show("Failed to delete product.");
                }
            }
        }
        private void LoadProducts()
        {
            dataGridView1.DataSource = _productController.GetProduct();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoResizeColumns();
        }


        private void BtnMultiInsert(object sender, EventArgs e)
        {
            var controller = new ProductController();
            try
            {
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.IsNewRow) continue;

                    string name = row.Cells["ProductName"].Value?.ToString();
                    string description = row.Cells["Description"].Value?.ToString();
                    string priceText = row.Cells["Price"].Value?.ToString();
                    string quantityText = row.Cells["Quantity"].Value?.ToString();

                    if (string.IsNullOrWhiteSpace(name) ||
                        string.IsNullOrWhiteSpace(description) ||
                        !decimal.TryParse(priceText, out decimal price) ||
                        !int.TryParse(quantityText, out int quantity))
                    {
                        MessageBox.Show("Invalid data in one of the rows. Please check and try again.");
                        return;
                    }

                    bool success = controller.AddProduct(name, description, price, quantity);
                    if (!success)
                    {
                        MessageBox.Show($"Failed to insert product: {name}");
                        return;
                    }
                }

                MessageBox.Show("All products inserted successfully.");
                dataGridView2.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during insert: " + ex.Message);
            }
        }

    }
}
