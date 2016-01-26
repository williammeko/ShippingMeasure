namespace ShippingMeasure
{
    partial class ReceiptReportViewerForm
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
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.ReceiptTankDetailBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.ReceiptReportModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ReceiptTankDetailBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReceiptReportModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // ReceiptTankDetailBindingSource
            // 
            this.ReceiptTankDetailBindingSource.DataSource = typeof(ShippingMeasure.Core.Models.ReceiptTankDetail);
            // 
            // reportViewer
            // 
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "ReceiptDataSet";
            reportDataSource1.Value = this.ReceiptReportModelBindingSource;
            reportDataSource2.Name = "ReceiptTankDetailDataSet";
            reportDataSource2.Value = this.ReceiptTankDetailBindingSource;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer.LocalReport.ReportPath = ".\\Controls\\ReceiptReport.rdlc";
            this.reportViewer.Location = new System.Drawing.Point(0, 0);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.Size = new System.Drawing.Size(977, 593);
            this.reportViewer.TabIndex = 0;
            // 
            // ReceiptReportModelBindingSource
            // 
            this.ReceiptReportModelBindingSource.DataSource = typeof(ShippingMeasure.Reports.ReceiptReportModel);
            // 
            // ReceiptReportViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 593);
            this.Controls.Add(this.reportViewer);
            this.Name = "ReceiptReportViewerForm";
            this.Text = "Receipt print preview";
            ((System.ComponentModel.ISupportInitialize)(this.ReceiptTankDetailBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReceiptReportModelBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        private System.Windows.Forms.BindingSource ReceiptTankDetailBindingSource;
        private System.Windows.Forms.BindingSource ReceiptReportModelBindingSource;
    }
}