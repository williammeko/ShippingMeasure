namespace ShippingMeasure.DataConverting
{
    partial class TankCapacityImportingForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuAddRawDataFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemoveSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSelectAccessFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuImport = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listTxtFiles = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkWrapOutput = new System.Windows.Forms.CheckBox();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAccessFile = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddRawDataFiles,
            this.menuRemoveSelected,
            this.menuSelectAccessFile,
            this.menuImport});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(976, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuAddRawDataFiles
            // 
            this.menuAddRawDataFiles.Name = "menuAddRawDataFiles";
            this.menuAddRawDataFiles.Size = new System.Drawing.Size(113, 20);
            this.menuAddRawDataFiles.Text = "Add raw data files";
            // 
            // menuRemoveSelected
            // 
            this.menuRemoveSelected.Enabled = false;
            this.menuRemoveSelected.Name = "menuRemoveSelected";
            this.menuRemoveSelected.Size = new System.Drawing.Size(108, 20);
            this.menuRemoveSelected.Text = "Remove selected";
            // 
            // menuSelectAccessFile
            // 
            this.menuSelectAccessFile.Name = "menuSelectAccessFile";
            this.menuSelectAccessFile.Size = new System.Drawing.Size(121, 20);
            this.menuSelectAccessFile.Text = "Select target DB file";
            // 
            // menuImport
            // 
            this.menuImport.Name = "menuImport";
            this.menuImport.Size = new System.Drawing.Size(82, 20);
            this.menuImport.Text = "Start import";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 478);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(976, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.MinimumSize = new System.Drawing.Size(760, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listTxtFiles);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.txtAccessFile);
            this.splitContainer1.Panel2MinSize = 600;
            this.splitContainer1.Size = new System.Drawing.Size(976, 454);
            this.splitContainer1.SplitterDistance = 358;
            this.splitContainer1.TabIndex = 2;
            // 
            // listTxtFiles
            // 
            this.listTxtFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listTxtFiles.FormattingEnabled = true;
            this.listTxtFiles.HorizontalScrollbar = true;
            this.listTxtFiles.Location = new System.Drawing.Point(3, 3);
            this.listTxtFiles.Name = "listTxtFiles";
            this.listTxtFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listTxtFiles.Size = new System.Drawing.Size(352, 446);
            this.listTxtFiles.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkWrapOutput);
            this.groupBox1.Controls.Add(this.txtOutput);
            this.groupBox1.Location = new System.Drawing.Point(7, 86);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(604, 365);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // chkWrapOutput
            // 
            this.chkWrapOutput.AutoSize = true;
            this.chkWrapOutput.Location = new System.Drawing.Point(6, 20);
            this.chkWrapOutput.Name = "chkWrapOutput";
            this.chkWrapOutput.Size = new System.Drawing.Size(52, 17);
            this.chkWrapOutput.TabIndex = 1;
            this.chkWrapOutput.Text = "Wrap";
            this.chkWrapOutput.UseVisualStyleBackColor = true;
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.HideSelection = false;
            this.txtOutput.Location = new System.Drawing.Point(6, 43);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(592, 316);
            this.txtOutput.TabIndex = 0;
            this.txtOutput.Text = "";
            this.txtOutput.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.MinimumSize = new System.Drawing.Size(595, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(595, 52);
            this.label1.TabIndex = 1;
            this.label1.Text = "Target Access file\r\n\r\nPlease pay attention that, ALL TANK CAPACITY DATA WILL BE R" +
    "EMOVED, during importing operation.\r\nMake sure that you have a backup file befor" +
    "e importing operation.";
            // 
            // txtAccessFile
            // 
            this.txtAccessFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAccessFile.Location = new System.Drawing.Point(3, 59);
            this.txtAccessFile.Name = "txtAccessFile";
            this.txtAccessFile.ReadOnly = true;
            this.txtAccessFile.Size = new System.Drawing.Size(608, 20);
            this.txtAccessFile.TabIndex = 0;
            // 
            // TankCapacityImportingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 500);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TankCapacityImportingForm";
            this.Text = "Import tank capacity data";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuAddRawDataFiles;
        private System.Windows.Forms.ToolStripMenuItem menuRemoveSelected;
        private System.Windows.Forms.ToolStripMenuItem menuSelectAccessFile;
        private System.Windows.Forms.ToolStripMenuItem menuImport;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listTxtFiles;
        private System.Windows.Forms.TextBox txtAccessFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox txtOutput;
        private System.Windows.Forms.CheckBox chkWrapOutput;
    }
}

