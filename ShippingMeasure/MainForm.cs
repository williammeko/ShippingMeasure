using ShippingMeasure.Common;
using ShippingMeasure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShippingMeasure
{
    public partial class MainForm : ShippingMeasureFormBase
    {
        private bool authorized = false;
        private List<ToolStripMenuItem> toggleControls = new List<ToolStripMenuItem>();
        private Color toggleControlBackColor;
        private Color toggleControlForeColor;

        public MainForm()
        {
            this.Login();

            InitializeComponent();

            this.toggleControls.AddRange(new[] { this.menuMassOfOil, this.menuDeliveryReceiptLoading, this.menuDeliveryReceiptDestination });
            this.toggleControlBackColor = this.menuMassOfOil.BackColor;
            this.toggleControlForeColor = this.menuMassOfOil.ForeColor;

            this.menuMassOfOil.Click += (s, a) => this.ShowForm<MassOfOilForm>(this.menuMassOfOil);
            this.menuDeliveryReceiptLoading.Click += (s, a) => this.ShowForm<DeliveryReceiptLoadingForm>(this.menuDeliveryReceiptLoading);
            this.menuDeliveryReceiptDestination.Click += (s, a) => this.ShowForm<DeliveryReceiptDestinationForm>(this.menuDeliveryReceiptDestination);
            this.menuWindowCloseAll.Click += (s, a) => this.MdiChildren.Each(w => w.Close());
            this.menuChangePassword.Click += (s, a) => new ChangePasswordForm().ShowDialog(this);
            this.menuLangEnus.Click += (s, a) =>
            {
                CommonHelper.ChangeUICultureTo("en-us");
                this.AlertToExitForLanguageChange();
            };
            this.menuLangZhcn.Click += (s, a) =>
            {
                CommonHelper.ChangeUICultureTo("zh-cn");
                this.AlertToExitForLanguageChange();
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (!this.authorized)
            {
                Application.Exit();
                this.Close();
                return;
            }

            this.ShowConsentDialogIfNeed();
        }

        private void ShowForm<T>(ToolStripMenuItem toggleControl) where T : Form, new()
        {
            var activate = new Action(() => { toggleControl.BackColor = SystemColors.ActiveCaption; toggleControl.ForeColor = SystemColors.ActiveCaptionText; });
            var deactivate = new Action(() => this.toggleControls.Each(c => { c.BackColor = this.toggleControlBackColor; c.ForeColor = this.toggleControlForeColor; }));

            var form = this.MdiChildren.FirstOrDefault(f => f is T);
            if (form == null)
            {
                form = new T() { MdiParent = this };
                form.Activated += (s, a) => activate();
                form.Deactivate += (s, a) => deactivate();
                if (this.MdiChildren.Length < 1)
                {
                    form.WindowState = FormWindowState.Maximized;
                }
            }

            var state =
                (ActiveMdiChild != null && ActiveMdiChild.WindowState != FormWindowState.Minimized)
                ? ActiveMdiChild.WindowState
                : FormWindowState.Maximized;
            form.WindowState = state;
            form.Show();
            deactivate();
            activate();
        }

        private void AlertToExitForLanguageChange()
        {
            if (MessageBox.Show(Strings.MsgLanguageChangedPleaseRestart, Strings.MsgConfirmation, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Login()
        {
            using (var welcomeForm = new WelcomeForm())
            {
                if (welcomeForm.ShowDialog() == DialogResult.OK)
                {
                    this.authorized = true;
                }
            }
        }

        private void ShowConsentDialogIfNeed()
        {
            var needConsentDialog = true;
            var consentInfo = Config.ConsentInfo;

            if (String.IsNullOrEmpty(consentInfo) || consentInfo.Trim().Length < 1)
            {
                MessageBox.Show(this, Strings.MsgErrorConfigIssue, Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            var info = consentInfo.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime expirationDate;
            var validExpirationDate = DateTime.TryParse(info[0], out expirationDate);
            var now = DateTime.Today;

            if (validExpirationDate && now > expirationDate)
            {
                Config.ConsentInfo = "     ";   // set ConsentInfo invalid

                MessageBox.Show(this, Strings.MsgErrorSoftwareExpired, Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            if (validExpirationDate && now > expirationDate.AddDays(-90))
            {
                MessageBox.Show(this, String.Format(Strings.MsgWarnSoftwareExpiration, expirationDate.ToString("yyyy-MM-dd")), Strings.MsgWarn, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (info.Length > 1 && Boolean.Parse(info[1]))
            {
                needConsentDialog = false;
            }

            if (needConsentDialog)
            {
                new ConsentForm().ShowDialog(this);
            }
        }
    }
}
