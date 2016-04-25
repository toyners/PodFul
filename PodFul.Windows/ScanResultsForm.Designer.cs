namespace PodFul.Windows
{
  partial class ScanResultsForm
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
      this.podcastList = new System.Windows.Forms.DataGridView();
      this.titleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.descriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.fileSizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.latestDownloadDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cancelButton = new System.Windows.Forms.Button();
      this.startDownload = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.podcastList)).BeginInit();
      this.SuspendLayout();
      // 
      // podcastList
      // 
      this.podcastList.AllowUserToAddRows = false;
      this.podcastList.AllowUserToDeleteRows = false;
      this.podcastList.AllowUserToResizeRows = false;
      this.podcastList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.podcastList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.podcastList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.titleColumn,
            this.descriptionColumn,
            this.fileSizeColumn,
            this.latestDownloadDateColumn});
      this.podcastList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
      this.podcastList.Location = new System.Drawing.Point(0, 0);
      this.podcastList.Name = "podcastList";
      this.podcastList.ReadOnly = true;
      this.podcastList.RowHeadersVisible = false;
      this.podcastList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.podcastList.Size = new System.Drawing.Size(723, 258);
      this.podcastList.TabIndex = 1;
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
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(583, 267);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(64, 35);
      this.cancelButton.TabIndex = 7;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // startDownload
      // 
      this.startDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.startDownload.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.startDownload.Location = new System.Drawing.Point(653, 267);
      this.startDownload.Name = "startDownload";
      this.startDownload.Size = new System.Drawing.Size(64, 35);
      this.startDownload.TabIndex = 6;
      this.startDownload.Text = "Start";
      this.startDownload.UseVisualStyleBackColor = true;
      // 
      // ScanResultsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(724, 309);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.startDownload);
      this.Controls.Add(this.podcastList);
      this.Name = "ScanResultsForm";
      this.Text = "ScanResultsForm";
      ((System.ComponentModel.ISupportInitialize)(this.podcastList)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView podcastList;
    private System.Windows.Forms.DataGridViewTextBoxColumn titleColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn descriptionColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn fileSizeColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn latestDownloadDateColumn;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button startDownload;
  }
}