using ShippingMeasure.Core.Models;
using ShippingMeasure.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShippingMeasure
{
    public partial class ReceiptReportViewerForm : Form
    {
        public ReceiptType ReceiptType
        {
            get; set;
        }

        public ReceiptReportModel Receipt
        {
            get { return receipt; }
            set
            {
                this.receipt = value;
                this.ReceiptReportModelBindingSource.DataSource = value;
                this.ReceiptTankDetailBindingSource.DataSource = (value != null) ? value.ReceiptTankDetails : null;
            }
        }

        private ReceiptReportModel receipt;

        public ReceiptReportViewerForm(ReceiptReportModel receipt, ReceiptType type)
        {
            InitializeComponent();
            this.Receipt = receipt;
            this.ReceiptType = type;
        }

        public void RefreshReport()
        {
            this.reportViewer.RefreshReport();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.reportViewer.LocalReport.ReportPath = this.GetReportPath();
            new Localization(this.reportViewer.LocalReport).Localize();
            this.RefreshReport();
        }

        private string GetReportPath()
        {
            switch(this.ReceiptType)
            {
                case ReceiptType.DeliveryReceiptLoading:
                    return ".\\Reports\\DeliveryReceiptLoadingReport.rdlc";
                case ReceiptType.DeliveryReceiptDestination:
                    return ".\\Reports\\DeliveryReceiptDestinationReport.rdlc";
                case ReceiptType.MassOfOil:
                default:
                    return ".\\Reports\\MassOfOilReport.rdlc";
            }
        }
    }
}
