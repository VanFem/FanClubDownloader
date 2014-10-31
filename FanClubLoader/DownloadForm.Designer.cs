namespace FanClubLoader
{
    partial class DownloadForm
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnBrowseDownloadLocation = new System.Windows.Forms.Button();
            this.txtDownloadLocation = new System.Windows.Forms.TextBox();
            this.lblPageFrom = new System.Windows.Forms.Label();
            this.numFromPage = new System.Windows.Forms.NumericUpDown();
            this.lblToPage = new System.Windows.Forms.Label();
            this.numToPage = new System.Windows.Forms.NumericUpDown();
            this.lblPostsPerPage = new System.Windows.Forms.Label();
            this.cmbPostsPerPage = new System.Windows.Forms.ComboBox();
            this.lblDownloadLocation = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dlFolderSelectDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.bLImageBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.urlDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.postDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.downloadedDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.localPathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pageNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contentDataGridViewImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.thumbnailDataGridViewImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFromPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numToPage)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bLImageBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1163, 607);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 17;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 106F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 194F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.btnBrowseDownloadLocation, 8, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtDownloadLocation, 7, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblPageFrom, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.numFromPage, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblToPage, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.numToPage, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblPostsPerPage, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmbPostsPerPage, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblDownloadLocation, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.button1, 9, 0);
            this.tableLayoutPanel2.Controls.Add(this.button2, 11, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1157, 29);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // btnBrowseDownloadLocation
            // 
            this.btnBrowseDownloadLocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowseDownloadLocation.Location = new System.Drawing.Point(728, 3);
            this.btnBrowseDownloadLocation.Name = "btnBrowseDownloadLocation";
            this.btnBrowseDownloadLocation.Size = new System.Drawing.Size(24, 23);
            this.btnBrowseDownloadLocation.TabIndex = 8;
            this.btnBrowseDownloadLocation.Text = "...";
            this.btnBrowseDownloadLocation.UseVisualStyleBackColor = true;
            this.btnBrowseDownloadLocation.Click += new System.EventHandler(this.btnBrowseDownloadLocation_Click);
            // 
            // txtDownloadLocation
            // 
            this.txtDownloadLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDownloadLocation.Location = new System.Drawing.Point(534, 4);
            this.txtDownloadLocation.Name = "txtDownloadLocation";
            this.txtDownloadLocation.Size = new System.Drawing.Size(188, 20);
            this.txtDownloadLocation.TabIndex = 7;
            this.txtDownloadLocation.WordWrap = false;
            // 
            // lblPageFrom
            // 
            this.lblPageFrom.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPageFrom.AutoSize = true;
            this.lblPageFrom.Location = new System.Drawing.Point(7, 8);
            this.lblPageFrom.Name = "lblPageFrom";
            this.lblPageFrom.Size = new System.Drawing.Size(60, 13);
            this.lblPageFrom.TabIndex = 0;
            this.lblPageFrom.Text = "From page:";
            // 
            // numFromPage
            // 
            this.numFromPage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numFromPage.Location = new System.Drawing.Point(73, 4);
            this.numFromPage.Name = "numFromPage";
            this.numFromPage.Size = new System.Drawing.Size(59, 20);
            this.numFromPage.TabIndex = 1;
            this.numFromPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblToPage
            // 
            this.lblToPage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblToPage.AutoSize = true;
            this.lblToPage.Location = new System.Drawing.Point(146, 8);
            this.lblToPage.Name = "lblToPage";
            this.lblToPage.Size = new System.Drawing.Size(46, 13);
            this.lblToPage.TabIndex = 2;
            this.lblToPage.Text = "to page:";
            // 
            // numToPage
            // 
            this.numToPage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numToPage.Location = new System.Drawing.Point(198, 4);
            this.numToPage.Name = "numToPage";
            this.numToPage.Size = new System.Drawing.Size(59, 20);
            this.numToPage.TabIndex = 3;
            this.numToPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblPostsPerPage
            // 
            this.lblPostsPerPage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPostsPerPage.AutoSize = true;
            this.lblPostsPerPage.Location = new System.Drawing.Point(276, 8);
            this.lblPostsPerPage.Name = "lblPostsPerPage";
            this.lblPostsPerPage.Size = new System.Drawing.Size(81, 13);
            this.lblPostsPerPage.TabIndex = 4;
            this.lblPostsPerPage.Text = "Posts per page:";
            // 
            // cmbPostsPerPage
            // 
            this.cmbPostsPerPage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cmbPostsPerPage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPostsPerPage.FormattingEnabled = true;
            this.cmbPostsPerPage.Items.AddRange(new object[] {
            "10,",
            "20,",
            "30,",
            "40,",
            "50"});
            this.cmbPostsPerPage.Location = new System.Drawing.Point(363, 4);
            this.cmbPostsPerPage.Name = "cmbPostsPerPage";
            this.cmbPostsPerPage.Size = new System.Drawing.Size(59, 21);
            this.cmbPostsPerPage.TabIndex = 5;
            this.cmbPostsPerPage.SelectedIndexChanged += new System.EventHandler(this.cmbPostsPerPage_SelectedIndexChanged);
            // 
            // lblDownloadLocation
            // 
            this.lblDownloadLocation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDownloadLocation.AutoSize = true;
            this.lblDownloadLocation.Location = new System.Drawing.Point(430, 8);
            this.lblDownloadLocation.Name = "lblDownloadLocation";
            this.lblDownloadLocation.Size = new System.Drawing.Size(98, 13);
            this.lblDownloadLocation.TabIndex = 6;
            this.lblDownloadLocation.Text = "Download location:";
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(758, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Download";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(878, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(44, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "Scan";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.16681F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.83319F));
            this.tableLayoutPanel3.Controls.Add(this.dataGridView1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 38);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1157, 566);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.urlDataGridViewTextBoxColumn,
            this.postDateDataGridViewTextBoxColumn,
            this.downloadedDataGridViewCheckBoxColumn,
            this.localPathDataGridViewTextBoxColumn,
            this.pageNumberDataGridViewTextBoxColumn,
            this.contentDataGridViewImageColumn,
            this.thumbnailDataGridViewImageColumn});
            this.dataGridView1.DataSource = this.bLImageBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(585, 560);
            this.dataGridView1.TabIndex = 0;
            // 
            // bLImageBindingSource
            // 
            this.bLImageBindingSource.DataSource = typeof(BeastsLairConnector.BLImage);
            // 
            // urlDataGridViewTextBoxColumn
            // 
            this.urlDataGridViewTextBoxColumn.DataPropertyName = "Url";
            this.urlDataGridViewTextBoxColumn.HeaderText = "Url";
            this.urlDataGridViewTextBoxColumn.Name = "urlDataGridViewTextBoxColumn";
            this.urlDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // postDateDataGridViewTextBoxColumn
            // 
            this.postDateDataGridViewTextBoxColumn.DataPropertyName = "PostDate";
            this.postDateDataGridViewTextBoxColumn.HeaderText = "PostDate";
            this.postDateDataGridViewTextBoxColumn.Name = "postDateDataGridViewTextBoxColumn";
            this.postDateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // downloadedDataGridViewCheckBoxColumn
            // 
            this.downloadedDataGridViewCheckBoxColumn.DataPropertyName = "Downloaded";
            this.downloadedDataGridViewCheckBoxColumn.HeaderText = "Downloaded";
            this.downloadedDataGridViewCheckBoxColumn.Name = "downloadedDataGridViewCheckBoxColumn";
            this.downloadedDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // localPathDataGridViewTextBoxColumn
            // 
            this.localPathDataGridViewTextBoxColumn.DataPropertyName = "LocalPath";
            this.localPathDataGridViewTextBoxColumn.HeaderText = "LocalPath";
            this.localPathDataGridViewTextBoxColumn.Name = "localPathDataGridViewTextBoxColumn";
            this.localPathDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // pageNumberDataGridViewTextBoxColumn
            // 
            this.pageNumberDataGridViewTextBoxColumn.DataPropertyName = "PageNumber";
            this.pageNumberDataGridViewTextBoxColumn.HeaderText = "PageNumber";
            this.pageNumberDataGridViewTextBoxColumn.Name = "pageNumberDataGridViewTextBoxColumn";
            this.pageNumberDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // contentDataGridViewImageColumn
            // 
            this.contentDataGridViewImageColumn.DataPropertyName = "Content";
            this.contentDataGridViewImageColumn.HeaderText = "Content";
            this.contentDataGridViewImageColumn.Name = "contentDataGridViewImageColumn";
            this.contentDataGridViewImageColumn.ReadOnly = true;
            // 
            // thumbnailDataGridViewImageColumn
            // 
            this.thumbnailDataGridViewImageColumn.DataPropertyName = "Thumbnail";
            this.thumbnailDataGridViewImageColumn.HeaderText = "Thumbnail";
            this.thumbnailDataGridViewImageColumn.Name = "thumbnailDataGridViewImageColumn";
            this.thumbnailDataGridViewImageColumn.ReadOnly = true;
            // 
            // DownloadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1163, 607);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DownloadForm";
            this.Text = "DownloadForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFromPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numToPage)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bLImageBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblPageFrom;
        private System.Windows.Forms.NumericUpDown numFromPage;
        private System.Windows.Forms.Label lblToPage;
        private System.Windows.Forms.NumericUpDown numToPage;
        private System.Windows.Forms.Label lblPostsPerPage;
        private System.Windows.Forms.ComboBox cmbPostsPerPage;
        private System.Windows.Forms.Label lblDownloadLocation;
        private System.Windows.Forms.TextBox txtDownloadLocation;
        private System.Windows.Forms.Button btnBrowseDownloadLocation;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.FolderBrowserDialog dlFolderSelectDialog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn urlDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn postDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn downloadedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn localPathDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pageNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewImageColumn contentDataGridViewImageColumn;
        private System.Windows.Forms.DataGridViewImageColumn thumbnailDataGridViewImageColumn;
        private System.Windows.Forms.BindingSource bLImageBindingSource;
    }
}