namespace Nolte.UI
{
    partial class VirtualFolder
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
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFilesize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFiledate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVirtualFiledate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.largeImageList = new System.Windows.Forms.ImageList(this.components);
            this.smallImageList = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.listeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kachelnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kleineSymboleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.großeSymboleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.itemContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.öffnenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.öffnenMitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speichernUnterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.löschenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.umbenennenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.itemContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFilename,
            this.colFilesize,
            this.colFiledate,
            this.colVirtualFiledate});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.LargeImageList = this.largeImageList;
            this.listView1.Location = new System.Drawing.Point(0, 25);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(900, 416);
            this.listView1.SmallImageList = this.smallImageList;
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView1_DragDrop);
            this.listView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView1_DragEnter);
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // colFilename
            // 
            this.colFilename.Text = "Dateiname";
            this.colFilename.Width = 350;
            // 
            // colFilesize
            // 
            this.colFilesize.Text = "Dateigröße";
            this.colFilesize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colFilesize.Width = 80;
            // 
            // colFiledate
            // 
            this.colFiledate.Text = "Änderungsdatum";
            this.colFiledate.Width = 120;
            // 
            // colVirtualFiledate
            // 
            this.colVirtualFiledate.Text = "Hinzufügedatum";
            this.colVirtualFiledate.Width = 120;
            // 
            // largeImageList
            // 
            this.largeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.largeImageList.ImageSize = new System.Drawing.Size(32, 32);
            this.largeImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // smallImageList
            // 
            this.smallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.smallImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.smallImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(900, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listeToolStripMenuItem,
            this.detailsToolStripMenuItem,
            this.kachelnToolStripMenuItem,
            this.kleineSymboleToolStripMenuItem,
            this.großeSymboleToolStripMenuItem});
            this.toolStripButton1.Image = global::Nolte.UI.Resources.application_sidebar_list;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(29, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // listeToolStripMenuItem
            // 
            this.listeToolStripMenuItem.Name = "listeToolStripMenuItem";
            this.listeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.listeToolStripMenuItem.Text = "Liste";
            this.listeToolStripMenuItem.Click += new System.EventHandler(this.changeDisplayStyle);
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.detailsToolStripMenuItem.Text = "Details";
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.changeDisplayStyle);
            // 
            // kachelnToolStripMenuItem
            // 
            this.kachelnToolStripMenuItem.Name = "kachelnToolStripMenuItem";
            this.kachelnToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.kachelnToolStripMenuItem.Text = "Kacheln";
            this.kachelnToolStripMenuItem.Click += new System.EventHandler(this.changeDisplayStyle);
            // 
            // kleineSymboleToolStripMenuItem
            // 
            this.kleineSymboleToolStripMenuItem.Name = "kleineSymboleToolStripMenuItem";
            this.kleineSymboleToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.kleineSymboleToolStripMenuItem.Text = "Kleine Symbole";
            this.kleineSymboleToolStripMenuItem.Click += new System.EventHandler(this.changeDisplayStyle);
            // 
            // großeSymboleToolStripMenuItem
            // 
            this.großeSymboleToolStripMenuItem.Checked = true;
            this.großeSymboleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.großeSymboleToolStripMenuItem.Name = "großeSymboleToolStripMenuItem";
            this.großeSymboleToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.großeSymboleToolStripMenuItem.Text = "Große Symbole";
            this.großeSymboleToolStripMenuItem.Click += new System.EventHandler(this.changeDisplayStyle);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::Nolte.UI.Resources.folder_open_document;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Datei hinzufügen";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::Nolte.UI.Resources.folder__plus;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "Ordner hinzufügen";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // itemContextMenu
            // 
            this.itemContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.öffnenToolStripMenuItem,
            this.öffnenMitToolStripMenuItem,
            this.speichernUnterToolStripMenuItem,
            this.umbenennenToolStripMenuItem,
            this.löschenToolStripMenuItem});
            this.itemContextMenu.Name = "itemContextMenu";
            this.itemContextMenu.Size = new System.Drawing.Size(170, 136);
            // 
            // öffnenToolStripMenuItem
            // 
            this.öffnenToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.öffnenToolStripMenuItem.Name = "öffnenToolStripMenuItem";
            this.öffnenToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.öffnenToolStripMenuItem.Text = "Öffnen";
            this.öffnenToolStripMenuItem.Click += new System.EventHandler(this.öffnenToolStripMenuItem_Click);
            // 
            // öffnenMitToolStripMenuItem
            // 
            this.öffnenMitToolStripMenuItem.Name = "öffnenMitToolStripMenuItem";
            this.öffnenMitToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.öffnenMitToolStripMenuItem.Text = "Öffnen mit ...";
            this.öffnenMitToolStripMenuItem.Click += new System.EventHandler(this.öffnenMitToolStripMenuItem_Click);
            // 
            // speichernUnterToolStripMenuItem
            // 
            this.speichernUnterToolStripMenuItem.Image = global::Nolte.UI.Resources.save;
            this.speichernUnterToolStripMenuItem.Name = "speichernUnterToolStripMenuItem";
            this.speichernUnterToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.speichernUnterToolStripMenuItem.Text = "Speichern unter ...";
            this.speichernUnterToolStripMenuItem.Click += new System.EventHandler(this.speichernUnterToolStripMenuItem_Click);
            // 
            // löschenToolStripMenuItem
            // 
            this.löschenToolStripMenuItem.Image = global::Nolte.UI.Resources.cross_script;
            this.löschenToolStripMenuItem.Name = "löschenToolStripMenuItem";
            this.löschenToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.löschenToolStripMenuItem.Text = "Löschen";
            this.löschenToolStripMenuItem.Click += new System.EventHandler(this.löschenToolStripMenuItem_Click);
            // 
            // umbenennenToolStripMenuItem
            // 
            this.umbenennenToolStripMenuItem.Image = global::Nolte.UI.Resources.document_rename;
            this.umbenennenToolStripMenuItem.Name = "umbenennenToolStripMenuItem";
            this.umbenennenToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.umbenennenToolStripMenuItem.Text = "Umbenennen";
            this.umbenennenToolStripMenuItem.Click += new System.EventHandler(this.umbenennenToolStripMenuItem_Click);
            // 
            // VirtualFolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "VirtualFolder";
            this.Size = new System.Drawing.Size(900, 441);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.itemContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ImageList smallImageList;
        private System.Windows.Forms.ImageList largeImageList;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem listeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kachelnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kleineSymboleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem großeSymboleToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader colFilename;
        private System.Windows.Forms.ColumnHeader colFilesize;
        private System.Windows.Forms.ColumnHeader colFiledate;
        private System.Windows.Forms.ColumnHeader colVirtualFiledate;
        private System.Windows.Forms.ContextMenuStrip itemContextMenu;
        private System.Windows.Forms.ToolStripMenuItem speichernUnterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem löschenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem öffnenMitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem öffnenToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripMenuItem umbenennenToolStripMenuItem;
    }
}
