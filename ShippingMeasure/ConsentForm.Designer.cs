namespace ShippingMeasure
{
    partial class ConsentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsentForm));
            this.txtAgreement = new System.Windows.Forms.TextBox();
            this.rdoAgree = new System.Windows.Forms.RadioButton();
            this.rdoDisagree = new System.Windows.Forms.RadioButton();
            this.chkDoNotShowAgain = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtAgreement
            // 
            resources.ApplyResources(this.txtAgreement, "txtAgreement");
            this.txtAgreement.Name = "txtAgreement";
            this.txtAgreement.ReadOnly = true;
            // 
            // rdoAgree
            // 
            resources.ApplyResources(this.rdoAgree, "rdoAgree");
            this.rdoAgree.Name = "rdoAgree";
            this.rdoAgree.TabStop = true;
            this.rdoAgree.UseVisualStyleBackColor = true;
            // 
            // rdoDisagree
            // 
            resources.ApplyResources(this.rdoDisagree, "rdoDisagree");
            this.rdoDisagree.Name = "rdoDisagree";
            this.rdoDisagree.TabStop = true;
            this.rdoDisagree.UseVisualStyleBackColor = true;
            // 
            // chkDoNotShowAgain
            // 
            resources.ApplyResources(this.chkDoNotShowAgain, "chkDoNotShowAgain");
            this.chkDoNotShowAgain.Name = "chkDoNotShowAgain";
            this.chkDoNotShowAgain.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.chkDoNotShowAgain);
            this.groupBox1.Controls.Add(this.btnOk);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // ConsentForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rdoDisagree);
            this.Controls.Add(this.rdoAgree);
            this.Controls.Add(this.txtAgreement);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConsentForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAgreement;
        private System.Windows.Forms.RadioButton rdoAgree;
        private System.Windows.Forms.RadioButton rdoDisagree;
        private System.Windows.Forms.CheckBox chkDoNotShowAgain;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}