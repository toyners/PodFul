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
      ((System.ComponentModel.ISupportInitialize)(this.podcastList)).BeginInit();
      this.SuspendLayout();
      // 
      // podcastList
      // 
      this.podcastList.AllowUserToAddRows = false;
      this.podcastList.AllowUserToDeleteRows = false;
      this.podcastList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.podcastList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.podcastList.Location = new System.Drawing.Point(0, 0);
      this.podcastList.Name = "podcastList";
      this.podcastList.ReadOnly = true;
      this.podcastList.Size = new System.Drawing.Size(370, 258);
      this.podcastList.TabIndex = 0;
      // 
      // ScanResultsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(370, 258);
      this.Controls.Add(this.podcastList);
      this.MinimizeBox = false;
      this.Name = "ScanResultsForm";
      this.Text = "Scan Results";
      ((System.ComponentModel.ISupportInitialize)(this.podcastList)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView podcastList;
  }
}