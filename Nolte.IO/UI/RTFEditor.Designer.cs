namespace Nolte.UI
{
    partial class RTFEditor
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.rtfTextEditor = new System.Windows.Forms.RichTextBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.rtfCmbFontFamily = new System.Windows.Forms.ToolStripComboBox();
            this.rtfCmbFontSize = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.rtfToolBar = new System.Windows.Forms.ToolStrip();
            this.rtfBtnSaveAs = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnClearDocument = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnUndo = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnRedo = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnCopy = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnCut = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnBold = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnItalic = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnUnderline = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnStrikeout = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnChooseColor = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnFontDialog = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnHangingOutdent = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnHangingIndent = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnList = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnOutdent = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnIndent = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnLeft = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnCenter = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnRight = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnWordWrap = new System.Windows.Forms.ToolStripButton();
            this.rtfBtnInsertImage = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.rtfToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.rtfTextEditor);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10, 10, 0, 10);
            this.panel1.Size = new System.Drawing.Size(650, 397);
            this.panel1.TabIndex = 1;
            // 
            // rtfTextEditor
            // 
            this.rtfTextEditor.AcceptsTab = true;
            this.rtfTextEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtfTextEditor.BulletIndent = 5;
            this.rtfTextEditor.EnableAutoDragDrop = true;
            this.rtfTextEditor.Location = new System.Drawing.Point(13, 57);
            this.rtfTextEditor.Margin = new System.Windows.Forms.Padding(10);
            this.rtfTextEditor.Name = "rtfTextEditor";
            this.rtfTextEditor.RightMargin = 10;
            this.rtfTextEditor.Size = new System.Drawing.Size(620, 323);
            this.rtfTextEditor.TabIndex = 0;
            this.rtfTextEditor.Text = "";
            this.rtfTextEditor.TextChanged += new System.EventHandler(this.rtfTextEditor_TextChanged);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 23);
            // 
            // rtfCmbFontFamily
            // 
            this.rtfCmbFontFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rtfCmbFontFamily.Name = "rtfCmbFontFamily";
            this.rtfCmbFontFamily.Size = new System.Drawing.Size(200, 23);
            this.rtfCmbFontFamily.SelectedIndexChanged += new System.EventHandler(this.rtfCmbFontFamily_SelectedIndexChanged);
            // 
            // rtfCmbFontSize
            // 
            this.rtfCmbFontSize.DropDownWidth = 50;
            this.rtfCmbFontSize.Items.AddRange(new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72"});
            this.rtfCmbFontSize.Name = "rtfCmbFontSize";
            this.rtfCmbFontSize.Size = new System.Drawing.Size(75, 23);
            this.rtfCmbFontSize.SelectedIndexChanged += new System.EventHandler(this.rtfCmbFontSize_SelectedIndexChanged);
            this.rtfCmbFontSize.TextChanged += new System.EventHandler(this.rtfCmbFontSize_TextChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
            // 
            // rtfToolBar
            // 
            this.rtfToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rtfBtnSaveAs,
            this.toolStripButton6,
            this.rtfBtnClearDocument,
            this.toolStripSeparator6,
            this.rtfBtnUndo,
            this.rtfBtnRedo,
            this.rtfBtnCopy,
            this.rtfBtnCut,
            this.rtfBtnPaste,
            this.toolStripButton7,
            this.toolStripButton8,
            this.toolStripSeparator5,
            this.rtfBtnBold,
            this.rtfBtnItalic,
            this.rtfBtnUnderline,
            this.rtfBtnStrikeout,
            this.toolStripSeparator4,
            this.toolStripButton1,
            this.rtfBtnChooseColor,
            this.rtfCmbFontFamily,
            this.rtfCmbFontSize,
            this.rtfBtnFontDialog,
            this.toolStripSeparator2,
            this.rtfBtnHangingOutdent,
            this.rtfBtnHangingIndent,
            this.rtfBtnList,
            this.rtfBtnOutdent,
            this.rtfBtnIndent,
            this.rtfBtnLeft,
            this.rtfBtnCenter,
            this.rtfBtnRight,
            this.toolStripSeparator3,
            this.rtfBtnWordWrap,
            this.rtfBtnInsertImage,
            this.toolStripSeparator1,
            this.toolStripButton2});
            this.rtfToolBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.rtfToolBar.Location = new System.Drawing.Point(0, 0);
            this.rtfToolBar.Name = "rtfToolBar";
            this.rtfToolBar.Size = new System.Drawing.Size(650, 46);
            this.rtfToolBar.TabIndex = 2;
            this.rtfToolBar.Text = "rtfToolBar";
            // 
            // rtfBtnSaveAs
            // 
            this.rtfBtnSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnSaveAs.Image = global::Nolte.UI.Resources.save;
            this.rtfBtnSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnSaveAs.Name = "rtfBtnSaveAs";
            this.rtfBtnSaveAs.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnSaveAs.Text = "(extern) Speichern";
            this.rtfBtnSaveAs.Click += new System.EventHandler(this.rtfBtnSaveAs_Click);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton6.Image = global::Nolte.UI.Resources.folder_open_document;
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(23, 20);
            this.toolStripButton6.Text = "toolStripButton6";
            this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // rtfBtnClearDocument
            // 
            this.rtfBtnClearDocument.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnClearDocument.Image = global::Nolte.UI.Resources.document_new;
            this.rtfBtnClearDocument.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnClearDocument.Name = "rtfBtnClearDocument";
            this.rtfBtnClearDocument.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnClearDocument.Text = "Leeren";
            this.rtfBtnClearDocument.Click += new System.EventHandler(this.rtfBtnClearDocument_Click);
            // 
            // rtfBtnUndo
            // 
            this.rtfBtnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnUndo.Image = global::Nolte.UI.Resources.undo;
            this.rtfBtnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnUndo.Name = "rtfBtnUndo";
            this.rtfBtnUndo.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnUndo.Text = "Rückgängig";
            this.rtfBtnUndo.Click += new System.EventHandler(this.rtfBtnUndo_Click);
            // 
            // rtfBtnRedo
            // 
            this.rtfBtnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnRedo.Image = global::Nolte.UI.Resources.redo;
            this.rtfBtnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnRedo.Name = "rtfBtnRedo";
            this.rtfBtnRedo.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnRedo.Text = "Wiederholen";
            this.rtfBtnRedo.Click += new System.EventHandler(this.rtfBtnRedo_Click);
            // 
            // rtfBtnCopy
            // 
            this.rtfBtnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnCopy.Image = global::Nolte.UI.Resources.copy;
            this.rtfBtnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnCopy.Name = "rtfBtnCopy";
            this.rtfBtnCopy.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnCopy.Text = "Kopieren";
            this.rtfBtnCopy.Click += new System.EventHandler(this.rtfBtnCopy_Click);
            // 
            // rtfBtnCut
            // 
            this.rtfBtnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnCut.Image = global::Nolte.UI.Resources.cut;
            this.rtfBtnCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnCut.Name = "rtfBtnCut";
            this.rtfBtnCut.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnCut.Text = "Ausschneiden";
            this.rtfBtnCut.Click += new System.EventHandler(this.rtfBtnCut_Click);
            // 
            // rtfBtnPaste
            // 
            this.rtfBtnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnPaste.Image = global::Nolte.UI.Resources.paste;
            this.rtfBtnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnPaste.Name = "rtfBtnPaste";
            this.rtfBtnPaste.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnPaste.Text = "Einfügen";
            this.rtfBtnPaste.Click += new System.EventHandler(this.rtfBtnPaste_Click);
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton7.Image = global::Nolte.UI.Resources.spectacle_sunglass;
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(23, 20);
            this.toolStripButton7.Text = "Suchen";
            this.toolStripButton7.Click += new System.EventHandler(this.toolStripButton7_Click);
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton8.Image = global::Nolte.UI.Resources.edit_replace;
            this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Size = new System.Drawing.Size(23, 20);
            this.toolStripButton8.Text = "Ersetzen";
            this.toolStripButton8.Click += new System.EventHandler(this.toolStripButton8_Click);
            // 
            // rtfBtnBold
            // 
            this.rtfBtnBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnBold.Image = global::Nolte.UI.Resources.edit_bold;
            this.rtfBtnBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnBold.Name = "rtfBtnBold";
            this.rtfBtnBold.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnBold.Text = "Fett";
            this.rtfBtnBold.Click += new System.EventHandler(this.rtfBtnBold_Click);
            // 
            // rtfBtnItalic
            // 
            this.rtfBtnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnItalic.Image = global::Nolte.UI.Resources.edit_italic;
            this.rtfBtnItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnItalic.Name = "rtfBtnItalic";
            this.rtfBtnItalic.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnItalic.Text = "Kursiv";
            this.rtfBtnItalic.Click += new System.EventHandler(this.rtfBtnItalic_Click);
            // 
            // rtfBtnUnderline
            // 
            this.rtfBtnUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnUnderline.Image = global::Nolte.UI.Resources.edit_underline;
            this.rtfBtnUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnUnderline.Name = "rtfBtnUnderline";
            this.rtfBtnUnderline.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnUnderline.Text = "Unterstrichen";
            this.rtfBtnUnderline.Click += new System.EventHandler(this.rtfBtnUnderline_Click);
            // 
            // rtfBtnStrikeout
            // 
            this.rtfBtnStrikeout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnStrikeout.Image = global::Nolte.UI.Resources.edit_strike;
            this.rtfBtnStrikeout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnStrikeout.Name = "rtfBtnStrikeout";
            this.rtfBtnStrikeout.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnStrikeout.Text = "Durcgestrichen";
            this.rtfBtnStrikeout.Click += new System.EventHandler(this.rtfBtnStrikeout_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Nolte.UI.Resources.paint_can_color;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 20);
            this.toolStripButton1.Text = "Hintergrundfarbe wählen";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // rtfBtnChooseColor
            // 
            this.rtfBtnChooseColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnChooseColor.Image = global::Nolte.UI.Resources.edit_color;
            this.rtfBtnChooseColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnChooseColor.Name = "rtfBtnChooseColor";
            this.rtfBtnChooseColor.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnChooseColor.Text = "Schriftfarbe";
            this.rtfBtnChooseColor.Click += new System.EventHandler(this.rtfBtnChooseColor_Click);
            // 
            // rtfBtnFontDialog
            // 
            this.rtfBtnFontDialog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnFontDialog.Image = global::Nolte.UI.Resources.font;
            this.rtfBtnFontDialog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnFontDialog.Name = "rtfBtnFontDialog";
            this.rtfBtnFontDialog.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnFontDialog.Text = "Schriftart wählen";
            this.rtfBtnFontDialog.Click += new System.EventHandler(this.rtfBtnFontDialog_Click);
            // 
            // rtfBtnHangingOutdent
            // 
            this.rtfBtnHangingOutdent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnHangingOutdent.Image = global::Nolte.UI.Resources.T_Outdent_Sm_N;
            this.rtfBtnHangingOutdent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnHangingOutdent.Name = "rtfBtnHangingOutdent";
            this.rtfBtnHangingOutdent.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnHangingOutdent.Text = "Ausrücken";
            this.rtfBtnHangingOutdent.Click += new System.EventHandler(this.rtfBtnHangingOutdent_Click);
            // 
            // rtfBtnHangingIndent
            // 
            this.rtfBtnHangingIndent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnHangingIndent.Image = global::Nolte.UI.Resources.T_Indent_Sm_N;
            this.rtfBtnHangingIndent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnHangingIndent.Name = "rtfBtnHangingIndent";
            this.rtfBtnHangingIndent.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnHangingIndent.Text = "Einrücken";
            this.rtfBtnHangingIndent.Click += new System.EventHandler(this.rtfBtnHangingIndent_Click);
            // 
            // rtfBtnList
            // 
            this.rtfBtnList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnList.Image = global::Nolte.UI.Resources.edit_list;
            this.rtfBtnList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnList.Name = "rtfBtnList";
            this.rtfBtnList.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnList.Text = "Aufzählung";
            this.rtfBtnList.Click += new System.EventHandler(this.rtfBtnList_Click);
            // 
            // rtfBtnOutdent
            // 
            this.rtfBtnOutdent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnOutdent.Image = global::Nolte.UI.Resources.outdent;
            this.rtfBtnOutdent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnOutdent.Name = "rtfBtnOutdent";
            this.rtfBtnOutdent.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnOutdent.Text = "Ausrücken";
            this.rtfBtnOutdent.Click += new System.EventHandler(this.rtfBtnOutdent_Click);
            // 
            // rtfBtnIndent
            // 
            this.rtfBtnIndent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnIndent.Image = global::Nolte.UI.Resources.indent;
            this.rtfBtnIndent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnIndent.Name = "rtfBtnIndent";
            this.rtfBtnIndent.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnIndent.Text = "Einrücken";
            this.rtfBtnIndent.Click += new System.EventHandler(this.rtfBtnIndent_Click);
            // 
            // rtfBtnLeft
            // 
            this.rtfBtnLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnLeft.Image = global::Nolte.UI.Resources.edit_alignment;
            this.rtfBtnLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnLeft.Name = "rtfBtnLeft";
            this.rtfBtnLeft.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnLeft.Text = "Linksbündig";
            this.rtfBtnLeft.Click += new System.EventHandler(this.rtfBtnLeft_Click);
            // 
            // rtfBtnCenter
            // 
            this.rtfBtnCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnCenter.Image = global::Nolte.UI.Resources.edit_alignment_center;
            this.rtfBtnCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnCenter.Name = "rtfBtnCenter";
            this.rtfBtnCenter.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnCenter.Text = "Zentriert";
            this.rtfBtnCenter.Click += new System.EventHandler(this.rtfBtnCenter_Click);
            // 
            // rtfBtnRight
            // 
            this.rtfBtnRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnRight.Image = global::Nolte.UI.Resources.edit_alignment_right;
            this.rtfBtnRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnRight.Name = "rtfBtnRight";
            this.rtfBtnRight.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnRight.Text = "Rechtsbündig";
            this.rtfBtnRight.Click += new System.EventHandler(this.rtfBtnRight_Click);
            // 
            // rtfBtnWordWrap
            // 
            this.rtfBtnWordWrap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnWordWrap.Image = global::Nolte.UI.Resources.edit_pilcrow;
            this.rtfBtnWordWrap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnWordWrap.Name = "rtfBtnWordWrap";
            this.rtfBtnWordWrap.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnWordWrap.Text = "Automatischer Zeilenumbruch";
            this.rtfBtnWordWrap.Click += new System.EventHandler(this.rtfBtnWordWrap_Click);
            // 
            // rtfBtnInsertImage
            // 
            this.rtfBtnInsertImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rtfBtnInsertImage.Image = global::Nolte.UI.Resources.image__plus;
            this.rtfBtnInsertImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rtfBtnInsertImage.Name = "rtfBtnInsertImage";
            this.rtfBtnInsertImage.Size = new System.Drawing.Size(23, 20);
            this.rtfBtnInsertImage.Text = "Bild einfügen";
            this.rtfBtnInsertImage.Click += new System.EventHandler(this.rtfBtnInsertImage_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::Nolte.UI.Resources.ui_color_picker;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 20);
            this.toolStripButton2.Text = "Dokument-Hintergrund";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // RTFEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtfToolBar);
            this.Controls.Add(this.panel1);
            this.Name = "RTFEditor";
            this.Size = new System.Drawing.Size(650, 397);
            this.panel1.ResumeLayout(false);
            this.rtfToolBar.ResumeLayout(false);
            this.rtfToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox rtfTextEditor;
        private System.Windows.Forms.ToolStripButton rtfBtnSaveAs;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ToolStripButton rtfBtnClearDocument;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton rtfBtnUndo;
        private System.Windows.Forms.ToolStripButton rtfBtnRedo;
        private System.Windows.Forms.ToolStripButton rtfBtnCopy;
        private System.Windows.Forms.ToolStripButton rtfBtnCut;
        private System.Windows.Forms.ToolStripButton rtfBtnPaste;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripButton toolStripButton8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton rtfBtnBold;
        private System.Windows.Forms.ToolStripButton rtfBtnItalic;
        private System.Windows.Forms.ToolStripButton rtfBtnUnderline;
        private System.Windows.Forms.ToolStripButton rtfBtnStrikeout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton rtfBtnChooseColor;
        private System.Windows.Forms.ToolStripComboBox rtfCmbFontFamily;
        private System.Windows.Forms.ToolStripComboBox rtfCmbFontSize;
        private System.Windows.Forms.ToolStripButton rtfBtnFontDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton rtfBtnHangingOutdent;
        private System.Windows.Forms.ToolStripButton rtfBtnHangingIndent;
        private System.Windows.Forms.ToolStripButton rtfBtnList;
        private System.Windows.Forms.ToolStripButton rtfBtnOutdent;
        private System.Windows.Forms.ToolStripButton rtfBtnIndent;
        private System.Windows.Forms.ToolStripButton rtfBtnLeft;
        private System.Windows.Forms.ToolStripButton rtfBtnCenter;
        private System.Windows.Forms.ToolStripButton rtfBtnRight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton rtfBtnWordWrap;
        private System.Windows.Forms.ToolStripButton rtfBtnInsertImage;
        private System.Windows.Forms.ToolStrip rtfToolBar;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;

    }
}
