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
      this.feedList = new System.Windows.Forms.ListBox();
      this.label1 = new System.Windows.Forms.Label();
      this.podcastList = new System.Windows.Forms.ListBox();
      this.addFeed = new System.Windows.Forms.Button();
      this.removeFeed = new System.Windows.Forms.Button();
      this.scanFeeds = new System.Windows.Forms.Button();
      this.feedDescription = new System.Windows.Forms.TextBox();
      this.podcastDescription = new System.Windows.Forms.TextBox();
      this.label10 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // feedList
      // 
      this.feedList.FormattingEnabled = true;
      this.feedList.Location = new System.Drawing.Point(12, 25);
      this.feedList.Name = "feedList";
      this.feedList.Size = new System.Drawing.Size(223, 251);
      this.feedList.TabIndex = 0;
      this.feedList.SelectedIndexChanged += new System.EventHandler(this.feeds_SelectedIndexChanged);
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
      // podcastList
      // 
      this.podcastList.FormattingEnabled = true;
      this.podcastList.Location = new System.Drawing.Point(260, 25);
      this.podcastList.Name = "podcastList";
      this.podcastList.Size = new System.Drawing.Size(223, 251);
      this.podcastList.TabIndex = 0;
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
      this.removeFeed.Click += new System.EventHandler(this.removeFeed_Click);
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
      // feedDescription
      // 
      this.feedDescription.Location = new System.Drawing.Point(12, 282);
      this.feedDescription.Multiline = true;
      this.feedDescription.Name = "feedDescription";
      this.feedDescription.ReadOnly = true;
      this.feedDescription.Size = new System.Drawing.Size(223, 83);
      this.feedDescription.TabIndex = 4;
      // 
      // podcastDescription
      // 
      this.podcastDescription.Location = new System.Drawing.Point(260, 282);
      this.podcastDescription.Multiline = true;
      this.podcastDescription.Name = "podcastDescription";
      this.podcastDescription.ReadOnly = true;
      this.podcastDescription.Size = new System.Drawing.Size(223, 83);
      this.podcastDescription.TabIndex = 4;
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
      this.Controls.Add(this.podcastDescription);
      this.Controls.Add(this.feedDescription);
      this.Controls.Add(this.scanFeeds);
      this.Controls.Add(this.removeFeed);
      this.Controls.Add(this.addFeed);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.podcastList);
      this.Controls.Add(this.feedList);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.Text = "Podful";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox feedList;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListBox podcastList;
    private System.Windows.Forms.Button addFeed;
    private System.Windows.Forms.Button removeFeed;
    private System.Windows.Forms.Button scanFeeds;
    private System.Windows.Forms.TextBox feedDescription;
    private System.Windows.Forms.TextBox podcastDescription;
    private System.Windows.Forms.Label label10;
  }
}

