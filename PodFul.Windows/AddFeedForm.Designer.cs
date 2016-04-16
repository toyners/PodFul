namespace PodFul.Windows
{
  partial class AddFeedForm
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
      this.label1 = new System.Windows.Forms.Label();
      this.FeedDirectory = new System.Windows.Forms.TextBox();
      this.FeedURL = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.addFeed = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(52, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Directory:";
      // 
      // FeedDirectory
      // 
      this.FeedDirectory.Location = new System.Drawing.Point(61, 6);
      this.FeedDirectory.Name = "FeedDirectory";
      this.FeedDirectory.Size = new System.Drawing.Size(298, 20);
      this.FeedDirectory.TabIndex = 1;
      // 
      // FeedURL
      // 
      this.FeedURL.Location = new System.Drawing.Point(61, 32);
      this.FeedURL.Name = "FeedURL";
      this.FeedURL.Size = new System.Drawing.Size(298, 20);
      this.FeedURL.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 35);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(32, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "URL:";
      // 
      // addFeed
      // 
      this.addFeed.Location = new System.Drawing.Point(298, 58);
      this.addFeed.Name = "addFeed";
      this.addFeed.Size = new System.Drawing.Size(61, 35);
      this.addFeed.TabIndex = 4;
      this.addFeed.Text = "Add";
      this.addFeed.UseVisualStyleBackColor = true;
      this.addFeed.Click += new System.EventHandler(this.addFeed_Click);
      // 
      // AddFeedForm
      // 
      this.AcceptButton = this.addFeed;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(368, 100);
      this.Controls.Add(this.addFeed);
      this.Controls.Add(this.FeedURL);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.FeedDirectory);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AddFeedForm";
      this.ShowInTaskbar = false;
      this.Text = "Add Feed";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button addFeed;
    internal System.Windows.Forms.TextBox FeedDirectory;
    internal System.Windows.Forms.TextBox FeedURL;
  }
}