namespace client_one_shop.Nika
{
    partial class AdminFrm
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
            labelHeader = new Label();
            labelISBN13 = new Label();
            textBoxISBN13 = new TextBox();
            labelAuthorId = new Label();
            textBoxAuthorId = new TextBox();
            labelListPrice = new Label();
            textBoxListPrice = new TextBox();
            labelCostPrice = new Label();
            textBoxCostPrice = new TextBox();
            labelStock = new Label();
            textBoxStock = new TextBox();
            buttonCreate = new Button();
            buttonUpdate = new Button();
            buttonDelete = new Button();
            buttonClear = new Button();
            dataGridViewBooks = new DataGridView();
            buttonPOS = new Button();
            groupBox1 = new GroupBox();
            button5 = new Button();
            button4 = new Button();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            textBox1 = new TextBox();
            button3 = new Button();
            label3 = new Label();
            textBoxBookId = new TextBox();
            button2 = new Button();
            button1 = new Button();
            label2 = new Label();
            label1 = new Label();
            dataGridView1 = new DataGridView();
            groupBox2 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)dataGridViewBooks).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // labelHeader
            // 
            labelHeader.AutoSize = true;
            labelHeader.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelHeader.Location = new Point(12, 11);
            labelHeader.Name = "labelHeader";
            labelHeader.Size = new Size(148, 21);
            labelHeader.TabIndex = 0;
            labelHeader.Text = "Admin Dashboard";
            // 
            // labelISBN13
            // 
            labelISBN13.AutoSize = true;
            labelISBN13.Location = new Point(17, 55);
            labelISBN13.Name = "labelISBN13";
            labelISBN13.Size = new Size(42, 15);
            labelISBN13.TabIndex = 3;
            labelISBN13.Text = "Name ";
            // 
            // textBoxISBN13
            // 
            textBoxISBN13.Location = new Point(125, 52);
            textBoxISBN13.Name = "textBoxISBN13";
            textBoxISBN13.Size = new Size(230, 23);
            textBoxISBN13.TabIndex = 4;
            // 
            // labelAuthorId
            // 
            labelAuthorId.AutoSize = true;
            labelAuthorId.Location = new Point(18, 84);
            labelAuthorId.Name = "labelAuthorId";
            labelAuthorId.Size = new Size(47, 15);
            labelAuthorId.TabIndex = 7;
            labelAuthorId.Text = "Author ";
            // 
            // textBoxAuthorId
            // 
            textBoxAuthorId.Location = new Point(125, 81);
            textBoxAuthorId.Name = "textBoxAuthorId";
            textBoxAuthorId.Size = new Size(230, 23);
            textBoxAuthorId.TabIndex = 8;
            // 
            // labelListPrice
            // 
            labelListPrice.AutoSize = true;
            labelListPrice.Location = new Point(18, 113);
            labelListPrice.Name = "labelListPrice";
            labelListPrice.Size = new Size(36, 15);
            labelListPrice.TabIndex = 9;
            labelListPrice.Text = "Price ";
            // 
            // textBoxListPrice
            // 
            textBoxListPrice.Location = new Point(125, 110);
            textBoxListPrice.Name = "textBoxListPrice";
            textBoxListPrice.Size = new Size(230, 23);
            textBoxListPrice.TabIndex = 10;
            // 
            // labelCostPrice
            // 
            labelCostPrice.AutoSize = true;
            labelCostPrice.Location = new Point(19, 142);
            labelCostPrice.Name = "labelCostPrice";
            labelCostPrice.Size = new Size(35, 15);
            labelCostPrice.TabIndex = 11;
            labelCostPrice.Text = "ISBN ";
            // 
            // textBoxCostPrice
            // 
            textBoxCostPrice.Location = new Point(125, 139);
            textBoxCostPrice.Name = "textBoxCostPrice";
            textBoxCostPrice.Size = new Size(230, 23);
            textBoxCostPrice.TabIndex = 12;
            // 
            // labelStock
            // 
            labelStock.AutoSize = true;
            labelStock.Location = new Point(17, 171);
            labelStock.Name = "labelStock";
            labelStock.Size = new Size(83, 15);
            labelStock.TabIndex = 13;
            labelStock.Text = "PublishedDate";
            // 
            // textBoxStock
            // 
            textBoxStock.Location = new Point(125, 168);
            textBoxStock.Name = "textBoxStock";
            textBoxStock.Size = new Size(230, 23);
            textBoxStock.TabIndex = 14;
            // 
            // buttonCreate
            // 
            buttonCreate.Location = new Point(37, 380);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(75, 23);
            buttonCreate.TabIndex = 15;
            buttonCreate.Text = "Create";
            buttonCreate.UseVisualStyleBackColor = true;
            buttonCreate.Click += buttonCreate_Click;
            // 
            // buttonUpdate
            // 
            buttonUpdate.Location = new Point(118, 380);
            buttonUpdate.Name = "buttonUpdate";
            buttonUpdate.Size = new Size(75, 23);
            buttonUpdate.TabIndex = 16;
            buttonUpdate.Text = "Update";
            buttonUpdate.UseVisualStyleBackColor = true;
            buttonUpdate.Click += buttonUpdate_Click;
            // 
            // buttonDelete
            // 
            buttonDelete.Location = new Point(280, 380);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new Size(75, 23);
            buttonDelete.TabIndex = 17;
            buttonDelete.Text = "Delete";
            buttonDelete.UseVisualStyleBackColor = true;
            buttonDelete.Click += buttonDelete_Click;
            // 
            // buttonClear
            // 
            buttonClear.Location = new Point(199, 380);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new Size(75, 23);
            buttonClear.TabIndex = 18;
            buttonClear.Text = "Clear";
            buttonClear.UseVisualStyleBackColor = true;
            buttonClear.Click += BtnClearTextBox;
            // 
            // dataGridViewBooks
            // 
            dataGridViewBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewBooks.Location = new Point(379, 76);
            dataGridViewBooks.Name = "dataGridViewBooks";
            dataGridViewBooks.Size = new Size(685, 561);
            dataGridViewBooks.TabIndex = 19;
            dataGridViewBooks.CellClick += dataGridViewBooks_CellClick;
            dataGridViewBooks.SelectionChanged += dataGridViewBooks_SelectionChanged;
            // 
            // buttonPOS
            // 
            buttonPOS.Location = new Point(989, 12);
            buttonPOS.Name = "buttonPOS";
            buttonPOS.Size = new Size(75, 23);
            buttonPOS.TabIndex = 20;
            buttonPOS.Text = "POS";
            buttonPOS.UseVisualStyleBackColor = true;
            buttonPOS.Click += buttonPOS_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button4);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(textBoxBookId);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(labelISBN13);
            groupBox1.Controls.Add(buttonClear);
            groupBox1.Controls.Add(textBoxISBN13);
            groupBox1.Controls.Add(buttonDelete);
            groupBox1.Controls.Add(buttonUpdate);
            groupBox1.Controls.Add(buttonCreate);
            groupBox1.Controls.Add(labelAuthorId);
            groupBox1.Controls.Add(textBoxStock);
            groupBox1.Controls.Add(textBoxAuthorId);
            groupBox1.Controls.Add(labelStock);
            groupBox1.Controls.Add(labelListPrice);
            groupBox1.Controls.Add(textBoxCostPrice);
            groupBox1.Controls.Add(textBoxListPrice);
            groupBox1.Controls.Add(labelCostPrice);
            groupBox1.Location = new Point(12, 234);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(361, 409);
            groupBox1.TabIndex = 21;
            groupBox1.TabStop = false;
            groupBox1.Text = "Information";
            // 
            // button5
            // 
            button5.Location = new Point(989, 47);
            button5.Name = "button5";
            button5.Size = new Size(75, 23);
            button5.TabIndex = 32;
            button5.Text = "Sales";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button4
            // 
            button4.Location = new Point(125, 276);
            button4.Name = "button4";
            button4.Size = new Size(230, 23);
            button4.TabIndex = 31;
            button4.Text = "Browse Image";
            button4.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(125, 305);
            label6.Name = "label6";
            label6.Size = new Size(16, 15);
            label6.TabIndex = 30;
            label6.Text = "...";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(17, 305);
            label5.Name = "label5";
            label5.Size = new Size(73, 15);
            label5.TabIndex = 29;
            label5.Text = "Image Path: ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(17, 200);
            label4.Name = "label4";
            label4.Size = new Size(81, 15);
            label4.TabIndex = 28;
            label4.Text = "CreatedAtUtc ";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(125, 197);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(230, 23);
            textBox1.TabIndex = 27;
            // 
            // button3
            // 
            button3.Location = new Point(199, 351);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 26;
            button3.Text = "Add";
            button3.UseVisualStyleBackColor = true;
            button3.Click += BtnAdd;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(17, 26);
            label3.Name = "label3";
            label3.Size = new Size(48, 15);
            label3.TabIndex = 25;
            label3.Text = "BookID:";
            // 
            // textBoxBookId
            // 
            textBoxBookId.Location = new Point(125, 23);
            textBoxBookId.Name = "textBoxBookId";
            textBoxBookId.Size = new Size(230, 23);
            textBoxBookId.TabIndex = 24;
            // 
            // button2
            // 
            button2.Location = new Point(125, 322);
            button2.Name = "button2";
            button2.Size = new Size(230, 23);
            button2.TabIndex = 21;
            button2.Text = "Browse Pdf";
            button2.UseVisualStyleBackColor = true;
            button2.Click += BtnBrowsePDF;
            // 
            // button1
            // 
            button1.Location = new Point(280, 351);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 23;
            button1.Text = "->";
            button1.UseVisualStyleBackColor = true;
            button1.Click += BtnMultiInsert;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(125, 258);
            label2.Name = "label2";
            label2.Size = new Size(16, 15);
            label2.TabIndex = 20;
            label2.Text = "...";
            label2.Click += lbPdfPath;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 258);
            label1.Name = "label1";
            label1.Size = new Size(73, 15);
            label1.TabIndex = 19;
            label1.Text = "Image Path: ";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(6, 22);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(349, 149);
            dataGridView1.TabIndex = 22;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(dataGridView1);
            groupBox2.Location = new Point(12, 47);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(361, 181);
            groupBox2.TabIndex = 24;
            groupBox2.TabStop = false;
            groupBox2.Text = "groupBox2";
            // 
            // AdminFrm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1077, 655);
            Controls.Add(button5);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(buttonPOS);
            Controls.Add(dataGridViewBooks);
            Controls.Add(labelHeader);
            Name = "AdminFrm";
            Text = "AdminFrm";
            Load += AdminFrm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewBooks).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelHeader;
        private Label labelISBN13;
        private TextBox textBoxISBN13;
        private Label labelAuthorId;
        private TextBox textBoxAuthorId;
        private Label labelListPrice;
        private TextBox textBoxListPrice;
        private Label labelCostPrice;
        private TextBox textBoxCostPrice;
        private Label labelStock;
        private TextBox textBoxStock;
        private Button buttonCreate;
        private Button buttonUpdate;
        private Button buttonDelete;
        private Button buttonClear;
        private DataGridView dataGridViewBooks;
        private Button buttonPOS;
        private GroupBox groupBox1;
        private DataGridView dataGridView1;
        private Button button1;
        private GroupBox groupBox2;
        private Button button2;
        private Label label2;
        private Label label1;
        private Label label3;
        private TextBox textBoxBookId;
        private Button button3;
        private Label label4;
        private TextBox textBox1;
        private Label label6;
        private Label label5;
        private Button button4;
        private Button button5;
    }
}