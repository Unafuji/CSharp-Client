namespace client_one_shop.Nika
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            flowLayoutPanelBooks = new FlowLayoutPanel();
            dataGridViewCart = new DataGridView();
            labelTotal = new Label();
            labelItemCount = new Label();
            label1 = new Label();
            button1 = new Button();
            button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewCart).BeginInit();
            SuspendLayout();
            // 
            // flowLayoutPanelBooks
            // 
            flowLayoutPanelBooks.AutoScroll = true;
            flowLayoutPanelBooks.Location = new Point(13, 107);
            flowLayoutPanelBooks.Margin = new Padding(4, 3, 4, 3);
            flowLayoutPanelBooks.Name = "flowLayoutPanelBooks";
            flowLayoutPanelBooks.Size = new Size(758, 492);
            flowLayoutPanelBooks.TabIndex = 0;
            // 
            // dataGridViewCart
            // 
            dataGridViewCart.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCart.Location = new Point(779, 107);
            dataGridViewCart.Margin = new Padding(4, 3, 4, 3);
            dataGridViewCart.Name = "dataGridViewCart";
            dataGridViewCart.Size = new Size(350, 404);
            dataGridViewCart.TabIndex = 1;
            // 
            // labelTotal
            // 
            labelTotal.AutoSize = true;
            labelTotal.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelTotal.Location = new Point(779, 522);
            labelTotal.Margin = new Padding(4, 0, 4, 0);
            labelTotal.Name = "labelTotal";
            labelTotal.Size = new Size(96, 21);
            labelTotal.TabIndex = 2;
            labelTotal.Text = "Total: $0.00";
            // 
            // labelItemCount
            // 
            labelItemCount.AutoSize = true;
            labelItemCount.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelItemCount.Location = new Point(779, 543);
            labelItemCount.Margin = new Padding(4, 0, 4, 0);
            labelItemCount.Name = "labelItemCount";
            labelItemCount.Size = new Size(69, 21);
            labelItemCount.TabIndex = 3;
            labelItemCount.Text = "Items: 0";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(236, 40);
            label1.TabIndex = 4;
            label1.Text = "Nika Book Shop";
            // 
            // button1
            // 
            button1.Location = new Point(1054, 78);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Admin";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(1054, 576);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 5;
            button2.Text = "Sale";
            button2.UseVisualStyleBackColor = true;
            button2.Click += BtnSale;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1159, 611);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(labelItemCount);
            Controls.Add(labelTotal);
            Controls.Add(dataGridViewCart);
            Controls.Add(flowLayoutPanelBooks);
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "Bookstore App";
            ((System.ComponentModel.ISupportInitialize)dataGridViewCart).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelBooks;
        private System.Windows.Forms.DataGridView dataGridViewCart;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Label labelItemCount;
        private Label label1;
        private Button button1;
        private Button button2;
    }
}