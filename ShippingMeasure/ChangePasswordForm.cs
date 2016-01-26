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
using System.Windows.Forms;

namespace ShippingMeasure
{
    public partial class ChangePasswordForm : Form
    {
        public ChangePasswordForm()
        {
            InitializeComponent();

            this.txtOldPassword.KeyUp += (s, a) => { if (!a.Alt && !a.Control && !a.Shift && a.KeyCode == Keys.Enter) { this.txtNewPassword.Focus(); this.txtNewPassword.SelectAll(); } };
            this.txtNewPassword.KeyUp += (s, a) => { if (!a.Alt && !a.Control && !a.Shift && a.KeyCode == Keys.Enter) { this.txtConfirmNewPassword.Focus(); this.txtConfirmNewPassword.SelectAll(); } };
            this.txtConfirmNewPassword.KeyUp += (s, a) => { if (!a.Alt && !a.Control && !a.Shift && a.KeyCode == Keys.Enter) this.ChangePassword(); };
            this.btnChange.Click += (s, a) => this.ChangePassword();
        }

        private void ChangePassword()
        {
            if (!txtNewPassword.Text.Equals(txtConfirmNewPassword.Text))
            {
                MessageBox.Show(this, Strings.MsgErrorPasswordNotSame, Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtNewPassword.Text.Length < 1)
            {
                MessageBox.Show(this, Strings.MsgErrorInvalidPassword, Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var userDb = new UserDb();

            try
            {
                if (!userDb.Authorize("sdari", this.txtOldPassword.Text.Aggregate(new SecureString(), (s, c) => { s.AppendChar(c); return s; })))
                {
                    MessageBox.Show(this, Strings.MsgErrorOldPasswordNotMatch, Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                userDb.Save("sdari", this.txtNewPassword.Text.Aggregate(new SecureString(), (s, c) => { s.AppendChar(c); return s; }));

                this.Close();
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex);
                MessageBox.Show(this, String.Format(Strings.MsgErrorChangePasswordFailed, ex.Message), Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
