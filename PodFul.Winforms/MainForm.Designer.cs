namespace PodFul.Winforms
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.podcastDescription = new System.Windows.Forms.TextBox();
      this.feedDescription = new System.Windows.Forms.TextBox();
      this.downloadPodcast = new System.Windows.Forms.Button();
      this.scanFeeds = new System.Windows.Forms.Button();
      this.removeFeed = new System.Windows.Forms.Button();
      this.addFeed = new System.Windows.Forms.Button();
      this.podcastList = new System.Windows.Forms.ListBox();
      this.feedList = new System.Windows.Forms.ListBox();
      this.addToWinamp = new System.Windows.Forms.CheckBox();
      this.syncPodcasts = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(244, 28);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(51, 13);
      this.label4.TabIndex = 44;
      this.label4.Text = "Podcasts";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(8, 28);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(36, 13);
      this.label3.TabIndex = 45;
      this.label3.Text = "Feeds";
      // 
      // podcastDescription
      // 
      this.podcastDescription.Location = new System.Drawing.Point(247, 301);
      this.podcastDescription.Multiline = true;
      this.podcastDescription.Name = "podcastDescription";
      this.podcastDescription.ReadOnly = true;
      this.podcastDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.podcastDescription.Size = new System.Drawing.Size(234, 109);
      this.podcastDescription.TabIndex = 42;
      // 
      // feedDescription
      // 
      this.feedDescription.Location = new System.Drawing.Point(10, 301);
      this.feedDescription.Multiline = true;
      this.feedDescription.Name = "feedDescription";
      this.feedDescription.ReadOnly = true;
      this.feedDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.feedDescription.Size = new System.Drawing.Size(231, 109);
      this.feedDescription.TabIndex = 43;
      // 
      // downloadPodcast
      // 
      this.downloadPodcast.Enabled = false;
      this.downloadPodcast.Location = new System.Drawing.Point(417, 416);
      this.downloadPodcast.Name = "downloadPodcast";
      this.downloadPodcast.Size = new System.Drawing.Size(64, 35);
      this.downloadPodcast.TabIndex = 38;
      this.downloadPodcast.Text = "Download";
      this.downloadPodcast.UseVisualStyleBackColor = true;
      this.downloadPodcast.Click += new System.EventHandler(this.downloadPodcast_Click);
      // 
      // scanFeeds
      // 
      this.scanFeeds.Location = new System.Drawing.Point(350, 416);
      this.scanFeeds.Name = "scanFeeds";
      this.scanFeeds.Size = new System.Drawing.Size(61, 35);
      this.scanFeeds.TabIndex = 39;
      this.scanFeeds.Text = "Scan";
      this.scanFeeds.UseVisualStyleBackColor = true;
      this.scanFeeds.Click += new System.EventHandler(this.scanFeeds_Click);
      // 
      // removeFeed
      // 
      this.removeFeed.Location = new System.Drawing.Point(75, 416);
      this.removeFeed.Name = "removeFeed";
      this.removeFeed.Size = new System.Drawing.Size(61, 35);
      this.removeFeed.TabIndex = 40;
      this.removeFeed.Text = "Remove";
      this.removeFeed.UseVisualStyleBackColor = true;
      this.removeFeed.Click += new System.EventHandler(this.removeFeed_Click);
      // 
      // addFeed
      // 
      this.addFeed.Location = new System.Drawing.Point(8, 416);
      this.addFeed.Name = "addFeed";
      this.addFeed.Size = new System.Drawing.Size(61, 35);
      this.addFeed.TabIndex = 41;
      this.addFeed.Text = "Add";
      this.addFeed.UseVisualStyleBackColor = true;
      this.addFeed.Click += new System.EventHandler(this.addFeed_Click);
      // 
      // podcastList
      // 
      this.podcastList.FormattingEnabled = true;
      this.podcastList.Location = new System.Drawing.Point(247, 44);
      this.podcastList.Name = "podcastList";
      this.podcastList.Size = new System.Drawing.Size(234, 251);
      this.podcastList.TabIndex = 36;
      this.podcastList.SelectedIndexChanged += new System.EventHandler(this.podcastList_SelectedIndexChanged);
      // 
      // feedList
      // 
      this.feedList.FormattingEnabled = true;
      this.feedList.Location = new System.Drawing.Point(10, 44);
      this.feedList.Name = "feedList";
      this.feedList.Size = new System.Drawing.Size(231, 251);
      this.feedList.TabIndex = 37;
      this.feedList.SelectedIndexChanged += new System.EventHandler(this.feedList_SelectedIndexChanged);
      // 
      // addToWinamp
      // 
      this.addToWinamp.AutoSize = true;
      this.addToWinamp.Checked = true;
      this.addToWinamp.CheckState = System.Windows.Forms.CheckState.Checked;
      this.addToWinamp.Location = new System.Drawing.Point(249, 434);
      this.addToWinamp.Name = "addToWinamp";
      this.addToWinamp.Size = new System.Drawing.Size(100, 17);
      this.addToWinamp.TabIndex = 46;
      this.addToWinamp.Text = "Add to WinAmp";
      this.addToWinamp.UseVisualStyleBackColor = true;
      // 
      // syncPodcasts
      // 
      this.syncPodcasts.Location = new System.Drawing.Point(142, 416);
      this.syncPodcasts.Name = "syncPodcasts";
      this.syncPodcasts.Size = new System.Drawing.Size(61, 35);
      this.syncPodcasts.TabIndex = 47;
      this.syncPodcasts.Text = "Sync";
      this.syncPodcasts.UseVisualStyleBackColor = true;
      this.syncPodcasts.Click += new System.EventHandler(this.syncPodcasts_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(490, 459);
      this.Controls.Add(this.syncPodcasts);
      this.Controls.Add(this.addToWinamp);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.podcastDescription);
      this.Controls.Add(this.feedDescription);
      this.Controls.Add(this.downloadPodcast);
      this.Controls.Add(this.scanFeeds);
      this.Controls.Add(this.removeFeed);
      this.Controls.Add(this.addFeed);
      this.Controls.Add(this.podcastList);
      this.Controls.Add(this.feedList);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.Text = "Podful";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox podcastDescription;
    private System.Windows.Forms.TextBox feedDescription;
    private System.Windows.Forms.Button downloadPodcast;
    private System.Windows.Forms.Button scanFeeds;
    private System.Windows.Forms.Button removeFeed;
    private System.Windows.Forms.Button addFeed;
    private System.Windows.Forms.ListBox podcastList;
    private System.Windows.Forms.ListBox feedList;
    private System.Windows.Forms.CheckBox addToWinamp;
    private System.Windows.Forms.Button syncPodcasts;
  }
}

