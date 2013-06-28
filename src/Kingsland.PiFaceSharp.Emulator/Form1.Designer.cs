namespace Kingsland.PiFaceSharp.Emulator
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.PiFacePreview = new System.Windows.Forms.PictureBox();
            this.lblLocalAddress = new System.Windows.Forms.Label();
            this.txtLocalAddress = new System.Windows.Forms.TextBox();
            this.lblLocalPort = new System.Windows.Forms.Label();
            this.txtLocalPort = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lnkGitHub = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.PiFacePreview)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PiFacePreview
            // 
            this.PiFacePreview.Image = global::Kingsland.PiFaceSharp.Emulator.Properties.Resources.PiFaceBackground;
            this.PiFacePreview.Location = new System.Drawing.Point(12, 12);
            this.PiFacePreview.Name = "PiFacePreview";
            this.PiFacePreview.Size = new System.Drawing.Size(302, 201);
            this.PiFacePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PiFacePreview.TabIndex = 0;
            this.PiFacePreview.TabStop = false;
            // 
            // lblLocalAddress
            // 
            this.lblLocalAddress.Location = new System.Drawing.Point(320, 12);
            this.lblLocalAddress.Name = "lblLocalAddress";
            this.lblLocalAddress.Size = new System.Drawing.Size(132, 18);
            this.lblLocalAddress.TabIndex = 1;
            this.lblLocalAddress.Text = "Local Address";
            // 
            // txtLocalAddress
            // 
            this.txtLocalAddress.Enabled = false;
            this.txtLocalAddress.Location = new System.Drawing.Point(320, 33);
            this.txtLocalAddress.Name = "txtLocalAddress";
            this.txtLocalAddress.Size = new System.Drawing.Size(132, 20);
            this.txtLocalAddress.TabIndex = 2;
            // 
            // lblLocalPort
            // 
            this.lblLocalPort.Location = new System.Drawing.Point(320, 61);
            this.lblLocalPort.Name = "lblLocalPort";
            this.lblLocalPort.Size = new System.Drawing.Size(132, 18);
            this.lblLocalPort.TabIndex = 3;
            this.lblLocalPort.Text = "Local Port";
            // 
            // txtLocalPort
            // 
            this.txtLocalPort.Enabled = false;
            this.txtLocalPort.Location = new System.Drawing.Point(320, 82);
            this.txtLocalPort.Name = "txtLocalPort";
            this.txtLocalPort.Size = new System.Drawing.Size(132, 20);
            this.txtLocalPort.TabIndex = 4;
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(320, 117);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(132, 23);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(320, 146);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(132, 23);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 242);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(464, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(54, 17);
            this.lblStatus.Text = "<status>";
            // 
            // lnkGitHub
            // 
            this.lnkGitHub.Location = new System.Drawing.Point(12, 216);
            this.lnkGitHub.Name = "lnkGitHub";
            this.lnkGitHub.Size = new System.Drawing.Size(302, 23);
            this.lnkGitHub.TabIndex = 8;
            this.lnkGitHub.TabStop = true;
            this.lnkGitHub.Text = "https://github.com/mikeclayton/PiFaceSharp";
            this.lnkGitHub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGitHub_LinkClicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 264);
            this.Controls.Add(this.lnkGitHub);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtLocalPort);
            this.Controls.Add(this.lblLocalPort);
            this.Controls.Add(this.txtLocalAddress);
            this.Controls.Add(this.lblLocalAddress);
            this.Controls.Add(this.PiFacePreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Virtual PiFace";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PiFacePreview)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PiFacePreview;
        private System.Windows.Forms.Label lblLocalAddress;
        private System.Windows.Forms.TextBox txtLocalAddress;
        private System.Windows.Forms.Label lblLocalPort;
        private System.Windows.Forms.TextBox txtLocalPort;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.LinkLabel lnkGitHub;
    }
}

