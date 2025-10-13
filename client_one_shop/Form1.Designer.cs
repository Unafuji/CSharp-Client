namespace client_one_shop
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            mainLayout = new TableLayoutPanel();
            leftProductsHost = new Panel();
            productsFlow = new FlowLayoutPanel();
            rightLayout = new TableLayoutPanel();
            cartList = new ListView();
            colName = new ColumnHeader();
            colQty = new ColumnHeader();
            colPrice = new ColumnHeader();
            colLineTotal = new ColumnHeader();
            rightBottomPanel = new Panel();
            actionsPanel = new Panel();
            btnClear = new Button();
            btnPay = new Button();
            totalsLayout = new TableLayoutPanel();
            lblSubtotal = new Label();
            txtSubtotal = new TextBox();
            lblDiscount = new Label();
            txtDiscount = new TextBox();
            lblTax = new Label();
            txtTax = new TextBox();
            lblGrand = new Label();
            txtGrand = new TextBox();
            mainLayout.SuspendLayout();
            leftProductsHost.SuspendLayout();
            rightLayout.SuspendLayout();
            rightBottomPanel.SuspendLayout();
            actionsPanel.SuspendLayout();
            totalsLayout.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainLayout.Controls.Add(leftProductsHost, 0, 0);
            mainLayout.Controls.Add(rightLayout, 1, 0);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 0);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 1;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Size = new Size(1200, 800);
            mainLayout.TabIndex = 0;
            // 
            // leftProductsHost
            // 
            leftProductsHost.Controls.Add(productsFlow);
            leftProductsHost.Dock = DockStyle.Fill;
            leftProductsHost.Location = new Point(3, 3);
            leftProductsHost.Name = "leftProductsHost";
            leftProductsHost.Size = new Size(714, 794);
            leftProductsHost.TabIndex = 0;
            // 
            // productsFlow
            // 
            productsFlow.AutoScroll = true;
            productsFlow.Dock = DockStyle.Fill;
            productsFlow.Location = new Point(0, 0);
            productsFlow.Margin = new Padding(0);
            productsFlow.Name = "productsFlow";
            productsFlow.Padding = new Padding(10);
            productsFlow.Size = new Size(714, 794);
            productsFlow.TabIndex = 0;
            // 
            // rightLayout
            // 
            rightLayout.ColumnCount = 1;
            rightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rightLayout.Controls.Add(cartList, 0, 0);
            rightLayout.Controls.Add(rightBottomPanel, 0, 1);
            rightLayout.Dock = DockStyle.Fill;
            rightLayout.Location = new Point(723, 3);
            rightLayout.Name = "rightLayout";
            rightLayout.RowCount = 2;
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
            rightLayout.Size = new Size(474, 794);
            rightLayout.TabIndex = 1;
            // 
            // cartList
            // 
            cartList.Columns.AddRange(new ColumnHeader[] { colName, colQty, colPrice, colLineTotal });
            cartList.Dock = DockStyle.Fill;
            cartList.FullRowSelect = true;
            cartList.GridLines = true;
            cartList.Location = new Point(3, 3);
            cartList.Name = "cartList";
            cartList.Size = new Size(468, 510);
            cartList.TabIndex = 0;
            cartList.UseCompatibleStateImageBehavior = false;
            cartList.View = View.Details;
            // 
            // colName
            // 
            colName.Text = "Product";
            colName.Width = 220;
            // 
            // colQty
            // 
            colQty.Text = "Qty";
            // 
            // colPrice
            // 
            colPrice.Text = "Price";
            colPrice.Width = 90;
            // 
            // colLineTotal
            // 
            colLineTotal.Text = "Total";
            colLineTotal.Width = 110;
            // 
            // rightBottomPanel
            // 
            rightBottomPanel.Controls.Add(actionsPanel);
            rightBottomPanel.Controls.Add(totalsLayout);
            rightBottomPanel.Dock = DockStyle.Fill;
            rightBottomPanel.Location = new Point(3, 519);
            rightBottomPanel.Name = "rightBottomPanel";
            rightBottomPanel.Size = new Size(468, 272);
            rightBottomPanel.TabIndex = 1;
            // 
            // actionsPanel
            // 
            actionsPanel.Controls.Add(btnClear);
            actionsPanel.Controls.Add(btnPay);
            actionsPanel.Dock = DockStyle.Bottom;
            actionsPanel.Location = new Point(0, 212);
            actionsPanel.Name = "actionsPanel";
            actionsPanel.Padding = new Padding(10);
            actionsPanel.Size = new Size(468, 60);
            actionsPanel.TabIndex = 0;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Right;
            btnClear.Location = new Point(278, 10);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(100, 36);
            btnClear.TabIndex = 0;
            btnClear.Text = "Clear";
            // 
            // btnPay
            // 
            btnPay.Anchor = AnchorStyles.Right;
            btnPay.Location = new Point(388, 10);
            btnPay.Name = "btnPay";
            btnPay.Size = new Size(100, 36);
            btnPay.TabIndex = 1;
            btnPay.Text = "Pay";
            btnPay.Click += btnPay_Click;
            // 
            // totalsLayout
            // 
            totalsLayout.ColumnCount = 4;
            totalsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            totalsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            totalsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            totalsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            totalsLayout.Controls.Add(lblSubtotal, 0, 0);
            totalsLayout.Controls.Add(txtSubtotal, 1, 0);
            totalsLayout.Controls.Add(lblDiscount, 2, 0);
            totalsLayout.Controls.Add(txtDiscount, 3, 0);
            totalsLayout.Controls.Add(lblTax, 0, 1);
            totalsLayout.Controls.Add(txtTax, 1, 1);
            totalsLayout.Controls.Add(lblGrand, 2, 1);
            totalsLayout.Controls.Add(txtGrand, 3, 1);
            totalsLayout.Dock = DockStyle.Top;
            totalsLayout.Location = new Point(0, 0);
            totalsLayout.Name = "totalsLayout";
            totalsLayout.Padding = new Padding(10);
            totalsLayout.RowCount = 3;
            totalsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            totalsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            totalsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            totalsLayout.Size = new Size(468, 100);
            totalsLayout.TabIndex = 1;
            // 
            // lblSubtotal
            // 
            lblSubtotal.Location = new Point(13, 10);
            lblSubtotal.Name = "lblSubtotal";
            lblSubtotal.Size = new Size(83, 23);
            lblSubtotal.TabIndex = 0;
            lblSubtotal.Text = "Subtotal:";
            lblSubtotal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtSubtotal
            // 
            txtSubtotal.Dock = DockStyle.Fill;
            txtSubtotal.Location = new Point(102, 13);
            txtSubtotal.Name = "txtSubtotal";
            txtSubtotal.ReadOnly = true;
            txtSubtotal.Size = new Size(128, 23);
            txtSubtotal.TabIndex = 1;
            txtSubtotal.TextAlign = HorizontalAlignment.Right;
            // 
            // lblDiscount
            // 
            lblDiscount.Location = new Point(236, 10);
            lblDiscount.Name = "lblDiscount";
            lblDiscount.Size = new Size(83, 23);
            lblDiscount.TabIndex = 2;
            lblDiscount.Text = "Discount:";
            lblDiscount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtDiscount
            // 
            txtDiscount.Dock = DockStyle.Fill;
            txtDiscount.Location = new Point(325, 13);
            txtDiscount.Name = "txtDiscount";
            txtDiscount.ReadOnly = true;
            txtDiscount.Size = new Size(130, 23);
            txtDiscount.TabIndex = 3;
            txtDiscount.TextAlign = HorizontalAlignment.Right;
            // 
            // lblTax
            // 
            lblTax.Location = new Point(13, 50);
            lblTax.Name = "lblTax";
            lblTax.Size = new Size(83, 23);
            lblTax.TabIndex = 4;
            lblTax.Text = "Tax:";
            lblTax.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtTax
            // 
            txtTax.Dock = DockStyle.Fill;
            txtTax.Location = new Point(102, 53);
            txtTax.Name = "txtTax";
            txtTax.ReadOnly = true;
            txtTax.Size = new Size(128, 23);
            txtTax.TabIndex = 5;
            txtTax.TextAlign = HorizontalAlignment.Right;
            // 
            // lblGrand
            // 
            lblGrand.Location = new Point(236, 50);
            lblGrand.Name = "lblGrand";
            lblGrand.Size = new Size(83, 23);
            lblGrand.TabIndex = 6;
            lblGrand.Text = "Grand Total:";
            lblGrand.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtGrand
            // 
            txtGrand.Dock = DockStyle.Fill;
            txtGrand.Location = new Point(325, 53);
            txtGrand.Name = "txtGrand";
            txtGrand.ReadOnly = true;
            txtGrand.Size = new Size(130, 23);
            txtGrand.TabIndex = 7;
            txtGrand.TextAlign = HorizontalAlignment.Right;
            // 
            // Form1
            // 
            ClientSize = new Size(1200, 800);
            Controls.Add(mainLayout);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "POS";
            mainLayout.ResumeLayout(false);
            leftProductsHost.ResumeLayout(false);
            rightLayout.ResumeLayout(false);
            rightBottomPanel.ResumeLayout(false);
            actionsPanel.ResumeLayout(false);
            totalsLayout.ResumeLayout(false);
            totalsLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.Panel leftProductsHost;
        private System.Windows.Forms.FlowLayoutPanel productsFlow;

        private System.Windows.Forms.TableLayoutPanel rightLayout;
        private System.Windows.Forms.ListView cartList;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colQty;
        private System.Windows.Forms.ColumnHeader colPrice;
        private System.Windows.Forms.ColumnHeader colLineTotal;

        private System.Windows.Forms.Panel rightBottomPanel;
        private System.Windows.Forms.TableLayoutPanel totalsLayout;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblDiscount;
        private System.Windows.Forms.Label lblTax;
        private System.Windows.Forms.Label lblGrand;
        private System.Windows.Forms.TextBox txtSubtotal;
        private System.Windows.Forms.TextBox txtDiscount;
        private System.Windows.Forms.TextBox txtTax;
        private System.Windows.Forms.TextBox txtGrand;

        private System.Windows.Forms.Panel actionsPanel;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnPay;
    }
}
