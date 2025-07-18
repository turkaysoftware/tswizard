﻿namespace TSWizard
{
    partial class TS_AppLauncher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TS_AppLauncher));
            this.BackPanel = new System.Windows.Forms.Panel();
            this.BtnLauncherARM64 = new System.Windows.Forms.Button();
            this.BtnLauncherX64 = new System.Windows.Forms.Button();
            this.HeaderLabel = new System.Windows.Forms.Label();
            this.BackPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BackPanel
            // 
            this.BackPanel.Controls.Add(this.BtnLauncherARM64);
            this.BackPanel.Controls.Add(this.BtnLauncherX64);
            this.BackPanel.Controls.Add(this.HeaderLabel);
            this.BackPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BackPanel.Location = new System.Drawing.Point(0, 0);
            this.BackPanel.Name = "BackPanel";
            this.BackPanel.Padding = new System.Windows.Forms.Padding(5);
            this.BackPanel.Size = new System.Drawing.Size(434, 161);
            this.BackPanel.TabIndex = 0;
            // 
            // BtnLauncherARM64
            // 
            this.BtnLauncherARM64.BackColor = System.Drawing.Color.DarkOrange;
            this.BtnLauncherARM64.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.BtnLauncherARM64.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnLauncherARM64.FlatAppearance.BorderSize = 0;
            this.BtnLauncherARM64.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnLauncherARM64.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.BtnLauncherARM64.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.BtnLauncherARM64.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnLauncherARM64.Location = new System.Drawing.Point(224, 82);
            this.BtnLauncherARM64.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.BtnLauncherARM64.Name = "BtnLauncherARM64";
            this.BtnLauncherARM64.Size = new System.Drawing.Size(185, 32);
            this.BtnLauncherARM64.TabIndex = 2;
            this.BtnLauncherARM64.Text = "ARM64 (ARM İşlemci)";
            this.BtnLauncherARM64.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnLauncherARM64.UseVisualStyleBackColor = false;
            this.BtnLauncherARM64.Click += new System.EventHandler(this.BtnLauncherARM64_Click);
            // 
            // BtnLauncherX64
            // 
            this.BtnLauncherX64.BackColor = System.Drawing.Color.DarkOrange;
            this.BtnLauncherX64.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.BtnLauncherX64.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnLauncherX64.FlatAppearance.BorderSize = 0;
            this.BtnLauncherX64.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnLauncherX64.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.BtnLauncherX64.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.BtnLauncherX64.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnLauncherX64.Location = new System.Drawing.Point(25, 82);
            this.BtnLauncherX64.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.BtnLauncherX64.Name = "BtnLauncherX64";
            this.BtnLauncherX64.Size = new System.Drawing.Size(185, 32);
            this.BtnLauncherX64.TabIndex = 1;
            this.BtnLauncherX64.Text = "x64 (64 Bit)";
            this.BtnLauncherX64.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnLauncherX64.UseVisualStyleBackColor = false;
            this.BtnLauncherX64.Click += new System.EventHandler(this.BtnLauncherX64_Click);
            // 
            // HeaderLabel
            // 
            this.HeaderLabel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.HeaderLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.HeaderLabel.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.HeaderLabel.Location = new System.Drawing.Point(5, 5);
            this.HeaderLabel.Margin = new System.Windows.Forms.Padding(3);
            this.HeaderLabel.Name = "HeaderLabel";
            this.HeaderLabel.Size = new System.Drawing.Size(424, 35);
            this.HeaderLabel.TabIndex = 0;
            this.HeaderLabel.Text = "Yazılım Mimarisi Seçiniz";
            this.HeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TS_AppLauncher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(434, 161);
            this.Controls.Add(this.BackPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(450, 200);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 200);
            this.Name = "TS_AppLauncher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TS_AppLauncher";
            this.Load += new System.EventHandler(this.TS_AppLauncher_Load);
            this.BackPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel BackPanel;
        private System.Windows.Forms.Label HeaderLabel;
        private System.Windows.Forms.Button BtnLauncherX64;
        private System.Windows.Forms.Button BtnLauncherARM64;
    }
}