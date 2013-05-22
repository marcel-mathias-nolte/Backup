namespace Nolte.UI
{
    partial class FindForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearchText = new System.Windows.Forms.TextBox();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.chkWholeWord = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Suchbegriff:";
            // 
            // txtSearchText
            // 
            this.txtSearchText.Location = new System.Drawing.Point(87, 13);
            this.txtSearchText.Name = "txtSearchText";
            this.txtSearchText.Size = new System.Drawing.Size(166, 20);
            this.txtSearchText.TabIndex = 1;
            // 
            // btnFindNext
            // 
            this.btnFindNext.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFindNext.Location = new System.Drawing.Point(171, 39);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(82, 23);
            this.btnFindNext.TabIndex = 4;
            this.btnFindNext.Text = "&Weitersuchen";
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
            // 
            // btnDone
            // 
            this.btnDone.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDone.Location = new System.Drawing.Point(171, 68);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(82, 23);
            this.btnDone.TabIndex = 5;
            this.btnDone.Text = "Schließen";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(15, 43);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(132, 17);
            this.chkMatchCase.TabIndex = 2;
            this.chkMatchCase.Text = "Groß-/Kleinschreibung";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // chkWholeWord
            // 
            this.chkWholeWord.AutoSize = true;
            this.chkWholeWord.Location = new System.Drawing.Point(15, 66);
            this.chkWholeWord.Name = "chkWholeWord";
            this.chkWholeWord.Size = new System.Drawing.Size(88, 17);
            this.chkWholeWord.TabIndex = 3;
            this.chkWholeWord.Text = "Ganzes Wort";
            this.chkWholeWord.UseVisualStyleBackColor = true;
            // 
            // FindForm
            // 
            this.AcceptButton = this.btnFindNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnDone;
            this.ClientSize = new System.Drawing.Size(274, 114);
            this.Controls.Add(this.chkWholeWord);
            this.Controls.Add(this.chkMatchCase);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.btnFindNext);
            this.Controls.Add(this.txtSearchText);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Suchen";
            this.Load += new System.EventHandler(this.FindForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSearchText;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.CheckBox chkMatchCase;
        private System.Windows.Forms.CheckBox chkWholeWord;
    }
}