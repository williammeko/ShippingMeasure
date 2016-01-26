using ShippingMeasure.Common;
using ShippingMeasure.Core;
using ShippingMeasure.Core.Models;
using ShippingMeasure.Db;
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
    public partial class KindsOfGoodsForm : Form
    {
        public ReceiptDb ReceiptDb { get; private set; }
        public KindOfGoods SelectedKind { get; private set; } = null;

        public KindsOfGoodsForm(ReceiptDb receiptDb)
        {
            InitializeComponent();
            this.ReceiptDb = receiptDb;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.LoadKindsOfGoods();

            this.txtName.TextChanged += (s, a) => this.btnSave.Enabled = this.txtName.Text.Trim().Length > 0;
            this.txtName.KeyUp += (s, a) => this.btnSave.Enabled = this.txtName.Text.Trim().Length > 0;
            this.btnAdd.Click += (s, a) => this.AddNew();
            this.btnSave.Click += (s, a) => this.Save();
            this.btnRemove.Click += (s, a) => this.Remove();
            this.btnSelect.Click += (s, a) => this.SelectKind();
            this.list.KeyUp += (s, a) =>
            {
                if (!a.Alt && !a.Control && !a.Shift && a.KeyCode == Keys.Enter)
                {
                    this.SelectKind();
                }
            };
            this.list.DoubleClick += (s, a) =>
            {
                if (this.list.SelectedItem != null)
                {
                    this.SelectKind();
                }
            };
            this.list.SelectedIndexChanged += (s, a) =>
            {
                if (this.list.SelectedItems.Count > 0)
                {
                    this.btnRemove.Enabled = this.btnSelect.Enabled = true;
                    var selectedKind = this.list.SelectedItem as KindOfGoods;
                    this.txtName.Text = selectedKind.Name;
                    this.txtName.Tag = selectedKind;
                }
                else
                {
                    this.btnRemove.Enabled = this.btnSelect.Enabled = false;
                    this.txtName.Text = String.Empty;
                    this.txtName.Tag = null;
                }
            };
        }

        private void LoadKindsOfGoods()
        {
            this.ReceiptDb.GetAllKindsOfGoods()
                .OrderBy(k => k.Name)
                .Each(k => this.list.Items.Add(k));
        }

        private void SelectKind()
        {
            if (this.list.SelectedIndices.Count < 1)
            {
                return;
            }

            this.SelectedKind = this.ReceiptDb.GetAllKindsOfGoods().First(k => k.Name.Equals(this.list.SelectedItem.ToString()));
            this.DialogResult = DialogResult.OK;
        }

        private void AddNew()
        {
            this.list.SelectedIndices.Clear();
            this.txtName.Text = String.Empty;
            this.txtName.Tag = null;
        }

        private void Save()
        {
            try
            {
                this.txtName.Text = this.txtName.Text.Trim();
                var currentKind = this.txtName.Tag as KindOfGoods;
                var isNew = currentKind == null;
                if (!isNew)
                {
                    currentKind.Name = this.txtName.Text;
                }
                else
                {
                    currentKind = new KindOfGoods { UId = Guid.NewGuid().ToString(), Name = this.txtName.Text, Customized = true };
                    txtName.Tag = currentKind;
                }

                this.ReceiptDb.Save(currentKind);

                this.list.Items.Clear();
                this.list.Items.AddRange(this.ReceiptDb.GetAllKindsOfGoods().OrderBy(k => k.Name).ToArray());
                this.list.SelectedItem = currentKind;
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex);
                MessageBox.Show(this, String.Format("{0}\r\n\r\n{1}", Strings.MsgSaveKindOfGoodsFailed, ex.Message), Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Remove()
        {
            try
            {
                this.list.SelectedItems.Cast<KindOfGoods>()
                    .Each(k => this.ReceiptDb.RemoveKindOfGoods(k.UId));
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex);
                MessageBox.Show(this, String.Format("{0}\r\n\r\n{1}", Strings.MsgRemoveKindOfGoodsFailed, ex.Message), Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.list.Items.Clear();
                this.list.Items.AddRange(this.ReceiptDb.GetAllKindsOfGoods().OrderBy(k => k.Name).ToArray());
            }
        }
    }
}
