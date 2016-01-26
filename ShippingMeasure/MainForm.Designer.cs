namespace ShippingMeasure
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuMassOfOil = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeliveryReceiptLoading = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeliveryReceiptDestination = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWindowCloseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLang = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLangZhcn = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLangEnus = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpManual = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuChangePassword = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMassOfOil,
            this.menuDeliveryReceiptLoading,
            this.menuDeliveryReceiptDestination,
            this.menuWindow,
            this.menuLang,
            this.menuHelp});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.MdiWindowListItem = this.menuWindow;
            this.menuStrip1.Name = "menuStrip1";
            // 
            // menuMassOfOil
            // 
            this.menuMassOfOil.Name = "menuMassOfOil";
            resources.ApplyResources(this.menuMassOfOil, "menuMassOfOil");
            // 
            // menuDeliveryReceiptLoading
            // 
            this.menuDeliveryReceiptLoading.Name = "menuDeliveryReceiptLoading";
            resources.ApplyResources(this.menuDeliveryReceiptLoading, "menuDeliveryReceiptLoading");
            // 
            // menuDeliveryReceiptDestination
            // 
            this.menuDeliveryReceiptDestination.Name = "menuDeliveryReceiptDestination";
            resources.ApplyResources(this.menuDeliveryReceiptDestination, "menuDeliveryReceiptDestination");
            // 
            // menuWindow
            // 
            this.menuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuWindowCloseAll});
            this.menuWindow.Name = "menuWindow";
            resources.ApplyResources(this.menuWindow, "menuWindow");
            // 
            // menuWindowCloseAll
            // 
            this.menuWindowCloseAll.Name = "menuWindowCloseAll";
            resources.ApplyResources(this.menuWindowCloseAll, "menuWindowCloseAll");
            // 
            // menuLang
            // 
            this.menuLang.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLangZhcn,
            this.menuLangEnus});
            this.menuLang.Name = "menuLang";
            resources.ApplyResources(this.menuLang, "menuLang");
            // 
            // menuLangZhcn
            // 
            this.menuLangZhcn.Name = "menuLangZhcn";
            resources.ApplyResources(this.menuLangZhcn, "menuLangZhcn");
            // 
            // menuLangEnus
            // 
            this.menuLangEnus.Name = "menuLangEnus";
            resources.ApplyResources(this.menuLangEnus, "menuLangEnus");
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpManual,
            this.toolStripMenuItem4,
            this.menuChangePassword});
            this.menuHelp.Name = "menuHelp";
            resources.ApplyResources(this.menuHelp, "menuHelp");
            // 
            // menuHelpManual
            // 
            this.menuHelpManual.Name = "menuHelpManual";
            resources.ApplyResources(this.menuHelpManual, "menuHelpManual");
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // menuChangePassword
            // 
            this.menuChangePassword.Name = "menuChangePassword";
            resources.ApplyResources(this.menuChangePassword, "menuChangePassword");
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuMassOfOil;
        private System.Windows.Forms.ToolStripMenuItem menuDeliveryReceiptLoading;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuHelpManual;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem menuLang;
        private System.Windows.Forms.ToolStripMenuItem menuLangZhcn;
        private System.Windows.Forms.ToolStripMenuItem menuLangEnus;
        private System.Windows.Forms.ToolStripMenuItem menuWindow;
        private System.Windows.Forms.ToolStripMenuItem menuWindowCloseAll;
        private System.Windows.Forms.ToolStripMenuItem menuChangePassword;
        private System.Windows.Forms.ToolStripMenuItem menuDeliveryReceiptDestination;
    }
}

