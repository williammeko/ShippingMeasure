namespace ShippingMeasure
{
    partial class ReceiptForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReceiptForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuVolumeByHeight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVolumeByUllage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAutoCalculate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCalculate = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pnlMassOfOil = new System.Windows.Forms.Panel();
            this.txtOperatorName = new System.Windows.Forms.TextBox();
            this.lblOperatorName = new System.Windows.Forms.Label();
            this.pnlDeliveryReceiptLoading = new System.Windows.Forms.Panel();
            this.txtConsigner = new System.Windows.Forms.TextBox();
            this.lblConsigner = new System.Windows.Forms.Label();
            this.txtShipperLoading = new System.Windows.Forms.TextBox();
            this.lblShipperLoading = new System.Windows.Forms.Label();
            this.txtAgentLoading = new System.Windows.Forms.TextBox();
            this.lblAgentLoading = new System.Windows.Forms.Label();
            this.pnlDeliverReceiptDestination = new System.Windows.Forms.Panel();
            this.txtConsignee = new System.Windows.Forms.TextBox();
            this.lblConsignee = new System.Windows.Forms.Label();
            this.txtShipperDestination = new System.Windows.Forms.TextBox();
            this.lblShipperDestination = new System.Windows.Forms.Label();
            this.txtAgentDestination = new System.Windows.Forms.TextBox();
            this.lblAgentDestination = new System.Windows.Forms.Label();
            this.grid = new ShippingMeasure.Controls.MeasureGrid();
            this.summary = new ShippingMeasure.Controls.MeasureGridSummary();
            this.header = new ShippingMeasure.Controls.MeasureHeader();
            this.menuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.pnlMassOfOil.SuspendLayout();
            this.pnlDeliveryReceiptLoading.SuspendLayout();
            this.pnlDeliverReceiptDestination.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.summary)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuTools});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // menuTools
            // 
            this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNew,
            this.menuLoad,
            this.menuSave,
            this.menuPrint,
            this.toolStripMenuItem1,
            this.menuVolumeByHeight,
            this.menuVolumeByUllage,
            this.toolStripMenuItem2,
            this.menuAutoCalculate,
            this.menuCalculate});
            this.menuTools.Name = "menuTools";
            resources.ApplyResources(this.menuTools, "menuTools");
            // 
            // menuNew
            // 
            this.menuNew.Name = "menuNew";
            resources.ApplyResources(this.menuNew, "menuNew");
            // 
            // menuLoad
            // 
            this.menuLoad.Name = "menuLoad";
            resources.ApplyResources(this.menuLoad, "menuLoad");
            // 
            // menuSave
            // 
            resources.ApplyResources(this.menuSave, "menuSave");
            this.menuSave.Name = "menuSave";
            // 
            // menuPrint
            // 
            this.menuPrint.Name = "menuPrint";
            resources.ApplyResources(this.menuPrint, "menuPrint");
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // menuVolumeByHeight
            // 
            this.menuVolumeByHeight.Checked = true;
            this.menuVolumeByHeight.CheckOnClick = true;
            this.menuVolumeByHeight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuVolumeByHeight.Name = "menuVolumeByHeight";
            resources.ApplyResources(this.menuVolumeByHeight, "menuVolumeByHeight");
            // 
            // menuVolumeByUllage
            // 
            this.menuVolumeByUllage.CheckOnClick = true;
            this.menuVolumeByUllage.Name = "menuVolumeByUllage";
            resources.ApplyResources(this.menuVolumeByUllage, "menuVolumeByUllage");
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // menuAutoCalculate
            // 
            this.menuAutoCalculate.Name = "menuAutoCalculate";
            resources.ApplyResources(this.menuAutoCalculate, "menuAutoCalculate");
            // 
            // menuCalculate
            // 
            this.menuCalculate.Name = "menuCalculate";
            resources.ApplyResources(this.menuCalculate, "menuCalculate");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            resources.ApplyResources(this.lblStatus, "lblStatus");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // pnlMassOfOil
            // 
            resources.ApplyResources(this.pnlMassOfOil, "pnlMassOfOil");
            this.pnlMassOfOil.Controls.Add(this.txtOperatorName);
            this.pnlMassOfOil.Controls.Add(this.lblOperatorName);
            this.pnlMassOfOil.Name = "pnlMassOfOil";
            // 
            // txtOperatorName
            // 
            resources.ApplyResources(this.txtOperatorName, "txtOperatorName");
            this.txtOperatorName.Name = "txtOperatorName";
            // 
            // lblOperatorName
            // 
            resources.ApplyResources(this.lblOperatorName, "lblOperatorName");
            this.lblOperatorName.Name = "lblOperatorName";
            // 
            // pnlDeliveryReceiptLoading
            // 
            resources.ApplyResources(this.pnlDeliveryReceiptLoading, "pnlDeliveryReceiptLoading");
            this.pnlDeliveryReceiptLoading.Controls.Add(this.txtConsigner);
            this.pnlDeliveryReceiptLoading.Controls.Add(this.lblConsigner);
            this.pnlDeliveryReceiptLoading.Controls.Add(this.txtShipperLoading);
            this.pnlDeliveryReceiptLoading.Controls.Add(this.lblShipperLoading);
            this.pnlDeliveryReceiptLoading.Controls.Add(this.txtAgentLoading);
            this.pnlDeliveryReceiptLoading.Controls.Add(this.lblAgentLoading);
            this.pnlDeliveryReceiptLoading.Name = "pnlDeliveryReceiptLoading";
            // 
            // txtConsigner
            // 
            resources.ApplyResources(this.txtConsigner, "txtConsigner");
            this.txtConsigner.Name = "txtConsigner";
            // 
            // lblConsigner
            // 
            resources.ApplyResources(this.lblConsigner, "lblConsigner");
            this.lblConsigner.Name = "lblConsigner";
            // 
            // txtShipperLoading
            // 
            resources.ApplyResources(this.txtShipperLoading, "txtShipperLoading");
            this.txtShipperLoading.Name = "txtShipperLoading";
            // 
            // lblShipperLoading
            // 
            resources.ApplyResources(this.lblShipperLoading, "lblShipperLoading");
            this.lblShipperLoading.Name = "lblShipperLoading";
            // 
            // txtAgentLoading
            // 
            resources.ApplyResources(this.txtAgentLoading, "txtAgentLoading");
            this.txtAgentLoading.Name = "txtAgentLoading";
            // 
            // lblAgentLoading
            // 
            resources.ApplyResources(this.lblAgentLoading, "lblAgentLoading");
            this.lblAgentLoading.Name = "lblAgentLoading";
            // 
            // pnlDeliverReceiptDestination
            // 
            resources.ApplyResources(this.pnlDeliverReceiptDestination, "pnlDeliverReceiptDestination");
            this.pnlDeliverReceiptDestination.Controls.Add(this.txtConsignee);
            this.pnlDeliverReceiptDestination.Controls.Add(this.lblConsignee);
            this.pnlDeliverReceiptDestination.Controls.Add(this.txtShipperDestination);
            this.pnlDeliverReceiptDestination.Controls.Add(this.lblShipperDestination);
            this.pnlDeliverReceiptDestination.Controls.Add(this.txtAgentDestination);
            this.pnlDeliverReceiptDestination.Controls.Add(this.lblAgentDestination);
            this.pnlDeliverReceiptDestination.Name = "pnlDeliverReceiptDestination";
            // 
            // txtConsignee
            // 
            resources.ApplyResources(this.txtConsignee, "txtConsignee");
            this.txtConsignee.Name = "txtConsignee";
            // 
            // lblConsignee
            // 
            resources.ApplyResources(this.lblConsignee, "lblConsignee");
            this.lblConsignee.Name = "lblConsignee";
            // 
            // txtShipperDestination
            // 
            resources.ApplyResources(this.txtShipperDestination, "txtShipperDestination");
            this.txtShipperDestination.Name = "txtShipperDestination";
            // 
            // lblShipperDestination
            // 
            resources.ApplyResources(this.lblShipperDestination, "lblShipperDestination");
            this.lblShipperDestination.Name = "lblShipperDestination";
            // 
            // txtAgentDestination
            // 
            resources.ApplyResources(this.txtAgentDestination, "txtAgentDestination");
            this.txtAgentDestination.Name = "txtAgentDestination";
            // 
            // lblAgentDestination
            // 
            resources.ApplyResources(this.lblAgentDestination, "lblAgentDestination");
            this.lblAgentDestination.Name = "lblAgentDestination";
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            resources.ApplyResources(this.grid, "grid");
            this.grid.AutoCalculate = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.HInclination = null;
            this.grid.Name = "grid";
            this.grid.StandardDensityDb = null;
            this.grid.TankDb = null;
            this.grid.ValidationErrorMessage = "";
            this.grid.VcfDb = null;
            this.grid.VInclination = null;
            this.grid.VolumeByHeight = true;
            // 
            // summary
            // 
            this.summary.AllowUserToAddRows = false;
            resources.ApplyResources(this.summary, "summary");
            this.summary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.summary.ColumnHeadersVisible = false;
            this.summary.Name = "summary";
            this.summary.RowHeadersVisible = false;
            this.summary.TotalOfMass = null;
            this.summary.TotalOfVolume = null;
            this.summary.TotalOfVolumeOfStandard = null;
            this.summary.TotalOfVolumeOfWater = null;
            this.summary.ValidationErrorMessage = "";
            // 
            // header
            // 
            resources.ApplyResources(this.header, "header");
            this.header.Name = "header";
            this.header.ReceiptDb = null;
            this.header.TankDb = null;
            this.header.ValidationErrorMessage = "";
            this.header.VesselName = null;
            // 
            // ReceiptForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlDeliverReceiptDestination);
            this.Controls.Add(this.pnlDeliveryReceiptLoading);
            this.Controls.Add(this.pnlMassOfOil);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.summary);
            this.Controls.Add(this.header);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "ReceiptForm";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pnlMassOfOil.ResumeLayout(false);
            this.pnlMassOfOil.PerformLayout();
            this.pnlDeliveryReceiptLoading.ResumeLayout(false);
            this.pnlDeliveryReceiptLoading.PerformLayout();
            this.pnlDeliverReceiptDestination.ResumeLayout(false);
            this.pnlDeliverReceiptDestination.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.summary)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Controls.MeasureHeader header;
        private Controls.MeasureGridSummary summary;
        private System.Windows.Forms.MenuStrip menuStrip;
        private Controls.MeasureGrid grid;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuNew;
        private System.Windows.Forms.ToolStripMenuItem menuLoad;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.ToolStripMenuItem menuPrint;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuVolumeByHeight;
        private System.Windows.Forms.ToolStripMenuItem menuVolumeByUllage;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuCalculate;
        private System.Windows.Forms.ToolStripMenuItem menuAutoCalculate;
        private System.Windows.Forms.Panel pnlMassOfOil;
        private System.Windows.Forms.TextBox txtOperatorName;
        private System.Windows.Forms.Label lblOperatorName;
        private System.Windows.Forms.Panel pnlDeliveryReceiptLoading;
        private System.Windows.Forms.Label lblAgentLoading;
        private System.Windows.Forms.TextBox txtAgentLoading;
        private System.Windows.Forms.TextBox txtConsigner;
        private System.Windows.Forms.Label lblConsigner;
        private System.Windows.Forms.TextBox txtShipperLoading;
        private System.Windows.Forms.Label lblShipperLoading;
        private System.Windows.Forms.Panel pnlDeliverReceiptDestination;
        private System.Windows.Forms.TextBox txtConsignee;
        private System.Windows.Forms.Label lblConsignee;
        private System.Windows.Forms.TextBox txtShipperDestination;
        private System.Windows.Forms.Label lblShipperDestination;
        private System.Windows.Forms.TextBox txtAgentDestination;
        private System.Windows.Forms.Label lblAgentDestination;
    }
}