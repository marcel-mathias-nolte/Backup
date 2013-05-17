namespace WinBackup
{
    partial class pnlTaskSelect
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(pnlTaskSelect));
            this.lblWhatWouldYouDo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBackupIncremental = new System.Windows.Forms.Button();
            this.btnBackupMirror = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBackupProtocol = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWhatWouldYouDo
            // 
            this.lblWhatWouldYouDo.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWhatWouldYouDo.Location = new System.Drawing.Point(3, 23);
            this.lblWhatWouldYouDo.Name = "lblWhatWouldYouDo";
            this.lblWhatWouldYouDo.Size = new System.Drawing.Size(1138, 33);
            this.lblWhatWouldYouDo.TabIndex = 0;
            this.lblWhatWouldYouDo.Text = "Wie möchten Sie sichern?";
            this.lblWhatWouldYouDo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnBackupIncremental, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnBackupMirror, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnBackupProtocol, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(21, 73);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1108, 355);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(372, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(363, 247);
            this.label3.TabIndex = 10;
            this.label3.Text = resources.GetString("label3.Text");
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(363, 247);
            this.label2.TabIndex = 9;
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnBackupIncremental
            // 
            this.btnBackupIncremental.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBackupIncremental.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackupIncremental.Location = new System.Drawing.Point(372, 250);
            this.btnBackupIncremental.Name = "btnBackupIncremental";
            this.btnBackupIncremental.Size = new System.Drawing.Size(363, 102);
            this.btnBackupIncremental.TabIndex = 5;
            this.btnBackupIncremental.Text = "Snapshot-Erstellung";
            this.btnBackupIncremental.UseVisualStyleBackColor = true;
            // 
            // btnBackupMirror
            // 
            this.btnBackupMirror.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBackupMirror.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackupMirror.Location = new System.Drawing.Point(3, 250);
            this.btnBackupMirror.Name = "btnBackupMirror";
            this.btnBackupMirror.Size = new System.Drawing.Size(363, 102);
            this.btnBackupMirror.TabIndex = 4;
            this.btnBackupMirror.Text = "Spiegeln / Synchronisieren";
            this.btnBackupMirror.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(741, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(364, 247);
            this.label1.TabIndex = 8;
            this.label1.Text = "Protokolliert alle Änderungen und \r\n\r\nberechnet die zu übertragende Datenmenge.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnBackupProtocol
            // 
            this.btnBackupProtocol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBackupProtocol.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackupProtocol.Location = new System.Drawing.Point(741, 250);
            this.btnBackupProtocol.Name = "btnBackupProtocol";
            this.btnBackupProtocol.Size = new System.Drawing.Size(364, 102);
            this.btnBackupProtocol.TabIndex = 6;
            this.btnBackupProtocol.Text = "Änderungsprotokoll";
            this.btnBackupProtocol.UseVisualStyleBackColor = true;
            // 
            // pnlTaskSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblWhatWouldYouDo);
            this.Name = "pnlTaskSelect";
            this.Size = new System.Drawing.Size(1155, 450);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblWhatWouldYouDo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnBackupMirror;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBackupIncremental;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBackupProtocol;
    }
}
