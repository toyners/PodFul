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
      this.FeedList = new System.Windows.Forms.ListBox();
      this.label1 = new System.Windows.Forms.Label();
      this.PodcastList = new System.Windows.Forms.ListBox();
      this.AddFeed = new System.Windows.Forms.Button();
      this.RemoveFeed = new System.Windows.Forms.Button();
      this.ScanFeeds = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.textBox3 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.textBox4 = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.textBox5 = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.textBox6 = new System.Windows.Forms.TextBox();
      this.label8 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.textBox7 = new System.Windows.Forms.TextBox();
      this.textBox8 = new System.Windows.Forms.TextBox();
      this.label10 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // FeedList
      // 
      this.FeedList.FormattingEnabled = true;
      this.FeedList.Location = new System.Drawing.Point(12, 25);
      this.FeedList.Name = "FeedList";
      this.FeedList.Size = new System.Drawing.Size(156, 251);
      this.FeedList.TabIndex = 0;
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
      // PodcastList
      // 
      this.PodcastList.FormattingEnabled = true;
      this.PodcastList.Location = new System.Drawing.Point(446, 25);
      this.PodcastList.Name = "PodcastList";
      this.PodcastList.Size = new System.Drawing.Size(205, 290);
      this.PodcastList.TabIndex = 0;
      // 
      // AddFeed
      // 
      this.AddFeed.Location = new System.Drawing.Point(12, 281);
      this.AddFeed.Name = "AddFeed";
      this.AddFeed.Size = new System.Drawing.Size(61, 35);
      this.AddFeed.TabIndex = 2;
      this.AddFeed.Text = "Add";
      this.AddFeed.UseVisualStyleBackColor = true;
      this.AddFeed.Click += new System.EventHandler(this.AddFeed_Click);
      // 
      // RemoveFeed
      // 
      this.RemoveFeed.Location = new System.Drawing.Point(107, 281);
      this.RemoveFeed.Name = "RemoveFeed";
      this.RemoveFeed.Size = new System.Drawing.Size(61, 35);
      this.RemoveFeed.TabIndex = 2;
      this.RemoveFeed.Text = "Remove";
      this.RemoveFeed.UseVisualStyleBackColor = true;
      // 
      // ScanFeeds
      // 
      this.ScanFeeds.Location = new System.Drawing.Point(174, 281);
      this.ScanFeeds.Name = "ScanFeeds";
      this.ScanFeeds.Size = new System.Drawing.Size(61, 35);
      this.ScanFeeds.TabIndex = 2;
      this.ScanFeeds.Text = "Scan";
      this.ScanFeeds.UseVisualStyleBackColor = true;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(171, 25);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(60, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Description";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(171, 160);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(46, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Website";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(171, 199);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(49, 13);
      this.label5.TabIndex = 3;
      this.label5.Text = "Directory";
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(174, 41);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new System.Drawing.Size(266, 116);
      this.textBox1.TabIndex = 4;
      // 
      // textBox2
      // 
      this.textBox2.Location = new System.Drawing.Point(174, 176);
      this.textBox2.Name = "textBox2";
      this.textBox2.ReadOnly = true;
      this.textBox2.Size = new System.Drawing.Size(266, 20);
      this.textBox2.TabIndex = 5;
      // 
      // textBox3
      // 
      this.textBox3.Location = new System.Drawing.Point(174, 215);
      this.textBox3.Name = "textBox3";
      this.textBox3.ReadOnly = true;
      this.textBox3.Size = new System.Drawing.Size(266, 20);
      this.textBox3.TabIndex = 5;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(171, 239);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(31, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Feed";
      // 
      // textBox4
      // 
      this.textBox4.Location = new System.Drawing.Point(174, 255);
      this.textBox4.Name = "textBox4";
      this.textBox4.ReadOnly = true;
      this.textBox4.Size = new System.Drawing.Size(266, 20);
      this.textBox4.TabIndex = 5;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(654, 25);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(60, 13);
      this.label6.TabIndex = 3;
      this.label6.Text = "Description";
      // 
      // textBox5
      // 
      this.textBox5.Location = new System.Drawing.Point(657, 41);
      this.textBox5.Multiline = true;
      this.textBox5.Name = "textBox5";
      this.textBox5.ReadOnly = true;
      this.textBox5.Size = new System.Drawing.Size(266, 116);
      this.textBox5.TabIndex = 4;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(654, 160);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(46, 13);
      this.label7.TabIndex = 3;
      this.label7.Text = "Website";
      // 
      // textBox6
      // 
      this.textBox6.Location = new System.Drawing.Point(657, 176);
      this.textBox6.Name = "textBox6";
      this.textBox6.ReadOnly = true;
      this.textBox6.Size = new System.Drawing.Size(266, 20);
      this.textBox6.TabIndex = 5;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(654, 199);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(49, 13);
      this.label8.TabIndex = 3;
      this.label8.Text = "Directory";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(654, 239);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(31, 13);
      this.label9.TabIndex = 3;
      this.label9.Text = "Feed";
      // 
      // textBox7
      // 
      this.textBox7.Location = new System.Drawing.Point(657, 215);
      this.textBox7.Name = "textBox7";
      this.textBox7.ReadOnly = true;
      this.textBox7.Size = new System.Drawing.Size(266, 20);
      this.textBox7.TabIndex = 5;
      // 
      // textBox8
      // 
      this.textBox8.Location = new System.Drawing.Point(657, 255);
      this.textBox8.Name = "textBox8";
      this.textBox8.ReadOnly = true;
      this.textBox8.Size = new System.Drawing.Size(266, 20);
      this.textBox8.TabIndex = 5;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(443, 9);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(51, 13);
      this.label10.TabIndex = 1;
      this.label10.Text = "Podcasts";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(932, 323);
      this.Controls.Add(this.textBox8);
      this.Controls.Add(this.textBox4);
      this.Controls.Add(this.textBox7);
      this.Controls.Add(this.textBox3);
      this.Controls.Add(this.textBox6);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.textBox2);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.textBox5);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.ScanFeeds);
      this.Controls.Add(this.RemoveFeed);
      this.Controls.Add(this.AddFeed);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.PodcastList);
      this.Controls.Add(this.FeedList);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.Text = "Podful";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox FeedList;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListBox PodcastList;
    private System.Windows.Forms.Button AddFeed;
    private System.Windows.Forms.Button RemoveFeed;
    private System.Windows.Forms.Button ScanFeeds;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.TextBox textBox3;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textBox4;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox textBox5;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox textBox6;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TextBox textBox7;
    private System.Windows.Forms.TextBox textBox8;
    private System.Windows.Forms.Label label10;
  }
}

