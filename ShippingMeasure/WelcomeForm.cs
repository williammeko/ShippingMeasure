using ShippingMeasure.Common;
using ShippingMeasure.Core;
using ShippingMeasure.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShippingMeasure
{
    public partial class WelcomeForm : Form
    {
        private UserDb userDb = new UserDb();

        public WelcomeForm()
        {
            InitializeComponent();

            var isZhcn = Config.Language.ToUpper().Equals("ZH-CN");
            if (isZhcn)
            {
                //this.cmbLanguage.SelectedIndex = 0;
                this.rdoZhcn.Checked = true;
            }
            else
            {
                //this.cmbLanguage.SelectedIndex = 1;
                this.rdoEnus.Checked = true;
            }

            this.btnLogin.Click += (s, a) => this.Login();
            this.txtUser.KeyUp += (s, a) =>
            {
                if (!a.Alt && !a.Control && !a.Shift && a.KeyCode == Keys.Enter)
                {
                    this.txtPassword.Focus();
                    this.txtPassword.SelectAll();
                }
            };
            this.txtPassword.KeyUp += (s, a) =>
            {
                if (!a.Alt && !a.Control && !a.Shift && a.KeyCode == Keys.Enter)
                {
                    this.btnLogin.Focus();
                    this.Login();
                }
            };
        }

        private void Login()
        {
            var password = new SecureString();
            this.txtPassword.Text.ToCharArray().Each(password.AppendChar);
            bool authorized = false;

            try
            {
                authorized = this.userDb.Authorize(txtUser.Text, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(Strings.MsgErrorFailToVerifyUserLogin, ex.Message), Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!authorized)
            {
                MessageBox.Show(this, Strings.MsgErrorIncorrectUserAndPassword, Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //CommonHelper.ChangeUICultureTo((this.cmbLanguage.SelectedIndex == 0) ? "zh-cn" : "en-us");
            CommonHelper.ChangeUICultureTo(this.rdoZhcn.Checked ? "zh-cn" : "en-us");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void lblUsername_Click(object sender, EventArgs e)
        {

        }

        private void lblPassword_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

        }
    }
}
