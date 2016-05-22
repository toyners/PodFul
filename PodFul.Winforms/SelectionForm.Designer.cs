namespace PodFul.Winforms
{
  partial class SelectionForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectionForm));
      this.list = new System.Windows.Forms.DataGridView();
      this.titleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.descriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.fileSizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.latestDownloadDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.start = new System.Windows.Forms.Button();
      this.clearButton = new System.Windows.Forms.Button();
      this.allButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.list)).BeginInit();
      this.SuspendLayout();
      // 
      // list
      // 
      this.list.AllowUserToAddRows = false;
      this.list.AllowUserToDeleteRows = false;
      this.list.AllowUserToResizeRows = false;
      this.list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.list.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.list.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.titleColumn,
            this.descriptionColumn,
            this.fileSizeColumn,
            this.latestDownloadDateColumn});
      this.list.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
      this.list.Location = new System.Drawing.Point(0, 0);
      this.list.Name = "list";
      this.list.ReadOnly = true;
      this.list.RowHeadersVisible = false;
      this.list.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.list.Size = new System.Drawing.Size(723, 258);
      this.list.TabIndex = 0;
      this.list.SelectionChanged += new System.EventHandler(this.podcastList_SelectionChanged);
      // 
      // titleColumn
      // 
      this.titleColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.titleColumn.HeaderText = "Title";
      this.titleColumn.Name = "titleColumn";
      this.titleColumn.ReadOnly = true;
      // 
      // descriptionColumn
      // 
      this.descriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.descriptionColumn.FillWeight = 200F;
      this.descriptionColumn.HeaderText = "Description";
      this.descriptionColumn.Name = "descriptionColumn";
      this.descriptionColumn.ReadOnly = true;
      // 
      // fileSizeColumn
      // 
      this.fileSizeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.fileSizeColumn.FillWeight = 40F;
      this.fileSizeColumn.HeaderText = "File Size";
      this.fileSizeColumn.Name = "fileSizeColumn";
      this.fileSizeColumn.ReadOnly = true;
      this.fileSizeColumn.Width = 80;
      // 
      // latestDownloadDateColumn
      // 
      this.latestDownloadDateColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.latestDownloadDateColumn.FillWeight = 60F;
      this.latestDownloadDateColumn.HeaderText = "Last Download Date";
      this.latestDownloadDateColumn.Name = "latestDownloadDateColumn";
      this.latestDownloadDateColumn.ReadOnly = true;
      this.latestDownloadDateColumn.Width = 140;
      // 
      // start
      // 
      this.start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.start.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.start.Location = new System.Drawing.Point(653, 267);
      this.start.Name = "start";
      this.start.Size = new System.Drawing.Size(64, 35);
      this.start.TabIndex = 3;
      this.start.Text = "Start";
      this.start.UseVisualStyleBackColor = true;
      // 
      // clearButton
      // 
      this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.clearButton.Location = new System.Drawing.Point(7, 267);
      this.clearButton.Name = "clearButton";
      this.clearButton.Size = new System.Drawing.Size(64, 35);
      this.clearButton.TabIndex = 3;
      this.clearButton.Text = "Clear";
      this.clearButton.UseVisualStyleBackColor = true;
      this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
      // 
      // allButton
      // 
      this.allButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.allButton.Location = new System.Drawing.Point(77, 267);
      this.allButton.Name = "allButton";
      this.allButton.Size = new System.Drawing.Size(64, 35);
      this.allButton.TabIndex = 3;
      this.allButton.Text = "All";
      this.allButton.UseVisualStyleBackColor = true;
      this.allButton.Click += new System.EventHandler(this.allButton_Click);
      // 
      // SelectionForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(724, 309);
      this.Controls.Add(this.allButton);
      this.Controls.Add(this.clearButton);
      this.Controls.Add(this.start);
      this.Controls.Add(this.list);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MinimizeBox = false;
      this.Name = "SelectionForm";
      this.Text = "Download Podcasts";
      ((System.ComponentModel.ISupportInitialize)(this.list)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView list;
    private System.Windows.Forms.Button start;
    private System.Windows.Forms.DataGridViewTextBoxColumn titleColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn descriptionColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn fileSizeColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn latestDownloadDateColumn;
    private System.Windows.Forms.Button clearButton;
    private System.Windows.Forms.Button allButton;
  }
}