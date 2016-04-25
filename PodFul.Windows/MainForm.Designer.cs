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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.feedsTab = new System.Windows.Forms.TabPage();
      this.panel1 = new System.Windows.Forms.Panel();
      this.podcastDescription = new System.Windows.Forms.TextBox();
      this.feedDescription = new System.Windows.Forms.TextBox();
      this.downloadPodcast = new System.Windows.Forms.Button();
      this.scanFeeds = new System.Windows.Forms.Button();
      this.removeFeed = new System.Windows.Forms.Button();
      this.addFeed = new System.Windows.Forms.Button();
      this.podcastList = new System.Windows.Forms.ListBox();
      this.feedList = new System.Windows.Forms.ListBox();
      this.downloadsTab = new System.Windows.Forms.TabPage();
      this.panel2 = new System.Windows.Forms.Panel();
      this.completedList = new System.Windows.Forms.ListBox();
      this.workingList = new System.Windows.Forms.ListBox();
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.cancelButton = new System.Windows.Forms.Button();
      this.tabControl.SuspendLayout();
      this.feedsTab.SuspendLayout();
      this.panel1.SuspendLayout();
      this.downloadsTab.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
      this.tabControl.Controls.Add(this.feedsTab);
      this.tabControl.Controls.Add(this.downloadsTab);
      this.tabControl.Location = new System.Drawing.Point(2, 9);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(495, 469);
      this.tabControl.TabIndex = 0;
      // 
      // feedsTab
      // 
      this.feedsTab.Controls.Add(this.panel1);
      this.feedsTab.Location = new System.Drawing.Point(4, 25);
      this.feedsTab.Name = "feedsTab";
      this.feedsTab.Padding = new System.Windows.Forms.Padding(3);
      this.feedsTab.Size = new System.Drawing.Size(487, 440);
      this.feedsTab.TabIndex = 0;
      this.feedsTab.Text = "Feeds";
      this.feedsTab.UseVisualStyleBackColor = true;
      // 
      // panel1
      // 
      this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel1.Controls.Add(this.podcastDescription);
      this.panel1.Controls.Add(this.feedDescription);
      this.panel1.Controls.Add(this.downloadPodcast);
      this.panel1.Controls.Add(this.scanFeeds);
      this.panel1.Controls.Add(this.removeFeed);
      this.panel1.Controls.Add(this.addFeed);
      this.panel1.Controls.Add(this.podcastList);
      this.panel1.Controls.Add(this.feedList);
      this.panel1.Location = new System.Drawing.Point(0, 5);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(483, 434);
      this.panel1.TabIndex = 0;
      // 
      // podcastDescription
      // 
      this.podcastDescription.Location = new System.Drawing.Point(242, 277);
      this.podcastDescription.Multiline = true;
      this.podcastDescription.Name = "podcastDescription";
      this.podcastDescription.ReadOnly = true;
      this.podcastDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.podcastDescription.Size = new System.Drawing.Size(234, 109);
      this.podcastDescription.TabIndex = 33;
      // 
      // feedDescription
      // 
      this.feedDescription.Location = new System.Drawing.Point(5, 277);
      this.feedDescription.Multiline = true;
      this.feedDescription.Name = "feedDescription";
      this.feedDescription.ReadOnly = true;
      this.feedDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.feedDescription.Size = new System.Drawing.Size(231, 109);
      this.feedDescription.TabIndex = 34;
      // 
      // downloadPodcast
      // 
      this.downloadPodcast.Enabled = false;
      this.downloadPodcast.Location = new System.Drawing.Point(412, 392);
      this.downloadPodcast.Name = "downloadPodcast";
      this.downloadPodcast.Size = new System.Drawing.Size(64, 35);
      this.downloadPodcast.TabIndex = 29;
      this.downloadPodcast.Text = "Download";
      this.downloadPodcast.UseVisualStyleBackColor = true;
      this.downloadPodcast.Click += new System.EventHandler(this.downloadPodcast_Click);
      // 
      // scanFeeds
      // 
      this.scanFeeds.Location = new System.Drawing.Point(173, 392);
      this.scanFeeds.Name = "scanFeeds";
      this.scanFeeds.Size = new System.Drawing.Size(61, 35);
      this.scanFeeds.TabIndex = 30;
      this.scanFeeds.Text = "Scan";
      this.scanFeeds.UseVisualStyleBackColor = true;
      this.scanFeeds.Click += new System.EventHandler(this.scanFeeds_Click);
      // 
      // removeFeed
      // 
      this.removeFeed.Location = new System.Drawing.Point(70, 392);
      this.removeFeed.Name = "removeFeed";
      this.removeFeed.Size = new System.Drawing.Size(61, 35);
      this.removeFeed.TabIndex = 31;
      this.removeFeed.Text = "Remove";
      this.removeFeed.UseVisualStyleBackColor = true;
      this.removeFeed.Click += new System.EventHandler(this.removeFeed_Click);
      // 
      // addFeed
      // 
      this.addFeed.Location = new System.Drawing.Point(3, 392);
      this.addFeed.Name = "addFeed";
      this.addFeed.Size = new System.Drawing.Size(61, 35);
      this.addFeed.TabIndex = 32;
      this.addFeed.Text = "Add";
      this.addFeed.UseVisualStyleBackColor = true;
      this.addFeed.Click += new System.EventHandler(this.AddFeed_Click);
      // 
      // podcastList
      // 
      this.podcastList.FormattingEnabled = true;
      this.podcastList.Location = new System.Drawing.Point(242, 7);
      this.podcastList.Name = "podcastList";
      this.podcastList.Size = new System.Drawing.Size(234, 264);
      this.podcastList.TabIndex = 25;
      this.podcastList.SelectedIndexChanged += new System.EventHandler(this.podcastList_SelectedIndexChanged);
      // 
      // feedList
      // 
      this.feedList.FormattingEnabled = true;
      this.feedList.Location = new System.Drawing.Point(5, 7);
      this.feedList.Name = "feedList";
      this.feedList.Size = new System.Drawing.Size(231, 264);
      this.feedList.TabIndex = 26;
      this.feedList.SelectedIndexChanged += new System.EventHandler(this.feedList_SelectedIndexChanged);
      // 
      // downloadsTab
      // 
      this.downloadsTab.Controls.Add(this.panel2);
      this.downloadsTab.Location = new System.Drawing.Point(4, 25);
      this.downloadsTab.Name = "downloadsTab";
      this.downloadsTab.Padding = new System.Windows.Forms.Padding(3);
      this.downloadsTab.Size = new System.Drawing.Size(487, 440);
      this.downloadsTab.TabIndex = 1;
      this.downloadsTab.Text = "Downloads";
      this.downloadsTab.UseVisualStyleBackColor = true;
      // 
      // panel2
      // 
      this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel2.Controls.Add(this.completedList);
      this.panel2.Controls.Add(this.workingList);
      this.panel2.Controls.Add(this.progressBar);
      this.panel2.Controls.Add(this.cancelButton);
      this.panel2.Location = new System.Drawing.Point(0, 5);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(483, 434);
      this.panel2.TabIndex = 1;
      // 
      // completedList
      // 
      this.completedList.FormattingEnabled = true;
      this.completedList.Location = new System.Drawing.Point(242, 7);
      this.completedList.Name = "completedList";
      this.completedList.Size = new System.Drawing.Size(234, 342);
      this.completedList.TabIndex = 31;
      // 
      // workingList
      // 
      this.workingList.FormattingEnabled = true;
      this.workingList.Location = new System.Drawing.Point(5, 7);
      this.workingList.Name = "workingList";
      this.workingList.Size = new System.Drawing.Size(231, 342);
      this.workingList.TabIndex = 32;
      // 
      // progressBar
      // 
      this.progressBar.Location = new System.Drawing.Point(5, 355);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(471, 31);
      this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.progressBar.TabIndex = 30;
      // 
      // cancelButton
      // 
      this.cancelButton.Enabled = false;
      this.cancelButton.Location = new System.Drawing.Point(412, 392);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(64, 35);
      this.cancelButton.TabIndex = 29;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(498, 479);
      this.Controls.Add(this.tabControl);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.Text = "Podful";
      this.tabControl.ResumeLayout(false);
      this.feedsTab.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.downloadsTab.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage feedsTab;
    private System.Windows.Forms.TabPage downloadsTab;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.TextBox podcastDescription;
    private System.Windows.Forms.TextBox feedDescription;
    private System.Windows.Forms.Button downloadPodcast;
    private System.Windows.Forms.Button scanFeeds;
    private System.Windows.Forms.Button removeFeed;
    private System.Windows.Forms.Button addFeed;
    private System.Windows.Forms.ListBox podcastList;
    private System.Windows.Forms.ListBox feedList;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.ListBox completedList;
    private System.Windows.Forms.ListBox workingList;
    private System.Windows.Forms.ProgressBar progressBar;
  }
}

