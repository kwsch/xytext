namespace xytext
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
            this.B_SaveText = new System.Windows.Forms.Button();
            this.TB_Path = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_openFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_DumpTXT = new System.Windows.Forms.ToolStripMenuItem();
            this.CB_Entry = new System.Windows.Forms.ComboBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.B_AddLine = new System.Windows.Forms.Button();
            this.B_RemoveLine = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // B_SaveText
            // 
            this.B_SaveText.Enabled = false;
            this.B_SaveText.Location = new System.Drawing.Point(12, 26);
            this.B_SaveText.Name = "B_SaveText";
            this.B_SaveText.Size = new System.Drawing.Size(88, 24);
            this.B_SaveText.TabIndex = 2;
            this.B_SaveText.Text = "Save Text File";
            this.B_SaveText.UseVisualStyleBackColor = true;
            this.B_SaveText.Click += new System.EventHandler(this.B_SaveText_Click);
            // 
            // TB_Path
            // 
            this.TB_Path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Path.Location = new System.Drawing.Point(191, 28);
            this.TB_Path.Name = "TB_Path";
            this.TB_Path.ReadOnly = true;
            this.TB_Path.Size = new System.Drawing.Size(431, 20);
            this.TB_Path.TabIndex = 3;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(634, 23);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_openFolder});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 19);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // menu_openFolder
            // 
            this.menu_openFolder.Name = "menu_openFolder";
            this.menu_openFolder.Size = new System.Drawing.Size(139, 22);
            this.menu_openFolder.Text = "Open Folder";
            this.menu_openFolder.Click += new System.EventHandler(this.openFolder_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_DumpTXT});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 19);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // menu_DumpTXT
            // 
            this.menu_DumpTXT.Name = "menu_DumpTXT";
            this.menu_DumpTXT.Size = new System.Drawing.Size(152, 22);
            this.menu_DumpTXT.Text = "Dump to TXT";
            this.menu_DumpTXT.Click += new System.EventHandler(this.dumpTXT_Click);
            // 
            // CB_Entry
            // 
            this.CB_Entry.Enabled = false;
            this.CB_Entry.FormattingEnabled = true;
            this.CB_Entry.Location = new System.Drawing.Point(106, 28);
            this.CB_Entry.Name = "CB_Entry";
            this.CB_Entry.Size = new System.Drawing.Size(79, 21);
            this.CB_Entry.TabIndex = 5;
            this.CB_Entry.SelectedIndexChanged += new System.EventHandler(this.changeEntry);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(12, 56);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv.ShowEditingIcon = false;
            this.dgv.Size = new System.Drawing.Size(610, 294);
            this.dgv.TabIndex = 0;
            // 
            // B_AddLine
            // 
            this.B_AddLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_AddLine.Enabled = false;
            this.B_AddLine.Location = new System.Drawing.Point(355, 2);
            this.B_AddLine.Name = "B_AddLine";
            this.B_AddLine.Size = new System.Drawing.Size(130, 23);
            this.B_AddLine.TabIndex = 6;
            this.B_AddLine.Text = "Add Line After Line";
            this.B_AddLine.UseVisualStyleBackColor = true;
            this.B_AddLine.Click += new System.EventHandler(this.B_AddLine_Click);
            // 
            // B_RemoveLine
            // 
            this.B_RemoveLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_RemoveLine.Enabled = false;
            this.B_RemoveLine.Location = new System.Drawing.Point(492, 2);
            this.B_RemoveLine.Name = "B_RemoveLine";
            this.B_RemoveLine.Size = new System.Drawing.Size(130, 23);
            this.B_RemoveLine.TabIndex = 7;
            this.B_RemoveLine.Text = "Remove Selected Line";
            this.B_RemoveLine.UseVisualStyleBackColor = true;
            this.B_RemoveLine.Click += new System.EventHandler(this.B_RemoveLine_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 362);
            this.Controls.Add(this.B_RemoveLine);
            this.Controls.Add(this.B_AddLine);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.CB_Entry);
            this.Controls.Add(this.TB_Path);
            this.Controls.Add(this.B_SaveText);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "Form1";
            this.Text = "xytext";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_SaveText;
        private System.Windows.Forms.TextBox TB_Path;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menu_openFolder;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menu_DumpTXT;
        private System.Windows.Forms.ComboBox CB_Entry;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button B_AddLine;
        private System.Windows.Forms.Button B_RemoveLine;

    }
}

