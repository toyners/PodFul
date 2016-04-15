namespace PodFul.Windows
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
      this.feeds = new System.Windows.Forms.ListBox();
      this.label1 = new System.Windows.Forms.Label();
      this.podcasts = new System.Windows.Forms.ListBox();
      this.addFeed = new System.Windows.Forms.Button();
      this.removeFeed = new System.Windows.Forms.Button();
      this.scanFeeds = new System.Windows.Forms.Button();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.textBox5 = new System.Windows.Forms.TextBox();
      this.label10 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // feeds
      // 
      this.feeds.FormattingEnabled = true;
      this.feeds.Location = new System.Drawing.Point(12, 25);
      this.feeds.Name = "feeds";
      this.feeds.Size = new System.Drawing.Size(223, 251);
      this.feeds.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(36, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Feeds";
      // 
      // podcasts
      // 
      this.podcasts.FormattingEnabled = true;
      this.podcasts.Location = new System.Drawing.Point(260, 25);
      this.podcasts.Name = "podcasts";
      this.podcasts.Size = new System.Drawing.Size(223, 251);
      this.podcasts.TabIndex = 0;
      // 
      // addFeed
      // 
      this.addFeed.Location = new System.Drawing.Point(13, 371);
      this.addFeed.Name = "addFeed";
      this.addFeed.Size = new System.Drawing.Size(61, 35);
      this.addFeed.TabIndex = 2;
      this.addFeed.Text = "Add";
      this.addFeed.UseVisualStyleBackColor = true;
      this.addFeed.Click += new System.EventHandler(this.AddFeed_Click);
      // 
      // removeFeed
      // 
      this.removeFeed.Location = new System.Drawing.Point(108, 371);
      this.removeFeed.Name = "removeFeed";
      this.removeFeed.Size = new System.Drawing.Size(61, 35);
      this.removeFeed.TabIndex = 2;
      this.removeFeed.Text = "Remove";
      this.removeFeed.UseVisualStyleBackColor = true;
      // 
      // scanFeeds
      // 
      this.scanFeeds.Location = new System.Drawing.Point(175, 371);
      this.scanFeeds.Name = "scanFeeds";
      this.scanFeeds.Size = new System.Drawing.Size(61, 35);
      this.scanFeeds.TabIndex = 2;
      this.scanFeeds.Text = "Scan";
      this.scanFeeds.UseVisualStyleBackColor = true;
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(12, 282);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new System.Drawing.Size(223, 83);
      this.textBox1.TabIndex = 4;
      // 
      // textBox5
      // 
      this.textBox5.Location = new System.Drawing.Point(260, 282);
      this.textBox5.Multiline = true;
      this.textBox5.Name = "textBox5";
      this.textBox5.ReadOnly = true;
      this.textBox5.Size = new System.Drawing.Size(223, 83);
      this.textBox5.TabIndex = 4;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(257, 9);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(51, 13);
      this.label10.TabIndex = 1;
      this.label10.Text = "Podcasts";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(497, 415);
      this.Controls.Add(this.textBox5);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.scanFeeds);
      this.Controls.Add(this.removeFeed);
      this.Controls.Add(this.addFeed);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.podcasts);
      this.Controls.Add(this.feeds);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.Text = "Podful";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox feeds;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListBox podcasts;
    private System.Windows.Forms.Button addFeed;
    private System.Windows.Forms.Button removeFeed;
    private System.Windows.Forms.Button scanFeeds;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TextBox textBox5;
    private System.Windows.Forms.Label label10;
  }
}

