//-----------------------------------------------------------------------
// <copyright file="frmAbout.Designer.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace WinBackup
{
    using System.Diagnostics.CodeAnalysis;

    /// <content>
    /// Designer part of about form
    /// </content>
    public partial class FrmAbout
    {
        /// <summary>
        /// Needed designer variable
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// App title label
        /// </summary>
        private System.Windows.Forms.Label lblAppTitle;

        /// <summary>
        /// App Version label
        /// </summary>
        private System.Windows.Forms.Label lblAppVersion;

        /// <summary>
        /// App Company label
        /// </summary>
        private System.Windows.Forms.Label lblAppCompany;

        /// <summary>
        /// Close button
        /// </summary>
        private System.Windows.Forms.Button btnOk;

        /// <summary>
        /// Cleanup resources
        /// </summary>
        /// <param name="disposing">Whether deposing is in progress.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.lblAppVersion = new System.Windows.Forms.Label();
            this.lblAppCompany = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblAppTitle
            // 
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAppTitle.Location = new System.Drawing.Point(12, 9);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(143, 29);
            this.lblAppTitle.TabIndex = 0;
            this.lblAppTitle.Text = "WinBackup";
            // 
            // lblAppVersion
            // 
            this.lblAppVersion.AutoSize = true;
            this.lblAppVersion.Location = new System.Drawing.Point(163, 21);
            this.lblAppVersion.Name = "lblAppVersion";
            this.lblAppVersion.Size = new System.Drawing.Size(0, 13);
            this.lblAppVersion.TabIndex = 1;
            // 
            // lblAppCompany
            // 
            this.lblAppCompany.AutoSize = true;
            this.lblAppCompany.Location = new System.Drawing.Point(14, 49);
            this.lblAppCompany.Name = "lblAppCompany";
            this.lblAppCompany.Size = new System.Drawing.Size(0, 13);
            this.lblAppCompany.TabIndex = 2;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(157, 93);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 128);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblAppCompany);
            this.Controls.Add(this.lblAppVersion);
            this.Controls.Add(this.lblAppTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAbout";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
