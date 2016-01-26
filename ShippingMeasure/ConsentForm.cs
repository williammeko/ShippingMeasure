using ShippingMeasure.Common;
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
    public partial class ConsentForm : Form
    {

        public ConsentForm()
        {
            InitializeComponent();

            var expirationDate = this.GetExpirationDate();
            this.txtAgreement.Text = String.Format(this.txtAgreement.Text, expirationDate);

            this.rdoDisagree.CheckedChanged += (s, a) => this.btnOk.Enabled = this.rdoAgree.Checked || this.rdoDisagree.Checked;
            this.rdoAgree.CheckedChanged += (s, a) => this.btnOk.Enabled = this.rdoAgree.Checked || this.rdoDisagree.Checked;

            this.btnOk.Click += (s, a) =>
            {
                if (this.rdoDisagree.Checked)
                {
                    Application.Exit();
                    return;
                }

                Config.ConsentInfo = String.Format("{0};{1}", expirationDate, this.chkDoNotShowAgain.Checked.ToString());
                this.Close();
            };
        }

        private string GetExpirationDate()
        {
            var consentInfo = Config.ConsentInfo;
            if (!String.IsNullOrEmpty(consentInfo))
            {
                try
                {
                    var expirationDate = consentInfo.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    DateTime tmp;
                    if (DateTime.TryParse(expirationDate, out tmp))
                    {
                        return expirationDate;
                    }
                }
                catch
                {
                }
            }
            return DateTime.Today.AddYears(3).ToString("yyyy-MM-dd");
        }
    }
}
