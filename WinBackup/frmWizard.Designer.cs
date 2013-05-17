namespace WinBackup
{
    partial class frmWizard
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
            this.pnlTaskSelect1 = new WinBackup.pnlTaskSelect();
            this.SuspendLayout();
            // 
            // pnlTaskSelect1
            // 
            this.pnlTaskSelect1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTaskSelect1.Location = new System.Drawing.Point(-1, -2);
            this.pnlTaskSelect1.Name = "pnlTaskSelect1";
            this.pnlTaskSelect1.Size = new System.Drawing.Size(1134, 427);
            this.pnlTaskSelect1.TabIndex = 0;
            // 
            // frmWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1133, 426);
            this.Controls.Add(this.pnlTaskSelect1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1149, 464);
            this.Name = "frmWizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Job-Assistent";
            this.ResumeLayout(false);

        }

        #endregion

        private pnlTaskSelect pnlTaskSelect1;
    }
}