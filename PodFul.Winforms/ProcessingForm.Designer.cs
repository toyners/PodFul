﻿namespace PodFul.Winforms
{
  partial class ProcessingForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessingForm));
      this.cancelButton = new System.Windows.Forms.Button();
      this.feedback = new System.Windows.Forms.TextBox();
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.SuspendLayout();
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Enabled = false;
      this.cancelButton.Location = new System.Drawing.Point(411, 310);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(64, 35);
      this.cancelButton.TabIndex = 7;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // feedback
      // 
      this.feedback.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.feedback.Location = new System.Drawing.Point(10, 9);
      this.feedback.Multiline = true;
      this.feedback.Name = "feedback";
      this.feedback.ReadOnly = true;
      this.feedback.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.feedback.Size = new System.Drawing.Size(465, 251);
      this.feedback.TabIndex = 8;
      // 
      // progressBar
      // 
      this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar.Location = new System.Drawing.Point(10, 267);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(465, 37);
      this.progressBar.TabIndex = 9;
      // 
      // ProcessingForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(481, 350);
      this.Controls.Add(this.progressBar);
      this.Controls.Add(this.feedback);
      this.Controls.Add(this.cancelButton);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MinimizeBox = false;
      this.Name = "ProcessingForm";
      this.Text = "ScanResultsForm";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.TextBox feedback;
    private System.Windows.Forms.ProgressBar progressBar;
  }
}